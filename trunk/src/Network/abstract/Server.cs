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
using System.Threading;
using System.Net.Sockets;
using System.Diagnostics;

namespace Niry.Network {
	public abstract class Server {
		// ===================================
		// PRIVATE Members
		// ===================================
		private readonly object syncRoot;
		private readonly Thread thread;
		private Socket socketListener;
		private IPAddress localIP;
		private bool started;
		private int port;
		
		// ===================================
		// PROTECTED Constructors
		// ===================================
		protected Server (IPAddress localIP, int port) {
			Debug.Assert(port > 0);
			Debug.Assert(localIP != null);
		
			// Check Port Range
			if (port > IPEndPoint.MaxPort) {
				throw(new ArgumentOutOfRangeException("Port"));
			}
			
			// Initialize IPAddress & Port
			this.localIP = localIP;
			this.port = port;
			
			// Initialize SyncRoot
			this.syncRoot = new object();
			
			// Initialize Thread 'ServerStart'
			this.thread = new Thread(new ThreadStart(ServerStart));
			this.thread.Name = "Server Listener";
		}

		// ===================================
		// PUBLIC VIRTUAL Methods
		// ===================================
		public virtual void Start() {
			lock (this.syncRoot) {
				if (this.started == false) {
					this.started = true;
					this.thread.Start();
				}
			}
		}
		
		public virtual void Stop() {
			lock (this.syncRoot) {
				if (this.started == false) return;
				
				// Shutdown Server Socket Listener
				this.started = false;
				if (this.socketListener != null && this.socketListener.Connected) {
					this.socketListener.Shutdown(SocketShutdown.Both);
					this.socketListener.Close();
				}
				
				// Stop Server Thread
				if (this.thread.IsAlive) {
					this.thread.Abort();
				}
			}
		}
		
		// ===================================
		// PROTECTED ABSTRACT Methods
		// ===================================
		protected abstract void ServerStart();
		
		// ===================================
		// PUBLIC Properties
		// ===================================
		public IPAddress LocalIP {
			get { return(this.localIP); }
		}
		
		public int Port {
			get { return(this.port); }
		}
		
		public Socket Listener {
			get { return(this.socketListener); }
			protected set { this.socketListener = value; }
		}
		
		public bool IsStarted {
			get { return(this.started); }
		}
	}
}
