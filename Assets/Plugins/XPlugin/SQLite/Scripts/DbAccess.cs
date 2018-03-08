//
// DbAccess.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System;
using System.Data;
using System.Collections.Generic;
using Mono.Data.Sqlite;

namespace XPlugin.Data.SQLite {

	public class DbAccess : IDisposable {
		private SqliteConnection dbConnection = null;
		private SqliteCommand dbCommand = null;
		private SqliteDataReader dbReader = null;

		#region Base operation

		/// <summary>
		/// 打开Sqlite文件
		/// </summary>
		/// <param name="dbFilePath"></param>
		public bool OpenFromFile(string dbFilePath, string password) {
			return OpenWithConnStr("Data Source=" + dbFilePath, password);
		}

		/// <summary>
		/// 用自定义的连接字符串打开数据库
		/// </summary>
		/// <param name="connectionString"></param>
		public bool OpenWithConnStr(string connectionString, string password) {
			try {
				dbConnection = new SqliteConnection(connectionString);
				if (!string.IsNullOrEmpty(password)) {
					dbConnection.SetPassword(password);
				}
				dbConnection.Open();
				return true;
			} catch (Exception exception) {
				//dbConnection = null;
				Debug.LogException(exception);
				return false;
			}
		}

		/// <summary>
		/// 数据库是否打开
		/// </summary>
		/// <returns></returns>
		public bool IsOpen {
			get {
				if (dbConnection != null) {
					return (dbConnection.State & System.Data.ConnectionState.Open) != 0;
				} else {
					return false;
				}
			}
		}

		/// <summary>
		/// 关闭数据库
		/// </summary>
		public void CloseDB() {
			if (dbReader != null) {
				columnCache.Remove(dbReader);
				dbReader.Close();
				dbReader.Dispose();
				dbReader = null;
			}

			if (dbCommand != null) {
				dbCommand.Dispose();
				dbCommand = null;
			}

			if (dbConnection != null) {
				dbConnection.Close();
				dbConnection.Dispose();
				dbConnection = null;
			}
			//AllowDebug.Log("Disconnected from db.");
		}

		public SqliteConnection Connection {
			get {
				return dbConnection;
			}
		}

		public SqliteDataReader ExecuteQuery(string sqlQuery) {
			if (dbReader != null) {
				columnCache.Remove(dbReader);
				dbReader.Close();
				dbReader.Dispose();
				dbReader = null;
			}

			if (dbCommand != null) {
				dbCommand.Dispose();
				dbCommand = null;
			}

			dbCommand = dbConnection.CreateCommand();
			dbCommand.CommandText = sqlQuery;
			dbReader = dbCommand.ExecuteReader();
			ReadColumnToCache();
			return dbReader;
		}

		#endregion

		#region Create

		public SqliteDataReader CreateTable(string name, string[] col, string[] colType) {
			if (col.Length != colType.Length) {
				throw new SqliteException("columns.Length != colType.Length");
			}
			string[] textArray1 = new string[] { "CREATE TABLE ", name, " (", col[0], " ", colType[0] };
			string sqlQuery = string.Concat(textArray1);
			for (int i = 1; i < col.Length; i++) {
				string str2 = sqlQuery;
				string[] textArray2 = new string[] { str2, ", ", col[i], " ", colType[i] };
				sqlQuery = string.Concat(textArray2);
			}
			sqlQuery = sqlQuery + ")";
			return ExecuteQuery(sqlQuery);
		}

		#endregion

		#region Query

		public int GetTabelRecordsCount(string tableName) {
			string sqlQuery = "SELECT * FROM " + tableName;
			SqliteCommand cmd = dbConnection.CreateCommand();
			cmd.CommandText = sqlQuery;
			return (int)cmd.ExecuteScalar();
		}

		public SqliteDataReader ReadFullTable(string tableName) {
			string sqlQuery = "SELECT * FROM " + tableName;
			return ExecuteQuery(sqlQuery);
		}

		public SqliteDataReader SelectWhere(string tableName, string[] items, string[] col, string[] operation, string[] values) {
			if ((col.Length != operation.Length) || (operation.Length != values.Length)) {
				throw new SqliteException("col.Length != operation.Length != values.Length");
			}
			string sqlQuery = "SELECT " + items[0];
			for (int i = 1; i < items.Length; i++) {
				sqlQuery = sqlQuery + ", " + items[i];
			}
			string str2 = sqlQuery;
			string[] textArray1 = new string[] {
			str2,
			" FROM ",
			tableName,
			" WHERE ",
			col [0],
			operation [0],
			"'",
			values [0],
			"' "
		};
			sqlQuery = string.Concat(textArray1);
			for (int j = 1; j < col.Length; j++) {
				str2 = sqlQuery;
				string[] textArray2 = new string[] { str2, " AND ", col[j], operation[j], "'", values[0], "' " };
				sqlQuery = string.Concat(textArray2);
			}
			return ExecuteQuery(sqlQuery);
		}

