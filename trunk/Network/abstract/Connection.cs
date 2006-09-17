/* [ Network/abstract/Server.cs ] - Niry Abstract Server
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
using System.Timers;
using System.Threading;
using System.Net.Sockets;
using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace Niry.Network {
	public enum ConnectionState { Disconnected, Connecting, Connected }
	public enum ConnectionError { Dns, Timeout, General }

	public class ConnectionErrorEventArgs : EventArgs {
		private ConnectionError error;

		public ConnectionErrorEventArgs (ConnectionError error) {
			this.error = error;
		}
		
		public ConnectionError Error {
			get { return(error); }
		}
	}
	
	public delegate void ConnectionErrorEventHandler (object sender, 
											  ConnectionErrorEventArgs args);

	public abstract class Connection : IDisposable {
		// ===================================
		// PUBLIC Events
		// ===================================
		public event ConnectionErrorEventHandler Error = null;
		public event EventHandler StateChanged = null;

		public event BlankEventHandler Connected = null;
		public event StringEventHandler Received = null;

		// ===================================
		// PRIVATE Members
		// ===================================
		private Timer connectionTimer;
		private ConnectionState state;
		private string commandString;
		private string commandBuffer;
		private IPEndPoint endPoint;
		private Encoding encoding;
		private Socket client;
		private byte[] buffer;
		private bool incoming;
		
		// ===================================
		// PROTECTED Constructors
		// ===================================
		protected Connection() {
			state = ConnectionState.Disconnected;
			encoding = Encoding.GetEncoding(1252);

			// Initialize Connection Timer
			connectionTimer = new Timer();
			connectionTimer.Interval = 10000;
			connectionTimer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);

			// Initialize Buffer
			buffer = new byte[16384];
		}

		// ===================================
		// PUBLIC VIRTUAL Methods
		// ===================================
		public virtual void Connect() {
			State = ConnectionState.Connecting;
			connectionTimer.Start();
			ThreadPool.QueueUserWorkItem(new WaitCallback(Connect), null);
		}
		
		public virtual void Disconnect() {
			if (State != ConnectionState.Disconnected) {
				State = ConnectionState.Disconnected;

				if (Client != null && Client.Connected) {
					try {
						Client.Shutdown(SocketShutdown.Both);
					} catch (SocketException) {
					} finally {
						Client.Close();
					}
				}
				connectionTimer.Stop();
			}
		}
		
		public virtual void Send (string message) {
			if (State != ConnectionState.Disconnected && Client.Connected) {
				byte[] bytes = Encoding.GetBytes(message);
				SendBytes(bytes);
			}
		}
		
		public virtual void ConnectionSend (string message) {
			if (State != ConnectionState.Disconnected && Client.Connected) {
				byte[] bytes = new byte[message.Length];
				for(int i = 0; i < message.Length; i++) {
					bytes[i] = Convert.ToByte(message[i]);
				}
				SendBytes(bytes);
			}
		}
		
		public virtual void SendBytes(byte[] bytes) {
			SendBytes(bytes, bytes.Length);
		}
		
		public virtual void SendBytes(byte[] bytes, int length) {
			if (State != ConnectionState.Disconnected && Client.Connected) {
				client.BeginSend(bytes, 0, length, SocketFlags.None,
								 new AsyncCallback(OnSentData), Client);
			}
		}

		// ===================================
		// PROTECTED VIRTUAL Methods
		// ===================================
		protected virtual void Receive() {
			try {
				client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, 
									new AsyncCallback(OnReceivedData), client);
			} catch {
				Disconnect();
			}
		}
		
		protected virtual void Dispose (bool disposing) {
			if (disposing) {
				if (Client != null && Client.Connected) {
					try {
						Client.Shutdown(SocketShutdown.Both);
					} catch (SocketException) {
					} finally {
						Client.Close();
					}
				}
			}
		}

		// ===================================
		// PUBLIC Methods
		// ===================================
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// ===================================
		// PROTECTED Methods
		// ===================================
		protected void OnError(ConnectionError error) {
			Debug.WriteLine(String.Format("Connection error: {0}", error));
			
			if(Error != null) {
				Error(this, new ConnectionErrorEventArgs(error));
			}
		}

		// ===================================
		// PRIVATE Methods
		// ===================================
		private void Connect (object state) {
			if (client == null) {
				client = new Socket(AddressFamily.InterNetwork,
									SocketType.Stream, ProtocolType.Tcp);
			}
			
			// Is this an incoming or outgoing connection?
			if (!client.Connected) {
				// An outgoing connection
				try {
					client.BeginConnect(endPoint, 
										new AsyncCallback(SocketConnected), 
										client);
				} catch (SocketException) {
					OnError(ConnectionError.General);
					Disconnect();
				}
			} else {
				// An incoming connection
				incoming = true;
				
				// Send Connected Event (Initialize Protocol...)
				if (Connected != null) Connected(this);
				
				State = ConnectionState.Connected;
				Receive();
			}
		}
		
		private void SocketConnected (IAsyncResult result) {
			Socket client = (Socket)result.AsyncState;
			try {
				client.EndConnect(result);
			} catch (SocketException) {
				OnError(ConnectionError.General);
				Disconnect();
				return;
			}
			
			// Send Connected Event (Initialize Protocol...)
			if (Connected != null) Connected(this);
			
			State = ConnectionState.Connected;
			Receive();
		}

		private void OnTimerElapsed (object obj, ElapsedEventArgs args) {
			if (State == ConnectionState.Connecting) {
				OnError(ConnectionError.Timeout);
				Disconnect();
			}
		}
	
		private void OnSentData (IAsyncResult result) {
			try {
				Socket socket = result.AsyncState as Socket;
				socket.EndSend(result);
			} catch (SocketException) {
				Disconnect();
			}
		}
		
		private void OnReceivedData (IAsyncResult result) {
			int length = 0;
			try {
				length = Client.EndReceive(result);
			} catch (SocketException) {
				Disconnect();
			}
			
			if (length > 0) {
				commandString = commandBuffer + Encoding.GetString(buffer, 0, length);				

				// Send Received Event
				if (Received != null) Received(this, commandString);

				Receive();
			}
		}

		// ===================================
		// PUBLIC Properties
		// ===================================
		public ConnectionState State {
			get { return(this.state); }
			internal set {
				this.state = value;
				if (StateChanged != null) StateChanged(this, EventArgs.Empty);
			}
		}

		public Socket Client {
			get { return(this.client); }
			protected set { this.client = value; }
		}

		public Encoding Encoding {
			get { return(this.encoding); }
			protected set { this.encoding = value; }
		}

		public bool Incoming {
			get { return(this.incoming); }
		}

		// ===================================
		// PROTECTED Properties
		// ===================================
		protected IPEndPoint EndPoint {
			get { return(this.endPoint); }
			set { this.endPoint = value; }
		}
	}
}
