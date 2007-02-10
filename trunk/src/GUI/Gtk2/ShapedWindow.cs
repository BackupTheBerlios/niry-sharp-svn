/* [ GUI/Gtk2/ShapedWindow.cs ] - Shaped Window
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

using Gtk;
using System;

namespace Niry.GUI.Gtk2 {
	public class ShapedWindow : Gtk.Window {
		// ============================================
		// PRIVATE Members
		// ============================================
		private Gdk.Pixmap maskPixmapWindow;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public ShapedWindow() : base(WindowType.Toplevel) {
		}

		public ShapedWindow (WindowType type) : base(type) {
		}

		public ShapedWindow (string fileName) : base(WindowType.Toplevel) {
			InitializeWindow(new Gdk.Pixbuf(fileName));
		}

		public ShapedWindow (Gdk.Pixbuf pixbuf) : base(WindowType.Toplevel) {
			InitializeWindow(pixbuf);
		}

		public ShapedWindow (Gdk.Pixbuf pixbuf, WindowType type) : base(type) {
			InitializeWindow(pixbuf);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void Setup (string fileName) {
			InitializeWindow(new Gdk.Pixbuf(fileName));
		}

		public void Setup (Gdk.Pixbuf pixbuf) {
			InitializeWindow(pixbuf);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void InitializeWindow (Gdk.Pixbuf pixbuf) {
			// Realize Window
			this.Realize();

			// Set Window Options
			this.Decorated = false;
			this.Resizable = false;
			this.AppPaintable = true;
			this.SetSizeRequest(pixbuf.Width, pixbuf.Height);
			this.SetDefaultSize(pixbuf.Width, pixbuf.Height);

			// Initialize GUI Shape
			Gdk.Pixmap maskBitmapWindow;
			pixbuf.RenderPixmapAndMask (out this.maskPixmapWindow, 
										out maskBitmapWindow, 128);
			this.Events = Gdk.EventMask.AllEventsMask;
			this.ShapeCombineMask(maskBitmapWindow, 0, 0);

			// Set Back Pixmap
			GdkWindow.SetBackPixmap(this.maskPixmapWindow, false);
			GdkWindow.Clear();

			// Window Expose Event
			this.ExposeEvent += new ExposeEventHandler(OnWindowExpose);
		}

		// ============================================
		// PRIVATE (Methods) Event Handler
		// ============================================
		private void OnWindowExpose (object sender, ExposeEventArgs args) {
			Gdk.Window gdkWindow = ((Gtk.Window) sender).GdkWindow;
			gdkWindow.SetBackPixmap(this.maskPixmapWindow, false);
			QueueDraw();
			args.RetVal = false;
		}
	}
}
