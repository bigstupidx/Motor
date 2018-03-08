//
// JsonLiteDB.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using System.Collections.Generic;
using XPlugin.Data.Json;

namespace XPlugin.Data.JsonLiteDB {
	public class JsonLiteDB {
		protected Dictionary<string, Table> tables = new Dictionary<string, Table>();
		protected bool inited = false;

		/// <summary>
		/// 从提供的字符串中读取
		/// </summary>
		/// <param name="json"></param>
		public void Load(string json) {
			Load(JObject.Parse(json));
		}

		public void Load(JObject json) {
			Reset();
			foreach (JToken t in json) {
				tables.Add(t.Name, new Table(t.AsObject()));
			}
			inited = true;
		}

		public void Reset() {
			tables.Clear();
			inited = false;
		}

		/// <summary>
		/// 根据表名获取相应的表
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public Table this[string tableName] {
			get {
				if (!inited) {
					return null;
				}

				Table table;
				tables.TryGetValue(tableName, out table);
				return table;
			}
		}

		public class Table {
			protected Dictionary<String, int> columes;
			protected List<JArray> values;

			public Table(JObject table) {
				columes = new Dictionary<string, int>();
				JArray columeNode = table["Columes"].AsArray();
				for (int i = 0; i < columeNode.Count; i++) {
					columes.Add(columeNode[i].AsString(), i);
				}

				values = new List<JArray>();
				foreach (JToken token in table["Values"].AsArray()) {
					values.Add(token.AsArray());
				}
			}

			/// <summary>
			/// 获取读取器
			/// </summary>
			/// <returns></returns>
			public TableReader GetReader() {
				return new TableReader(this);
			}

			/// <summary>
			/// 行数
			/// </summary>
			public int RowsCount {
				get {
					return values.Count;
				}
			}

			/// <summary>
			/// 获取字段对应的下标
			/// </summary>
			/// <param name="columeName"></param>
			/// <returns></returns>
			public int GetColumeOrdinal(string columeName) {
				int index;
				if (columes.TryGetValue(columeName, out index)) {
					return index;
				} else {
					return -1;
				}
			}

			/// <summary>
			/// 获取值
			/// </summary>
			/// <param name="row"></param>
			/// <param name="colume"></param>
			/// <returns></returns>
			public JToken GetValue(int row, int colume) {
				if (row >= 0 && row < values.Count) {
					JArray arr = values[row];
					if (colume >= 0 && colume < arr.Count) {
						return arr[colume];
					}
				}
				return null;
			}

			public JArray GetRow(int row) {
				if (row >= 0 && row < values.Count) {
					return values[row];
				}
				return null;
			}
		}

		public class TableReader {
			protected Table table;
			protected int index;

			public TableReader(Table table) {
				this.table = table;
				index = -1;
			}

			public bool Read() {
				if (index == -1) {
					index = 0;
				} else {
					index++;
				}

				return index < table.RowsCount;
			}

			public JToken this[string columeName] {
				get {
					return table.GetValue(index, table.GetColumeOrdinal(columeName));
				}
			}

			public override string ToString() {
				return table.GetRow(index).ToString();
			}
		}
	}
}
