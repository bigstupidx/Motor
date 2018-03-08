//
// SqlReaderExt.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System;
using System.Collections;
using Mono.Data.Sqlite;

namespace XPlugin.Data.SQLite {
	public static class SqlReaderExt {
		public static object GetValue(this SqliteDataReader reader, string columeName) {
			int index = DbAccess.GetColumnIndex(reader, columeName);
			//int index = reader.GetOrdinal(columeName);
			if (index == -1) {
				throw new Exception("SqliteDataReader:GetValue() colume \"" + columeName + "\" not found!");
			} else {
				return reader.GetValue(index);
			}
		}

		public static T OptValue<T>(this SqliteDataReader reader, string columeName, T defValue = default(T)) {
			object obj = reader.GetValue(columeName);
			if (obj == System.DBNull.Value) {
				return defValue;
			} else {
				return (T)obj;
			}
		}

		public static T? GetNullable<T>(this SqliteDataReader reader, string columeName) where T : struct {
			object obj = reader.GetValue(columeName);
			if (obj == System.DBNull.Value) {
				return null;
			} else {
				return (T)obj;
			}
		}

		public static string GetNullableString(this SqliteDataReader reader, string columeName) {
			object obj = reader.GetValue(columeName);
			if (obj == System.DBNull.Value) {
				return null;
			} else {
				return (string)obj;
			}
		}
	}
}

