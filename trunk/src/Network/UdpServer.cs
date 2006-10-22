/* [ Network/UdpServer.cs ] - Niry UDP Server
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
	public sealed class UdpServer : Server {
		// ===================================
		// PUBLIC Event Handler
		// ===================================
		public static event ExceptionEventHandler Error = null;
		public static event StringEventHandler Received = null;

		// ===================================
		// PRIVATE STATIC Members
		// ===================================
		private static UdpServer instance = null;
		
		// ===================================
		// PRIVATE Members
		// ===================================
		private SocketReader reader;
		
		// ===================================
		// PRIVATE Constructors
		// ===================================
		private UdpServer (IPAddress localIP, int port) : base(localIP, port) {
		}
		
		// ===================================
		// PUBLIC STATIC Methods
		// ===================================
		public static void StartListening (int port) {
			instance = new UdpServer(IPAddress.Any, port);
			instance.Start();
		}
		
		public static void StopListening() {
			if (instance != null) {
				instance.Stop();
			}
		}

		public static void Send (string hostname, string message) {
			Send(Util.AddressPart(hostname), Util.PortPart(hostname), message);
		}

		public static void Send (string hostname, int port, string message) {
			IPAddress ip = Dns.GetHostEntry(hostname).AddressList[0];

			// Check Port
			if (port <= 0)
				throw(new ArgumentOutOfRangeException("Port"));

			try {
				Socket socket;
				EndPoint endPoint = new IPEndPoint(ip, port);

				// Initialize Socket
				socket = new Socket(AddressFamily.InterNetwork,
									SocketType.Dgram, 
									ProtocolType.Udp);

				// Connect Socket
				socket.Connect(endPoint);

				socket.Send(Encoding.Default.GetBytes(message));
				//socket.Close();
			} catch (Exception e) {
				Debug.WriteLine(e.Message);
			}
		}

		// ===================================
		// PROTECTED Methods
		// ===================================
		protected override void ServerStart() {
			// Initialize Listener
			Listener = new Socket(AddressFamily.InterNetwork,
								  SocketType.Dgram,
								  ProtocolType.Udp);
			Listener.Bind(new IPEndPoint(LocalIP, Port));
			
			// Initialize Socket Reader
			reader = new SocketReader(Listener);
			reader.Encoding = Encoding.GetEncoding(1252);
			reader.Receive (new StringEventHandler(Receive),
							new ExceptionEventHandler(OnReaderException));
		}
		
		// ===================================
		// PRIVATE Methods
		// ===================================
		private void Receive (object sender, string message) {
			Debug.WriteLine("UdpServer.Received(): '{0}'", message);
			if (Received != null) Received(this, message);
		}

		private void OnReaderException (object sender, Exception e) {
			Debug.WriteLine("UdpServer.ReaderException(): '{0}'", e.Message);
			if (Error != null) Error(this, e);
		}

		// ===================================
		// PUBLIC Properties
		// ===================================
	}
}
