/* [ Network/NetMonitor.cs ] - Niry Network Monitor
 * Author: Matteo Bertozzi
 * =============================================================================
 * Niry Sharp
 * Copyright (C) 2007 Matteo Bertozzi.
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

namespace Niry.Network {
	public class NetMonitor {
		// ============================================
		// PRIVATE Const
		// ============================================
		private const int SpeedStoreLength = 12;

		// ============================================
		// PRIVATE Members
		// ============================================
		private int lastUpdateTime;
		private double[] dwSpeeds;
		private double[] upSpeeds;
		private int dwSpeedIndex;
		private int upSpeedIndex;
		private int received;
		private int dwSpeed;
		private int upSpeed;
		private int sended;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public NetMonitor() {
			this.sended = this.received = 0;
			this.dwSpeedIndex = this.upSpeedIndex = 0;
			this.upSpeeds = new double[SpeedStoreLength];
			this.dwSpeeds = new double[SpeedStoreLength];
			this.lastUpdateTime = Environment.TickCount;
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void UpdateStats() {
			int currentTime = Environment.TickCount;
			int difference = currentTime - lastUpdateTime;

			if (difference <= 0) {
				difference = 1000;
			} else if (difference < 500) {
				return;
			}

			double t = ((double) difference / 1000.0);
			this.upSpeeds[this.upSpeedIndex++] = this.sended / t;
			this.dwSpeeds[this.dwSpeedIndex++] = this.received / t;

			if (this.dwSpeedIndex == SpeedStoreLength) this.dwSpeedIndex = 0;
			if (this.upSpeedIndex == SpeedStoreLength) this.upSpeedIndex = 0;

			int total = 0;
			int count = 0;

			// Get Download Speed
			foreach (int speed in this.dwSpeeds) {
				if (speed != 0) {
					total += speed;
					count++;
				}
			}
			if (count == 0) count = 1;
			this.dwSpeed = total / count;

			// Get Upload Speed
			total = count = 0;
			foreach (int speed in this.upSpeeds) {
				if (speed != 0) {
					total += speed;
					count++;
				}
			}
			if (count == 0) count = 1;
			this.upSpeed = total / count;

			// ReSetup Variables
			this.sended = this.received = 0;
			this.lastUpdateTime = currentTime;
		}

		public void UpdateBytesSended (int bytesUploaded) {
			this.sended = bytesUploaded;
		}

		public void UpdateBytesReceived (int bytesReceived) {
			this.received = bytesReceived;
		}

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
		public int DownloadSpeed {
			get { return(this.dwSpeed); }
		}

		public int UploadSpeed {
			get { return(this.upSpeed); }
		}

		public int LastUpdateTime {
			get { return(this.lastUpdateTime); }
		}
	}
}
