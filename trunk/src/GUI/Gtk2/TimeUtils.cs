/* [ GUI/Gtk2/TimeUtils.cs ] 
 * Author: Matteo Bertozzi
 * ============================================================================
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

namespace Niry.GUI.Gtk2 {
	internal class Sleep {
		private GLib.TimeoutHandler handler;
		private bool sleepMoveon;
		
		public Sleep () {
			this.handler = new GLib.TimeoutHandler(SleepStop);
			this.sleepMoveon = false;
		}
		
		public void Start (uint msec) {
			GLib.Timeout.Add(msec, this.handler);
			while (this.sleepMoveon != true) {
				while (Gtk.Application.EventsPending() == true) {
					Gtk.Application.RunIteration();
				}
			}
		}
		
		private bool SleepStop () {
			this.sleepMoveon = true;
			return(false);
		}
	}
	
	public static class TimeUtils {
		public static void Sleep (uint msec) {
			Sleep sleep = new Sleep();
			sleep.Start(msec);
		}
	}
}
