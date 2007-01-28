/* [ GUI/Gtk2/FolderChooser.cs ] - Gtk 2.x FolderChooser
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
using System.Threading;
using System.Reflection;

namespace Niry.GUI.Gtk2 {
	internal class FolderNode : Gtk.TreeNode {
		[TreeNodeValue (Column=0)]
		public Gdk.Pixbuf Pixbuf;
		
		[TreeNodeValue (Column=1)]
		public string Name;

		public FolderNode (Gdk.Pixbuf pixbuf, string name) {
			this.Pixbuf = pixbuf;
			this.Name = name;
		}
	}

	public class FolderChooser : Gtk.NodeView {
		// ========================================
		// PRIVATE Members
		// ========================================
		private Gtk.NodeStore store;
		private Gdk.Pixbuf pixbuf;

		// ========================================
		// PUBLIC Constructors
		// ========================================
		public FolderChooser (Gdk.Pixbuf folderPixbuf, string basePath) {
			// Setup Folder Pixbuf
			this.pixbuf = folderPixbuf;

			// Initialize Trasfers Store
			store = new Gtk.NodeStore(typeof(FolderNode));
			FillStore(null, basePath);

			// Initialize TreeView Properties
			HeadersVisible = false;
			Reorderable = false;
			NodeStore = store;
			Selection.Mode = SelectionMode.Multiple;

			// Setup TreeView Columns
			AppendColumn("", new Gtk.CellRendererPixbuf(), "pixbuf", 0);
			AppendColumn("Name", new Gtk.CellRendererText(), "text", 1);

			// Initialize TreeView Event
			RowExpanded += new RowExpandedHandler(OnRowExpanded);
			RowCollapsed += new RowCollapsedHandler(OnRowCollapsed);
		}

		// ========================================
		// PRIVATE (Methods) Event Handlers
		// ========================================
		private void OnRowExpanded (object o, RowExpandedArgs args) {
			Gtk.Application.Invoke(delegate {
				NodeSelection.SelectPath(args.Path);

				if (NodeSelection.SelectedNode != null) {
					FolderNode node = NodeSelection.SelectedNode as FolderNode;
					FillStore(node, node.Name);
				}
			});
		}

		private void OnRowCollapsed (object o, RowCollapsedArgs args) {
			Gtk.Application.Invoke(delegate {
				NodeSelection.SelectPath(args.Path);

				if (NodeSelection.SelectedNode != null) {
					FolderNode node = NodeSelection.SelectedNode as FolderNode;
					UnFillStore(node);
				}
			});
		}

		// ========================================
		// PRIVATE Methods
		// ========================================
		private void FillStore (FolderNode parent, string path) {
			string[] subDirs = Directory.GetDirectories(path);

			foreach (string dir in subDirs) {
				FolderNode node = new FolderNode(pixbuf, dir);
				if (parent == null) {
					store.AddNode(node);
					try {
						if (Directory.GetDirectories(dir).Length > 0) {
							node.AddChild(new FolderNode(pixbuf, "."));
						}
					} catch {}
				} else {
					parent.AddChild(node);
				}
			}

			// Remove First Node (Temp For Expander)
			if (parent != null && parent.ChildCount > 0)
				parent.RemoveChild((Gtk.TreeNode) parent[0]);
		}

		private void UnFillStore (FolderNode node) {
			while (node.ChildCount > 1) {
				node.RemoveChild((Gtk.TreeNode) node[0]);
			}
		}

		// ========================================
		// PUBLIC Properties
		// ========================================
		public NodeStore Store {
			get { return(this.store); }
		}
	}
}
