using System;
using System.Collections.Generic;
using System.Text;
using GameUI;
using Mono.Data.Sqlite;
using UnityEngine;
using XPlugin.Data.Json;
using XPlugin.Data.SQLite;

namespace GameClient {
	public enum IAPType {
		Diamond = 0,
		Coin = 1,
		Stamina = 2,
		Spree = 3,
		Star = 4
	}

	public class ModIAP : ClientModule {
		[NonSerialized]
		public bool BuyConfirm = false;

		public bool DebugNoServerIAPCheck = false;

		public List<IAPData> CoinList = new List<IAPData>();
		public List<IAPData> DiamondList = new List<IAPData>();
		public List<IAPData> StaminaList = new List<IAPData>();

		public override void InitData(DbAccess db) {
			SqliteDataReader reader = db.ReadFullTable("IAP");
			while (reader.Read()) {
				IAPData data = new IAPData(reader);
				switch (data.Type) {
					case IAPType.Coin:
						CoinList.Add(data);
						break;
					case IAPType.Diamond:
						DiamondList.Add(data);
						break;
					case IAPType.Stamina:
						StaminaList.Add(data);
						break;
				}
			}
		}

		public IAPData GetDataByID(int id) {
			var ret = CoinList.Find(data => data.ID == id);
			if (ret != null) {
				return ret;
			}
			ret = DiamondList.Find(data => data.ID == id);
			if (ret != null) {
				return ret;
			}
			ret = StaminaList.Find(data => data.ID == id);
			if (ret != null) {
				return ret;
			}
			return ret;
		}

		public void BuySuccess(IAPData data) {
			Client.Item.GainItem(data.Item, data.Amount, true);
			Client.EventMgr.SendEvent(EventEnum.Item_Buy, data.Item.ID, data.Amount);
			//充值统计

			string iapId = data.PayCode.ToString();
			double price = data.CurrencyAmount;
			if (data.Item.Type == ItemType.Diamond) {
				double currency = data.Amount;
				AnalyticsMgrBase.Ins.Pay(iapId, price, currency);
			} else {
				AnalyticsMgrBase.Ins.Pay(iapId, price, 0);
			}

			CommonDialog.Show("", LString.GAMECLIENT_MODIAP_BUY_1.ToLocalized(), LString.CommonDialog_BG_BtnConfirm_Text.ToLocalized(), null);

			//检测是否达到领取首充礼包资格
			if (!Client.User.UserInfo.Spree.CanGetFirstSpree && data.CurrencyAmount >= Client.Spree.FirstCost) {
				Client.User.UserInfo.Spree.CanGetFirstSpree = true;
			}
		}

		/// <summary>
		/// 查询服务器未消费的订单，并发货
		/// </summary>
		public void QueryOrder(bool showError, Action<IEnumerable<string>> consumOrdersCallback) {
			Action<string> error = (reason) => {
				if (showError) {
					CommonDialog.Show("没有找到成功支付的订单", "如果您已成功支付，请点击重试。", "重试",
						LString.CommonDialog_BG_BtnCancel_Text.ToLocalized(),
						() => {
							QueryOrder(showError, consumOrdersCallback);
						}, null);
					Debug.Log("[LTH] 向服务器查询订单号 失败 " + reason);
				}
			};

			WaittingTip.Show((LString.GAMEUI_GAMEUIINTERFACE_LOADSCENE).ToLocalized());
			//向服务器查询
			WebManager.Ins.AddItem(new WebItem {
				M = "Order",
				A = "QueryOrder",
				P = new JArray(),
				Callback = item => {
					WaittingTip.Hide();
					if (item.Success) {
						JObject root = JObject.Parse(item.content);
						if (root["code"].AsInt() == 0) {
							var orders = root["result"].AsArray();
							if (orders.Count == 0) {
								error("没有查找到未完成订单");
							}
							List<string> orderIds = new List<string>();
							List<int> payIds = new List<int>();
							for (int i = 0; i < orders.Count; i++) {
								var order = orders[i].AsArray();
								var orderId = order[0].AsString();
								var payId = order[1].AsInt();
								orderIds.Add(orderId);
								payIds.Add(payId);
							}
							if (orderIds.Count != 0) {
								Debug.Log("[LTH] 有未完成的订单");
								WaittingTip.Show((LString.GAMECLIENT_MODGAME).ToLocalized());
								var p = new JArray();
								p.Add(new JArray(orderIds));
								WebManager.Ins.AddItem(new WebItem {
									M = "Order",
									A = "ConsumOrder",
									P = p,
									Callback = it => {
										WaittingTip.Hide();
										Debug.Log("[LTH] 向服务器消费订单号");
										if (it.Success) {
											JObject r = JObject.Parse(it.content);
											if (r["code"].AsInt() == 0) {
												foreach (var payId in payIds) {
													BuySuccess(GetDataByID(payId));
												}
											}
										}
										if (consumOrdersCallback != null) {
											consumOrdersCallback(orderIds);
										}
									}
								});
							}
						} else {
							error("查找订单失败 code:" + root["code"].AsInt());
						}
					} else {
						error("");
					}
				}
			});
		}

		public void Buy(IAPData data) {
			if (data.Currency.Type == ItemType.RMB) {
				WaittingTip.Show("正在支付...");
				SfxManager.Ins.PlayOneShot(SfxType.SFX_Buy);
				data.Pay((b) => {
					WaittingTip.Hide();
					if (b) {
						if (this.DebugNoServerIAPCheck) {
							BuySuccess(data);
							return;
						}

						if (ShopBoard.Ins == null) {
							QueryOrder(false, null);
						} else {
							QueryOrder(true, null);
						}
					} else {
						CommonDialog.Show("", LString.GAMECLIENT_MODIAP_BUY_2.ToLocalized(), LString.CommonDialog_BG_BtnConfirm_Text.ToLocalized(), null);
					}
				});
			} else {
				ItemInfo costItem = Client.Item.GetItem(data.Currency.ID);
				if (costItem.ChangeAmount(-data.CurrencyAmount)) {
					Client.Item.GainItem(data.Item, data.Amount, true);
					Client.EventMgr.SendEvent(EventEnum.Item_Buy, data.Item.ID, data.Amount);
					AnalyticsMgrBase.Ins.Purchase(data);
					CommonTip.Show((LString.GAMECLIENT_MODIAP_BUY_1).ToLocalized());
					SfxManager.Ins.PlayOneShot(SfxType.SFX_Buy);
				} else {
					CommonTip.Show(costItem.Data.Name + (LString.GAMECLIENT_MODIAP_BUY_3).ToLocalized());
					SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
					ShowShopBoardForSupply(costItem.Data.ID);
				}
			}


		}

		public void ShowShopBoardForSupply(int id) {
			if (!Client.Config.OpenStore) {
				CommonDialog.Show("", LString.GAMEUI_BTNFIRSTSPREE_ONPOINTERCLICK_1.ToLocalized(), LString.CommonDialog_BG_BtnConfirm_Text.ToLocalized(), null);
				return;
			}
			if (id == Client.Item.CoinData.ID) {
				ShopBoard.Show(IAPType.Coin);
			} else if (id == Client.Item.DiamondData.ID) {
				ShopBoard.Show(IAPType.Diamond);
			} else if (id == Client.Item.StaminaData.ID) {
				ShopBoard.Show(IAPType.Stamina);
			}
		}

	}
}
