/* [ GUI/Gtk2/BandwidthGraph.cs ] Gtk 2.x Bandwidth Graph
 * Author: Matteo Bertozzi
 * ============================================================================
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

using Gtk;
using Gdk;
using Pango;
using System;

using Niry;
using Niry.Utils;

namespace Niry.GUI.Gtk2 {
	public class BandwidthGraph : Gtk.DrawingArea {
		// ============================================
		// PRIVATE Members
		// ============================================
		private Gdk.Color downloadColor;
		private Gdk.Color borderColor;
		private Gdk.Color uploadColor;
		private Gdk.Color textColor;
		private float secInterval;
		private int maxByteSpeed;
		private float secTotal;

		private int[] dw_speeds;
		private int[] up_speeds;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public BandwidthGraph (float secTotal, float secInterval) {
			this.secTotal = secTotal;
			this.secInterval = secInterval;

			int blk = (int) (this.secTotal/this.secInterval);
			this.dw_speeds = new int[blk];
			this.up_speeds = new int[blk];

			this.maxByteSpeed = 1;
			this.downloadColor = new Gdk.Color(0x6a, 0xcd, 0x5a);
			this.borderColor = new Gdk.Color(0x80, 0x80, 0x80);
			this.uploadColor = new Gdk.Color(0x6a, 0x5a, 0xcd);
			this.textColor = new Gdk.Color(0xD3, 0xD3, 0xD3);

			this.Realized += new EventHandler(OnRealized);
			this.ExposeEvent += new ExposeEventHandler(OnExposed);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void Update (int[] download, int[] upload) {
			// Shift Downloads Speeds
			int old_val_length = dw_speeds.Length - download.Length;
			Array.Copy(dw_speeds, download.Length, dw_speeds, 0, old_val_length);
			Array.Copy(download, 0, dw_speeds, old_val_length, download.Length);

			// Shift Uploads Speeds
			old_val_length = up_speeds.Length - upload.Length;
			Array.Copy(up_speeds, upload.Length, up_speeds, 0, old_val_length);
			Array.Copy(upload, 0, up_speeds, old_val_length, upload.Length);

			// Search Max Speed
			maxByteSpeed = dw_speeds[0];
			for (int i=1; i < dw_speeds.Length; i++)
				if (dw_speeds[i] > maxByteSpeed) maxByteSpeed = dw_speeds[i];
			for (int i=0; i < up_speeds.Length; i++)
				if (up_speeds[i] > maxByteSpeed) maxByteSpeed = up_speeds[i];
			if (maxByteSpeed == 0) maxByteSpeed = 1;
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		// Setup Graph Border
		private void DrawGraphBorder() {
			int width  = Allocation.Width - 70;
			int height = Allocation.Height - 50;

			Gdk.GC gc = Style.TextGC(StateType.Normal);
			gc.RgbFgColor = this.borderColor;
			GdkWindow.DrawRectangle(gc, false, 10, 10, width, height);

			int ystep = (height / 4);
			for (int i=0; i < 4; i++) {
				int y = (ystep * i) + 10;
				GdkWindow.DrawLine(gc, 10, y, width + 10, y);
			}
		}

		// Setup Graph Speed
		private void DrawGraphSpeed() {
			int width  = Allocation.Width - 55;
			int height = Allocation.Height - 50;

			Gdk.GC gc = Style.TextGC(StateType.Normal);

			// Setup 0 Speed
			Pango.Layout layout = new Pango.Layout(this.PangoContext);			
			layout.FontDescription = FontDescription.FromString("Tahoma 8");
			layout.SetMarkup("0 Kb/s");
			gc.RgbFgColor = this.textColor;
			GdkWindow.DrawLayout(gc, width, height, layout);

			// Setup Max Speed
			layout = new Pango.Layout(this.PangoContext);			
			layout.FontDescription = FontDescription.FromString("Tahoma 8");
			layout.SetMarkup(FileUtils.GetSizeString(this.maxByteSpeed) + "/s");
			gc.RgbFgColor = this.textColor;
			GdkWindow.DrawLayout(gc, width, 10, layout);
		}			

		// Draw Graph Legend
		private void DrawGraphLegend() {
			int width  = Allocation.Width - 70;
			int height = Allocation.Height - 20;

			Gdk.GC gc = Style.TextGC(StateType.Normal);

			// Setup Download Rate Graph Notes
			gc = Style.TextGC(StateType.Normal);
			gc.RgbFgColor = this.downloadColor;
			GdkWindow.DrawRectangle(gc, true, 20, height, 10, 10);

			// Setup Upload Rate Graph Notes
			gc = Style.TextGC(StateType.Normal);
			gc.RgbFgColor = this.uploadColor;
			GdkWindow.DrawRectangle(gc, true, 140, height, 10, 10);

			// Setup Text Color
			gc.RgbFgColor = this.textColor;

			// Setup Download Rate Text Graph Notes
			Pango.Layout layout = new Pango.Layout(this.PangoContext);			
			layout.FontDescription = FontDescription.FromString("Tahoma 8");
			layout.SetMarkup("Download Rate");
			GdkWindow.DrawLayout(gc, 35, height - 1, layout);

			// Setup Upload Rate Text Graph Notes
			layout = new Pango.Layout(this.PangoContext);			
			layout.FontDescription = FontDescription.FromString("Tahoma 8");
			layout.SetMarkup("Upload Rate");
			GdkWindow.DrawLayout(gc, 155, height - 1, layout);

			// Set Intervals Graph Notes
			layout = new Pango.Layout(this.PangoContext);
			layout.Alignment = Pango.Alignment.Center;
			layout.FontDescription = FontDescription.FromString("Tahoma 8");
			layout.SetMarkup(secTotal.ToString() + " Seconds, "+ secInterval.ToString() +" seconds Interval");
			GdkWindow.DrawLayout(gc, (width / 3) + 15, height - 15, layout);
		}

		private void DrawGraph() {
			int width  = Allocation.Width - 70;
			int height = Allocation.Height - 50;
			int x_step = width / dw_speeds.Length;

			Gdk.GC gc = Style.TextGC(StateType.Normal);

			gc.RgbFgColor = this.downloadColor;
			for (int i=0; i < dw_speeds.Length; i++) {
				if (dw_speeds[i] <= 0) continue;

				int x_start = (i * x_step) + 11;

				int y0 = height - (height/((maxByteSpeed + 1)-dw_speeds[i])) + 10;
				int y1 = y0;
				if (i < (dw_speeds.Length - 1)) {
					y1 = height - (height/((maxByteSpeed + 1)-dw_speeds[i + 1])) + 10;
				}
				
				GdkWindow.DrawLine(gc, x_start, y0, x_start + x_step, y1);
			}

			gc.RgbFgColor = this.uploadColor;
			for (int i=0; i < up_speeds.Length - 1; i++) {
				if (up_speeds[i] <= 0) continue;
				
				int x_start = (i * x_step) + 11;
				int y0 = height - (height/((maxByteSpeed + 1)-up_speeds[i])) + 10;
				int y1 = height - (height/((maxByteSpeed + 1)-up_speeds[i + 1])) + 10;
				GdkWindow.DrawLine(gc, x_start, y0, x_start + x_step, y1);
			}
		}

		// ============================================
		// PROTECTED Methods
		// ============================================
		protected void OnExposed (object obj, ExposeEventArgs args) {
			DrawGraphBorder();
			DrawGraphSpeed();
			DrawGraphLegend();
			DrawGraph();
			this.QueueDraw();
		}

		protected void OnRealized (object obj, EventArgs args) {
			// Setup Network Graph Background
			Gdk.Color bgcolor = new Gdk.Color(0x00, 0x20, 0x00);
			ModifyBg(StateType.Normal, bgcolor);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		public Gdk.Color DownloadColor {
			get { return(this.downloadColor); }
			set { this.downloadColor = value; }
		}

		public Gdk.Color BorderColor {
			get { return(this.borderColor); }
			set { this.borderColor = value; }
		}

		public Gdk.Color UploadColor {
			get { return(this.uploadColor); }
			set { this.uploadColor = value; }
		}

		public Gdk.Color TextColor {
			get { return(this.textColor); }
			set { this.textColor = value; }
		}

		public float SecondsInterval {
			get { return(this.secInterval); }
			set { this.secInterval = value; }
		}

		public float TotalSeconds {
			get { return(this.secTotal); }
			set { this.secTotal = value; }
		}
	}
}
