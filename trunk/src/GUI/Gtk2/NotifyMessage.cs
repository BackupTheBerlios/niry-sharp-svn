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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Niry.GUI.Gtk2 {
	internal class NotifyArea {
		public enum BeakPos {TopLeft, TopRight, BottomLeft, BottomRight};

		private Color backgroundColor = Color.White;
		private Color imageBgColor = Color.LightGray;
		private Color borderColor = Color.Black;
		private BeakPos beakPos;
		private Bitmap bitmap;

		public NotifyArea (int x, int y, int width, int height) {
			if ((x - width) < 0) {
				// Beak left
			} else {
				// Beak Right
			}

			if ((y - height) < 0) {
				// Beak Top
			} else {
				// Beak Bottom
			}
			height = height + 14;
			bitmap = new Bitmap(width, height);
			beakPos = BeakPos.BottomLeft;
			DrawBeak();
		}

		public void Save (string filename) {
			bitmap.Save(filename);
		}

		private void DrawBeak() {
			switch (beakPos) {
				case BeakPos.TopLeft:
					DrawTopLeft();
					break;
				case BeakPos.TopRight:
					DrawTopRight();
					break;
				case BeakPos.BottomLeft:
					DrawBottomLeft();
					break;
				case BeakPos.BottomRight:
					DrawBottomRight();
					break;
			}
		}

		private void DrawTopLeft() {
			// Beak
			for (int y=14; y >= 0; y--) {
				for (int x=1; x < y; x++) bitmap.SetPixel(x, y, backgroundColor);
				bitmap.SetPixel(y, y, borderColor);
			}

			// Border Top/Bottom
			for (int x=0; x < bitmap.Width; x++) {
				if (x > 14) bitmap.SetPixel(x, 14, borderColor);
				bitmap.SetPixel(x, bitmap.Height - 1, borderColor);
			}

			// Border Left/Right
			for (int y=0; y < bitmap.Height; y++) {
				bitmap.SetPixel(0, y, borderColor);
				if (y > 14) bitmap.SetPixel(bitmap.Width - 1, y, borderColor);
			}

			// Color Background
			for (int x=1; x < (bitmap.Width - 1); x++) {
				for (int y=15; y < (bitmap.Height - 1); y++) {
					bitmap.SetPixel(x, y, backgroundColor);
				}
			}

			// Image Background
			for (int x=0; x < 34; x++) {
				for (int y=16; y < (bitmap.Height - 2); y++) {
					bitmap.SetPixel(bitmap.Width - 4 - x, y, imageBgColor);
				}
			}
		}

		private void DrawTopRight() {
			// Beak
			for (int y=14; y >= 0; y--) {
				for (int x=1; x < y; x++)
					bitmap.SetPixel(bitmap.Width - x, y, backgroundColor);
				bitmap.SetPixel(bitmap.Width - y, y, borderColor);
			}

			// Border Top/Bottom
			for (int x=0; x < bitmap.Width; x++) {
				if (x < (bitmap.Width - 14)) bitmap.SetPixel(x, 14, borderColor);
				bitmap.SetPixel(x, bitmap.Height - 1, borderColor);
			}

			// Border Left/Right
			for (int y=0; y < bitmap.Height; y++) {
				if (y > 14) bitmap.SetPixel(0, y, borderColor);
				bitmap.SetPixel(bitmap.Width - 1, y, borderColor);
			}

			// Color Background
			for (int x=1; x < (bitmap.Width - 1); x++) {
				for (int y=15; y < (bitmap.Height - 1); y++) {
					bitmap.SetPixel(x, y, backgroundColor);
				}
			}

			// Image Background
			for (int x=0; x < 34; x++) {
				for (int y=16; y < (bitmap.Height - 2); y++) {
					bitmap.SetPixel(2 + x, y, imageBgColor);
				}
			}
		}

		private void DrawBottomLeft() {
			// Beak
			for (int y=14; y >= 0; y--) {
				for (int x=1; x < y; x++) 
					bitmap.SetPixel(x, bitmap.Height - y, backgroundColor);
				bitmap.SetPixel(y, bitmap.Height - y, borderColor);
			}

			// Border Top/Bottom
			for (int x=0; x < bitmap.Width; x++) {
				bitmap.SetPixel(x, 0, borderColor);
				if (x > 14) bitmap.SetPixel(x, bitmap.Height - 14, borderColor);
			}

			// Border Left/Right
			for (int y=0; y < bitmap.Height; y++) {				
				bitmap.SetPixel(0, y, borderColor);
				if (y < bitmap.Height - 14) 
					bitmap.SetPixel(bitmap.Width - 1, y, borderColor);
			}

			// Color Background
			for (int x=1; x < (bitmap.Width - 1); x++) {
				for (int y=1; y < (bitmap.Height - 15); y++) {
					bitmap.SetPixel(x, y, backgroundColor);
				}
			}

			// Image Background
			for (int x=0; x < 34; x++) {
				for (int y=0; y < (bitmap.Height - 16); y++) {
					bitmap.SetPixel(bitmap.Width - 4 - x, y + 2, imageBgColor);
				}
			}
		}

		private void DrawBottomRight() {
			// Beak
			for (int y=14; y >= 0; y--) {
				for (int x=1; x < y; x++)
					bitmap.SetPixel(bitmap.Width - x, bitmap.Height - y, backgroundColor);
				bitmap.SetPixel(bitmap.Width - y, bitmap.Height - y, borderColor);
			}

			// Border Top/Bottom
			for (int x=0; x < bitmap.Width; x++) {
				bitmap.SetPixel(x, 0, borderColor);
				if (x < (bitmap.Width - 14)) 
					bitmap.SetPixel(x, bitmap.Height - 14, borderColor);
			}

			// Border Left/Right
			for (int y=0; y < bitmap.Height; y++) {
				if (y < bitmap.Height - 14) bitmap.SetPixel(0, y, borderColor);
				bitmap.SetPixel(bitmap.Width - 1, y, borderColor);
			}

			// Color Background
			for (int x=1; x < (bitmap.Width - 1); x++) {
				for (int y=1; y < (bitmap.Height - 15); y++) {
					bitmap.SetPixel(x, y, backgroundColor);
				}
			}

			// Image Background
			for (int x=0; x < 34; x++) {
				for (int y=0; y < (bitmap.Height - 17); y++) {
					bitmap.SetPixel(2 + x, y + 2, imageBgColor);
				}
			}
		}
	}

	public class NotifyMessage : ShapedWindow {
		private Gtk.Button buttonClose;
		private Gtk.Label labelTitle;
		private Gtk.Fixed fixBox;

		public NotifyMessage (int x, int y, int width, int height) {
			// Setup Window
			SetDefaultSize(width, height);
			Move(x, y);

			// Setup Image
			string fileName = System.IO.Path.GetTempFileName();
			NotifyArea area = new NotifyArea(x, y, width, height);
			area.Save(fileName);
			Setup(fileName);
			File.Delete(fileName);

			// Setup Fixed
			this.fixBox = new Gtk.Fixed();
			this.Add(this.fixBox);

			buttonClose = new Gtk.Button(new Gtk.Image(Gtk.Stock.Close, Gtk.IconSize.Menu));
			buttonClose.Clicked += new EventHandler(OnButtonClose);
			buttonClose.Relief = Gtk.ReliefStyle.None;
			this.fixBox.Put(buttonClose, 5, 20);

			labelTitle = new Gtk.Label("<span size='x-large'><b>Prova</b></span>");
			labelTitle.UseMarkup = true;
			this.fixBox.Put(labelTitle, 40, 20);
		}

		private void OnButtonClose (object sender, EventArgs args) {
			this.Destroy();
		}

		public static void Main() {
			Gtk.Application.Init();
			NotifyMessage dialog = new NotifyMessage(300, 300, 250, 70);
			dialog.ShowAll();
			Gtk.Application.Run();
		}
	}
}
