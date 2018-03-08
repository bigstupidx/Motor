using System;
using UnityEngine;
using System.Collections.Generic;
using GameUI;
using Mono.Data.Sqlite;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;

namespace GameClient {
	public enum ShowPoint {
		None = -1,
		GuideOver = 0,     //新手教程结束
		EnterGame = 1,     //进入游戏
		NormalGame = 2,    //闯关模式
		LevelClick = 3,    //关卡
		GameWin = 4,       //游戏过关
		GameFail = 5,      //游戏失败
		BuyBike = 6,       //购买赛车
		BuyBikeFail = 7,
		BuyHero = 8,       //购买角色
		BuyHeroFail = 9,
		BuyProp = 10,       //购买道具
		BuyPropFail = 11,
		GameBuyChange = 12, //游戏中购买变身卷轴
		GameBuyProp = 13   //游戏中购买道具
	}

	public enum SpreeShowState {
		NoReady,//不能弹
		OverFree,//免费礼包处理完毕
		NoRMB,//不弹付费礼包
		BuyRMB,//购买了付费礼包
		RefuseRMB//取消了付费礼包
	}

	public class ModSpree : ClientModule {
		public bool DontGetSpreeFormServer;

		public SpreeShowPointData CurrentShowPointData;
		[NonSerialized]
		public bool isGetDataFormServer = false;

		[NonSerialized]
		public bool ShowSpree = false;
		[NonSerialized]
		public bool AutoBuy = false;
		[NonSerialized]
		public bool ShowRedeemCode = false;
		[NonSerialized]
		public bool ShowFirstSpree = false;
		[NonSerialized]
		public int FirstCostInServer = -1;

		private Dictionary<int, SpreeData> SpreeDatas = new Dictionary<int, SpreeData>();
		private Dictionary<ShowPoint, SpreeShowPointData> SpreeShowDatas = new Dictionary<ShowPoint, SpreeShowPointData>();
		private SpreeData firstSpree;
		private bool isShowing = false;
		private Action<SpreeShowState> _showPointCallback = null;

		public override void InitData(DbAccess db) {
			SqliteDataReader reader = db.ReadFullTable("Spree");
			while (reader.Read()) {
				SpreeData data = new SpreeData(reader);
				SpreeDatas.Add(data.ID, data);
				if (data.Type == SpreeType.First) {
					firstSpree = data;
				}
			}

			reader = db.ReadFullTable("SpreeShow");
			while (reader.Read()) {
				SpreeShowPointData showPointData = new SpreeShowPointData(reader);
				SpreeShowDatas.Add(showPointData.ShowPoint, showPointData);
			}
		}

		public void UpdateSpreeDatas(JArray json) {
			if (DontGetSpreeFormServer) {
				return;
			}
			if (json.Count <= 0) {
				return;
			}
			SpreeDatas = new Dictionary<int, SpreeData>();
			for (int i = 0; i < json.Count; i++) {
				JObject data = (JObject)json[i];
				SpreeData spreeData = new SpreeData(data);
				SpreeDatas.TryAdd(spreeData.ID, spreeData);
				if (spreeData.Type == SpreeType.First) {
					firstSpree = spreeData;
				}
			}

		}

		public void UpdateSpreeShowDatas(JArray json) {
			if (DontGetSpreeFormServer) {
				return;
			}
			if (json.Count <= 0) {
				return;
			}
			isGetDataFormServer = true;
			SpreeShowDatas = new Dictionary<ShowPoint, SpreeShowPointData>();
			for (int i = 0; i < json.Count; i++) {
				JArray data = (JArray)json[i];
				SpreeShowPointData showPointData = new SpreeShowPointData((ShowPoint)i, data);
				SpreeShowDatas.TryAdd(showPointData.ShowPoint, showPointData);
				//Debug.LogError ("show info i:" + i + "  data:" + showPointData.RMBSpreeID);
			}
		}

		public override void InitInfo(string s) {
			SpreeInfo spreeInfo;
			if (string.IsNullOrEmpty(s)) {
				spreeInfo = new SpreeInfo();
			} else {
				spreeInfo = JsonMapper.ToObject<SpreeInfo>(s);
			}

			Client.User.UserInfo.Spree = spreeInfo;
		}

