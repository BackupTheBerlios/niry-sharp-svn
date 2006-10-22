/* [ Network/FtpClient.cs ] - Niry Ftp Client
 * Author: Matteo Bertozzi
 * ============================================================================
 * Niry Sharp
 * Copyright (C) 2006 Matteo Bertozzi.
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301 USA
 */

using System;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;

namespace Niry.Network {
	public class FtpException : Exception {
		public FtpException (string msg) : base(msg) {}
		public FtpException (string msg, Exception inner) : base(msg, inner) {}
	}

	public class FtpClient {
		private static Encoding ASCII = Encoding.ASCII;
		private const int BufferSize = 512;

		private string username = "anonymous";
		private string password = "anonymous";
		private string server = "localost";
		private string remotePath = ".";
		private string message = null;
		private string result = null;

		private int resultCode = 0;
		private int bytes = 0;
		private int port = 21;

		private bool loggedIn = false;
		private bool binMode = false;

		private byte[] buffer = new byte[BufferSize];
		private int timeoutSeconds = 10;
		private Socket socket = null;

		// ========================================
		// PUBLIC Constructors
		// ========================================
		public FtpClient() {
		}

		public FtpClient (string server, string username, string password) {
			this.server = server;
			this.username = username;
			this.password = password;
		}

		public FtpClient (string server, int port, 
						  string username, string password,
						  int timeoutSeconds)
		{
			this.port = port;
			this.server = server;
			this.username = username;
			this.password = password;
			this.timeoutSeconds = timeoutSeconds;
		}

		// ========================================
		// PUBLIC Methods
		// ========================================
		public void Login() {
			if (this.loggedIn == true) this.Close();

			IPEndPoint endPoint = null;
			IPAddress addr = null;
			try {
				this.socket = new Socket(AddressFamily.InterNetwork, 
										 SocketType.Stream, ProtocolType.Tcp);
				addr = Dns.GetHostEntry(this.server).AddressList[0];
				endPoint = new IPEndPoint(addr, this.port);
				this.socket.Connect(endPoint);
			} catch (Exception e) {
				this.Close();
				throw(new FtpException("Couldn't connect to remote server", e));
			}

			// Read Response
			this.ReadResponse();

			// Check Result Code
			if (this.resultCode != 220) {
				this.Close();
				throw new FtpException(this.result.Substring(4));
			}

			// Send UserName
			this.SendCommand("USER " + username);

			// Check Result Code
			if (!(this.resultCode == 331 || this.resultCode == 230)) {
				this.Close();
				throw new FtpException(this.result.Substring(4));
			}

			if (this.resultCode != 230) {
				// Send Password
				this.SendCommand("PASS " + password);

				// Check Result Code
				if (!(this.resultCode == 230 || this.resultCode == 202)) {
					this.Close();
					throw new FtpException(this.result.Substring(4));
				}
			}

			this.loggedIn = true;
			this.ChangeDir(this.remotePath);
		}

		public void Close() {
			if (this.socket != null && this.socket.Connected) {
				this.SendCommand("QUIT");
			}
			this.Cleanup();
		}

		public string[] GetFileList() {
			return(this.GetFileList("*.*"));
		}

		public string[] GetFileList (string mask) {
			if (this.loggedIn == false) this.Login();

			Socket sock = CreateDataSocket();
			this.SendCommand("NLST " + mask);

			// Check Result Code
			if (!(this.resultCode == 150 || this.resultCode == 125)) {
				sock.Close();
				throw new FtpException(this.result.Substring(4));
			}

			DateTime timeout = DateTime.Now.AddSeconds(this.timeoutSeconds);
			this.message = "";
			while (timeout > DateTime.Now) {
				int bytes = sock.Receive(buffer, buffer.Length, 0);
				this.message += ASCII.GetString(buffer, 0, bytes);
				if (bytes < this.buffer.Length) break;
			}

			string[] msg = this.message.Replace("\r", "").Split('\n');
			sock.Close();

			if (this.message.IndexOf("No such file or directory") != -1)
				msg = new string[] {};

			// Read Response
			this.ReadResponse();

			if (this.resultCode != 226)
				msg = new string[] {};
		
			return(msg);		
		}

		public long GetFileSize (string fileName) {
			if (this.loggedIn == false) this.Login();

			this.SendCommand("SIZE " + fileName);

			if (this.resultCode != 213) {
				throw(new FtpException(this.result.Substring(4)));
			}

			return(long.Parse(this.result.Substring(4)));
		}

		public void Download (string fileName) {
			Download(fileName, null, false);
		}

		public void Download (string fileName, bool resume) {
			Download(fileName, null, resume);
		}

		public void Download (string fileName, string localFileName) {
			Download(fileName, localFileName, false);
		}

