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

using Gtk;

using System;
using System.IO;
using System.Collections;

namespace Niry.GUI.Gtk2 {
	internal class FolderNode : Gtk.TreeNode {
		[TreeNodeValue (Column=0)]
		public Gdk.Pixbuf Pixbuf;
		
		[TreeNodeValue (Column=1)]
		public string Name;

		public string Path;

		public FolderNode (Gdk.Pixbuf pixbuf, string name, string path) {
			this.Pixbuf = pixbuf;
			this.Name = name;
			this.Path = path;
		}
	}

	public class FolderChooser : Gtk.NodeView {
		// ========================================
		// PUBLIC Events
		// ========================================
		public event StringEventHandler Unselect = null;
		public event StringEventHandler Select = null;

		// ========================================
		// PRIVATE Members
		// ========================================
		private ArrayList sharedPaths;
		private Gtk.NodeStore store;
		private Gdk.Pixbuf pixbuf;
		private bool showHidden;

		// ========================================
		// PUBLIC Constructors
		// ========================================
		public FolderChooser (Gdk.Pixbuf folderPixbuf) {
			Initialize(folderPixbuf);

			// Load Drives
			string[] drives = Directory.GetLogicalDrives();
			if (drives != null) {
				foreach (string basePath in drives) {
					try {
						FillStore(null, basePath);
					} catch {}
				}
			}
		}

		public FolderChooser (Gdk.Pixbuf folderPixbuf, string basePath) {
			Initialize(folderPixbuf);
			FillStore(null, basePath);
		}

		// ========================================
		// PUBLIC Methods
		// ========================================
		public void UnselectPath (string path) {
			sharedPaths.Remove(path);
			//SetUnselected(node);
		}

		public bool IsSelected (string path) {
			return(sharedPaths.Contains(path));
		}

		// ========================================
		// PRIVATE (Methods) Event Handlers
		// ========================================
		private void OnRowExpanded (object o, RowExpandedArgs args) {
			//Gtk.Application.Invoke(delegate {
				NodeSelection.SelectPath(args.Path);

				if (NodeSelection.SelectedNode != null) {
					FolderNode node = NodeSelection.SelectedNode as FolderNode;
					FillStore(node, node.Path);
				}
			//});
		}

		private void OnRowCollapsed (object o, RowCollapsedArgs args) {
			//Gtk.Application.Invoke(delegate {
				NodeSelection.SelectPath(args.Path);

				if (NodeSelection.SelectedNode != null) {
					FolderNode node = NodeSelection.SelectedNode as FolderNode;
					UnFillStore(node);
				}
		//	});
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================
		protected override bool OnButtonPressEvent (Gdk.EventButton evnt) {
			if (evnt.Button == 3) {
				Gtk.Application.Invoke(delegate {
					if (NodeSelection.SelectedNode != null) {
						FolderNode node = NodeSelection.SelectedNode as FolderNode;
						if (node.Name.StartsWith("<b>") == false) {
							SetSelected(node);
							sharedPaths.Add(node.Path);
							// Raise Select Event
							if (Select != null) Select(this, node.Path);
						} else {
							SetUnselected(node);
							sharedPaths.Remove(node.Path);
							// Raise Unselect Event
							if (Unselect != null) Unselect(this, node.Path);
						}
					}
				});

				TreePath path;
				GetPathAtPos((int) evnt.X, (int) evnt.Y, out path);
				if (path != null && Selection.PathIsSelected(path)) {
					return true;
				}
			}
			return(base.OnButtonPressEvent(evnt));
		}

		// ========================================
		// PRIVATE Methods
		// ========================================
		private void FillStore (FolderNode parent, string path) {			
			DirectoryInfo currentDir = new DirectoryInfo(path);
			DirectoryInfo[] subDirs = currentDir.GetDirectories();
			if (subDirs == null) return;

			foreach (DirectoryInfo dir in subDirs) {
				// Skip if Hide Hidden is Set
				if (ShowFolder(dir.Name) == false)
					continue;

				FolderNode node = new FolderNode(pixbuf, dir.Name, dir.FullName);

				if (sharedPaths.Contains(dir.FullName) == true)
					SetSelected(node);

				if (parent == null) {
					store.AddNode(node);
				} else {
					parent.AddChild(node);
				}

				try {
					DirectoryInfo[] subSubDir = dir.GetDirectories();
					if (subSubDir != null && subSubDir.Length > 0) {
						node.AddChild(new FolderNode(pixbuf, ".", null));
					}
				} catch {}
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

		private bool ShowFolder (string name) {
			if (showHidden == true) return(true);
			return(name.StartsWith(".") == false);
		}

		private void Initialize (Gdk.Pixbuf folderPixbuf) {
			// Setup Shared Paths ArrayList
			sharedPaths = ArrayList.Synchronized(new ArrayList());
			showHidden = false;

			// Setup Folder Pixbuf
			this.pixbuf = folderPixbuf;

			// Initialize Trasfers Store
			store = new Gtk.NodeStore(typeof(FolderNode));

			// Initialize TreeView Properties
			HeadersVisible = false;
			Reorderable = false;
			NodeStore = store;
			//Selection.Mode = SelectionMode.Multiple;

			// Setup TreeView Columns
			AppendColumn("", new Gtk.CellRendererPixbuf(), "pixbuf", 0);
			AppendColumn("name", new Gtk.CellRendererText(), "markup", 1);

			// Initialize TreeView Event
			RowExpanded += new Gtk.RowExpandedHandler(OnRowExpanded);
			RowCollapsed += new Gtk.RowCollapsedHandler(OnRowCollapsed);
		}

		private void SetSelected (FolderNode node) {
			node.Name = "<b>" + node.Name + "</b> (Shared)";
		}

		private void SetUnselected (FolderNode node) {
			node.Name = node.Name.Substring(3, node.Name.Length - 16);
		}

		// ========================================
		// PUBLIC Properties
		// ========================================
		public string[] SharedPaths {
			get { return((string[]) sharedPaths.ToArray(typeof(string))); }
			set { foreach (string path in value) sharedPaths.Add(path); }
		}

		public NodeStore Store {
			get { return(this.store); }
		}

		public bool ShowHidden {
			get { return(this.showHidden); }
			set { this.showHidden = value; }
		}
	}
}
