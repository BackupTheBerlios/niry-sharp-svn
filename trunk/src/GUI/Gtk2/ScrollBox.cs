/* [ GUI/Gtk2/ScrollBox.cs ] Gtk 2.x ScrollBox
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

using Gdk;
using Gtk;
using GLib;
using Pango;

using System;

namespace Niry.GUI.Gtk2 {
	public class ScrollBox : Gtk.DrawingArea {
		// ============================================
		// PRIVATE Members
		// ============================================
		internal uint TimeHandle;
		private bool timeoutRet = true;
		private Pango.Layout layout;
		private int scrollPause;
		private int scrollStart;
		private string Text;
		private int textTop;
		private int scroll;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public ScrollBox (string text) {
			this.Text = text;
			InitializeScrollBox(51);
		}

		public ScrollBox (uint timeout, string text) {
			this.Text = text;
			InitializeScrollBox(timeout);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void InitializeScrollBox (uint timeout) {
			this.Realized += new EventHandler(OnRealized);
			this.ExposeEvent += new ExposeEventHandler(OnExposed);
			this.Unrealized += new EventHandler(OnUnrealized);

			TimeHandle = GLib.Timeout.Add(timeout, new TimeoutHandler(ScrollDown));
		}

		private void OnUnrealized (object o, EventArgs args) {
			this.timeoutRet = false;
		}

		private bool ScrollDown() {
			if (scrollPause > 0) {
				if (--scrollPause == 0) scroll++;
			} else {
				scroll++;
			}

			if (this.timeoutRet != false) {
				this.QueueDrawArea (0, 0, this.WidthRequest - 10, 
									this.HeightRequest - 10);
			}
			return(this.timeoutRet);
		}

		private int GetTextHeight() {
			int width, height;
			this.layout.GetPixelSize(out width, out height);
			return(height*2);
		}

		private void DrawText() {
			int maxHeight = GetTextHeight();

			this.GdkWindow.DrawLayout(this.Style.TextGC(StateType.Normal), 0, 
									  textTop - scroll, layout);

			if (scroll == maxHeight && scrollPause == 0) {
				scrollPause = 60;
			} else if (scroll > maxHeight) {
				scroll = scrollStart;
			}
		}

		// ============================================
		// PROTECTED Methods
		// ============================================
		protected void OnExposed (object obj, ExposeEventArgs args) {
			this.DrawText();
		}

		protected void OnRealized (object obj, EventArgs args) {
			layout = new Pango.Layout(this.PangoContext);			
			layout.Wrap = Pango.WrapMode.Word;
			layout.Alignment = Pango.Alignment.Center;
			layout.Width = this.WidthRequest * (int) Pango.Scale.PangoScale;
			layout.FontDescription = FontDescription.FromString("Tahoma 10");
			layout.SetMarkup(Text);			

			int x, y;
			GdkWindow.GetOrigin(out x, out y);
			textTop = y;
			scrollStart = -(this.HeightRequest - textTop);
			scroll = scrollStart;
		}
	}
}
