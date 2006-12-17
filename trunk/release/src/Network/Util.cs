/* [ Network/Util.cs ] - Niry Network Utils
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
using System.Text.RegularExpressions;

namespace Niry.Network {
	public static class Util {
		// ===================================
		// PRIVATE STATIC Members
		// ===================================
		private static string localIP;

		// ===================================
		// PUBLIC STATIC Methods
		// ===================================
		public static string AddressPart (string hostname) {
			int i = hostname.IndexOf(':');
			if (i > 0) {
				hostname = hostname.Substring(0, i);
			}
			hostname = Regex.Replace(hostname, @"\s+", "");
			return(hostname);
		}

		public static int PortPart (string hostname) {
			int i = hostname.LastIndexOf(':');
			if (i > 0 && i < hostname.Length - 1) {
				string port = hostname.Substring(i + 1);
				port = Regex.Match(port, @"\d+").Value;
				return(int.Parse(port));
			}
			return(-1);
		}

		public static string FixHostname (string hostname) {
			string address = AddressPart(hostname);
			int port = PortPart(hostname);
			return((port > 0) ? address + ':' + port.ToString() : address);
		}

		public static string GetLocalIPAddress() {
			if(localIP == null) {
				IPHostEntry iphostentry = Dns.GetHostEntry(Dns.GetHostName());
				localIP = iphostentry.AddressList[0].ToString();
			}
			return(localIP);
		}

		public static IPAddress GetRemoteIP (Socket socket) {
			IPEndPoint ipEndPoint = (IPEndPoint) socket.RemoteEndPoint;
			return(ipEndPoint.Address);
		}

		public static IPEndPoint Resolve (string address, int port) {
			IPAddress ip = Dns.GetHostEntry(address).AddressList[0];
			return(new IPEndPoint(ip, port));
		}
	}
}
