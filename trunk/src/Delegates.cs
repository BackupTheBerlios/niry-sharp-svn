/* [ Delegates.cs ] - Niry Base Delegates
 * Author: Matteo Bertozzi
 * =============================================================================
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
using System.Net;
using System.Net.Sockets;

namespace Niry {
	// ============================================
	// PUBLIC Delegates
	// ============================================
	/// Event Handler Without Arguments but with return value
	public delegate bool DecisionBlankEventHandler (object sender);

	// Event Handler Without Arguments but with Object as return value
	public delegate object RequestBlankEventHandler (object sender);

	// Event Handler Without Arguments but with Array as return value
	public delegate object[] MultiRequestBlankEventHandler (object sender);

	// Event Handler with String Arguments but with Array as return value
	public delegate object[] MultiRequestStringEventHandler (object sender, string arg);

	/// Event Handler without Arguments
	public delegate void BlankEventHandler (object sender);

	/// Event Handler with String Argument
	public delegate void StringEventHandler (object sender, string arg);

	/// Event Handler with Object Argument
	public delegate void ObjectEventHandler (object sender, object arg);

	/// Event handler with 2 Object Arguments
	public delegate void ThreeObjectEventHandler (object sender, object arg1, object arg2);

	/// Event Handler with Socket Argument
	public delegate void SocketEventHandler (object sender, Socket arg);

	/// Event Handler with Int Argument
	public delegate void IntEventHandler (object sender, int arg);

	/// Event Handler with Bool Argument
	public delegate void BoolEventHandler (object sender, bool arg);

	/// Event Handler with Exception Argument
	public delegate void ExceptionEventHandler (object sender, Exception arg);
}
