/* [ Utils/HashList.cs ] 
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
using System.Collections;

namespace Niry.Utils {
	public class HashList : IDictionary {
		// ============================================
		// PROTECTED Members
		// ============================================
		protected Hashtable data = null;

		// ============================================
		// PUBLIC Constructor
		// ============================================
		public HashList() {
			this.data = Hashtable.Synchronized(new Hashtable());
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Adds an entry with the provided key and value
		public void Add (object key, object value) {
			AddToList(key, value);
		}

		/// Removes all key and value pairs
		public void Clear() {
			RemoveAll();
		}

		/// Determines whether the instance contains an entry with the specified key
		public bool Contains (object key) {
			return(this.data.Contains(key));
		}

		/// Determines whether the instance contains an entry with the specified key
		public bool ContainsKey (object key) {
			return(this.data.ContainsKey(key));
		}

		/// Determines whether the instance contains an entry with the specified value
		public bool ContainsValue (object key, object value) {
			return(ListContains(key, value));
		}

		/// Copies the entries of the current instance to a one-dimensional 
		/// System.Array starting at the specified index.
		public void CopyTo (System.Array array, int arrayIndex) {
			this.data.CopyTo(array, arrayIndex);
		}

		/// Returns a IDictionaryEnumerator
		public IDictionaryEnumerator GetEnumerator() {
			return(this.data.GetEnumerator());
		}

		/// Removes the entry with the specified key
		public void Remove (object key) {
			RemoveList(key);
		}

		/// Removes the entry with the specified value from the key values.
		public void Remove (object key, object value) {
			RemoveFromList(key, value);
		}

		// ============================================
		// PROTECTED Methods
		// ============================================
		protected bool ListContains (object key, object value) {
			lock (this.data) {
				if (this.data.ContainsKey(key) == true) {
					ArrayList list = this.data[key] as ArrayList;
					return(list.Contains(value));
				}
				return(false);
			}
		}

		// ============================================
		// PRIVATE (Atomic) Methods
		// ============================================
		private void AddToList (object key, object value) {
			lock (this.data) {
				ArrayList list = null;
				if (this.data.ContainsKey(key) == false) {
					list = ArrayList.Synchronized(new ArrayList());
				} else {
					list = this.data[key] as ArrayList;
				}
				list.Add(value);
				this.data[key] = list;
			}
		}

		private void RemoveFromList (object key, object value) {
			lock (this.data) {
				if (this.data.ContainsKey(key) == true) {
					ArrayList list = this.data[key] as ArrayList;
					list.Remove(value);
					this.data[key] = list;
				}
			}
		}

		private void RemoveList (object key) {
			lock (this.data) {
				if (this.data.ContainsKey(key) == true) {
					ArrayList list = this.data[key] as ArrayList;
					list.Clear();
					list = null;
					this.data[key] = list;
					this.data.Remove(key);
				}
			}
		}

		private void RemoveAll() {
			lock (this.data) {
				foreach (object key in this.data.Keys) {
					ArrayList list = this.data[key] as ArrayList;
					list.Clear();
					list = null;
					this.data[key] = list;
				}
				this.data.Clear();
			}
		}

		// ============================================
		// PRIVATE Methods
		// ============================================
		IEnumerator IEnumerable.GetEnumerator() {
			return(((IDictionary) this).GetEnumerator());
		}

		// ============================================
		// PUBLIC Properties
		// ============================================
		/// Gets a System.Boolean indicating whether the current instance has a fixed size
		public bool IsFixedSize {
			get { return(this.data.IsFixedSize); }
		}

		/// Gets a value indicating whether the current instance is read-only
		public bool IsReadOnly {
			get { return(this.data.IsReadOnly); }
		}
 
		/// Gets or sets the element at the specified index in the current instance.
		public object this[object key] {
			get { return(this.data[key]); }
			set { this.data[key] = value; }
		}

		/// Gets a System.Boolean value indicating whether access to 
		/// the current instance is synchronized (thread-safe)
		public bool IsSynchronized {
			get { return(this.data.IsSynchronized); }
		}

		/// Gets a System.Object that can be used to synchronize 
		/// access to the current instance
		public object SyncRoot {
			get { return(this.data.SyncRoot); }
		}

		/// Gets the number of keys contained in the current instance.
		public int Count {
			get { return(this.data.Count); }
		}

		/// Gets a ICollection containing the keys of the current instance.
		public ICollection Keys {
			get { return(this.data.Keys); }
		}

		/// Gets a ICollection containing the values of the current instance.
		public ICollection Values {
			get { return(this.data.Values); }
		}


		public static void Main() {
			HashList hashList = new HashList();
			hashList.Add("Prova", "Ciao 1");
			hashList.Add("Prova", "Ciao 2");
			hashList.Add("Prova", "Ciao 3");
			hashList.Add("Pippo", "3 Ciao");
			hashList.Add("Pippo", "2 Ciao");
			hashList.Add("Pippo", "1 Ciao");

			Console.WriteLine();
			foreach (ArrayList list in hashList.Values) {
				foreach (string s in list)
					Console.WriteLine(s);
			}

			Console.WriteLine();
			hashList.Remove("Prova", "Ciao 1");
			foreach (ArrayList list in hashList.Values) {
				foreach (string s in list)
					Console.WriteLine(s);
			}

			Console.WriteLine();
			hashList.Remove("Pippo");
			foreach (ArrayList list in hashList.Values) {
				foreach (string s in list)
					Console.WriteLine(s);
			}
		}
	}
}
