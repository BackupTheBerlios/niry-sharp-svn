/* [ GUI/Gtk2/ExtMenuItem.cs ] - Gtk 2.x Extended Menu Item
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
	public class ExtMenuItem : Gtk.MenuItem {
		Gtk.Label label;
		Gtk.Image image;
		object extraData;

		public ExtMenuItem (string label, object extraData) {
			this.label = new Gtk.Label(label);
			this.extraData = extraData;

			this.Add(this.label);
		}

		public ExtMenuItem (string label, string stock_id, object extraData) {
			this.label = new Gtk.Label(label);
			this.image = new Gtk.Image(stock_id, Gtk.IconSize.Menu);
			this.extraData = extraData;

			Gtk.HBox hbox = new Gtk.HBox(false, 0);
			hbox.PackStart(this.image, false, false, 0);
			hbox.PackStart(this.label, true, true, 0);
			this.Add(hbox);
		}

		public ExtMenuItem (string label, Gdk.Pixbuf pixbuf, object extraData) {
			this.label = new Gtk.Label(label);
			this.image = new Gtk.Image(pixbuf);
			this.extraData = extraData;

			Gtk.HBox hbox = new Gtk.HBox(false, 0);
			hbox.PackStart(this.image, false, false, 0);
			hbox.PackStart(this.label, true, true, 0);
			this.Add(hbox);
		}

		public string Label {
			get { return(this.label.Text); }
			set { this.label.Text = value; }
		}

		public Gdk.Pixbuf Pixbuf {
			get { return(this.image.Pixbuf); }
			set { this.image.Pixbuf = value; }
		}

		public object ExtraData {
			get { return(this.extraData); }
			set { this.extraData = value; }
		}
	}
}
