/* [ Database/Sqlite ] - Niry Sqlite
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
using System.IO;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using Mono.Data.SqliteClient;

namespace Niry.Database {
	/// SQLite Utils (SQLite 3)
	public class SQLite : SqliteConnection {
		// ============================================
		// PUBLIC Events
		// ============================================

		// ============================================
		// PROTECTED Members
		// ============================================

		// ============================================
		// PRIVATE Members
		// ============================================

		// ============================================
		// PUBLIC Constructors
		// ============================================
		/// Create New Sqlite Connection From File
		public SQLite (string fileName) : base("URI=file:" + fileName +",version=3") {
		}

		/// Destroy SQLite Connection
		~SQLite() {
			this.Dispose();
		}

		// ============================================
		// PUBLIC Methods
		// ============================================
		/// Return True if Specified Table Exists
		public bool TableExists (string table) {
			string sql = "SELECT name FROM sqlite_master WHERE type='table' AND name=@Table;";
			SqliteCommand sqlCmd = CreateCommand();
			sqlCmd.CommandText = sql;
			sqlCmd.Parameters.Add("@Table", table);
			SqliteDataReader sqlReader = sqlCmd.ExecuteReader();

			bool tableExists = false;
			if (sqlReader.Read())
				tableExists = true;

			sqlReader.Close();
			return(tableExists);
		}

		// Create SQL Command Without Param
		public SqliteCommand CreateCommand (string sql) {
			SqliteCommand sqlCmd = CreateCommand();
			sqlCmd.CommandText = sql;
			return(sqlCmd);
		}

		// Create SQL Command With Param
		public SqliteCommand CreateCommand (string sql, Hashtable sqlParams) {
			SqliteCommand sqlCmd = CreateCommand();
			sqlCmd.CommandText = sql;
			if (sqlParams != null) {
				foreach (string key in sqlParams.Keys)
					sqlCmd.Parameters.Add(key, sqlParams[key]);
			}
			return(sqlCmd);
		}

		/// Execute Non Query
		public int ExecuteNonQuery (string sql) {
			SqliteCommand sqlCmd = CreateCommand(sql);
			int ret = sqlCmd.ExecuteNonQuery();
			sqlCmd.Dispose();
			return(ret);
		}

		/// Execute Non Query and Get Inserted ID
		public int ExecuteNonQueryGetID (string sql, Hashtable sqlParams) {
			SqliteCommand sqlCmd = CreateCommand(sql, sqlParams);
			sqlCmd.ExecuteNonQuery();
			int id = sqlCmd.LastInsertRowID();
			sqlCmd.Dispose();
			return(id);
		}

		/// Execute Query and Read The Only Int Value
		public int ExecuteReadInt (string sql, Hashtable sqlParams) {
			SqliteCommand sqlCmd = CreateCommand(sql, sqlParams);
			SqliteDataReader reader = sqlCmd.ExecuteReader();

			int intValue = -1;
			if (reader.Read())
				intValue = reader.GetInt32(0);

			reader.Dispose();
			sqlCmd.Dispose();
			return(intValue);
		}

		/// Execute Query and Read The Only Int Value
		public string ExecuteReadString (string sql) {
			return(ExecuteReadString(sql, null));
		}

		/// Execute Query and Read The Only Int Value
		public string ExecuteReadString (string sql, Hashtable sqlParams) {
			SqliteCommand sqlCmd = CreateCommand(sql, sqlParams);
			SqliteDataReader reader = sqlCmd.ExecuteReader();

			string strValue = null;
			if (reader.Read())
				strValue = reader.GetString(0);

			reader.Dispose();
			sqlCmd.Dispose();
			return(strValue);
		}

		/// Execute Query and Read The String Array
		public string[] ExecuteReadStrings (string sql) {
			return(ExecuteReadStrings(sql, null));
		}

		/// Execute Query and Read The String Array
		public string[] ExecuteReadStrings (string sql, Hashtable sqlParams) {
			SqliteCommand sqlCmd = CreateCommand(sql, sqlParams);
			SqliteDataReader reader = sqlCmd.ExecuteReader();
			
			ArrayList list = new ArrayList();
			while (reader.Read()) {				
				list.Add(reader.GetString(0));
			}

			reader.Dispose();
			sqlCmd.Dispose();
			return(list.Count > 0 ? (string[]) list.ToArray(typeof(string)) : null);
		}

		// ============================================
		// PROTECTED (Methods) Event Handlers
		// ============================================

		// ============================================
		// PRIVATE Methods
		// ============================================

		// ============================================
		// PROTECTED Properties
		// ============================================

		// ============================================
		// PUBLIC Properties
		// ============================================
	}
}