		public override string ToJson(UserInfo user) {
			if (user == null) {
				return JsonMapper.ToJson(Client.User.UserInfo.Spree);
			}
			return JsonMapper.ToJson(user.Spree);
		}

		public override void ResetData() {
			SpreeDatas = new Dictionary<int, SpreeData>();
			SpreeShowDatas = new Dictionary<ShowPoint, SpreeShowPointData>();
		}

		public SpreeData GetFirstSpreeData() {
			return firstSpree;
		}

		/// <summary>
		/// 首充金额
		/// </summary>
		public int FirstCost {
			get {
				if (FirstCostInServer > 0) {
					return FirstCostInServer;
				}
				return Client.System.GetMiscValue<int>("Spree.FirstCost");
			}
		}

		public SpreeData GetSpreeDataByID(int id) {
			SpreeData data;
			if (SpreeDatas.TryGetValue(id, out data)) {
				return data;
			}
			Debug.LogError("[Spree] No SpreeData For ID : " + id);
			return null;
		}

		public SpreeData GetSpreeDataByShowType(ShowType type) {
			foreach (SpreeData spree in SpreeDatas.Values) {
				if (spree.ShowType == type) {
					return spree;
				}
			}
			return null;
		}

		public List<SpreeData> GetSpreeDatasForShop() {
			List<SpreeData> data = new List<SpreeData>();
			foreach (SpreeData spree in SpreeDatas.Values) {
				if (spree.Type == SpreeType.RMB && ShouldShow(spree)) {
					data.Add(spree);
				}
			}
			data.Sort((p1, p2) => p1.Sort - p2.Sort);
			return data;
		}

		public SpreeShowPointData GetSpreeShowPointData(ShowPoint point) {
			SpreeShowPointData data;
			if (SpreeShowDatas.TryGetValue(point, out data)) {
				return data;
			}
			Debug.LogError("[Spree] No SpreeShowPointData For Point : " + point);
			return null;
		}

		#region 买礼包
		public void BuySpree(SpreeData data, Action<bool> onDone) {//for RMB

			WaittingTip.Show((LString.GAMECLIENT_MODIAP_BUY).ToLocalized());
			data.Pay((b) => {
				WaittingTip.Hide();
				if (b) {
					//支付成功
					Client.Item.GetRewards(data.AwardList, true);

					//付费统计
					string iapId = data.PayCode.ToString();
					double price = data.PayValue / 100.0;
					AnalyticsMgrBase.Ins.Pay(iapId, price, 0);

					//检测是否达到领取首充礼包资格
					if (!Client.User.UserInfo.Spree.CanGetFirstSpree && (data.PayValue / 100f) >= Client.Spree.FirstCost) {
						Client.User.UserInfo.Spree.CanGetFirstSpree = true;
					}

					//记录已买的礼包
					int count;
					if (Client.User.UserInfo.Spree.AlreadyPay.TryGetValue(data.PayCode, out count)) {
						count++;
					} else {
						Client.User.UserInfo.Spree.AlreadyPay.Add(data.PayCode, 1);
					}
					Client.Spree.SaveData();
					onDone(true);
				} else {
					onDone(false);
				}
			});


		}

		#endregion

		#region 弹礼包
		public bool ShouldShow(SpreeData spree) {
			bool show = false;
			if (spree.IsBuyMore) {
				show = true;
			} else {
				int count = 0;
				if (Client.User.UserInfo.Spree.AlreadyPay.TryGetValue(spree.PayCode, out count)) {
					show = count <= 0;
				} else {
					show = true;
				}
			}
			return show;
		}

		public void OnArriveShowPoint(ShowPoint point, Action<SpreeShowState> onDone = null) {
			Client.Log("[Spree] 到达礼包弹出点: " + point);
			_showPointCallback = onDone;
			if (point != ShowPoint.GuideOver && !Client.Guide.IsMainGuideFinished) {
				Client.Log("[Spree] 强制教程没做完，不弹礼包");
				PointCallback(SpreeShowState.NoReady);
				return;
			}
			if (isShowing) {
				Client.Log("[Spree] 上一个礼包弹出点还未处理完: " + CurrentShowPointData.ShowPoint);
				PointCallback(SpreeShowState.NoReady);
				return;
			}
			if (!ShowSpree) {
				PointCallback(SpreeShowState.NoRMB);
				return;
			}

			//处理特殊点
			if (!SpecialPointDeal(point)) {
				return;
			}

			CurrentShowPointData = GetSpreeShowPointData(point);
			if (CurrentShowPointData == null) {
				Client.Log("[Spree] 无该弹出点数据: " + point);
				PointCallback(SpreeShowState.NoRMB);
				return;
			}

			ShowFree();
		}

