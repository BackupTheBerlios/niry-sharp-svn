/* [ GUI/Gtk2/ProxySettings.cs ] Proxy Server Settings
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
	public class ProxySettings : VBox {
		// ============================================
		// PROTECTED Members
		// ============================================
		protected CheckButton ckEnableProxy;
		protected CheckButton ckProxyAuth;
		protected Label labelUsername;
		protected Entry entryUsername;
		protected Label labelPassword;
		protected Entry entryPassword;
		protected Label labelHost;
		protected Entry entryHost;
		protected Label labelPort;
		protected SpinButton spinPort;
		protected Table tableAuth;
		protected Table table;

		// ============================================
		// PUBLIC Constructors
		// ============================================
		public ProxySettings() : base (false, 2) {
			InitializeProxyHTTP();
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		private void InitializeProxyHTTP() {
			// Check Button (Enable Proxy)
			this.ckEnableProxy = new CheckButton("Enable Proxy");
			this.ckEnableProxy.Toggled += new EventHandler(EnableProxyToggled);
			this.ckEnableProxy.Active = false;
			this.PackStart(this.ckEnableProxy, false, false, 2);

			// Label Host
			this.labelHost = new Gtk.Label("<b>Proxy Host:</b>");
			this.labelHost.UseMarkup = true;
			this.labelHost.Xalign = 0.0f;

			// Entry Host
			this.entryHost = new Gtk.Entry();

			// Label Password
			this.labelPort = new Gtk.Label("<b>Proxy Port:</b>");
			this.labelPort.Xalign = 0.0f;
			this.labelPort.UseMarkup = true;

			// Entry Password
			this.spinPort = new Gtk.SpinButton(0.0f, 99999.0f, 1.0f);
			this.spinPort.Value = 80.0f;

			// Use Proxy Auth Table
			this.table = new Table(2, 2, false);
			this.table.RowSpacing = 3;
			this.table.ColumnSpacing = 3;
			this.table.Attach(this.labelHost, 0, 1, 0, 1);
			this.table.Attach(this.entryHost, 1, 2, 0, 1);
			this.table.Attach(this.labelPort, 0, 1, 1, 2);
			this.table.Attach(this.spinPort, 1, 2, 1, 2);
			this.PackStart(this.table, false, false, 2);

			// Check Button (Use Proxy Authentication)
			this.ckProxyAuth = new CheckButton("Use Proxy Authentication");
			this.ckProxyAuth.Toggled += new EventHandler(UseProxyAuthToggled);
			this.ckProxyAuth.Active = false;
			this.PackStart(this.ckProxyAuth, false, false, 2);

			// Label UserName
			this.labelUsername = new Gtk.Label("<b>Proxy Username:</b>");
			this.labelUsername.UseMarkup = true;
			this.labelUsername.Xalign = 0.0f;

			// Entry UserName
			this.entryUsername = new Gtk.Entry();

			// Label Password
			this.labelPassword = new Gtk.Label("<b>Proxy Password:</b>");
			this.labelPassword.Xalign = 0.0f;
			this.labelPassword.UseMarkup = true;

			// Entry Password
			this.entryPassword = new Gtk.Entry();
			this.entryPassword.Visibility = false;

			// Use Proxy Auth Table
			this.tableAuth = new Table(2, 2, false);
			this.tableAuth.RowSpacing = 3;
			this.tableAuth.ColumnSpacing = 3;
			this.tableAuth.Attach(this.labelUsername, 0, 1, 0, 1);
			this.tableAuth.Attach(this.entryUsername, 1, 2, 0, 1);
			this.tableAuth.Attach(this.labelPassword, 0, 1, 1, 2);
			this.tableAuth.Attach(this.entryPassword, 1, 2, 1, 2);
			this.PackStart(this.tableAuth, false, false, 2);

			EnableProxyToggled(this.ckEnableProxy, null);
			UseProxyAuthToggled(this.ckProxyAuth, null);
		}

		// ============================================
		// PRIVATE (Methods) Event Handler
		// ============================================
		private void EnableProxyToggled (object sender, EventArgs args) {
			this.table.Sensitive = EnableProxy;
			this.ckProxyAuth.Sensitive = EnableProxy;
			this.tableAuth.Sensitive = (EnableProxy) ? UseProxyAuth : false;
		}

		private void UseProxyAuthToggled (object sender, EventArgs args) {
			this.tableAuth.Sensitive = UseProxyAuth && EnableProxy;
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		public bool EnableProxy {
			set { this.ckEnableProxy.Active = value; }
			get { return(this.ckEnableProxy.Active); }
		}

		public bool UseProxyAuth {
			set { this.ckProxyAuth.Active = value; }
			get { return(this.ckProxyAuth.Active); }
		}

		public string Host {
			set { 
				if (value == null) value = "";
				this.entryHost.Text = value;
			}
			get { return(this.entryHost.Text); }
		}

		public int Port {
			set {this.spinPort.Value = value; }
			get { return(this.spinPort.ValueAsInt); }
		}

		public string Username {
			set {
				if (value == null) value = "";
				this.entryUsername.Text = value;
			}
			get { return(this.entryUsername.Text); }
		}

		public string Password {
			set {
				if (value == null) value = "";
				this.entryPassword.Text = value;
			}
			get { return(this.entryPassword.Text); }
		}
	}
}
