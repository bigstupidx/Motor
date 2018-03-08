using GameClient;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using System.Collections.Generic;
using System;
using XUI;

namespace GameUI {
	public class ChampionshipDetailBoard : SingleUIStackBehaviour<ChampionshipDetailBoard>, IEnhancedScrollerDelegate {

		public const string UIPrefabPath = "UI/Board/ChampionshipDetailBoard/ChampionshipDetailBoard";

		public static void Show(ChampionshipInfo info) {
			string[] UINames ={
				UIPrefabPath,
				UICommonItem.TOP_BOARD
			};
			ModelShow.Ins.ChangeCameraPos(ModelShow.CameraPos.ChampionPrepareBoard);
			GroupIns = ModMenu.Ins.Cover(UINames);
			ChampionshipDetailBoard ins = (GroupIns[0]).Instance.GetComponent<ChampionshipDetailBoard>();
			ins.currentInfo = info;
			ins.Init();
			var heroPrefab = Client.Hero[Client.User.UserInfo.ChoosedHeroID].Prefab;
			var bikePrefab = Client.Bike[Client.User.UserInfo.ChoosedBikeID].Prefab;
			ModelShow.Ins.ShowMainMenuModel(heroPrefab, bikePrefab);
			ModelShow.Ins.HideHero();
			ModelShow.Ins.ShowBike(bikePrefab);
		}

		public override void OnUISpawned() {
			base.OnUISpawned();
			ModelShow.Ins.ChangeCameraPos(ModelShow.CameraPos.ChampionPrepareBoard);
			var heroPrefab = Client.Hero[Client.User.UserInfo.ChoosedHeroID].Prefab;
			var bikePrefab = Client.Bike[Client.User.UserInfo.ChoosedBikeID].Prefab;
			ModelShow.Ins.ShowMainMenuModel(heroPrefab, bikePrefab);
			ModelShow.Ins.HideHero();
			ModelShow.Ins.ShowBike(bikePrefab);
			if (GroupIns != null) {
				currentInfo = Client.Championship.GetChampionshipInfo(currentInfo.ChampionshipData.Id);
				Reload();
				SetReward();
				RefreshRank();
			}
		}

		public override void OnUIDeOverlay() {
			base.OnUIDeOverlay();
			currentInfo = Client.Championship.GetChampionshipInfo(currentInfo.ChampionshipData.Id);
			Reload();
			SetReward();
			RefreshRank();
		}

		public override void OnUILeaveStack() {
			GroupIns = null;
		}

		public static UIGroup GroupIns { get; private set; }

		//奖励列表
		public ChampionshipRewardItem[] GameRewards;
		//public ChampionshipRewardItem[] RankRewards;
		public EnhancedScroller Scroller;
		public EnhancedScrollerCellView CellViewPrefab;
		public float CellSize;
		private List<ChampionshipRewardData> _data;

		//赛事信息
		public RawImage Img;
		public Text Name;
		public Text RemainTime;
		public GameObject TimeLable;
		//public Text ModeDesc;
		public Text NeedHero;
		public Text NeedBike;
		public Text MatchName;
		public Text SelfRank;
		public Text staminaCount;

		public GameObject CanReward;
		public Text GetRewardText;
		public GameObject OilCost;

		public Image Bg;
		public ChampionshipInfo currentInfo;
		private bool isFinish = false;

		void Update() {
			if (!isFinish) {
				RefreshTime();
			}
		}

