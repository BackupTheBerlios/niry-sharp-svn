/* [ GUI/Gtk2/ImageUtils.cs ] - Gtk 2.x Image Utils
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

using System;

using Niry;
using Niry.Utils;

namespace Niry.GUI.Gtk2 {
	public static class ImageUtils {
		public static bool IsImage (string extension) {
			if (TextUtils.IsEmpty(extension) == true) 
				return(false);
			extension = extension.Substring(1);

			string[] pixbufExt = new string[] {
				"wmf", "ani", "bmp", "gif", "ico", "jpg", "jpeg", "pcx", "png", 
				"pnm", "ras", "tga", "tiff", "wbmp", "xbm", "xpm", "svg"
			};

			foreach (string ext in pixbufExt)
				if (ext == extension) return(true);
			return(false);
		}

		public static Gdk.Pixbuf GetPixbuf (string filename, int size) {
			Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(filename);
			return(Resize(pixbuf, size, size));
		}

		public static Gdk.Pixbuf GetPixbuf (string filename, int maxWidth, int maxHeight) {
			Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(filename);
			return(Resize(pixbuf, maxWidth, maxHeight));
		}
	
		public static Gtk.Image Get (string filename, int size) {
			Gdk.Pixbuf pixbuf = GetPixbuf(filename, size, size);
			return((pixbuf != null) ? new Gtk.Image(pixbuf) : null);
		}

		public static Gtk.Image Get (string filename, int maxWidth, int maxHeight) {
			Gdk.Pixbuf pixbuf = GetPixbuf(filename, maxWidth, maxHeight);
			return((pixbuf != null) ? new Gtk.Image(pixbuf) : null);
		}
		
		public static Gtk.Image GetAnimation (string filename) {
			Gdk.PixbufAnimation anipixbuf;
			
			if ((anipixbuf = new Gdk.PixbufAnimation(filename)) == null)
				return(null);
			
			if (anipixbuf.IsStaticImage == true) {
				Gdk.Pixbuf pixbuf;
				if ((pixbuf = anipixbuf.StaticImage) == null)
					return(null);
				return(new Gtk.Image(pixbuf));
			}
			return(new Gtk.Image(anipixbuf));
		}
		
		public static Gdk.Pixbuf Resize (Gdk.Pixbuf pixbuf, int maxWidth, int maxHeight) {
			if (pixbuf == null) return(null);
			
			double scaleWidth = maxWidth / (double) pixbuf.Width;
			double scaleHeight = maxHeight / (double) pixbuf.Height;

			double s = Math.Min(scaleWidth, scaleHeight);
			//if (s >= 1.0) return(pixbuf);

			int w = (int) Math.Round(s*pixbuf.Width);
			int h = (int) Math.Round(s*pixbuf.Height);
			
			return(pixbuf.ScaleSimple(w, h, Gdk.InterpType.Bilinear));
		}
		
		public static Gtk.Image Resize (Gtk.Image image, int maxWidth, int maxHeight) {
			if (image == null) return(null);
			image.Pixbuf = Resize(image.Pixbuf, maxWidth, maxHeight);
			return(image);
		}
	}
}
