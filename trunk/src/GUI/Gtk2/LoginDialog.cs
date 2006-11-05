/* [ GUI/Gtk2/LoginDialog.cs ] Gtk 2.x Login Dialog 
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
using GLib;

namespace Niry.GUI.Gtk2 {
	public class LoginDialog : Gtk.Dialog {
		// ============================================
		// PUBLIC Events
		// ============================================
		public event FocusOutEventHandler UserFocusOut = null;

		// ============================================
		// PROTECTED Members
		// ============================================
		protected Gtk.Image imageLogo;
		protected Gtk.Label labelUsername;
		protected Gtk.Entry entryUsername;
		protected Gtk.Label labelPassword;
		protected Gtk.Entry entryPassword;
		protected Gtk.CheckButton checkRememberPassword;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public LoginDialog() {
			InitializeLoginDialog();
		}

		public LoginDialog (Gdk.Pixbuf logo) {
			InitializeLoginDialog();
			this.Logo = logo;
		}

		public LoginDialog (string title, Window parent) : 
				base(title, parent, DialogFlags.Modal, null)
		{
			InitializeLoginDialog();
		}

		public LoginDialog (string title, 
							Window parent, 
							DialogFlags flags, 
							params object[] button_data) :
				base(title, parent, flags, button_data)
		{
			InitializeLoginDialog();
		}

		// ============================================
		// PRIVATE (Methods) Event Handlers
		// ============================================
		private void OnUsernameFocusOut (object o, FocusOutEventArgs args) {
			if (UserFocusOut != null) UserFocusOut(o, args);
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void InitializeLoginDialog() {
			// Logo Image
			this.imageLogo = new Gtk.Image();
			this.VBox.PackStart(this.imageLogo, false, false, 0);

			// Label Username
			this.labelUsername = new Gtk.Label("<b>Username:</b>");
			this.labelUsername.UseMarkup = true;
			this.labelUsername.Xalign = 0.0f;
			this.VBox.PackStart(this.labelUsername, false, false, 2);

			// Entry Username
			this.entryUsername = new Gtk.Entry();
			this.entryUsername.Completion = new EntryCompletion();
			this.entryUsername.FocusOutEvent += new FocusOutEventHandler(OnUsernameFocusOut);
			this.VBox.PackStart(this.entryUsername, false, false, 3);

			// Label Password
			this.labelPassword = new Gtk.Label("<b>Password:</b>");
			this.labelPassword.Xalign = 0.0f;
			this.labelPassword.UseMarkup = true;
			this.VBox.PackStart(this.labelPassword, false, false, 2);

			// Entry Password
			this.entryPassword = new Gtk.Entry();
			this.entryPassword.Visibility = false;
			this.VBox.PackStart(this.entryPassword, false, false, 3);

			// Check Button Remember Password
			this.checkRememberPassword = new CheckButton("Remember Password");
			this.VBox.PackStart(this.checkRememberPassword, false, false, 3);
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		public Gdk.Pixbuf Logo {
			set { this.imageLogo.Pixbuf = value; }
			get { return(this.imageLogo.Pixbuf); }
		}

		public string Username {
			get { return(this.entryUsername.Text); }
			set { 
				if (value == null) {
					this.entryUsername.Text = "";
				} else {
					this.entryUsername.Text = value;
				}
			}
		}

		public string Password {
			get { return(this.entryPassword.Text); }
			set { 
				if (value == null) {
					this.entryPassword.Text = "";
				} else {
					this.entryPassword.Text = value;
				}
			}
		}

		public bool RememberPassword {
			set { this.checkRememberPassword.Active = value; }
			get { return(this.checkRememberPassword.Active); }
		}

		public Gtk.EntryCompletion UserNameCompletion {
			get { return(this.entryUsername.Completion); }
		}
	}
}