		public void Download (string fileName, string localFileName, bool resume) {
			if (this.loggedIn == false) this.Login();

			this.BinaryMode = true;

			if (localFileName == null) localFileName = fileName;
			FileStream output = null;

			if (!File.Exists(localFileName)) {
				output = File.Create(localFileName);
			} else {
				output = new FileStream(localFileName, FileMode.Open);
			}

			Socket sock = CreateDataSocket();

			long offset = 0;
			if (resume == true) {
				offset = output.Length;
				if (offset > 0) {
					this.SendCommand("REST " + offset);
					if (this.resultCode != 350) {
						// Resuming Not Supported
						offset = 0;
					} else {
						output.Seek(offset, SeekOrigin.Begin);
					}
				}
			}

			this.SendCommand("RETR " + fileName);
			if (this.resultCode != 150 && this.resultCode != 125)
				throw(new FtpException(this.result.Substring(4)));

			DateTime timeout = DateTime.Now.AddSeconds(this.timeoutSeconds);
			while (timeout > DateTime.Now) {
				this.bytes = sock.Receive(buffer, buffer.Length, 0);
				output.Write(this.buffer, 0, this.bytes);
				if (this.bytes <= 0) break;
			}

			output.Close();
			sock.Close();

			this.ReadResponse();

			if (this.resultCode != 226 && this.resultCode != 250)
				throw(new FtpException(this.result.Substring(4)));
		}

		public void Upload (string fileName) {
			Upload(fileName, false);
		}

		public void Upload (string fileName, bool resume) {
			if (this.loggedIn == false) this.Login();

			long offset = 0;
			if (resume == true) {
				try {
					this.BinaryMode = true;
					offset= GetFileSize(Path.GetFileName(fileName));
				} catch (Exception) {
					// File Not Exists
					offset = 0;
				}
			}

			// Open Stream to Read File
			FileStream input = new FileStream(fileName, FileMode.Open);

			if (resume && input.Length < offset) {
				// Different File Size, OverWriting
				offset = 0;
			} else if (resume && input.Length == offset) {
				// File Done
				input.Close();
				return;
			}

			Socket sock = this.CreateDataSocket();
			if (offset > 0) {
				this.SendCommand("REST " + offset);
				if (this.resultCode != 350) {
					// Resuming not supported
					offset = 0;
				}
			}

			this.SendCommand("STOR " + Path.GetFileName(fileName));
			if (this.resultCode != 125 && this.resultCode != 150) 
				throw(new FtpException(result.Substring(4)));

			if (offset != 0) {
				// Resuming at Offset
				input.Seek(offset, SeekOrigin.Begin);
			}

			// Uploading file
			while ((bytes = input.Read(buffer, 0, buffer.Length)) > 0) {
				sock.Send(buffer, bytes, 0);
			}

			input.Close();
			sock.Close();

			this.ReadResponse();
			if (this.resultCode != 226 && this.resultCode != 250)
				throw(new FtpException(this.result.Substring(4)));
		}		

		public void UploadDirectory (string path, bool recurse) {
			UploadDirectory(path, recurse, "*.*");
		}

