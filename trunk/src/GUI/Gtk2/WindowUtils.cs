/* [ GUI/Gtk2/WindowUtils.cs ] - Gtk 2.x Window Utils
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

namespace Niry.GUI.Gtk2 {
	public static class WindowUtils {
		public static void MoveBy (Gtk.Window window, int x, int y) {
			int winX, winY;

			window.GetPosition(out winX, out winY);
			window.Move(winX + x, winY + y);
		}

		public static void Shake (Gtk.Window window, int times) {
			int winX, winY;

			Gtk.Application.Invoke(delegate {
				window.GetPosition(out winX, out winY);

				for (int i=10; i > 0; i--) {
					for (int j=times; j > 0; j--) {
						MoveBy(window, 0, i);
						TimeUtils.Sleep(5);

						MoveBy(window, i, 0);
						TimeUtils.Sleep(5);

						MoveBy(window, 0, -i);
						TimeUtils.Sleep(5);

						MoveBy(window, -i, 0);
						TimeUtils.Sleep(5);
					}
				}

				window.Move(winX, winY);
			});
		}
		
		public static void FadeIn (Gtk.Window window, uint millisecondsTimeout) {
			int screenHeight = window.Screen.Height;
			int winWidth, winHeight;
			int winX, winY;
		
			// Hide Window At Bottom of The Screen
			window.GetPosition(out winX, out winY);
			//int firstWinY = winY;
			window.GetSize(out winWidth, out winHeight);
			window.Move(winX, screenHeight);
			
			// Rise Window
			do {
				window.GetPosition(out winX, out winY);
				window.Move(winX, winY - 1);
				//System.Threading.Thread.Sleep(millisecondsTimeout);
				TimeUtils.Sleep(millisecondsTimeout);
			//} while (winY >= firstWinY);
			} while (winY > (screenHeight - winHeight));
		}
		
		public static void FadeOut (Gtk.Window window, uint millisecondsTimeout) {
			int screenHeight = window.Screen.Height + 10;
			int winX, winY;
			
			do {
				window.GetPosition(out winX, out winY);
				window.Move(winX, winY + 1);
				//System.Threading.Thread.Sleep(millisecondsTimeout);
				TimeUtils.Sleep(millisecondsTimeout);
			} while (winY <= screenHeight);
		}
	}
}
