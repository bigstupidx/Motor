//
// ClientModule.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;
using XPlugin.Security;

namespace GameClient {
	public abstract class ClientModule : MonoBehaviour {
		/// <summary>
		/// 模块名
		/// </summary>
		public string ModuleName
		{
			get
			{
				if (string.IsNullOrEmpty(_moduleName)) {
					_moduleName = this.GetType().FullName;
					_moduleName = _moduleName.Replace("GameClient.Mod", "");
				}
				return _moduleName;
			}
		}
		private string _moduleName;

		private bool _isDirty = false;

		protected virtual void Update() {
			if (this._isDirty) {
				SaveDataNow();
			}
		}

		/// <summary>
		/// 加载数据
		/// </summary>
		/// <param name="db"></param>
		public virtual void InitData(DbAccess db) {
		}

		/// <summary>
		/// 加载信息
		/// </summary>
		/// <param name="s"></param>
		public virtual void InitInfo(string s) {

		}

		/// <summary>
		/// 重置数据
		/// </summary>
		public virtual void ResetData() {
		}

		/// <summary>
		/// 重置信息
		/// </summary>
		public virtual void ResetInfo() {
		}

		/// <summary>
		/// 转换为Json
		/// </summary>
		public virtual string ToJson(UserInfo user) {
			return "";
		}

		/// <summary>
		/// 读取数据
		/// </summary>
		public void ReadData() {
			Client.Log("<color=green> [Read] </color>" + ModuleName + " : " + PlayerPrefs.GetString(ModuleName, ""));
			string saveKey = PlayerPrefs.GetString(DataDef.UidSaveKey, "") + ModuleName;
			var json = "";
			if (Client.Ins.SaveEncrypt) {
				json = PlayerPrefsAES.GetString(saveKey, "");
			} else {
				json = PlayerPrefs.GetString(saveKey, "");
			}
			InitInfo(json);
		}

		/// <summary>
		/// 保存数据
		/// </summary>
		public virtual void SaveData() {
			this._isDirty = true;
		}

		/// <summary>
		/// 立即保存数据
		/// </summary>
		public void SaveDataNow() {
			var json = ToJson(Client.User.UserInfo);
			string saveKey = PlayerPrefs.GetString(DataDef.UidSaveKey, "") + ModuleName;
			Client.Log("<color=purple> [Save] </color>" + saveKey + " : " + json);
			if (Client.Ins.SaveEncrypt) {
				PlayerPrefsAES.SetString(saveKey, json);
			} else {
				PlayerPrefs.SetString(saveKey, json);
			}
			PlayerPrefs.Save();
			this._isDirty = false;
		}
	}
}