		public void UploadDirectory (string path, bool recurse, string mask) {
			string[] dirs = path.Replace("/", @"\").Split('\\');
			string rootDir = dirs[dirs.Length - 1];

			// Make the Root dir if it not exists
			if (this.GetFileList(rootDir).Length < 1) this.MakeDir(rootDir);
			this.ChangeDir(rootDir);

			foreach (string file in Directory.GetFiles(path, mask)) {
				this.Upload(file, true);
			}

			if (recurse == true) {
				foreach (string directory in Directory.GetDirectories(path)) {
					this.UploadDirectory(directory, recurse, mask);
				}
			}

			this.ChangeDir("..");
		}

		public void DeleteFile (string fileName) {
			if (this.loggedIn == false) this.Login();
			this.SendCommand("DELE " + fileName);

			if (this.resultCode != 250)
				throw(new FtpException(this.result.Substring(4)));
		}


		public void RenameFile (string oldFileName, string newFileName, bool overwrite) {
			if (this.loggedIn == false) this.Login();

			this.SendCommand("RNFR " + oldFileName);
			if (this.resultCode != 350)
				throw(new FtpException(this.result.Substring(4)));

			if (!overwrite && this.GetFileList(newFileName).Length > 0)
				throw(new FtpException("File Already Exists"));

			this.SendCommand("RNTO " + newFileName);
			if (this.resultCode != 250)
				throw(new FtpException(this.result.Substring(4)));
		}

		public void MakeDir (string dirName) {
			if (this.loggedIn == false) this.Login();

			this.SendCommand("MKD " + dirName);
			if (this.resultCode != 250 && this.resultCode != 257)
				throw(new FtpException(this.result.Substring(4)));
		}
		
		public void RemoveDir (string dirName) {
			if (this.loggedIn == false) this.Login();

			this.SendCommand("RMD " + dirName);
			if (this.resultCode != 250)
				throw(new FtpException(this.result.Substring(4)));
		}

		public void ChangeDir (string dirName) {
			if (dirName == null || dirName.Equals(".") || dirName.Length == 0)
				return;

			if (this.loggedIn == false) this.Login();

			this.SendCommand("CWD " + dirName);
			if (this.resultCode != 250)
				throw(new FtpException(this.result.Substring(4)));

			this.SendCommand("PWD");
			if (this.resultCode != 257)
				throw(new FtpException(this.result.Substring(4)));

			this.remotePath = this.message.Split('"')[1];
		}

		// ========================================
		// PROTECTED Methods
		// ========================================
		protected void ReadResponse() {
			this.message = "";
			this.result = this.ReadLine();

			if (this.result.Length > 0) {
				this.resultCode = int.Parse(this.result.Substring(0, 3));
			} else {
				this.result = null;
			}
		}

		protected void SendCommand (string command) {
			byte[] cmdBytes = Encoding.ASCII.GetBytes((command + "\r\n").ToCharArray());
			socket.Send(cmdBytes, cmdBytes.Length, 0);
			this.ReadResponse();
		}

		private Socket CreateDataSocket() {
			this.SendCommand("PASV");
			if (this.resultCode != 227)
				throw(new FtpException(this.result.Substring(4)));

			int index1 = this.result.IndexOf('(');
			int index2 = this.result.IndexOf(')');

			string ipData = this.result.Substring(index1 + 1, index2 - index1 - 1);
			int[] parts = new int[6];
			int len = ipData.Length;
			int partCount = 0;
			string buf = "";

			for (int i=0; i < len && partCount <= 6; i++) {
				char chr = char.Parse(ipData.Substring(i, 1));
				if (char.IsDigit(chr)) {
					buf += chr;
				} else if (chr != ',') {
					throw(new FtpException("Malformed PASV Result: " + result));
				}

				if (chr == ',' || i + 1 == len) {
					try {
						parts[partCount++] = int.Parse(buf);
						buf = "";
					} catch (Exception e) {
						throw(new FtpException("Malformed PASV Result: " + result, e));
					}
				}
			}

			string ipAddress = parts[0] + "." + parts[1] + "." + parts[2] + "." + parts[3];
			int port = (parts[4] << 8) + parts[5];

			IPEndPoint endPoint = null;
			Socket sock = null;

			try {
				sock = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
				endPoint = new IPEndPoint(Dns.GetHostEntry(ipAddress).AddressList[0], port);
				sock.Connect(endPoint);
			} catch (Exception e) {
				if (sock != null && sock.Connected) sock.Close();
				throw(new FtpException("Can't Connect to Remote Sever", e));
			}

			return(sock);
		}

		// ========================================
		// PRIVATE Methods
		// ========================================
		private string ReadLine() {
			while (true) {
				this.bytes = socket.Receive(this.buffer, this.buffer.Length, 0);
				this.message += ASCII.GetString(this.buffer, 0, this.bytes);
				if (this.bytes < this.buffer.Length) break;
			}

			string[] msg = this.message.Split('\n');
			if (this.message.Length > 2) {
				this.message = msg[msg.Length - 2];
			} else {
				this.message = msg[0];
			}

			if (this.message.Length > 4 && !this.message.Substring(3, 1).Equals(" "))
				return(this.ReadLine());

			return(message);
		}

		private void Cleanup() {
			if (this.socket != null) {
				this.socket.Close();
				this.socket = null;
			}
			this.loggedIn = false;
		}

		~FtpClient() {
			this.Cleanup();
		}

		// ========================================
		// PUBLIC Properties
		// =======================================
		public string Username {
			get { return(this.username); }
			set { this.username = value; }
		}

		public string Password {
			get { return(this.password); }
			set { this.password = value; }
		}

		public string Server {
			get { return(this.server); }
			set { this.server = value; }
		}

		public int Timeout {
			get { return(this.timeoutSeconds); }
			set { this.timeoutSeconds = value; }
		}

		public int Port {
			get { return(this.port); }
			set { this.port = value; }
		}

		public string RemotePath {
			get { return(this.remotePath); }
			set { this.remotePath = value; }
		}

		public bool BinaryMode {
			get { return(this.binMode); }
			set {
				if (this.binMode == value) return;
				this.binMode = value;

				if (value == true) {
					SendCommand("TYPE I");
				} else {
					SendCommand("TYPE A");
				}

				// Check Result Code
				if (this.resultCode != 200) 
					throw(new FtpException(result.Substring(4)));
			}
		}
	}
}