		public void Init() {
			if ((float)Screen.width / (float)Screen.height < 1.5) {
				Bg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(866, 928 + 94 * (currentInfo.ChampionshipData.RankReward.Count - 3));
				Bg.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(-527.2f, -233.7f + 47 * (currentInfo.ChampionshipData.RankReward.Count - 3), 0);
				Debug.LogError(Bg.gameObject.GetComponent<RectTransform>().localPosition);
			} else {
				Bg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(866, 928);
				Bg.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(-527.6f, -54f, 0);
			}
			Scroller.Delegate = this;
			//Img.texture = UIDataDef.GetModeTexture(currentInfo.Data.GameMode);
			Img.texture = UIDataDef.GetModeLinTexture("net1");
			Name.text = currentInfo.Data.Name;
			MatchName.text = currentInfo.Data.Scene.Name;
			staminaCount.text = currentInfo.Data.NeedStamina.ToString();
			string desc = currentInfo.ChampionshipData.Description;
			//ModeDesc.text = string.IsNullOrEmpty(desc)?DataDef.GameModeDesc(currentInfo.Data.GameMode):desc;
			if (currentInfo.ChampionshipData.LimitBikeType == LimitBikeType.ID) {
				NeedBike.text = currentInfo.Data.NeedBike == null ? (LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_INIT).ToLocalized() : currentInfo.Data.NeedBike.Name;
			} else {
				NeedBike.text = currentInfo.ChampionshipData.LimitBikeRank.Equals("-1") ? (LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_INIT).ToLocalized() : currentInfo.ChampionshipData.LimitBikeRank + (LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_INIT_1).ToLocalized();
			}
			NeedHero.text = currentInfo.Data.NeedHero == null ? (LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_INIT).ToLocalized() : currentInfo.Data.NeedHero.Name;
			RefreshTime();
			SetReward();
			Reload();
			RefreshRank();
		}

		private void RefreshTime() {
			if (currentInfo.RemainTime > 0) {
				RemainTime.text = CommonUtil.GetFormatTimeDHMS(currentInfo.RemainTime);
				isFinish = false;
			} else {
				RemainTime.text = (LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_REFRESHTIME).ToLocalized();
				isFinish = true;
			}
			TimeLable.SetActive(!isFinish);
			OilCost.SetActive(!isFinish);
			if (isFinish) {
				CanReward.SetActive(true);
				if (!currentInfo.IsGetReward) {
					if (currentInfo.Rank == -1) {
						GetRewardText.text = LString.GAMEUI_CHAMPIONSHIPITEM_ONCLICK_1.ToLocalized();
					} else {
						GetRewardText.text = LString.ChampionshipItem_CanReward_Image_Text.ToLocalized();
					}
				} else {
					GetRewardText.text = LString.GAMEUI_CHAMPIONSHIPITEM_ONCLICK.ToLocalized();
				}
			} else {
				CanReward.SetActive(false);
			}
		}


		private void SetReward() {
			var gameRewardList = currentInfo.ChampionshipData.GameReward;
			int j = 0;
			for (int i = 2; i >= 0; i--) {
				GameRewards[j].SetData(gameRewardList[i]);
				j++;
			}
		}



		public void OnBtnRankClick() {
			if (Client.Online.CheckNetwork()) {
				WaittingTip.Show((LString.GAMEUI_TOPFUNCLASS_ONBTNRANKCLICK).ToLocalized());
				Client.Rank.GetRank((b) => {
					WaittingTip.Hide();
					if (b) {
						Client.Rank.IsLocal = false;
						RankBoard.Show(Client.Rank.GetRankInfo(currentInfo.ChampionshipData.Id));
					} else {
						CommonTip.Show((LString.GAMEUI_TOPFUNCLASS_ONBTNRANKCLICK_2).ToLocalized());
					}
				});
			}
		}

		private void RefreshRank() {
			WaittingTip.Show((LString.GAMEUI_TOPFUNCLASS_ONBTNRANKCLICK).ToLocalized());
			Client.Championship.GetRank(currentInfo.ChampionshipData.Id, (b, list, self) => {
				WaittingTip.Hide();
				if (b) {
					if (self == null) {
						SelfRank.text = "0";
					} else {
						SelfRank.text = self.Rank.ToString();
					}
				} else {
					CommonTip.Show((LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_ONBTNRANKCLICK_1).ToLocalized());
				}
			});
		}

