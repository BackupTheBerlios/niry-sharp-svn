/* [ GUI/Graphics/ColorUtils.cs ] 
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

namespace Niry.Graphics {
	public static class ColorUtils {
		// Get Color Part
		public static byte GetAlpha (int color) {
			return((byte) ((color & 0x7F000000) >> 24));
		}
		
		public static byte GetRed (int color) {
			return((byte) ((color & 0xFF0000) >> 16));
		}

		public static byte GetGreen (int color) {
			return((byte) ((color & 0x00FF00) >> 8));
		}

		public static byte GetBlue (int color) {
			return((byte) (color & 0x0000FF));
		}

		// Get High & Low Color Part
		public static byte GetHighRed (int color) {
			return((byte) ((color & 0xF00000) >> 20));
		}

		public static byte GetLowRed (int color) {
			return((byte) ((color & 0x0F0000) >> 16));
		}

		public static byte GetHighGreen (int color) {
			return((byte) ((color & 0x00F000) >> 12));
		}

		public static byte GetLowGreen (int color) {
			return((byte) ((color & 0x000F00) >> 8));
		}

		public static byte GetHighBlue (int color) {
			return((byte) ((color & 0x0000F0) >> 4));
		}

		public static byte GetLowBlue (int color) {
			return((byte) (color & 0x00000F));
		}

		// Set Color Part
		public static int SetRed (int color, byte value) {
			return((color & 0x00FFFF) | (value << 16));
		}

		public static int SetGreen (int color, byte value) {
			return((color & 0xFF00FF) | (value << 8));
		}

		public static int SetBlue (int color, byte value) {
			return((color & 0xFFFF00) | value);
		}

		// Set Hight & Low Color Part
		public static int SetHighRed (int color, byte value) {
			return((color & 0x0FFFFF) | (value << 20));
		}

		public static int SetLowRed (int color, byte value) {
			return((color & 0xF0FFFF) | (value << 16));
		}

		public static int SetHighGreen (int color, byte value) {
			return((color & 0xFF0FFF) | (value << 12));
		}

		public static int SetLowGreen (int color, byte value) {
			return((color & 0xFFF0FF) | (value << 8));
		}

		public static int SetHighBlue (int color, byte value) {
			return((color & 0xFFFF0F) | (value << 4));
		}

		public static int SetLowBlue (int color, byte value) {
			return((color & 0xFFFFF0) | value);
		}

		// Setup Color
		public static int SetColor (byte r, byte g, byte b) {
			int color = 0x000000;
			color = SetRed(color, r);
			color = SetGreen(color, g);
			color = SetBlue(color, b);
			return(color);
		}
	}
}
