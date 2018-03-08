using System;
using GameUI;
using UnityEngine;
using XPlugin;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;
using XPlugin.Localization;

namespace GameClient {

	public enum ChangeNickBackType {
		Success,
		AlreadyExist,
		Fail,
		NetworkFail
	}

	public class ModNickName : ClientModule {

		public NicknameLibrary Lib { get; private set; }

		public override void InitData(DbAccess db) {
			base.InitData(db);
			this.Lib = new NicknameLibrary();
		}

		/// <summary>
		/// 昵称长度限制
		/// </summary>
		/// <returns></returns>
		public int CharacterLimit() {
			switch (Localization.Language) {
				case LanguageEnum.zh_CN:
				case LanguageEnum.zh_TW:
					return 10;
				case LanguageEnum.en_US:
				case LanguageEnum.fr_FR:
					return 20;
				default:
					return 10;
			}
		}

		public int Cost {
			get { return Client.System.GetMiscValue<int>("NickName.Cost"); }
		}

		public void ChangeNickName(string name, Action<ChangeNickBackType> onDone) {
			CommonDialog.Show((LString.GAMECLIENT_MODNICKNAME_CHANGENICKNAME).ToLocalized(), (LString.GAMECLIENT_MODNICKNAME_CHANGENICKNAME_1).ToLocalized() + Cost + (LString.GAMECLIENT_MODNICKNAME_CHANGENICKNAME_2).ToLocalized(), (LString.GAMECLIENT_MODCHAMPIONSHIP_UPLOADRESULT_2).ToLocalized(), (LString.GAMECLIENT_MODCHAMPIONSHIP_UPLOADRESULT_3).ToLocalized(), () => {
				int cost = Cost;
				ItemInfo diamond = Client.Item.Diamond;
				if (diamond.Amount < cost) {
					CommonTip.Show((LString.GAMECLIENT_MODNICKNAME_CHANGENICKNAME_3).ToLocalized());
					Client.IAP.ShowShopBoardForSupply(diamond.Data.ID);
					return;
				}
				WaittingTip.Show((LString.GAMECLIENT_MODNICKNAME_CHANGENICKNAME_4).ToLocalized());
				ChangeNickname(Client.User.UserInfo.Setting.UserId, name, (b) => {
					WaittingTip.Hide();
					if (b == ChangeNickBackType.Success) {
						Client.User.UserInfo.Setting.Nickname = name;
						Client.User.UserInfo.Setting.IsChangeName = true;
						diamond.ChangeAmount(-cost);
						AnalyticsMgrBase.Ins.Purchase((LString.GAMECLIENT_MODNICKNAME_CHANGENICKNAME).ToLocalized(), 1, cost);
					}
					onDone(b);
				});
			}, null);

		}

		public void GetPlayerId(string name, Action<ChangeNickBackType> onDone) {
			string uid = PlayerPrefs.GetString(DataDef.UidSaveKey, "");
			GetPlayerID(name, uid, (result, id) => {
				switch (result) {
					case ChangeNickBackType.Success:
						Client.User.UserInfo.Setting.UserId = id;
						Client.User.UserInfo.Setting.Nickname = name;
						Client.User.UserInfo.Setting.IsChangeName = true;
						onDone(ChangeNickBackType.Success);
						break;
				}
				onDone(result);
			});
		}

		/// <summary>
		/// 根据昵称获取ID，在本地没有存ID时调用
		/// </summary>
		/// <param name="nickNmae"></param>
		/// <param name="ondone"></param>
		private static void GetPlayerID(string nickNmae, string sdkUid, Action<ChangeNickBackType, int> ondone = null) {
			WebManager.Ins.AddItem(new WebItem() {
				A = "GetPlayerID",
				P = new JArray(new[] { nickNmae, sdkUid }),
				M = "Activity",
				Callback = (callback) => {
					if (WebManager.Ins.isShowLog) {
						Debug.Log("<color=yellow>[GetPlayerID]</color>" + callback.Success + " " + callback.CallBackType +
								  " content:" + callback.content);
					}
					if (callback.CallBackType == WebCallBackType.Success) {
						JObject root = JObject.Parse(callback.content);
						// Code 为20000时昵称重复
						var code = root["code"].AsInt();
						if (code == 0) {
							var id = root["result"].AsInt();

							if (ondone != null) {
								ondone(ChangeNickBackType.Success, id);
							}
						} else if (code == 20000) {
							if (ondone != null) {
								ondone(ChangeNickBackType.AlreadyExist, -1);
							}
						} else {
							if (ondone != null) {
								ondone(ChangeNickBackType.Fail, -1);
							}
						}
					} else {
						if (ondone != null) {
							ondone(ChangeNickBackType.NetworkFail, -1);
						}
					}
				}
			});
		}

		/// <summary>
		/// 修改昵称
		/// </summary>
		/// <param name="playerID"></param>
		/// <param name="nickNmae"></param>
		/// <param name="ondone"></param>
		public static void ChangeNickname(int playerID, string nickNmae, Action<ChangeNickBackType> ondone = null) {
			WebManager.Ins.AddItem(new WebItem() {
				A = "ChangeNickname",
				P = new JArray { playerID, nickNmae },
				M = "Activity",
				Callback = (callback) => {
					if (WebManager.Ins.isShowLog) {
						Debug.Log("<color=yellow>[ChangeNickname]</color>" + callback.Success + " " + callback.CallBackType +
								  " content:" + callback.content);
					}
					if (callback.CallBackType == WebCallBackType.Success) {
						JObject root = JObject.Parse(callback.content);
						var code = root["code"].AsInt();
						if (code == 0) {
							if (ondone != null) {
								ondone(ChangeNickBackType.Success);
							}

						} else {
							if (ondone != null) {
								ondone(ChangeNickBackType.Fail);
							}
						}
					} else {
						if (ondone != null) {
							ondone(ChangeNickBackType.NetworkFail);
						}
					}
				}
			});
		}
	}
}
