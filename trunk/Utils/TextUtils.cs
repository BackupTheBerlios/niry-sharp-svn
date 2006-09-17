/* [ Utils/TextUtils.cs ] 
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
using System.Text;
 
namespace Niry.Utils {
	public static class TextUtils {
		public static string Base64Encode (string data) {
			try {
				byte[] dataByte = new byte[data.Length];
				dataByte = Encoding.UTF8.GetBytes(data);
				return(Convert.ToBase64String(dataByte));
			} catch {}
			return(null);
		}

		public static string Base64Decode (string data) {
			try {
				UTF8Encoding encoder = new UTF8Encoding();
				Decoder utf8Decode = encoder.GetDecoder();

				byte[] toDecodeByte = Convert.FromBase64String(data);
				int chrCount = utf8Decode.GetCharCount(toDecodeByte, 0, 
														toDecodeByte.Length);
				char[] decodedChar = new char[chrCount];
				utf8Decode.GetChars(toDecodeByte, 0, toDecodeByte.Length, 
									decodedChar, 0);
				return(new String(decodedChar));
			} catch {}
			return(null);
		}
 	}
 }
 
