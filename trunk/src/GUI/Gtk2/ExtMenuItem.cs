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
	public class ExtMenuItem : Gtk.ImageMenuItem {
		// ========================================
		// PRIVATE Members
		// ========================================
		object extraData;

		// ========================================
		// PUBLIC Constructors
		// ========================================
		public ExtMenuItem (string label, object extraData) : base(label) {
			this.extraData = extraData;
		}

		public ExtMenuItem (string label, string stock_id, object extraData) : base(label)
		{
			this.Image = new Gtk.Image(stock_id, Gtk.IconSize.Menu);
			this.extraData = extraData;
		}

		public ExtMenuItem (string label, Gdk.Pixbuf pixbuf, object extraData) : base(label) {
			Image = new Gtk.Image(pixbuf);
			this.extraData = extraData;
		}

		// ========================================
		// PUBLIC Properties
		// ========================================
		public Gdk.Pixbuf Pixbuf {
			get { return(((Gtk.Image) this.Image).Pixbuf); }
			set { ((Gtk.Image) this.Image).Pixbuf = value; }
		}

		public object ExtraData {
			get { return(this.extraData); }
			set { this.extraData = value; }
		}
	}
}
