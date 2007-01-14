/* [ Utils/TimeUtils.cs ] 
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

namespace Niry.Utils {
	public static class TimeUtils {
		/// Returns a string of the amount of time the integer 
		/// (in seconds) refers to.
		public static string TimeLeft (int seconds) {
			double exact_days, exact_hours, exact_minutes, exact_seconds;
			int weeks, days, hours, minutes;
			string time = "";

			exact_days = exact_hours = exact_minutes = exact_seconds = 0;
			weeks = days = hours = minutes = 0;

			if ((seconds / 60) >= 1) {
				// Minutes
				exact_seconds = (double) ((double) seconds / 60);
				minutes = (int) Math.Floor(exact_seconds);
				if ((minutes / 60) >= 1) {
					// Hours
					exact_minutes = (double) ((double) minutes / 60);
					hours = (int) Math.Floor(exact_minutes);
					if ((hours / 24) >= 1) {
						// Days
						exact_hours = (double) ((double) hours / 24);
						days = (int) Math.Floor(exact_hours);
						if ((days / 7) >= 1) {
							// Weeks
							exact_days = (double) ((double) days / 60);
							weeks = (int) Math.Floor(exact_days);
							if (weeks >= 2) {
								time = weeks.ToString() + " Weeks";
							} else {
								time = weeks.ToString() + " Week";
							}
						}

						days -= ((int) (Math.Floor(exact_days)) * 7);
						if (weeks >= 1 && days >= 1) time += ", ";
						if (days >= 2) time += days.ToString() + " days";
						if (days == 1) time += days.ToString() + " day";
					}

					hours -= ((int) Math.Floor(exact_hours)) * 24;
					if (days >= 1 && hours >= 1) time += ", ";
					if (hours >= 2) time += hours.ToString() + " hours";
					if (hours == 1) time += hours.ToString() + " hour";
				}

				minutes -= ((int) Math.Floor(exact_minutes)) * 60;
				if (hours >= 1 && minutes >= 1) time += ", ";
				if (minutes >= 2) time += minutes.ToString() + " minutes";
				if (minutes == 1) time += minutes.ToString() + " minute";
			}

			seconds -= ((int) Math.Floor(exact_seconds)) * 60;
			if (minutes >= 1 && seconds >= 1) time += ", ";
			if (seconds >= 2) time += seconds.ToString() + " seconds";
			if (seconds == 1) time += seconds.ToString() + " second";

			return(time + ".");
		}
	}
}
