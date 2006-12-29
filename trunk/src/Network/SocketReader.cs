/* [ Network/SocketReader.cs ] - Niry Socket Reader
 * Author: Matteo Bertozzi
 * =============================================================================
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
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Niry.Network {
	public class SocketReader {
		// ===================================
		// PRIVATE Members
		// ===================================
		private ExceptionEventHandler exCallback;
		private string buffer = String.Empty;
		private StringEventHandler callback;
		private AsyncCallback receivedData;
		private Encoding encoding;
		private Socket socket;
		private byte[] bytes;
		private string split;
		private bool doSplit;
		
		// ===================================
		// PUBLIC Constructors
		// ===================================
		public SocketReader (Socket socket) : this(socket, 40096) {
		}
		
		public SocketReader (Socket socket, int length) {
			Debug.Assert(socket != null);
		
			this.socket = socket;
			this.bytes = new byte[length];
			this.receivedData = new AsyncCallback(OnReceivedData);
		}
		
		// ===================================
		// PUBLIC Methods
		// ===================================
		public void Receive (StringEventHandler callback,
							 ExceptionEventHandler exCallback)
		{
			Debug.Assert(callback != null);
			Debug.Assert(exCallback != null);
			
			this.callback = callback;
			this.exCallback = exCallback;
			this.doSplit = false;
			
			Receive();
		}
		
		public void Receive (StringEventHandler callback,
							 ExceptionEventHandler exCallback,
							 string split)
		{
			Debug.Assert(split != null);
			Debug.Assert(callback != null);
			Debug.Assert(exCallback != null);
		
			this.callback = callback;
			this.exCallback = exCallback;
			this.split = Regex.Escape(split);
			this.doSplit = true;
			
			Receive();
		}
		
		// ===================================
		// PRIVATE Methods
		// ===================================
		private void Receive() {
			try {
				this.socket.BeginReceive(bytes, 0, bytes.Length, 
										 SocketFlags.None, 
										 receivedData, socket);
			} catch (SocketException e) {
				exCallback(this, e);
			}
		}
		
		private void OnReceivedData (IAsyncResult result) {
			Socket socket = result.AsyncState as Socket;

			if (socket != null && socket.Handle != IntPtr.Zero) {
				int numBytes = 0;
				
				try {
					numBytes = socket.EndReceive(result);
				} catch (SocketException e) {
					exCallback(this, e);
				} catch (ObjectDisposedException e) {
					exCallback(this, e);
				}
				
				if (numBytes > 0) {
					string line = encoding.GetString(bytes, 0, numBytes);
					if (doSplit == true) {
						ProcessLine(line);
					} else {
						callback(this, line);
					}
					
					Receive();
				}
			} else {
				exCallback(this, new SocketException());
			}
		}
		
		private void ProcessLine (string line) {
			string[] commands = Regex.Split(buffer + line, split);
			
			for (int i=0; i < commands.Length - 1; i++) {
				if (commands[i].Length > 0)
					callback(this, commands[i]);
			}
			
			buffer = commands[commands.Length - 1];
		}
		
		// ===================================
		// PUBLIC Properties
		// ===================================
		public Encoding Encoding {
			set { encoding = value; }
			get { return((encoding == null) ? Encoding.Default : encoding); }			
		}

		public Socket Socket {
			get { return(this.socket); }
		}
	}
}
