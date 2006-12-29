/* [ GUI/Gtk2/ImageCombo.cs ] - Gtk 2.x Image/Text ComboBox
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
	public class ImageCombo : Gtk.ComboBox {
		// ========================================
		// PRIVATE Members
		// ========================================
		private int imageCell = 0;
		private int textCell = 1;

		// ========================================
		// PUBLIC Constructors
		// ========================================
		public ImageCombo() : this(new Gtk.ListStore(typeof(Gdk.Pixbuf), typeof(string)), 0, 1) {
		}

		public ImageCombo (Gtk.ListStore store, int image, int text) {
			// Setup Text Cell Position
			this.imageCell = image;
			this.textCell = text;

			// Setup Model
			Model = store;

			// Initialize Combo Image Cell
			Gtk.CellRenderer cellImage = new Gtk.CellRendererPixbuf();
			PackStart(cellImage, false);
			SetAttributes(cellImage, "pixbuf", this.imageCell);

			// Initialize Combo Text Cell
			Gtk.CellRenderer cellText = new Gtk.CellRendererText();
			PackStart(cellText, true);
			SetAttributes(cellText, "text", this.textCell);
		}

		// ========================================
		// PUBLIC (Append) Methods
		// ========================================
		public void Append (params object[] row) {
			Gtk.ListStore store = Model as Gtk.ListStore;
			store.AppendValues(row);
		}

		public void Append (System.Array row) {
			Gtk.ListStore store = Model as Gtk.ListStore;
			store.AppendValues(row);
		}

		public new void AppendText (string text) {
			Gtk.ListStore store = Model as Gtk.ListStore;
			TreeIter iter = store.Append();
			store.SetValue(iter, this.textCell, text);
		}

		public void AppendText (Gdk.Pixbuf pixbuf, string text) {
			Gtk.ListStore store = Model as Gtk.ListStore;
			TreeIter iter = store.Append();
			store.SetValue(iter, this.imageCell, pixbuf);
			store.SetValue(iter, this.textCell, text);
		}

		// ========================================
		// PUBLIC (Insert) Methods
		// ========================================
		public new void InsertText (int position, string text) {
			Gtk.ListStore store = Model as Gtk.ListStore;
			TreeIter iter = store.Insert(position);
			store.SetValue(iter, this.textCell, text);
		}

		public void InsertText (int position, Gdk.Pixbuf pixbuf, string text) {
			Gtk.ListStore store = Model as Gtk.ListStore;
			TreeIter iter = store.Insert(position);
			store.SetValue(iter, this.imageCell, pixbuf);
			store.SetValue(iter, this.textCell, text);
		}

		// ========================================
		// PUBLIC Properties
		// ========================================
		public Gtk.ListStore Store {
			get { return((Gtk.ListStore) Model); }
		}
	}
}
