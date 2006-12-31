/* [ GUI/Gtk2/CellRendererBitArray.cs ] Gtk2 Bit Array Cell Renderer
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
using GLib;

using System;
using System.Collections;

namespace Niry.GUI.Gtk2 {
	public class CellRendererBitArray : Gtk.CellRenderer {
		// ============================================
		// PRIVATE (Const) Members
		// ============================================
		private const int MIN_CELL_HEIGHT = 10;
		private const int MIN_CELL_WIDTH = 100;

		// ============================================
		// PRIVATE Members
		// ============================================
		private BitArray bitArray;
		private uint firstBit;

		// ============================================
		// PUBLIC Constructos
		// ============================================
		public CellRendererBitArray() : this(1) {
		}

		public CellRendererBitArray (int size) : this(new BitArray(size)) {
		}

		public CellRendererBitArray (BitArray bitArray) {
			this.FirstBit = 0;
			this.BitArray = bitArray;

			this.Xpad = 2;
			this.Ypad = 2;
			this.Mode = Gtk.CellRendererMode.Inert;
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Obtains the width and height needed to render the cell
		public override void GetSize	(Gtk.Widget widget, 
										 ref Gdk.Rectangle cell_area, 
										 out int x_offset, out int y_offset, 
										 out int width, out int height)
		{
			int calc_height;
			int calc_width;

			x_offset = 0;
			y_offset = 0;
			if (!cell_area.Equals(Gdk.Rectangle.Zero)) {
				calc_width  = Math.Max(MIN_CELL_WIDTH, cell_area.Width);
				calc_height = Math.Max(MIN_CELL_HEIGHT, cell_area.Height);

				x_offset = (int) (this.Xalign * (cell_area.Width - calc_width));
				x_offset = Math.Max(x_offset, 0);
			
				y_offset = (int) (this.Yalign * (cell_area.Height - calc_height));
				y_offset = Math.Max (y_offset, 0);
			} else {
				calc_width  = MIN_CELL_WIDTH;
				calc_height = MIN_CELL_HEIGHT;
			}

			width = calc_width;
			height = calc_height;
		}

		// ============================================
		// PROTECTED Methods
		// ============================================
		protected override void Render (Gdk.Drawable window,
										Gtk.Widget widget, 
										Gdk.Rectangle background_area, 
										Gdk.Rectangle cell_area, 
										Gdk.Rectangle expose_area,
										Gtk.CellRendererState flags)
		{
			int x, y, width, height, x_offset, y_offset;

			GetSize(widget, ref cell_area, 
					out x_offset, out y_offset,
					out width, out height);

			x = (int) (cell_area.X + this.Xpad + x_offset);
			y = (int) (cell_area.Y + this.Ypad + y_offset);
			height -= (int) (2 * this.Ypad);
			width  -= (int) (2 * this.Xpad);

			DrawRedGradientRectangle(ref window, x, y, width, height);

			// Return If No Block is Setted
			if (bitArray == null) return;

			// Drow Block
			for (uint i=0; i < width; i++) {
				uint p1 = (uint) ((i * bitArray.Length) / width);
				uint p2 = (uint) (((i + 1) * bitArray.Length) / width);
				if (p2 == p1) p2++;

				uint good = 0;
				for (uint j=p1; j < p2; j++) {
					if (bitArray[(int) (firstBit + j)] == true)
						good++;
				}

				// Draw Block
				if (good > 0) {
					DrawGreenGradientRectangle(ref window, (int)(x + i), y, 1, height);
				}
			}
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void DrawGreenGradientRectangle	(ref Gdk.Drawable window, 
												 int x, int y, 
												 int width, int height)
		{
			int step = (0xFF / height) - 2;

			for (int i=0; i < height; i++) {
				Gdk.GC gc = new Gdk.GC(window);
				byte r = 0;
				byte g = (byte) (0xFF - (i*step));
				byte b = 0;
				gc.RgbFgColor = new Gdk.Color(r, g, b);
				window.DrawRectangle(gc, true, x, y + i, width, 1);
			}
		}

		private void DrawRedGradientRectangle	(ref Gdk.Drawable window, 
												 int x, int y, 
												 int width, int height)
		{
			int step = (0xFF / height) - 3;

			for (int i=0; i < height; i++) {
				Gdk.GC gc = new Gdk.GC(window);
				byte r = (byte) (0xFF - (i*step));
				byte g = 0;
				byte b = 0;
				gc.RgbFgColor = new Gdk.Color(r, g, b);
				window.DrawRectangle(gc, true, x, y + i, width, 1);
			}
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		public BitArray BitArray {
			get { return(this.bitArray); }
			set { this.bitArray = value; }
		}

		public uint FirstBit {
			get { return(this.firstBit); }
			set { this.firstBit = value; }
		}
	}
}
