/* [ Utils/ArrayUtils.cs ] Niry Array Utils
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
	public static class ArrayUtils {
		public static object[] ExtractMissing (object[] full, object[] sub) {
			ArrayList missing = new ArrayList();
			foreach (object elem in full) {
				if (Array.IndexOf(sub, elem) < 0)
					missing.Add(elem);
			}
			return((missing.Count > 0) ? missing.ToArray() : null);
		}

		public static Array ExtractMissing (Array full, Array sub, Type type) {
			ArrayList missing = new ArrayList();
			foreach (object elem in full) {
				if (Array.IndexOf(sub, elem) < 0)
					missing.Add(elem);
			}
			return((missing.Count > 0) ? missing.ToArray(type) : null);
		}
	}
}
