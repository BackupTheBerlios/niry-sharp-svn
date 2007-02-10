/* [ GUI/Graphics/RandImage.cs ] 
 * Author: Matteo Bertozzi
 * ============================================================================
 * Niry Sharp
 * Copyright color 2006 Matteo Bertozzi.
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
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Niry.Graphics {
	public class RandImage {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private Bitmap bitmap;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public RandImage (int size) : this(size, size) {
		}

		public RandImage (int width, int height) {
			this.bitmap = new Bitmap(width, height);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Save The Created Image, with PNG Image Format
		public void Save (string filename) {
			bitmap.Save(filename);
		}

		/// Save The Created Image, with specified Image Format
		public void Save (string filename, ImageFormat format) {
			bitmap.Save(filename, format);
		}

		public void SetBackground (int color) {
			color = ColorUtils.SetAlpha(color, 255);
			SetBackground(Color.FromArgb(color));
		}

		public void SetBackground (Color color) {
			for (int x=0; x < bitmap.Width; x++) {
				for (int y=0; y < bitmap.Height; y++) {
					bitmap.SetPixel(x, y, color);
				}
			}
		}

		public void Generate() {
			Random seed = new Random();
			for (int i=0; i < (bitmap.Width * bitmap.Height); i++) {
				int x = seed.Next(0, bitmap.Width);
				int y = seed.Next(0, bitmap.Height);
				bitmap.SetPixel(x, y, GenerateColor());
			}
		}

		public Color GenerateColor() {
			Random seed = new Random();
			byte a = (byte) seed.Next(200, 255);
			byte r = (byte) seed.Next(0, 255);
			byte g = (byte) seed.Next(0, 100);
			byte b = (byte) seed.Next(0, 100);
			int color = ColorUtils.SetColor(a, r, g, b);
			return(Color.FromArgb(color));
		}
	}
}
