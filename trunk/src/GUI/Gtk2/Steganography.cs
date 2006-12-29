/* [ GUI/Gtk2/Steganography.cs ] 
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
using System.Text;

namespace Niry.GUI.Gtk2 {
	public class Steganography {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================
		protected Gdk.Image pixmap = null;
		protected Gtk.Image image = null;

		// ============================================
		// PRIVATE Members
		// ============================================

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public Steganography (string filename) : this(new Gtk.Image(filename))
		{
		}

		public Steganography (Gtk.Image image) {
			this.image = image;
			pixmap = new Gdk.Image (Gdk.ImageType.Normal, image.Visual, 
									image.Pixbuf.Width, image.Pixbuf.Height);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void AppendText (string text) {
		}

		public bool SaveImage (string filename, string type) {
			Gdk.Pixbuf pixbuf = image.Pixbuf.Copy();
			pixbuf.GetFromImage(pixmap, Gdk.Colormap.System, 0, 0, 0, 0, 
									  pixmap.Width, pixmap.Height);
			return(pixbuf.Save(filename, type));
		}

		// ============================================
		// PROTECTED Methods
		// ============================================
		public uint GetPixel (int x, int y) {
			return(this.pixmap.GetPixel(x, y));
		}

		public void PutPixel (int x, int y, uint color) {
			this.pixmap.PutPixel(x, y, color);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		// Write Message Length at the Begin of the Image
		private void WriteLength() {
		}

		// Write Message into Image
		private void WriteMessage() {
		}

		// Write Steganography Signature at the End of the Image
		private void WriteSign() {
		}

		// Read Message Length at the Begin of the Image
		private void ReadLength() {
		}

		// Read Message into Image
		private void ReadMessage() {
		}

		// Read Steganography Signature at the End of the Image
		private void ReadeSign() {
		}

		// ============================================
		// PROTECTED Properties
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================

		public static void Main() {
			Gtk.Application.Init();
			Steganography stego = new Steganography("prova.png");
			stego.SaveImage("prova-stego.png", "png");
		}
	}
}
