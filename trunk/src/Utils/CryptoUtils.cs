/* [ Utils/CryptoUtils.cs ] 
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
using System.Security.Cryptography;

namespace Niry.Utils {
	/// Hash/Crypto Utils
	public static class CryptoUtils {
		/// Return MD5 String
		public static string MD5String (string text) {
			ASCIIEncoding encoding = new ASCIIEncoding();
			MD5 md5 = MD5.Create();
			byte[] buffer = encoding.GetBytes(text);
			byte[] hash = md5.ComputeHash(buffer);
			StringBuilder md5Builder = new StringBuilder();
			foreach (byte b in hash) md5Builder.Append(b.ToString("x2"));
			return(md5Builder.ToString());
		}

		/// Return SHA1 String	
		public static string SHA1String (string text) {
			ASCIIEncoding encoding = new ASCIIEncoding();
			SHA1 sha = SHA1.Create();
			byte[] buffer = encoding.GetBytes(text);
			byte[] hash = sha.ComputeHash(buffer);
			StringBuilder shaBuilder = new StringBuilder();
			foreach (byte b in hash) shaBuilder.Append(b.ToString("x2"));
			return(shaBuilder.ToString());
		}
	
		/// Return SHA256 String	
		public static string SHA256String (string text) {
			ASCIIEncoding encoding = new ASCIIEncoding();
			SHA256 sha = SHA256.Create();
			byte[] buffer = encoding.GetBytes(text);
			byte[] hash = sha.ComputeHash(buffer);
			StringBuilder shaBuilder = new StringBuilder();
			foreach (byte b in hash) shaBuilder.Append(b.ToString("x2"));
			return(shaBuilder.ToString());
		}
	
		/// Return SHA348 String	
		public static string SHA384String (string text) {
			ASCIIEncoding encoding = new ASCIIEncoding();
			SHA384 sha = SHA384.Create();
			byte[] buffer = encoding.GetBytes(text);
			byte[] hash = sha.ComputeHash(buffer);
			StringBuilder shaBuilder = new StringBuilder();
			foreach (byte b in hash) shaBuilder.Append(b.ToString("x2"));
			return(shaBuilder.ToString());
		}
	
		/// Return SHA512 String	
		public static string SHA512String (string text) {
			ASCIIEncoding encoding = new ASCIIEncoding();
			SHA512 sha = SHA512.Create();
			byte[] buffer = encoding.GetBytes(text);
			byte[] hash = sha.ComputeHash(buffer);
			StringBuilder shaBuilder = new StringBuilder();
			foreach (byte b in hash) shaBuilder.Append(b.ToString("x2"));
			return(shaBuilder.ToString());
		}
	}
}
