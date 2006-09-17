/* [ Network/TcpServer.cs ] - Niry TCP Server
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
using System.Collections;
using System.Net.Sockets;
using System.Diagnostics;

namespace Niry.Network {
	public sealed class TcpServer : Server {
		// ===================================
		// PUBLIC Event Handler
		// ===================================
		public static event SocketEventHandler Connected = null;

		// ===================================
		// PRIVATE STATIC Members
		// ===================================
		private static TcpServer instance = null;

		// ===================================
		// PRIVATE Constructors
		// ===================================
		private TcpServer (IPAddress localIP, int port) : base(localIP, port) {
		}
		
		// ===================================
		// PUBLIC STATIC Methods
		// ===================================
		public static void StartListening (int port) {
			instance = new TcpServer(IPAddress.Any, port);
			instance.Start();
		}
		
		public static void StopListening() {
			if (instance != null) {
				instance.Stop();
			}
		}

		// ===================================
		// PROTECTED Methods
		// ===================================
		protected override void ServerStart() {
			// Initialize Listener
			Listener = new Socket(AddressFamily.InterNetwork,
								  SocketType.Stream,
								  ProtocolType.Tcp);
			Listener.Bind(new IPEndPoint(LocalIP, Port));
			Listener.Listen(64);
			
			Listener.BeginAccept(new AsyncCallback(OnConnectRequest), Listener);
		}
		
		// ===================================
		// PRIVATE Methods
		// ===================================
		private void OnConnectRequest (IAsyncResult result) {
			Socket listener = result.AsyncState as Socket;
			Socket client = listener.EndAccept(result);

			// Start User Connection Event
			if (Connected != null) Connected(this, client);

			// Wait Another Client...
			Listener.BeginAccept(new AsyncCallback(OnConnectRequest), Listener);
		}
		// ===================================
		// PUBLIC Properties
		// ===================================
	}
}