		private void PointCallback(SpreeShowState state) {
			if (_showPointCallback != null) {
				var call = _showPointCallback;
				call(state);

				_showPointCallback = null;
			}
		}

		private void ShowFree() {
			if (CurrentShowPointData.FreeSpreeID != 0) {
				SpreeData spree = GetSpreeDataByID(CurrentShowPointData.FreeSpreeID);
				if (spree != null && CurrentShowPointData.ShowPoint != ShowPoint.BuyBikeFail
					&& CurrentShowPointData.ShowPoint != ShowPoint.BuyHeroFail
					&& CurrentShowPointData.ShowPoint != ShowPoint.BuyPropFail
					&& CurrentShowPointData.ShowPoint != ShowPoint.GameBuyProp
					&& CurrentShowPointData.ShowPoint != ShowPoint.GameBuyChange) {
					isShowing = true;
					FreeSpreeDialog.Show(spree);
				} else {
					OnSpreeShowOver(true, SpreeShowState.OverFree);
				}
			} else {
				OnSpreeShowOver(true, SpreeShowState.OverFree);
			}
		}

		private void ShowRMB() {
			if (CurrentShowPointData.RMBSpreeID != 0) {
				SpreeData spree = GetSpreeDataByID(CurrentShowPointData.RMBSpreeID);
				if (spree != null && ShouldShow(spree) && ShouldShow(CurrentShowPointData.ShowPoint, spree)) {
					isShowing = true;
					RMBSpreeDialog.Show(spree, true);
				} else {
					OnSpreeShowOver(false, SpreeShowState.NoRMB);
				}
			} else {
				OnSpreeShowOver(false, SpreeShowState.NoRMB);
			}
		}

		public void OnSpreeShowOver(bool showRMB, SpreeShowState state) {
			if (showRMB) {
				ShowRMB();
			} else {
				Client.Log("[Spree] 礼包弹出点处理完毕: " + CurrentShowPointData.ShowPoint);
				isShowing = false;
				PointCallback(state);
			}
		}
		#endregion

		#region 特殊弹出点处理

		private bool SpecialPointDeal(ShowPoint point) {
			if (point == ShowPoint.BuyProp && !Client.Guide.IsPropGuideFinished) {
				Client.Log("[Spree] 道具教程没做完，不弹礼包");
				PointCallback(SpreeShowState.NoReady);
				return false;
			}


			return true;
		}

		private bool ShouldShow(ShowPoint point, SpreeData spree) {
			//未拥有礼包中的车辆才弹
			if (point == ShowPoint.BuyBike) {
				//				//判断当前要购买的车是否在礼包中
				//				bool currentBuyIn = false;
				//				foreach (var award in spree.AwardList)
				//				{
				//					if (award.Data.ID == BikeBoard.Ins.CurrentShowBike.Info.Data.ID)
				//					{
				//						currentBuyIn = true;
				//					}
				//				}

				//				if (currentBuyIn)
				//				{
				foreach (var award in spree.AwardList) {
					if (award.Data.Type == ItemType.Bike && !Client.User.UserInfo.BikeList.ContainsKey(award.Data.ID)) {
						return true;
					}
				}
				//				}

				return false;
			}

			//操作教程引导结束|游戏中购买  不能弹包含车辆和人物的礼包
			if (point == ShowPoint.GuideOver || point == ShowPoint.GameBuyChange || point == ShowPoint.GameBuyProp) {
				foreach (var award in spree.AwardList) {
					if (award.Data.Type == ItemType.Bike || award.Data.Type == ItemType.Hero) {
						return false;
					}
				}
			}

			return true;
		}

		#endregion
	}



}
