using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameUI;
using UnityEngine.SceneManagement;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;
using XPlugin.Update;

namespace GameClient {
	public partial class Client : MonoBehaviour {
		public static Client Ins;
		public string DbFileName;
		public bool SaveEncrypt;
		public bool EnableAnalyze;

		public static bool IsInited {
			get { return Ins != null && Ins.Inited; }
		}
		public bool Inited { get; protected set; }
		protected JObject infoJson = null;
		protected List<ClientModule> modulesList = new List<ClientModule>();
		private DBUtil db;

		void Awake() {
			DontDestroyOnLoad(gameObject);
			if (Ins == null) {
				Ins = this;
				Inited = false;
				FindModules();
				Analytics.enabled = this.EnableAnalyze;
				Config.InitConfig();

				//TODO for Test 读取保存的服务器地址
				var tcpServer = PlayerPrefs.GetString("TcpServer", null);
				if (!string.IsNullOrEmpty(tcpServer)) {
					this.config.PvpHostPort = tcpServer+":11700";
				}
				var webServer = PlayerPrefs.GetString("WebServer", null);
				if (!string.IsNullOrEmpty(webServer)) {
					this.config.WebHost = webServer;
				}


			} else {
				Debug.LogError("Client has been created mutiple times!");
			}
		}

		void Start() {
			StartInit((e) => {
				if (e != null) {
					CommonDialog.Show(e.Message, e.ToString(), LString.RewardDialog_Btn_BtnYellow_BtnOK_Text, null);
				}

				MainBoard.Show();
				IAP.QueryOrder(false, orders => {
					CommonDialog.Show("", LString.GAMECLIENT_CACHED_ORDER_ARRIVE.ToLocalized(), LString.CommonDialog_BG_BtnConfirm_Text.ToLocalized(), null, null, null);
				});
			});
		}

		void OnApplicationQuit() {//为了避免编辑器下读取数据过程中关闭游戏导致数据库文件没有关闭
			if (this.db != null && this.db.IsOpen) {
				Debug.Log("中途退出，关闭数据库");
				this.db.Close();
			}
		}

		public void StartInit(Action<Exception> onDone = null) {
			if (Inited) {
				return;
			}
			StartCoroutine(Init(onDone));
		}

		Exception exception = null;

		IEnumerator Init(Action<Exception> onDone = null) {
			LoadingBoard.Show("", 0, false);
			Log("[Client] Init Start !");
			exception = null;

			LString.Load_UI_Prefab_Text();
			LString.Load_Client_Script_Text();
			LString.Load_UI_Script_Text();
			yield return null;

			yield return StartCoroutine(InitData());

			if (exception != null) {
				if (onDone != null) {
					onDone(exception);
				}
				yield break;
			}

			yield return StartCoroutine(InitInfo());

			if (exception != null) {
				if (onDone != null) {
					onDone(exception);
				}
				yield break;
			}

			Inited = true;
			Client.EventMgr.SendEvent(EventEnum.System_ClientInited);

			Log("[Client] Init End !");

			yield return StartCoroutine(CheckInvited());

			UResources.ReqScene(DataDef.MenuBG_Scene);
			SceneManager.LoadScene(DataDef.MenuBG_Scene);
			yield return null;

			if (onDone != null) {
				onDone(null);
			}

			if (this.EnableAnalyze) {
				Analytics.StartAnalytics();
			}

			EventMgr.SendEvent(EventEnum.System_ClientInited);
		}

		private IEnumerator CheckInvited() {
			if (!this.config.NeedInvite) {
				yield return null;
			} else {
				int alreadyInvited = PlayerPrefs.GetInt("AlreadyInvited", 0);
				if (alreadyInvited != 0) {
					yield return null;
				} else {
					InputInviteCodeDialog.Show();
					while (alreadyInvited == 0) {
						alreadyInvited = PlayerPrefs.GetInt("AlreadyInvited", 0);
						yield return null;
					}
				}
			}
		}

		IEnumerator InitData() {
			float percent = 0.05f;
			LoadingBoard.Show((LString.GAMECLIENT_CLIENT_INITDATA).ToLocalized() + percent.ToString("00%"), percent, false);

			// 连接数据库
			db = new DBUtil(DbFileName, null, Client.Config.IsLogEnable);
			try {
				db.Open();
			} catch (Exception e) {
				this.exception = e;
				Debug.LogException(e);
				db.Close();
				yield break;
			}

			percent = 0.1f;
			LoadingBoard.Ins.UpdateContent((LString.GAMECLIENT_CLIENT_INITDATA).ToLocalized() + percent.ToString("00%"), percent, false);
			yield return null;

			// 初始化模块数值
			int count = 0;
			foreach (ClientModule mod in modulesList) {
				try {
					count++;
					mod.InitData(db.dbAccess);
				} catch (Exception e) {
					exception = e;
					break;
				}
				percent = 0.1f + ((float)count / modulesList.Count) * 0.4f;
				LoadingBoard.Ins.UpdateContent((LString.GAMECLIENT_CLIENT_INITDATA).ToLocalized() + percent.ToString("00%"), percent, false);
				yield return null;
			}

			// 关闭数据库
			db.Close();

			if (exception == null) {
				Log("[Client] Data inited !");
			} else {
				Log("[Client] Data not inited !");
				Debug.LogException(exception);
			}

			// 连接到礼包服务器
			Interface.GetServerInfo(SDKManager.Instance.GetChannelId(),
				Config.VersionId,
				SDKManager.Instance.GetProvinceId(),
				SDKManager.Instance.GetCityId(),
				SDKManager.Instance.GetNetworkID(),
				(success) => {
					Debug.Log("[Client] Get Server Info :" + success);
				});

			yield return null;
		}

		IEnumerator InitInfo() {
			float percent = 0.5f;
			LoadingBoard.Ins.UpdateContent((LString.GAMECLIENT_CLIENT_INITDATA).ToLocalized() + percent.ToString("00%"), percent, false);

			// 读取存档并初始化模块数据
			int count = 0;
			foreach (ClientModule mod in modulesList) {
				count++;
				try {
					mod.ReadData();
				} catch (Exception e) {
					exception = e;
					break;
				}
				percent = 0.5f + ((float)count / modulesList.Count) * 0.3f;
				LoadingBoard.Ins.UpdateContent((LString.GAMECLIENT_CLIENT_INITDATA).ToLocalized() + percent.ToString("00%"), percent, false);
				yield return null;
			}

			if (exception == null) {
				Log("[Client] Info inited !");
			} else {
				Log("[Client] Info not inited !");
				Debug.LogException(exception);
			}

			yield return null;
			percent = 0.99f;
			LoadingBoard.Ins.UpdateContent((LString.GAMECLIENT_CLIENT_INITDATA).ToLocalized() + percent.ToString("00%"), percent, false);
			yield return null;
		}

		public static void Log(string content) {
			if (Config.IsLogEnable) {
				Debug.Log(content);
			}
		}

		public static void LogError(string content) {
			if (Client.Config.IsLogEnable) {
				Debug.LogError(content);
			}
		}
	}
}
