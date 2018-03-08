// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using UnityEngine;
using System.IO;
using XPlugin.Localization;
using XPlugin.Update;

namespace XPlugin.Data.SQLite {

	public class DBUtil {
		/// <summary>
		/// 数据库文件名称（不带扩展名）
		/// </summary>
		public string DbName;

		public bool AllowDebug;

		private bool _useDebugDb;

		private string _realDbPath;
		public string Password;

		public DbAccess dbAccess;

		public bool IsOpen {
			get { return dbAccess != null && dbAccess.IsOpen; }
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="dbName">数据库文件名称（不带扩展名）</param>
		/// <param name="password">密码</param>
		/// <param name="allowDebug">允许调试，打开该项，将会试图读取根目录下的数据库文件（文件名需要以bytes为扩展名，如dbName.bytes），通常用于调试数值</param>
		public DBUtil(string dbName, string password, bool allowDebug = false) {
			DbName = dbName;
			AllowDebug = allowDebug;
			Password = password;
			dbAccess = new DbAccess();
		}

		/// <summary>
		/// 打开数据库<para/>
		/// 会自动将数据库文件写入到外部，Close的时候删除
		/// </summary>
		/// <returns></returns>
		public bool Open() {
			if (dbAccess.IsOpen) {
				Debug.LogError(DbName + "已经是打开的了");
				return false;
			}

			_realDbPath = null;
			if (AllowDebug) {
				if (File.Exists(DbName)) {
					_realDbPath = DbName;
					Debug.Log("找到调试数据库" + _realDbPath);
					_useDebugDb = true;
					Password = "";//调试数据库的密码应该为空
				}
			}

			if (_realDbPath == null) {
				var nameWithoutExt = Path.GetFileNameWithoutExtension(DbName);
				var asset = LResources.Load<TextAsset>(nameWithoutExt);
				if (asset == null || asset.bytes == null) {
					throw new Exception("没有找到数据库资源");
				}
				// 写出数据库文件
				var tmpFileName = string.Format(".{0}.tmp", nameWithoutExt);
				_realDbPath = Path.Combine(Application.persistentDataPath, tmpFileName);
				File.WriteAllBytes(_realDbPath, asset.bytes);
			}
			if (!File.Exists(_realDbPath)) {
				throw new Exception("数据库文件不存在");
			}
			if (this.AllowDebug) {
				Debug.Log("DB File path:"+this._realDbPath);
			}
			// 连接数据库
			return dbAccess.OpenFromFile(_realDbPath, Password);
		}

		/// <summary>
		/// 关闭数据库<para/>
		/// 将会删除外部的数据库文件
		/// </summary>
		public void Close() {
			dbAccess.CloseDB();
			if (!_useDebugDb) {
				//删除数据库文件
				if (File.Exists(_realDbPath)) {
					File.Delete(_realDbPath);
				}
			}
		}


	}
}