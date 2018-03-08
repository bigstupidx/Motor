using System;
using System.Collections.Generic;
using System.Text;
using Game;
using UnityEngine;
using XPlugin.Data.Json;

namespace GameClient {
	public class PlayerInfo {
		// 以下为游戏初始化所需数据
		public bool IsPlayer;
		public string NickName;
		public string Avatar;
		public HeroInfo ChoosedHero;
		public BikeInfo ChoosedBike;
		public WeaponInfo ChoosedWeapon;
		public List<PropInfo> EquipedProps = new List<PropInfo>();
		public int AI = 5;
		public int SpawnPos=-1;//用于联网模式下的出生点

		// 以下为游戏内数据
		public BikeBase Bike;
		public JObject LevelStatValue = new JObject();
		public JObject DailyStatValue = new JObject();
		public JObject AchieveStatValue = new JObject();

		public PlayerInfo() {
		}

		public void ResetStatValue() {
			LevelStatValue = new JObject();
			Client.User.UserInfo.SetGameData();
		}

		#region 存取LevelStat数据
		public void SetStat(string key, object value) {
			JToken token = JToken.Create(value);
			LevelStatValue[key] = token;
		}

		public object GetStat(string key) {
			JToken value = LevelStatValue[key];
			if (value.IsValid) {
				if (value.IsValue) {
					return value.GetValue();
				} else {
					return value;
				}
			} else {
				return null;
			}
		}

		public T GetStat<T>(string key) {
			object va = GetStat(key);
			if (va != null) {
				return (T)Convert.ChangeType(va, typeof(T));
			} else {
				return default(T);
			}
		}
		#endregion

		public static int RandomAILevel() {
			return UnityEngine.Random.Range(1, 6);
		}

		/// <summary>
		/// 启用，关闭玩家特殊能力
		/// </summary>
		/// <param name="active"></param>
		public void SetAllAbilitiesActive(bool active) {
			// 英雄
			if (ChoosedHero.Data.AbilityFuncName != null) {
				for (var index = 0; index < ChoosedHero.Data.AbilityFuncName.Length; index++) {
					var ability = ChoosedHero.Data.AbilityFuncName[index];
					var value = ChoosedHero.Data.GetAbilityValue(index, ChoosedHero.Level);
					SetAbilityActive(ability, value, active);
				}
			}

			// 车辆
//			if (ChoosedBike.Data.AbilityFuncName != null) {
//				if (this.ChoosedBike.IsAllUpgradeMax) {
//					for (var index = 0; index < ChoosedBike.Data.AbilityFuncName.Length; index++) {
//						var ability = ChoosedBike.Data.AbilityFuncName[index];
//						var value = ChoosedBike.Data.AbilityValues[index];
//						SetAbilityActive(ability, value, active);
//					}
//				}
//			}

			// 武器
			if (ChoosedWeapon.WeaponData.AbilityFuncName != null) {
				for (var index = 0; index < ChoosedWeapon.WeaponData.AbilityFuncName.Length; index++) {
					var ability = ChoosedWeapon.WeaponData.AbilityFuncName[index];
					var value = ChoosedWeapon.WeaponData.AbilityValues[index];
					SetAbilityActive(ability, value, active);
				}
			}
		}

		private void SetAbilityActive(string abilityName, float value, bool active) {
			var path = "Game.Ability" + abilityName;
			var type = Type.GetType(path);
			if (type != null) {
				//				switch (abilityType)
				//				{
				//					case AbilityType.Menu:
				//						var ins = (AbilityBase)type.Assembly.CreateInstance(type.FullName);
				//						if (ins == null) return;
				//						if (active) ins.Init(value);
				//						ins.SetActive(this, active);
				//						break;
				//					case AbilityType.Game:
				var ability = (AbilityBase)Bike.gameObject.AddComponent(type);
				if (ability != null) {
					if (active) ability.Init(value);
					Debug.Log("<color=yellow> [Ability] </color>" + NickName + " : " + abilityName + " - " + value);
					ability.SetActive(this, active);
				}
				//						break;
				//				}
			}
		}

		public JObject ToJson() {
			JObject ret = new JObject();
			ret["NickName"] = this.NickName;
			ret["SpawnPos"] = this.SpawnPos;
			ret["ChoosedHero"] = this.ChoosedHero.ToJson();
			ret["ChoosedBike"] = this.ChoosedBike.ToJson();
			ret["ChoosedWeapon"] = this.ChoosedWeapon.ToJson();
			return ret;
		}

		public PlayerInfo(JObject json) {
			this.NickName = json["NickName"].AsString();
			this.SpawnPos = json["SpawnPos"].AsInt();
			this.ChoosedHero = new HeroInfo(json["ChoosedHero"].AsObject());
			this.ChoosedBike = new BikeInfo(json["ChoosedBike"].AsObject());
			this.ChoosedWeapon = new WeaponInfo(json["ChoosedWeapon"].AsObject());
		}

		public static byte[] OnlineSerializeList(object obj) {
			List<PlayerInfo> info = (List<PlayerInfo>) obj;
			JArray ret=new JArray();
			foreach (var playerInfo in info) {
				ret.Add(playerInfo.ToJson());
			}
			return Encoding.UTF8.GetBytes(ret.ToString());
		}

		public static object OnlineDeserializeList(byte[] bytes) {
			List<PlayerInfo> infos=new List<PlayerInfo>();

			JArray array=JArray.Parse(Encoding.UTF8.GetString(bytes));
			for (int i = 0; i < array.Count; i++) {
				infos.Add(new PlayerInfo(array[i].AsObject()));
			}
			return infos;
		}

		public static byte[] OnlineSerialize(object obj) {
			PlayerInfo info = (PlayerInfo)obj;
			return Encoding.UTF8.GetBytes(info.ToJson().ToString());
		}

		public static object OnlineDeserialize(byte[] bytes) {
			JObject ret = JObject.Parse(Encoding.UTF8.GetString(bytes));
			return new PlayerInfo(ret);
		}

	}
}
