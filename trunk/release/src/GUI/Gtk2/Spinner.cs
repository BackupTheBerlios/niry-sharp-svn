/* [ GUI/Gtk2/Spinner.cs ] - Niry Gtk2 Spinner
 * Author: Christian Hergert <christian.hergert@gmail.com>
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

using Gtk;
using Cairo;
using System;

namespace Niry.GUI.Gtk2 {
	/// Gtk2/Cairo Spinner
	public class Spinner : DrawingArea {
		// ============================================
		// PRIVATE Members
		// ============================================
		private bool running = false;
		private int current = 0;
		private int lines = 8;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create a New Spinner
		public Spinner() {
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Start The Spinner Motion
		public void Start() {
			this.running = true;
			GLib.Timeout.Add(100, ExposeTimeoutHandler);
		}

		/// Stop The Spinner Motion
		public void Stop() {
			this.running = false;
		}

		// ============================================
		// PROTECTED Methods
		// ============================================
		protected override bool OnExposeEvent (Gdk.EventExpose evnt) {
			using (Context cr = Gdk.CairoHelper.Create (this.GdkWindow)) {
				// Set Clip Region
				cr.Rectangle(evnt.Area.X, evnt.Area.Y, 
							 evnt.Area.Width, evnt.Area.Height);
				cr.Clip();

				// Draw Widget
				this.Draw(cr);
			}
			return(false);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private bool ExposeTimeoutHandler() {
			if (this.current + 1 > this.lines) {
				this.current = 0;
			} else {
				this.current++;
			}
			this.QueueDraw();
			return(this.running);
		}

		private void Draw (Context cr) {
			double radius;
			double half;
			double x, y;

			x = this.Allocation.X + this.Allocation.Width / 2;
			y = this.Allocation.Y + this.Allocation.Height / 2;
			radius = Math.Min(this.Allocation.Width / 2, 
							  this.Allocation.Height / 2) - 5;
			half = lines / 2;

			for (int i=0; i < lines; i++) {
				double t = (double) ((i + lines - current) % lines) / lines;
				double inset = 0.7 * radius;

				cr.Save();

				cr.Color = new Cairo.Color(0, 0, 0, t);
				cr.LineWidth *= 2;
				cr.MoveTo(x + (radius - inset) * Math.Cos (i * Math.PI/half),
						  y + (radius - inset) * Math.Sin (i * Math.PI/half));
				cr.LineTo(x + radius * Math.Cos (i * Math.PI / half),
						  y + radius * Math.Sin (i * Math.PI / half));
				cr.Stroke();

				cr.Restore();
			}
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Get or Set Spinner Line Numbers
		public int Lines {
			get { return(this.lines); }
			set {
				this.lines = value;
				this.QueueDraw();
			}
		}

		/// Return Boolean That indicate if Spinner is Running
		public bool IsRunning {
			get { return(this.running); }
		}
	}
}
