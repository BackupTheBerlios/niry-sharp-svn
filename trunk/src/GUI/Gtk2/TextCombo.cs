/* [ GUI/Gtk2/TextCombo.cs ] - Gtk 2.x Text ComboBox
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
	public class TextCombo : Gtk.ComboBox {
		// ========================================
		// PRIVATE Members
		// ========================================
		private int textCell = 0;

		// ========================================
		// PUBLIC (Append) Methods
		// ========================================
		public TextCombo() : this(new Gtk.ListStore(typeof(string)), 0) {
		}

		public TextCombo (Gtk.ListStore store, int text) {
			// Setup Text Cell Position
			this.textCell = text;

			// Initialize ComboBox
			Gtk.CellRenderer cell = new Gtk.CellRendererText();
			Model = store;
			PackStart(cell, true);
			SetAttributes(cell, "text", this.textCell);
		}

		// ========================================
		// PUBLIC Methods
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

		public void AppendText (string[] entries) {
			foreach (string text in entries) {
				AppendText(text);
			}
		}

		// ========================================
		// PUBLIC (Insert) Methods
		// ========================================
		public new void InsertText (int position, string text) {
			Gtk.ListStore store = Model as Gtk.ListStore;
			TreeIter iter = store.Insert(position);
			store.SetValue(iter, this.textCell, text);
		}

		// ========================================
		// PUBLIC (Clear) Methods
		// ========================================
		public new void Clear() {
			Gtk.ListStore store = Model as Gtk.ListStore;
			store.Clear();
		}

		// ========================================
		// PUBLIC (Get) Methods
		// ========================================
		public string GetActiveText() {
			TreeIter iter;
			if (GetActiveIter(out iter))
				return((string) Model.GetValue(iter, textCell));
			return(null);
		}

		// ========================================
		// PUBLIC Properties
		// ========================================
		public Gtk.ListStore Store {
			get { return((Gtk.ListStore) Model); }
		}
	}
}