		#endregion

		#region Modufy

		public SqliteDataReader InsertInto(string tableName, string[] values) {
			string sqlQuery = "INSERT INTO " + tableName + " VALUES (" + values[0];
			for (int i = 1; i < values.Length; i++) {
				sqlQuery = sqlQuery + ", " + values[i];
			}
			sqlQuery = sqlQuery + ")";
			return ExecuteQuery(sqlQuery);
		}

		public SqliteDataReader InsertIntoSpecific(string tableName, string[] cols, string[] values) {
			if (cols.Length != values.Length) {
				throw new SqliteException("columns.Length != values.Length");
			}
			string sqlQuery = "INSERT INTO " + tableName + "(" + cols[0];
			for (int i = 1; i < cols.Length; i++) {
				sqlQuery = sqlQuery + ", " + cols[i];
			}
			sqlQuery = sqlQuery + ") VALUES (" + values[0];
			for (int j = 1; j < values.Length; j++) {
				sqlQuery = sqlQuery + ", " + values[j];
			}
			sqlQuery = sqlQuery + ")";
			return ExecuteQuery(sqlQuery);
		}

		public SqliteDataReader UpdateInto(string tableName, string[] cols, string[] colsvalues, string selectkey, string selectvalue) {
			string str2;
			string[] textArray1 = new string[] { "UPDATE ", tableName, " SET ", cols[0], " = ", colsvalues[0] };
			string sqlQuery = string.Concat(textArray1);
			for (int i = 1; i < colsvalues.Length; i++) {
				str2 = sqlQuery;
				string[] textArray2 = new string[] { str2, ", ", cols[i], " =", colsvalues[i] };
				sqlQuery = string.Concat(textArray2);
			}
			str2 = sqlQuery;
			string[] textArray3 = new string[] { str2, " WHERE ", selectkey, " = ", selectvalue, " " };
			sqlQuery = string.Concat(textArray3);
			return ExecuteQuery(sqlQuery);
		}

		#endregion

		#region Delete

		public void DeleteFullTable(string tableName) {
			string sqlQuery = "delete  FROM " + tableName;
			ExecuteQuery(sqlQuery);
		}

		public SqliteDataReader DeleteContents(string tableName) {
			string sqlQuery = "DELETE FROM " + tableName;
			return ExecuteQuery(sqlQuery);
		}

		public SqliteDataReader Delete(string tableName, string[] cols, string[] colsvalues) {
			string[] textArray1 = new string[] { "DELETE FROM ", tableName, " WHERE ", cols[0], " = ", colsvalues[0] };
			string sqlQuery = string.Concat(textArray1);
			for (int i = 1; i < colsvalues.Length; i++) {
				string str2 = sqlQuery;
				string[] textArray2 = new string[] { str2, " or ", cols[i], " = ", colsvalues[i] };
				sqlQuery = string.Concat(textArray2);
			}
			return ExecuteQuery(sqlQuery);
		}

		#endregion

		#region Column Cache

		private static Dictionary<SqliteDataReader, Dictionary<string, int>> columnCache = new Dictionary<SqliteDataReader, Dictionary<string, int>>();

		public static int GetColumnIndex(SqliteDataReader reader, string name) {
			Dictionary<string, int> cache;
			columnCache.TryGetValue(reader, out cache);
			if (cache != null) {
				int index;
				if (cache.TryGetValue(name, out index)) {
					return index;
				}
			}
			//return reader.GetOrdinal(name);
			return -1;
		}

		private void ReadColumnToCache() {
			if (dbReader != null) {
				Dictionary<string, int> cache;
				columnCache.TryGetValue(dbReader, out cache);
				if (cache == null) {
					cache = new Dictionary<string, int>();
					columnCache.Add(dbReader, cache);
				}

				int count = dbReader.FieldCount;
				for (int i = 0; i < count; i++) {
					string name = dbReader.GetName(i);
					if (!cache.ContainsKey(name)) {
						cache.Add(name, i);
					}
				}
			}
		}

		#endregion

		public void Dispose() {
			if (IsOpen) {
				CloseDB();
			}
		}
	}

}
