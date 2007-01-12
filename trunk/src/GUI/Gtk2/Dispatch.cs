/* [ GUI/Gtk2/Dispatch.cs ] - Gtk 2.x Dispatch (GUI Event Handler)
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
	public class Dispatch {
		// ========================================
		// PRIVATE Members
		// ========================================
		private object methodClass = null;
		private object[] arguments = null;
		private string methodName = null;
		private Thread thread = null;

		// ========================================
		// PUBLIC Constructors
		// ========================================
		public Dispatch (ThreadStart method) {
			thread = new Thread(method);
			thread.Start();
		}
   		
		public Dispatch (object methodClass,
						 string methodName,
						 object[] arguments)
		{
			this.methodClass = methodClass;
			this.methodName = methodName;
			this.arguments = arguments;

			thread = new Thread(new ThreadStart(Go));
			thread.Start();
		}

		// ========================================
		// PUBLIC STATIC Methods
		// ========================================
		public static void Run (ThreadStart method) {
			new Dispatch(method);
		}

		public static void Run (object methodClass,
								string methodName,
								object[] arguments)
		{
			new Dispatch(methodClass, methodName, arguments);
		}

		public static void GUIRun (EventHandler method) {
			Gtk.Application.Invoke(method);
		}

		// ========================================
		// PRIVATE Methods
		// ========================================
		private void Go() {
			Type t = methodClass.GetType();
			t.InvokeMember (methodName, BindingFlags.Default | 
							BindingFlags.Public | BindingFlags.NonPublic | 
							BindingFlags.InvokeMethod, null,
							methodClass, arguments);
		}

		// ========================================
		// PUBLIC Properties
		// ========================================
		public Thread Thread {
			get { return(this.thread); }
		}
	}
}