		public void OnEnterPrepare() {
			if (isFinish) {
				if (!currentInfo.IsGetReward && currentInfo.Rank != -1) {
					Client.Championship.GetRewards(currentInfo.ChampionshipData.Id, (b, list) => {
						currentInfo = Client.Championship.GetChampionshipInfo(currentInfo.ChampionshipData.Id);
						if (b) {
							RewardDialog.Show(list);
						} else {
							CommonTip.Show((LString.GAMEUI_CHAMPIONSHIPITEM_ONCLICK_1).ToLocalized());
						}
					});
					return;
				} else {
					if (currentInfo.IsGetReward) {
						CommonTip.Show((LString.GAMEUI_CHAMPIONSHIPITEM_ONCLICK).ToLocalized());

					} else if (currentInfo.Rank == -1) {
						CommonTip.Show((LString.GAMEUI_CHAMPIONSHIPITEM_ONCLICK_1).ToLocalized());
					}
				}
				return;
			}


			Action enterAction = () => {
				if (currentInfo.Data.NeedHero != null && Client.User.UserInfo.ChoosedHeroID != currentInfo.Data.NeedHero.ID) {
					CommonTip.Show((LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_ONENTERPREPARE).ToLocalized() + currentInfo.Data.NeedHero.Name + "</color>");
					return;
				}

				if (currentInfo.ChampionshipData.LimitBikeType == LimitBikeType.Rank) {
					if (!currentInfo.ChampionshipData.LimitBikeRank.Equals("-1") && !Client.User.ChoosedBikeInfo.Data.Rank.ToString().Equals(currentInfo.ChampionshipData.LimitBikeRank)) {
						CommonTip.Show((LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_ONENTERPREPARE_1).ToLocalized() + currentInfo.ChampionshipData.LimitBikeRank + "</color>");
						return;
					}
				} else {
					if (currentInfo.Data.NeedBike != null && Client.User.UserInfo.ChoosedBikeID != currentInfo.Data.NeedBike.ID) {
						CommonTip.Show((LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_ONENTERPREPARE_2).ToLocalized() + currentInfo.Data.NeedBike.Name + "</color>");
						return;
					}
				}

				//消耗体力
				if (Client.Stamina.ChangeStamina(-currentInfo.Data.NeedStamina)) {
					SfxManager.Ins.PlayOneShot(SfxType.sfx_menu_start_race);
					Client.Championship.CurrentId = currentInfo.ChampionshipData.Id;
					Client.Game.StartGame(currentInfo);
				} else {
					SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
					CommonTip.Show((LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_ONENTERPREPARE_3).ToLocalized());
				}

			};
			if (Client.Config.OnlyIntouchVipCanEnterChampionship) {
				WaittingTip.Show(LString.WaittingTip_Text.ToLocalized());
				Interface.IsInTouchVip(SDKManager.Instance.GetCountryOrRegionId(), (result, callback) => {
					WaittingTip.Hide();
					if (result) {
						enterAction();
					} else {
						if (callback.CallBackType == WebCallBackType.Success) {
							CommonDialog.Show("", LString.ONLY_MTN_MEMBER.ToLocalized(), LString.CommonDialog_BG_BtnConfirm_Text.ToLocalized(), null, null, null);
						} else if (callback.CallBackType == WebCallBackType.TimeOut) {
							CommonTip.Show(LString.WEBREQUESTMANAGER_2.ToLocalized());
						} else if (callback.CallBackType == WebCallBackType.NoConnect) {
							CommonTip.Show(LString.WEBREQUESTMANAGER_1.ToLocalized());
						}
					}
				});
			} else {
				enterAction();
			}
		}

		#region Scroll
		private void Reload() {
			_data = new List<ChampionshipRewardData>();

			foreach (var reward in currentInfo.ChampionshipData.RankReward) {
				_data.Add(reward);
			}
			Scroller.ReloadData();
		}


		public int GetNumberOfCells(EnhancedScroller scroller) {
			return _data.Count;
		}

		public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
			return CellSize;
		}

		public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
			ChampionshipRankRewardItem cellView = scroller.GetCellView(CellViewPrefab) as ChampionshipRankRewardItem;
			cellView.SetData(dataIndex, _data[dataIndex]);
			return cellView;
		}
		#endregion

	}

}
