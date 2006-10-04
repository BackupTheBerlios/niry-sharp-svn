/* [ Network/PeerSocket.cs ] - Niry Peer Socket
 * Author: Matteo Bertozzi
 * ============================================================================
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of version 2 of the Lesser GNU General 
 * Public License as published by the Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this program; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections;

using Niry;
using Niry.Utils;

namespace Niry.Network {
	public class PeerSocket {
		// Temp Class for Recv()
		protected class StateObject {
			public StringBuilder sBuffer = new StringBuilder();
			// 8K, 10240 (10K), 12288 (12K)
			public const int BufferSize = 12288;			
			public byte[] Buffer = new Byte[BufferSize];
			public Socket Sock = null;
		}

		// ============================================
		// PUBLIC Events
		// ============================================
		public event PeerEventHandler Disconnecting = null;
		public event PeerEventHandler Connected = null;
		public event PeerEventHandler Received = null;
		public event PeerEventHandler Sending = null;
		public event PeerEventHandler Sended = null;
		public event PeerEventHandler Error = null;

		// ============================================
		// PROTECTED Members
		// ============================================
		protected StringBuilder response = new StringBuilder();		

		// ============================================
		// PROTECTED Members
		// ============================================
		protected EndPoint remoteEndPoint;
		protected Socket socket;
		protected object info;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public PeerSocket() {
			this.socket = new Socket(AddressFamily.InterNetwork, 
									 SocketType.Stream, ProtocolType.Tcp);
		}

		public PeerSocket (Socket sock) {
			this.socket = sock;
		}

		public PeerSocket (AddressFamily addrFamily, 
							SocketType sockType, 
							ProtocolType protocolType)
		{
			this.socket = new Socket(addrFamily, sockType, protocolType);
		}

		// ============================================
		// PUBLIC STATIC Methods
		// ============================================
		public static PeerSocket GetPeer (string hostname, int port) {
			IPAddress ipAddress = IPAddress.Parse(hostname);
			IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
			Socket sock = new Socket(AddressFamily.InterNetwork,
									 SocketType.Stream, ProtocolType.Tcp);
			sock.Connect(ipEndPoint);
			return(new PeerSocket(sock));
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public IPAddress GetRemoteIP() {
			if (this.socket != null) {
				IPEndPoint ipEndPoint = (IPEndPoint) this.socket.RemoteEndPoint;
				return(IPAddress.Parse(ipEndPoint.Address.ToString()));
			}
			return(null);
		}

		public void Connect (EndPoint remoteEndPoint) {
			this.remoteEndPoint = (IPEndPoint) remoteEndPoint;

			AsyncCallback connectCb = new AsyncCallback(AsyncConnectCallBack);
			this.socket.BeginConnect(this.remoteEndPoint, connectCb, this.socket);
		}

		public bool IsAvailable() {
			return(this.socket != null && this.socket.Connected == true);
		}

		public void Disconnect() {
			// Destroy Peer
			if (this.socket != null && this.socket.Connected == true) {
				// Raise Disconnecting Event
				// ======================================================
				if (Disconnecting != null)
					Disconnecting(this, new PeerEventArgs(PeerEvent.Disconnecting));
				// ======================================================

				this.socket.Shutdown(SocketShutdown.Both);
				this.socket.Close();
			}
			this.socket = null;
			this.response = null;
		}

		public void Send (string data) {
			Send(Encoding.UTF8.GetBytes(data));
		}

		public void Send (byte[] data) {
			// Raise Sending Event
			// ======================================================
			if (Sending != null)
				Sending(this, new PeerEventArgs(PeerEvent.Sending, data));
			// ======================================================

			// Check Peer
			if (this.socket == null || this.socket.Connected == false) {
				RaiseErrorEvent("Send(): Peer Seems not Connected");
				return;
			}

			// Send
			AsyncCallback sendCb = new AsyncCallback(AsyncSendCallBack);
			this.socket.BeginSend(data, 0, data.Length, SocketFlags.None, sendCb, socket);
		}

		public void Receive() {
			StateObject state = new StateObject();
			state.Sock = this.socket;
			
			this.socket.BeginReceive(state.Buffer, 0, StateObject.BufferSize,
									 SocketFlags.None, 
									 new AsyncCallback(AsyncRecvCallBack), state);
		}

		public void ResetResponse() {
			lock (this.response) {
				this.response = null;
				this.response = new StringBuilder();
			}
		}

		public byte[] GetResponse() {
			return(Encoding.UTF8.GetBytes(this.response.ToString()));
		}

		public string GetResponseString() {
			return(this.response.ToString());
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void AsyncConnectCallBack (IAsyncResult asyncResult) {
			try {
				Socket sock = (Socket) asyncResult.AsyncState;
				sock.EndConnect(asyncResult);
				if (sock.Connected) {
					// Raise Connected Event
					// ======================================================
					if (Connected != null)
						Connected(this, new PeerEventArgs(PeerEvent.Connected));
					// ======================================================
				} else {
					Disconnect();
				}
			} catch (Exception e) {
				RaiseErrorEvent("AsyncConnect(): " + e.Message);
				Disconnect();
			} finally {
			}
		}

		private void AsyncSendCallBack (IAsyncResult asyncResult) {
			try {
				Socket sock;
				lock (this.socket) {
					sock = (Socket) asyncResult.AsyncState;
					sock.EndSend(asyncResult);
				}

				// Raise Sended Event
				// ======================================================
				if (Sended != null)
					Sended(this, new PeerEventArgs(PeerEvent.Sended));
				// ======================================================
			} catch (Exception e) {
				RaiseErrorEvent("AsyncSend(): " + e.Message);
				Disconnect();
			} finally {
			}
		}

		private void AsyncRecvCallBack (IAsyncResult asyncResult) {
			try {
				StateObject state;
				int byteSend;
				Socket sock;
				lock (this.socket) {
					state = (StateObject) asyncResult.AsyncState;
					sock = state.Sock;
					byteSend = sock.EndReceive(asyncResult);
				}

				if (byteSend > 0) {
					response.Append(Encoding.UTF8.GetString(state.Buffer, 0, byteSend));
					
					// Raise Received Event
					// ======================================================
					try {
						if (Received != null)
							Received(this, new PeerEventArgs(PeerEvent.Received, response));
					} catch (Exception e) {
						Debug.Log("[ !! ] PeerSocket.AsyncRecv Received Handler: {0}", e.Message);
					}
					// ======================================================

					AsyncCallback recvCb = new AsyncCallback(AsyncRecvCallBack);
					this.socket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 
											 SocketFlags.None, recvCb, state);
				} else {
					Disconnect();
				}
			} catch (System.NullReferenceException) {
				return;
			} catch (System.ObjectDisposedException) {
				return;
			} catch (Exception e) {
				RaiseErrorEvent("AsyncRecv(): " + e.Message);
				Disconnect();
			} finally {
			}
		}

		// ============================================
		// PROTECTED Methods
		// ============================================
		protected void RaiseErrorEvent (string message) {
			Debug.Log("Socket Error: {0}", message);
			// Raise Error Event
			// ======================================================
			if (Error != null)
				Error(this, new PeerEventArgs(PeerEvent.Error, message));
			// ======================================================
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		public StringBuilder Response {
			get { return(this.response); }
		}

		public object Info {
			set { this.info = value; }
			get { return(this.info); }
		}
		
		public Socket Sock {
			get { return(this.socket); }
		}
	}
}
