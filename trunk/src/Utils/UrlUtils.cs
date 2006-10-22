/* [ Utils/UrlUtils.cs ] 
 * Authors:
 *   Patrik Torstensson (Patrik.Torstensson@labs2.com)
 *   Wictor Wilén (decode/encode functions) (wictor@ibizkit.se)
 *   Tim Coleman (tim@timcoleman.com)
 *   Gonzalo Paniagua Javier (gonzalo@ximian.com)
 *
 * Copyright (C) 2005 Novell, Inc (http://www.novell.com)
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
using System.IO;
using System.Text;

namespace Niry.Utils {
	/// Url Utils
	public static class UrlUtils {
		private static int GetInt (byte b) {
			char c = (char) b;
			if (c >= '0' && c <= '9')
				return c - '0';

			if (c >= 'a' && c <= 'f')
				return c - 'a' + 10;

			if (c >= 'A' && c <= 'F')
				return c - 'A' + 10;

			return -1;
		}

		private static int GetChar (string str, int offset, int length) {
			int val = 0;
			int end = length + offset;
			for (int i = offset; i < end; i++) {
				char c = str [i];
				if (c > 127)
					return -1;

				int current = GetInt ((byte) c);
				if (current == -1)
					return -1;
				val = (val << 4) + current;
			}

			return val;
		}

		private static char [] GetChars (MemoryStream b, Encoding e) {
			return e.GetChars (b.GetBuffer (), 0, (int) b.Length);
		}

		/// Url Decode
		public static string UrlDecode (string s, Encoding e) {
			if (null == s) 
				return null;

			if (s.IndexOf ('%') == -1 && s.IndexOf ('+') == -1)
				return s;

			if (e == null)
				e = Encoding.UTF8;
	
			StringBuilder output = new StringBuilder ();
			long len = s.Length;
			MemoryStream bytes = new MemoryStream ();
			int xchar;
	
			for (int i = 0; i < len; i++) {
				if (s [i] == '%' && i + 2 < len && s [i + 1] != '%') {
					if (s [i + 1] == 'u' && i + 5 < len) {
						if (bytes.Length > 0) {
							output.Append (GetChars (bytes, e));
							bytes.SetLength (0);
						}

						xchar = GetChar (s, i + 2, 4);
						if (xchar != -1) {
							output.Append ((char) xchar);
							i += 5;
						} else {
							output.Append ('%');
						}
					} else if ((xchar = GetChar (s, i + 1, 2)) != -1) {
						bytes.WriteByte ((byte) xchar);
						i += 2;
					} else {
						output.Append ('%');
					}
					continue;
				}

				if (bytes.Length > 0) {
					output.Append (GetChars (bytes, e));
					bytes.SetLength (0);
				}

				if (s [i] == '+') {
					output.Append (' ');
				} else {
					output.Append (s [i]);
				}
	         }
	
			if (bytes.Length > 0) {
				output.Append (GetChars (bytes, e));
			}

			bytes = null;
			return output.ToString ();
		}

		/// Url Decode
		public static string UrlDecode (string str) {
			return UrlDecode(str, Encoding.UTF8);
		}
	}
}
