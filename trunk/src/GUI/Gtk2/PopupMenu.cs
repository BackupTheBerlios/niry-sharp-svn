/* [ GUI/Gtk2/PopupMenu.cs ] - Gtk 2.x Menu Utils
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

using Gtk;
using System;

namespace Niry.GUI.Gtk2 {
	public class PopupMenu : Gtk.Menu {
		public MenuItem AddItem (string label) {
			MenuItem item = new MenuItem(label);
			Append(item);
			return(item);
		}

		public void AddItem (ExtMenuItem item) {
			Append(item);
		}

		public void AddItem (ExtMenuItem item, EventHandler handler) {
			item.Activated += handler;
			Append(item);
		}

		public void AddItem (ExtCheckMenuItem item) {
			Append(item);
		}

		public void AddItem (ExtCheckMenuItem item, EventHandler handler) {
			item.Activated += handler;
			Append(item);
		}

		public MenuItem AddItem (string label, Menu subMenu) {
			MenuItem item = new MenuItem(label);
			item.Submenu = subMenu;
			Append(item);
			return(item);
		}

		public MenuItem AddItem (string label, EventHandler handler) {
			MenuItem item = new MenuItem(label);
			item.Activated += handler;
			Append(item);
			return(item);
		}

		public MenuItem AddItem (string label, EventHandler handler, Menu subMenu) {
			MenuItem item = new MenuItem(label);
			item.Activated += handler;
			item.Submenu = subMenu;
			Append(item);
			return(item);
		}

		public ImageMenuItem AddImageItem (string stock_id) {
			ImageMenuItem item = new ImageMenuItem(stock_id, null);
			Append(item);
			return(item);
		}

		public ImageMenuItem AddImageItem (string stock_id, Menu subMenu) {
			ImageMenuItem item = new ImageMenuItem(stock_id, null);
			item.Submenu = subMenu;
			Append(item);
			return(item);
		}

		public ImageMenuItem AddImageItem (string stock_id, EventHandler handler) {
			ImageMenuItem item = new ImageMenuItem(stock_id, null);
			item.Activated += handler;
			Append(item);
			return(item);
		}

		public ImageMenuItem AddImageItem (string stock_id, EventHandler handler, 
											Menu subMenu) 
		{
			ImageMenuItem item = new ImageMenuItem(stock_id, null);
			item.Activated += handler;
			item.Submenu = subMenu;
			Append(item);
			return(item);
		}

		public CheckMenuItem AddCheckItem (string label, EventHandler handler) {
			CheckMenuItem item = new CheckMenuItem(label);
			item.Activated += handler;
			Append(item);
			return(item);
		}

		public CheckMenuItem AddCheckItem (string label, bool active, EventHandler handler) {
			CheckMenuItem item = new CheckMenuItem(label);
			item.Active = active;
			item.Activated += handler;
			Append(item);
			return(item);
		}

		public SeparatorMenuItem AddSeparator() {
			SeparatorMenuItem item = new SeparatorMenuItem();
			Append(item);
			return(item);
		}

		public SeparatorMenuItem AddSeparator (Menu subMenu) {
			SeparatorMenuItem item = new SeparatorMenuItem();
			item.Submenu = subMenu;
			Append(item);
			return(item);
		}
	}
}
