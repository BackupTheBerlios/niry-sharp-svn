/* [ Utils/FileTypes.cs ] 
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
using System.Text.RegularExpressions;

namespace Niry.Utils {
	public enum FileType {
		Package = 0,
		Document,
		Executable,
		Image,
		Audio,
		Video,
		Directory
	}

	public static class FileTypes {
		// ============================================
		// PRIVATE Members
		// ============================================
		private static string[][] Extensions = {
			// FileType.Package
			new string[] {
				"arc", "arj", "gz", "lzh", "pak", "rar", "z", 
				"zip", "jar", "bz2", "tar", "rpm", "ar", "war",
				"ear"
			},

			// FileType.Document
			new string[] {
				"doc", "pdf", "ps", "tex", "txt", "wri"
			},
			// FileType.Executable
			new string[] {
				"bat", "com", "exe", "pm"
			},
			// FileType.Image
			new string[] {
				"bmp", "gif", "jpe", "jpg", "jpeg", "pcx", 
				"png", "psd", "wmf", "tiff", "ico", "xpm"
			},
			// FileType.Audio
			new string[] {
				"au", "mp2", "mp3", "mid", "ogg", "rm", "sm", "wav"
			},
			// FileType.Video
			new string[] {
				"avi", "asf", "mov", "mpeg", "mpg", "mpg4", "mp4",
				"rm", "divx", "qt", "wmv", "ram", "m1v", "m2v",
				"rv", "vob", "asx"
			}
		};

		// ============================================
		// PUBLIC (IsFormat) Methods
		// ============================================
		public static bool IsMatch (string path, FileType type) {
			if (type == FileType.Directory) return(false);

			string pattern = MakePattern(type);
			return(Regex.IsMatch(path, pattern, RegexOptions.IgnoreCase));	
		}

		public static bool IsMatch (string path, string type) {
			if (type == FileType.Directory.ToString()) return(false);

			string pattern = MakePattern(type);
			return(Regex.IsMatch(path, pattern, RegexOptions.IgnoreCase));	
		}

		public static bool IsPackage (string path) {
			string pattern = MakePattern(FileType.Package);
			return(Regex.IsMatch(path, pattern, RegexOptions.IgnoreCase));
		}

		public static bool IsDocument (string path) {
			string pattern = MakePattern(FileType.Document);
			return(Regex.IsMatch(path, pattern, RegexOptions.IgnoreCase));
		}

		public static bool IsExecutable (string path) {
			string pattern = MakePattern(FileType.Executable);
			return(Regex.IsMatch(path, pattern, RegexOptions.IgnoreCase));
		}

		public static bool IsImage (string path) {
			string pattern = MakePattern(FileType.Image);
			return(Regex.IsMatch(path, pattern, RegexOptions.IgnoreCase));
		}

		public static bool IsAudio (string path) {
			string pattern = MakePattern(FileType.Audio);
			return(Regex.IsMatch(path, pattern, RegexOptions.IgnoreCase));
		}

		public static bool IsVideo (string path) {
			string pattern = MakePattern(FileType.Video);
			return(Regex.IsMatch(path, pattern, RegexOptions.IgnoreCase));
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public static string[] GetExtension (FileType type) {
			int position = (int) Enum.Parse(typeof(FileType), type.ToString());
			return(Extensions[position]);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private static string MakePattern (FileType type) {
			return(MakePattern(type.ToString()));
		}

		private static string MakePattern (string type) {
			int position = (int) Enum.Parse(typeof(FileType), type);
			int numExts = Extensions[position].Length - 1;

			StringBuilder pattern = new StringBuilder();
			pattern.Append("[^?].(");
			for (int i=0; i < numExts; i++)
				pattern.AppendFormat("{0}|", Extensions[position][i]);
			pattern.AppendFormat("{0})$", Extensions[position][numExts]);

			return(pattern.ToString());
		}
	}
}
