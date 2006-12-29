/* [ Utils/FileUtils.cs ] 
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

namespace Niry.Utils {
	/// File Utils
	public static class FileUtils {
		// Try To Startup a Application
		public static void Run (string path) {
			try {
				// Try To Open This File
				System.Diagnostics.Process.Start(path);
			} catch (Exception e) {
				Debug.Log("Failed Process.Start() {0}: {1}", path, e.Message);
			}
		}

		/// Read Enteire File
		public static byte[] ReadEntireFile (string path) {
			// Read File from Hard Disk
			Stream stream = File.OpenRead(path);
			
			// Get Data from The Stream
			byte[] data = ReadStreamFully(stream, 0);
			
			// Close Stream
			stream.Close();

			// Return Data
			return(data);
		}

		/// Read All Data contained into Stream
		public static byte[] ReadStreamFully (Stream stream) {
			return(ReadStreamFully(stream, 32768));
		}

		/// Read All Data contained into Stream
		public static byte[] ReadStreamFully (Stream stream, int length) {
			if (length < 1) length = 32768;

			byte[] buffer = new byte[length];
			int read = 0;
			int chunk;

			while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0) {
				read += chunk;

				if (read == buffer.Length) {
					int nextByte = stream.ReadByte();
					if (nextByte == -1) return(buffer);

					byte[] newBuffer = new byte[buffer.Length*2];
					Array.Copy(buffer, newBuffer, buffer.Length);
					newBuffer[read] = (byte) nextByte;
					buffer = newBuffer;
					read++;
				}
			}

			byte[] ret = new byte[read];
			Array.Copy(buffer, ret, read);
			return(ret);
		}

		/// Get String Rapresenting byte Size
		public static string GetSizeString (long byteSize) {
			if (byteSize > 1073741824)
				return((byteSize / 1073741824).ToString() + "Gb");
			if (byteSize > 1048576)
				return((byteSize / 1048576).ToString() + "Mb");
			if (byteSize > 1024)
				return((byteSize / 1024).ToString() + "Kb");
			
			return(byteSize.ToString() + "byte");
		}

		/// Get File Extension
		public static string GetExtension (string path) {
			FileInfo fileInfo = new FileInfo(path);
			if (TextUtils.IsEmpty(fileInfo.Extension))
				return(null);
			return(fileInfo.Extension.Remove(0, 1));
		}

		/// Copy Files & Directory
		public static void CopyAll (string src, string dest) {
			if (IsDirectory(src) == true) {
				try {
					DirectoryInfo dir = new DirectoryInfo(src);
					string path = Path.Combine(dest, dir.Name);
					CopyDirectory(src, path);
				} catch (Exception e) {
					Console.WriteLine("Copy: {0}", e.Message);
				}
			} else {
				try {
					FileInfo file = new FileInfo(src);
					string path = Path.Combine(dest, file.Name);
					System.IO.File.Copy(src, path, true);
				} catch (Exception e) {
					Console.WriteLine("Copy: {0}", e.Message);
				}
			}
		}

		/// Remove Recursively file or Directory
		public static void RemoveAll (string path) {
			if (IsDirectory(path) == true) {
				Directory.Delete(path, true);
			} else {
				System.IO.File.Delete(path);
			}
		}

		/// Is Path a Directory?
		public static bool IsDirectory (string path) {
			return(Directory.Exists(path));
		}

		/// Create Directory
		public static void CreateDirectory (string dirPath) {
			DirectoryInfo dinfo = new DirectoryInfo(dirPath);
			if (!dinfo.Exists) dinfo.Create();
		}

		/// Create File of setted size, Filled With 0
		public static FileStream CreateNullFile (string fileName, long size) {
			// Setup Block Size
			int blocks = (int) size / 8192;
			int rest = (int) size % 8192;

			// Create File
			byte[] data = new byte[8192];
			FileStream stream = File.Create(fileName);
			while (blocks-- > 0) stream.Write(data, 0, 8192);
			if (rest > 0) stream.Write(data, 0, rest);
			
			// Rewind File Pointer
			stream.Seek(0, SeekOrigin.Begin);
			return(stream);
		}

		/// Copy Directory
		public static void CopyDirectory (string src, string dst) {
			string[] files;

			if (!Directory.Exists(dst)) CreateDirectory(dst);
			files = Directory.GetFileSystemEntries(src);
			foreach (string elem in files) {
				if (Directory.Exists(elem)) {
					// SubDirectory
					CopyDirectory(elem, Path.Combine(dst, Path.GetFileName(elem)));
				} else {
					// File
					File.Copy(elem, Path.Combine(dst, Path.GetFileName(elem)), true);
				}
			}
		}
	}
}
