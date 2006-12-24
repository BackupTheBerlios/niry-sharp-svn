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
using System.IO;
using System.Text;
using System.IO.Compression;
 
namespace Niry.Utils {
	/// Text Utils
	public static class TextUtils {
		/// Return True if The Current String is Empty
		public static bool IsEmpty (string text) {
			if (text == "") return(true);
			if (text == null) return(true);
			if (text.Length <= 0) return(true);
			return(false);
		}

		/// Return The Input String with The First Letter in Upper Size
		public static string UpFirstChar (string text) {
			if (text == null) return(null);

			char[] txtchr = text.ToCharArray();
			txtchr[0] = Char.ToUpper(txtchr[0]);
			return(new String(txtchr));
		}

		/// Base 64 Encode
		public static string Base64Encode (byte[] data) {
			try {
				return(Convert.ToBase64String(data));
			} catch {}
			return(null);
		}

		/// Base 64 Encode
		public static string Base64Encode (string data) {
			try {
				byte[] dataByte = new byte[data.Length];
				dataByte = Encoding.UTF8.GetBytes(data);
				return(Convert.ToBase64String(dataByte));
			} catch {}
			return(null);
		}

		/// Base 64 Decode
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

		public static byte[] GZipCompress (string data) {
			return(GZipCompress(Encoding.UTF8.GetBytes(data)));
		}

		public static byte[] GZipCompress (byte[] data) {
			MemoryStream memStream = new MemoryStream();
			GZipStream stream = new GZipStream(memStream, CompressionMode.Compress);
			stream.Write(data, 0, data.Length);
			stream.Close();
			memStream.Close();
			byte[] buffer = memStream.ToArray();
			return(buffer);
		}

		public static byte[] GZipDecompress (byte[] data) {
			return(GZipDecompress(data, 0, data.Length));
		}

		public static byte[] GZipDecompress (byte[] data, int index, int count) {
			MemoryStream memStream = new MemoryStream(data, index, count);
			GZipStream stream = new GZipStream(memStream, CompressionMode.Decompress);
			byte[] buffer = FileUtils.ReadStreamFully(stream);
			stream.Close();
			memStream.Close();
			return(buffer);
		}
 	}
 }
 
