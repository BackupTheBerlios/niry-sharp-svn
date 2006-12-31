/* [ GUI/Graphics/Steganography.cs ] 
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
using System.Drawing;
using System.Drawing.Imaging;

namespace Niry.Graphics {
	public unsafe class Steganography {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================
		private const uint sign = 0x57390;
		private StringBuilder buffer;
		private Bitmap bitmap;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public Steganography (string filename) : this(new Bitmap(filename)) {
		}

		public Steganography (Bitmap bitmap) {
			this.bitmap = bitmap;
			this.buffer = new StringBuilder();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void AppendText (string text) {
			buffer.Append(text);
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		public void GenerateImage() {
			byte[] values = GenerateValues();
			int i = 0;
			for (int x=0; x < bitmap.Width; x++) {
				for (int y=0; y < bitmap.Height; y++) {
					int r = (i < values.Length) ? values[i++] : -1;
					int g = (i < values.Length) ? values[i++] : -1;
					int b = (i < values.Length) ? values[i++] : -1;
					WriteBlock(x, y, r, g, b);
					if (i >= values.Length) return;
				}
			}
		}

		public void Print() {
			for (int x=0; x < bitmap.Width; x++) {
				for (int y=0; y < bitmap.Height; y++) {			
					Color color = bitmap.GetPixel(x, y);
					int c = ColorUtils.SetColor(color.R, color.G, color.B);
					Console.WriteLine("-: {0},{1} ({2:X}) {3} {4} {5}", x, y, c, color.R, color.G, color.B);
				}
			}
		}

		private uint GetMessageValues (uint length) {
			return(length - 68);
		}

		public string GetMessage() {
			uint length = ReadLength();
			byte[] values = new byte[length];

			ReadMessage(ref values, GetMessageValues(length));

			// Get Message
			int v = 0;
			byte[] message = new byte[GetMessageValues(length) + 1];
			try {
				for (int i=0; i < GetMessageValues(length); i++) {
					byte n = 0;
					for (int j=0; j < 17; j++) n += values[v++];
					message[i] = n;
				}
			} catch {}

			return(Encoding.UTF8.GetString(message, 0, (int) GetMessageValues(length)));
		}

		// ============================================
		// PUBLIC (Save) Methods
		// ============================================
		/// Save The Created Image, with PNG Image Format
		public void SaveImage (string filename) {
			bitmap.Save(filename);
		}

		/// Save The Created Image, with specified Image Format
		public void SaveImage (string filename, ImageFormat format) {
			bitmap.Save(filename, format);
		}

		// ============================================
		// PROTECTED Methods
		// ============================================
		// Write Steganography Signature at the End of the Image
		protected void WriteSign() {
		}

		protected byte[] GenerateValues() {
			byte[] msg = Encoding.UTF8.GetBytes(buffer.ToString());
			uint length = GetNumBytes((uint) msg.Length);
			byte[] values = new byte[length];

			// Fill Values Array
			WriteLength(ref values, length);
			WriteMessage(ref values, msg);

			return(values);
		}

		// Read Message Length at the Begin of the Image
		public uint ReadLength() {
			byte[] values = new byte[69];	// 17 SuperBlock * 4byte

			// Read Values From Image
			int v = 0;
			for (int x=0; v < 68 && x < bitmap.Width; x++) {
				for (int y=0; v < 68 && y < bitmap.Height; y++) {
					ReadBlock(x, y, out values[v], out values[v+1], out values[v+2]);
					v += 3;
				}
			}

			// Transform Values in unsigned long
			v = 0;
			uint length = 0;
			byte *p = (byte *) &length;
			for (int i=0; i < 4; i++) {
				byte n = 0;
				for (int j=0; j < 17; j++) n += values[v++];
				*p++ = n;
			}
			return(length);
		}

		// Read Message into Image
		protected void ReadMessage (ref byte[] values, uint length) {
			int x = 0, y = 0;

			// Reach X,Y Position at the end of Message Length
			int v = 0;
			for (x=0; v < 68 && x < bitmap.Width; x++) {
				for (y=0; v < 68 && y < bitmap.Height; y++) {
					v += 3;
				}
			}

			// Get Message, First Block is the Low Blue of Length Block
			byte r, g;
			v = 1;
			ReadBlock(--x, y - 1, out r, out g, out values[0]);
			for (; v < length && x < bitmap.Width; x++) {
				for (; v < length && y < bitmap.Height; y++) {
					ReadBlock(x, y, out values[v], out values[v+1], out values[v+2]);
					v += 3;
				}
			}
		}

		// Read Steganography Signature at the End of the Image
		protected void ReadeSign() {
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		/// Setup The RGB high part of the Block
		private void WriteBlock (int x, int y, int r, int g, int b) {
			// Get & Setup Pixel Color
			Color color = bitmap.GetPixel(x, y);
			int c = ColorUtils.SetColor(color.R, color.G, color.B);
			if (r >= 0) c = ColorUtils.SetLowRed(c, (byte) r);
			if (g >= 0) c = ColorUtils.SetLowGreen(c, (byte) g);
			if (b >= 0) c = ColorUtils.SetLowBlue(c, (byte) b);

			// Set Color & Set Pixel
			color = Color.FromArgb(c);
			//Console.WriteLine("Write: {0},{1} ({2}) {3} {4} {5}", x, y, ColorUtils.SetColor(color.R, color.G, color.B), r, g, b);
			bitmap.SetPixel(x, y, color);
		}

		/// Return The RGB low part of the Block
		private void ReadBlock (int x, int y, out byte r, out byte g, out byte b) {
			Color color = bitmap.GetPixel(x, y);
			int c = ColorUtils.SetColor(color.R, color.G, color.B);
			r = ColorUtils.GetLowRed(c);
			g = ColorUtils.GetLowGreen(c);
			b = ColorUtils.GetLowBlue(c);
			//Console.WriteLine("Read: {0},{1} ({2}) {3} {4} {5}", x, y, c, r, g, b);
		}

		private byte GetBlockSize (byte info, byte div) {
			return((byte) (info / div));
		}

		private byte GetLastBlockSize (byte info, byte div) {
			return((byte) (info % div));
		}

		private uint GetNumBytes (uint length) {
			return(68 + (length * 17));
		}

		private byte GetBlockContent (byte info) {
			byte blockSize;

			if ((blockSize = GetBlockSize(info, 17)) > 0xF)
				return(16);

			if ((blockSize + GetLastBlockSize(info, 17)) > 0xF)
				return(16);

			return(17);
		}

		private void WriteBytes (ref byte[] values, ref uint pos, byte info) {
			byte div = GetBlockContent(info);
			byte blockSize = GetBlockSize(info, div);
			byte lastBlockSize = GetLastBlockSize(info, div);

			for (byte i=0; i < div; i++) {
				if (i == 16 && div == 17) {
					values[pos++] = (byte) (blockSize + lastBlockSize);
				} else {
					values[pos++] = blockSize;
				}
			}

			if (div < 17) values[pos++] = lastBlockSize;
		}

		// Write Message Length at the begin
		private void WriteLength (ref byte[] values, uint length) {
			byte *p = (byte *) &length;
			uint pos = 0;

			//Console.Write(" (Write Len: {0}) ", length);

			for (int i=0; i < 4; i++, p++)
				WriteBytes(ref values, ref pos, *p);
		}

		// Write Message after the length
		private void WriteMessage (ref byte[] values, byte[] message) {
			uint pos = 68;

			for (uint i=0; i < message.Length; i++)
				WriteBytes(ref values, ref pos, message[i]);
		}

		// ============================================
		// PROTECTED Properties
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
#if false
		// Simple Example of Number Creation
		public static void Main() {
			uint length = 0x123456;
			byte *p = (byte *) &length;
			byte[] v = new byte[4];
			Console.WriteLine("Length: 0x{0:x}", length);
			for (int i=0; i < 4; i++) v[i] = *p++;
			length = 0;
			Console.WriteLine("Length: 0x{0:x}", length);
			p = (byte *) &length;
			for (int i=0; i < 4; i++) *p++ = v[i];
			Console.WriteLine("Length: 0x{0:x}", length);
		}
#endif
#if false
		public static void Main() {
			Console.WriteLine("Hello Steganography");
			try {
				Console.Write(" * Generating Image");
				Steganography stego = new Steganography("input.png");
				stego.AppendText("Hello World");
				stego.GenerateImage();
				stego.SaveImage("output.png", ImageFormat.Png);
				Console.WriteLine(" [ ok ]");
			} catch (Exception e) {
				Console.WriteLine(" [ !! ]");
				Console.WriteLine("   {0}", e.Message);
			}

			try {
				Console.Write(" * Getting Image Text");
				Steganography stego = new Steganography("output.png");
				Console.WriteLine(" [ ok ]");
				Console.WriteLine(" * Stego Text '{0}'", stego.GetMessage());
			} catch (Exception e) {
				Console.WriteLine(" [ !! ] ");
				Console.WriteLine("   {0}", e.Message);
			}
		}
#endif
	}
}
