/* [ Network/P2PManager.cs ] - Niry P2P Manager
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
using System.Collections;
using System.Net.Sockets;

using Niry;
using Niry.Utils;

namespace Niry.Network {
	/// P2P Exception
	public class P2PException : Exception {
		/// Create New P2P Exception
		public P2PException (string msg) : base(msg) {}

		/// Create New P2P Exception
		public P2PException (string msg, Exception inner) : base(msg, inner) {}
	}

	/// P2P Manager Singleton Class
	public sealed class P2PManager {
		// ============================================
		// PUBLIC Events
		// ============================================
		/// Event Raised when Peer is Disconnecting
		public static event PeerEventHandler PeerDisconnecting = null;
		/// Event Raised when Peer Connected
		public static event PeerEventHandler PeerConnected = null;
		/// Event Raised when Peer Received Data
		public static event PeerEventHandler PeerReceived = null;
		/// Event Raised when Peer Sending Data
		public static event PeerEventHandler PeerSending = null;
		/// Event Raised when Peer has Sended Data
		public static event PeerEventHandler PeerSended = null;
		/// Event Raised when Peer Error
		public static event PeerEventHandler PeerError = null;

		/// Event Raised when P2P Manager has Changed Status (Online/Offline)
		public static event BoolEventHandler StatusChanged = null;

		// ============================================
		// PUBLIC MEMBERS
		// ============================================
		/// Listen() Backlog
		public static int Backlog = 128;
		/// P2P Port
		public static int Port = 7085;
		
		// ============================================
		// PROTECTED MEMBERS
		// ============================================
		protected ArrayList unknownPeers = null;	// PeerSocket
		protected Hashtable knownPeers = null;		// [UserInfo] = PeerSocket		

		// ============================================
		// PRIVATE MEMBERS
		// ============================================
		private ManualResetEvent allDone = null;
		private Thread serverThread = null;
		private Socket listenSock = null;
		private int currentPort = Port;
		private bool dontRemove = false;

		// Singleton P2PManager
		private static P2PManager p2pManager = null;

		// ============================================
		// PRIVATE Constructors
		// ============================================
		private P2PManager() {
			// Setup Event (None)
			PeerDisconnecting = null;
			PeerConnected = null;
			PeerReceived = null;
			PeerSending = null;
			PeerSended = null;
			PeerError = null;
			StatusChanged = null;

			// Setup Members
			allDone = null;
			serverThread = null;
			listenSock = null;
			dontRemove = false;
			knownPeers = null;
			unknownPeers = null;

			InitializeMembers();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Close P2P and Disconnect all The Peers
		public void Kill() {
			if (p2pManager != null) {
				StopListening();
				p2pManager = null;
			}
		}

		/// Start Listening (P2P Connect)
		public void StartListening() {
			InitializeMembers();

			try {
				IPEndPoint ipLocalEndPoint;
				ipLocalEndPoint = new IPEndPoint(IPAddress.Any, P2PManager.Port);

				// Create Socket & Start Listening
				this.listenSock = new Socket(AddressFamily.InterNetwork, 
									 		 SocketType.Stream,
											 ProtocolType.Tcp);
				this.listenSock.Bind(ipLocalEndPoint);
				this.listenSock.Listen(P2PManager.Backlog);
			} catch (Exception e) {
				throw(new P2PException("P2PManager Listen: " + e.Message));
			}

			// Initialize Accept Thread
			this.serverThread = new Thread(new ThreadStart(AcceptThread));
			this.serverThread.Start();

			// Start P2P 'Status Online' Event
			if (StatusChanged != null) StatusChanged(this, true);
		}

		/// Start Listening (P2P Disconnect)
		public void StopListening() {
			// Destroy Accept Thread
			if (this.serverThread != null) {
				this.serverThread.Abort();
				this.serverThread = null;
			}

			this.dontRemove = true;
			DisconnectAllKnownPeers();
			DisconnectAllUnknownPeers();
			this.dontRemove = false;

			try {
				// Disconnect Listen Sock
				if (this.listenSock != null) {
					this.listenSock.Shutdown(SocketShutdown.Both);
					this.listenSock.Close();
					this.listenSock = null;
				}
			} catch (Exception e) {
				Debug.Log("P2PManager.StopListening(): {0}", e.Message);
			}

			// Manual ResetEvent
			this.allDone = null;			
			this.knownPeers = null;
			this.unknownPeers = null;

			// Start P2P 'Status Offline' Event
			if (StatusChanged != null) StatusChanged(this, false);
		}

		/// Disconnect All Logged Users
		public void DisconnectAllKnownPeers() {
			if (this.knownPeers == null)
				return;

			// Disconnect Known Peers
			foreach (object user in this.knownPeers.Keys) {
				try {
					PeerSocket peer = (PeerSocket) this.knownPeers[user];
					peer.Disconnect();
				} catch {}
			}
			
			// Remove All From Known Peers Hashtable
			this.knownPeers.Clear();
			this.knownPeers = null;
		}

		/// Disconnect All Non-Logged User
		public void DisconnectAllUnknownPeers() {
			if (this.unknownPeers == null)
				return;

			try {
				// Disconnect All Unknown Peers
				foreach (PeerSocket peer in this.unknownPeers) {
					try {
						peer.Disconnect();
					} catch {}
				}
			
				// Remove All From Unknown Peers ArrayList
				this.unknownPeers.Clear();
				this.unknownPeers = null;
			} catch (Exception e) {
				Debug.Log("P2PManager Disconnect Unknown Peers: {0}", e.Message);
			}
		}

		// ============================================
		// PUBLIC STATIC Methods
		// ============================================
		/// Get P2P Manager Instance
		public static P2PManager GetInstance() {
			if (p2pManager == null)
				p2pManager = new P2PManager();
			return(p2pManager);
		}

/* ===============================================
		public static IPAddress GetMyIP() {
			#warning TODO Get My IP
			return(null);
		}
   =============================================== */

		/// Return true if P2P is Listening
		public static bool IsListening() {
			if (p2pManager == null || p2pManager.serverThread == null)
				return(false);
			return(true);
		}

		#region Add Peer
		/// Add Peer
		public static void AddPeer (object user, PeerSocket peer) {
			try {
				lock (p2pManager.knownPeers) {
					peer.Info = user;
					p2pManager.knownPeers.Add(user, peer);
				}
				
				// Remove Peer from Unknown List if is present :)
				lock (p2pManager.unknownPeers) {
					p2pManager.unknownPeers.Remove(peer);
				}
			} catch (ArgumentException) {
				throw(new P2PException("User Already In"));
			}
		}

		/// Add Peer by Name
		public static void AddPeer (object user, string ip, int port) {
			PeerSocket peer = PeerSocket.GetPeer(ip, port);

			// Add Default Peer Event Handlers
			p2pManager.AddDefaultEventHandler(ref peer, true);

			// Start Peer Receive (Call Receive() Only Once)
			peer.Receive();

			// Add Peer To Known List
			AddPeer(user, peer);
		}
		#endregion

		#region Remove Peer
		/// Remove All Peer
		public static void RemoveAllPeer() {
			p2pManager.dontRemove = true;
			p2pManager.DisconnectAllKnownPeers();
			p2pManager.DisconnectAllUnknownPeers();
			p2pManager.dontRemove = false;
		}

		/// Remove Peer by Name
		public static void RemovePeer (object user) {
			if (user == null) return;
			if (p2pManager.knownPeers == null) return;

			lock (p2pManager.knownPeers) {
				PeerSocket peer = (PeerSocket) p2pManager.knownPeers[user];
				if (peer != null) {
					peer.Disconnect();					// Disconnect Peer
					p2pManager.DelDefaultEventHandler(ref peer);
					p2pManager.knownPeers.Remove(user);	// Remove Peer
				}
			}
		}

		/// Remove Peer
		public static void RemovePeer (PeerSocket peer) {
			if (peer == null) return;
			if (p2pManager.unknownPeers == null) return;

			lock (p2pManager.unknownPeers) {				
				peer.Disconnect();						// Disconnect Peer
				p2pManager.DelDefaultEventHandler(ref peer);
				p2pManager.unknownPeers.Remove(peer);	// Remove Unknown Peer
			}
		}
		#endregion
	
		#region Search User/Peer
		public static object ContainsUser (object user) {
			if (p2pManager.knownPeers != null) {
				lock (p2pManager.knownPeers) {
					foreach (object info in p2pManager.knownPeers.Keys) {
						IComparable obj = info as IComparable;
						if (obj != null) {
							if (obj.CompareTo(user) == 0) return(info);
						} else {
							if (info == user) return(info);
						}
					}
				}
			}
			return(null);
		}
		#endregion

		#region Send Data
		/// Send String To Named Peer
		public static bool SendToPeer (object user, string data) {
			return(SendToPeer(user, Encoding.UTF8.GetBytes(data)));
		}
		
		/// Send Data To Named Peer
		public static bool SendToPeer (object user, byte[] data) {
			lock (p2pManager.knownPeers) {
				PeerSocket peer = (PeerSocket) p2pManager.knownPeers[user];
				if (peer == null) return(false);
				peer.Send(data);
			}
			return(true);
		}

		/// Send String To All Peer
		public static void SendToAllPeer (string data) {
			SendToAllPeer(Encoding.UTF8.GetBytes(data));
		}
		
		/// Send Data To All Peer
		public static void SendToAllPeer (byte[] data) {
			SendToKnownPeers(data);
			SendToUnknownPeers(data);
		}
		
		/// Send String To Non-Logged Peer
		public static void SendToKnownPeers (string data) {
			SendToKnownPeers(Encoding.UTF8.GetBytes(data));
		}

		/// Send Data To Non-Logged Peer
		public static void SendToKnownPeers (byte[] data) {
			lock (p2pManager.knownPeers) {
				foreach (object user in p2pManager.knownPeers.Keys) {
					PeerSocket peer = (PeerSocket) p2pManager.knownPeers[user];
					peer.Send(data);
				}
			}
		}
		
		/// Send String To All Non-Logged Peer
		public static void SendToUnknownPeers (string data) {
			SendToUnknownPeers(Encoding.UTF8.GetBytes(data));
		}

		/// Send Data To All Non-Logged Peer		
		public static void SendToUnknownPeers (byte[] data) {
			lock (p2pManager.unknownPeers) {
				foreach (PeerSocket peer in p2pManager.unknownPeers)
					peer.Send(data);
			}
		}
		#endregion
		
		// ============================================
		// PRIVATE Methods
		// ============================================
		private void AcceptThread() {
			Thread.Sleep(512);
			try {
				// Accept Loop
				while (true) {
					this.allDone.Reset();
					AsyncCallback acceptCb = new AsyncCallback(AsyncAcceptCallback);
					this.listenSock.BeginAccept(acceptCb, this.listenSock);
					this.allDone.WaitOne();
				}
			} catch (ThreadAbortException) {
				// Do Nothing, Kill P2PManager
				return;
			} catch (Exception e) {
				throw(new P2PException("P2PManager Accept: " + e.Message));
			}
		}
		
		private void AsyncAcceptCallback (IAsyncResult asyncResult) {
			try {
				this.allDone.Set();
				
				// Get Peer Socket From Listener
				Socket listener = (Socket) asyncResult.AsyncState;
				Socket sock = (Socket) listener.EndAccept(asyncResult);
				PeerSocket peer = new PeerSocket(sock);

				Debug.Log("[ -- ] Accepted: {0}", peer.GetRemoteIP().ToString());
				
				// Add Peer To Unknown List
				this.unknownPeers.Add(peer);
				
				// Add Default Peer Event Handler
				AddDefaultEventHandler(ref peer, true);
				
				// Start Peer Receive (Call Receive() Only Once)
				peer.Receive();
			} catch (NullReferenceException) {
				return;
			} catch (Exception e) {
				Debug.Log("P2PManager Async Accept: {0}", e.Message);
				throw(new P2PException("P2PManager Accept: " + e.Message));
			}
		}

		private void InitializeMembers() {
			// Initialize Current Port
			this.currentPort = Port;

			// Create Thread Safe Hashtable
			if (this.knownPeers == null)
				this.knownPeers = Hashtable.Synchronized(new Hashtable());

			// Create Thread Safe ArrayList
			if (this.unknownPeers == null)
				this.unknownPeers = ArrayList.Synchronized(new ArrayList());

			// Initialize Manual Reset Event
			if (this.allDone == null)
				this.allDone = new ManualResetEvent(false);
		}

		private void AddDefaultEventHandler (ref PeerSocket peer, bool connected) {
			if (connected == true) {
				if (PeerConnected != null)
					PeerConnected(peer, new PeerEventArgs(PeerEvent.Connected));
			} else {
				peer.Connected += new PeerEventHandler(OnPeerConnected);
			}			
		
			peer.Disconnecting += new PeerEventHandler(OnPeerDisconnecting);
			peer.Connected += new PeerEventHandler(OnPeerConnected);
			peer.Received += new PeerEventHandler(OnPeerReceived);
			peer.Sending += new PeerEventHandler(OnPeerSending);
			peer.Sended += new PeerEventHandler(OnPeerSended);
			peer.Error += new PeerEventHandler(OnPeerError);
		}

		private void DelDefaultEventHandler (ref PeerSocket peer) {
			peer.Disconnecting -= new PeerEventHandler(OnPeerDisconnecting);
			peer.Connected -= new PeerEventHandler(OnPeerConnected);
			peer.Received -= new PeerEventHandler(OnPeerReceived);
			peer.Sending -= new PeerEventHandler(OnPeerSending);
			peer.Sended -= new PeerEventHandler(OnPeerSended);
			peer.Error -= new PeerEventHandler(OnPeerError);
		}
		
		private void OnPeerDisconnecting (object sender, PeerEventArgs args) {
			if (PeerDisconnecting != null) PeerDisconnecting(sender, args);

			// Remove Only if Authorized
			if (dontRemove == true) return;

			// Remove Peer
			PeerSocket peer = sender as PeerSocket;
			if (peer.Info != null) {
				// Remove Known Peer
				lock (p2pManager.knownPeers) {
					p2pManager.knownPeers.Remove(peer.Info);
				}
			} else {
				// Remove Unknown Peer
				lock (this.unknownPeers) {
					this.unknownPeers.Remove(peer);
				}
			}
		}
		
		private void OnPeerConnected (object sender, PeerEventArgs args) {
			if (PeerConnected != null) PeerConnected(sender, args);
		}
		
		private void OnPeerReceived (object sender, PeerEventArgs args) {
			if (PeerReceived != null) PeerReceived(sender, args);
		}
		
		private void OnPeerSending (object sender, PeerEventArgs args) {
			if (PeerSending != null) PeerSending(sender, args);
		}

		private void OnPeerSended (object sender, PeerEventArgs args) {
			if (PeerSended != null) PeerSended(sender, args);
		}
		
		private void OnPeerError (object sender, PeerEventArgs args) {
			if (PeerError != null) PeerError(sender, args);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get Logged Peer ArrayList, Value are PeerSocket
		public static ArrayList UnknownPeers {
			get {
				if (p2pManager == null)
					return(null);
				return(p2pManager.unknownPeers);
			}
		}

		/// Get Logged Peer Hashtable, Key are Name, Value are PeerSocket
		public static Hashtable KnownPeers {
			get {
				if (p2pManager == null)
					return(null);
				return(p2pManager.knownPeers);
			}
		}

		/// Get Current Listening Port
		public int CurrentPort {
			get { return(this.currentPort); }
		}
	}
}
