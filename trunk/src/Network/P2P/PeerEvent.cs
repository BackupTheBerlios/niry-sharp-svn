/* [ Network/PeerEvent.cs ] - Niry Peer Event
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
using System.Text;

namespace Niry.Network {
	public delegate void PeerEventHandler (object sender, PeerEventArgs args);

	public enum PeerEvent {
		Disconnecting,
		Disconnected, 
		Connected, 
		Received, 
		Sending, 
		Sended, 
		Error
	};

	public class PeerEventArgs : EventArgs {
		// ============================================
		// PRIVATE Members
		// ============================================
		private PeerEvent eventType;
		private string message;
		private byte[] data;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public PeerEventArgs (PeerEvent type) {
			this.eventType = type;
			this.data = null;
		}

		public PeerEventArgs (PeerEvent type, byte[] data) {
			this.eventType = type;
			this.data = data;
		}

		public PeerEventArgs (PeerEvent type, string data) {
			this.eventType = type;
			this.message = data;
		}

		public PeerEventArgs (PeerEvent type, StringBuilder data) {
			this.eventType = type;
			this.message = data.ToString();
			this.data = Encoding.UTF8.GetBytes(this.message);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		public PeerEvent EventType {
			get { return(this.eventType); }
		}

		public byte[] Data {
			get { return(this.data); }
		}

		public string Message {
			get { return(this.message); }
		}
	}
}
