using System;
using UnityEngine;
using XPlugin.Data.Json;
using XPlugin.Security;
using XPlugin.Update;

namespace GameClient {
	public class ModConfig : ClientModule {

		public string AESKey = "";
		public string AESIv = "";

		public string ConfigFileName;

		/// <summary>
		/// 是否显示Log
		/// </summary>
		public bool IsLogEnable = true;

		//给服务器的版本ID
		public int VersionId = 0;

		public bool OpenAbout = false;

		public bool OpenNetMatch = true;

		public bool ShowService = false;

		public bool SensorSpree = false;

		/// <summary>
		/// 允许使用体感器
		/// </summary>
		public bool CanUseSensor = false;

		public bool NeedInvite = false;
		public bool ShowInvite = false;

		public bool OnlyIntouchVipCanEnterChampionship = false;
		public bool OnlySensorCanEnterLobby = false;
		public bool TVMode = false;

		public bool IgnoreStory = false;

		public bool DefaultLowQuality = false;
		public bool OpenStore = true;
		public string PvpHostPort = "127.0.0.1:11700";
		public string WebHost = "http://motor4.xiaoxigame.com/";

		public string AnaylseAppKey = "";

		public void InitConfig() {
			var www = UResources.LoadStreamingAsset(ConfigFileName);
			if (string.IsNullOrEmpty(www.error)) {
				JObject json = null;
				try {
					json = JObject.Parse(www.text);
				} catch (Exception e) {
					json = JObject.Parse(AESUtil.Decrypt(www.text, this.AESKey, this.AESIv));
				}

				this.OpenAbout = json["OpenAbout"].OptBool(false);
				VersionId = json["VersionId"].OptInt(0);
				this.OpenNetMatch = json["OpenNetMatch"].OptBool(true);
				this.ShowService = json["ShowService"].OptBool(false);
				SensorSpree = json["SensorSpree"].OptBool(false);
				CanUseSensor = json["CanUseSensor"].OptBool(false);
				OnlyIntouchVipCanEnterChampionship = json["OnlyIntouchVipCanEnterChampionship"].OptBool(false);
				OnlySensorCanEnterLobby = json["OnlySensorCanEnterLobby"].OptBool(false);
				TVMode = json["TVMode"].OptBool(false);
				NeedInvite = json["NeedInvite"].OptBool(false);
				ShowInvite = json["ShowInvite"].OptBool(false);
				IgnoreStory = json["IgnoreStory"].OptBool(false);
				DefaultLowQuality = json["DefaultLowQuality"].OptBool(false);
				this.OpenStore = json["OpenStore"].OptBool(true);

				this.PvpHostPort = json["PvpHostPort"].OptString("127.0.0.1:11700");
				this.WebHost = json["WebHost"].OptString("http://motor4.xiaoxigame.com/");

				this.AnaylseAppKey = json["AnaylseAppKey"].OptString("");
			} else {
				Debug.LogError(www.error);
			}
		}

	}

}
