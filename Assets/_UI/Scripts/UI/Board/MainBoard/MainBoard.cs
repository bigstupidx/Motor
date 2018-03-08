using GameClient;
using UnityEngine.UI;
using System.Collections.Generic;
using Joystick;
using XUI;

namespace GameUI {

	public class MainBoard : SingleUIStackBehaviour<MainBoard> {
		#region base

		public const string UIPrefabPath = "UI/Board/MainBoard/MainBoard";

		public static void Show() {
			string[] UIPrefabNames ={
				UIPrefabPath,
				UICommonItem.TOP_BOARD,
			};
			GroupIns = ModMenu.Ins.Cover(UIPrefabNames);
			Client.EventMgr.SendEvent(EventEnum.UI_MainMenuBoard_Inited);
			Client.Spree.OnArriveShowPoint(ShowPoint.EnterGame);
		}
		public override void OnUISpawned() {
			base.OnUISpawned();
			ModelShow.Ins.ChangeCameraPos(ModelShow.CameraPos.MainMenu);
			var heroPrefab = Client.Hero[Client.User.UserInfo.ChoosedHeroID].Prefab;
			var bikePrefab = Client.Bike[Client.User.UserInfo.ChoosedBikeID].Prefab;
			ModelShow.Ins.ShowMainMenuModel(heroPrefab, bikePrefab);

			ChangeNickName(Client.User.UserInfo.Setting.Nickname);
			ChangeAvatar(string.Empty);

			SetBtnTag(DataDef.modetype);
			this.TopBtnList.SetTopBtns();
			SetRedPoint();
			IsShow = true;
			Client.EventMgr.AddListener(EventEnum.System_OverDay, OverDayListener);
			Client.EventMgr.SendEvent(EventEnum.UI_EnterMenu, "MainBoard");
		}

		private void OverDayListener(EventEnum eventEnum, object[] objects) {
			this.TopBtnList.SetTopBtns();
		}

		public override void OnUIDespawn() {
			base.OnUIDespawn();
			ModelShow.Ins.HideHero();
			ModelShow.Ins.HideBike();
			IsShow = false;
			Client.EventMgr.RemoveListener(EventEnum.System_OverDay, OverDayListener);
		}


		public override void OnUIBeenOverlay() {
			base.OnUIBeenOverlay();
			IsShow = false;
		}

		public override void OnUIDeOverlay() {
			base.OnUIDeOverlay();
			ChangeNickName(Client.User.UserInfo.Setting.Nickname);
			SetBtnTag(DataDef.modetype);
			this.TopBtnList.SetTopBtns();
			SetRedPoint();
			IsShow = true;
		}

		public override void OnUIBeenCover() {
			base.OnUIBeenCover();
			IsShow = false;
		}

		public override void OnUIDeCover() {
			base.OnUIDeCover();
			ChangeNickName(Client.User.UserInfo.Setting.Nickname);
			SetBtnTag(DataDef.modetype);
			this.TopBtnList.SetTopBtns();
			SetRedPoint();
			IsShow = true;
		}


		public override void OnUILeaveStack() {
			base.OnUILeaveStack();
			GroupIns = null;
		}

		void OnEnable() {
			foreach (var t in this.Tweeners) {
				t.ResetToBeginning();
				t.PlayForward();
			}
		}

		#endregion

		public static bool IsShow = false;

		public static UIGroup GroupIns { get; private set; }

		public Text NickName;
		public Image PortraitIcon;
		public RedPoint[] RedPoints;
		public List<UITweener> Tweeners;
		public ModeTag[] BtnTags;

		public MainBoardTopBtnList TopBtnList;

		public void ChangeNickName(string name) {
			NickName.text = name;
		}

		public void ChangeAvatar(string index) {
			PortraitIcon.sprite = Client.User.ChoosedHeroInfo.Data.Icon.Sprite;
		}

		public void SetRedPoint() {
			for (int i = 0; i < RedPoints.Length; i++) {
				RedPoints[i].SetState();
			}
		}

		public void SetBtnTag(ModeType type) {
			if (FocusManager.TVMode) {
				return;
			}
			for (int i = 0; i < BtnTags.Length; i++) {
				if (BtnTags[i].Type == type) {
					BtnTags[i].SetSelectState(true);
				} else {
					BtnTags[i].SetSelectState(false);
				}
			}
		}


		#region Btns

		//成就&每日任务
		public void OnBtnAchiveClick() {
			TaskBoard.Show();
		}

		//道具商店
		public void OnBtnPropShopClick() {
			PropBoard.Show();
		}

		//武器
		public void OnBtnWeaponClick() {
			WeaponBoard.Show();
		}

		//角色
		public void OnBtnHeroClick() {
			HeroBoard.Show(Client.User.UserInfo.ChoosedHeroID);
		}

		//车库
		public void OnBtnBikeClick() {
			GarageBoard.Show(Client.User.UserInfo.ChoosedBikeID);
		}

		//商城
		public void OnBtnShopClick() {
			ShopBoard.Show(IAPType.Diamond);
		}

		//我的信息
		public void OnBtnUserInfoClick() {
			UserInfoBoard.Show();
		}

		//闯关模式
		public void OnBtnAdventureClick() {
			DataDef.modetype = ModeType.Stage;
			SetBtnTag(DataDef.modetype);
			OnModeClick();
			this.DelayInvoke(ChapterChooseBoard.Show, 0.4f);
			//ChapterChooseBoard.Show();
		}

		void OnModeClick() {
			foreach (var t in this.Tweeners) {
				t.PlayReverse();
			}
		}

		//多人对战
		public void OnBtnNetMatchClick() {
			DataDef.modetype = ModeType.Network;
			SetBtnTag(DataDef.modetype);

			if (!Client.Config.OpenNetMatch) {
				CommonTip.Show((LString.GAMEUI_MAINMENUBOARD_ONBTNNETMATCHCLICK).ToLocalized());
				return;
			}
			//			if (Client.Match.GetTotalOwnedStar() < 9) {
			//				CommonTip.Show((LString.GAMEUI_MAINMENUBOARD_ONBTNNETMATCHCLICK_1).ToLocalized());
			//				return;
			//			}

			if (Client.Online.CheckNetwork()) {
				if (Client.User.UserInfo.Setting.UserId == -1) {//未注册用户id
					if (Client.User.UserInfo.Setting.Nickname == Client.User.InitNickName) {//如果是初始昵称，要求用户修改昵称
						ChangeNickNameDialog.Show();
					} else {//可能从sdk获取了昵称,直接尝试注册
						ChangeNickNameDialog.CheckNickName(Client.User.UserInfo.Setting.Nickname, b => {
							switch (b) {
								case ChangeNickBackType.Success:
									DelayShowNetMatchBoard();
									break;
								case ChangeNickBackType.Fail:
									ChangeNickNameDialog.Show();
									break;
								case ChangeNickBackType.AlreadyExist:
									ChangeNickNameDialog.Show();
									break;
								case ChangeNickBackType.NetworkFail:
									break;
							}
						});
					}

					return;
				}
				DelayShowNetMatchBoard();
			} else {
				CommonDialog.Show((LString.GAMEUI_MAINMENUBOARD_ONBTNNETMATCHCLICK_3).ToLocalized(), (LString.GAMEUI_MAINMENUBOARD_ONBTNNETMATCHCLICK_4).ToLocalized(), (LString.GAMEUI_ANDROIDRETURNKEY_UPDATE_2).ToLocalized(), null, null, null);
			}
		}

		private void DelayShowNetMatchBoard() {
			OnModeClick();
			ChooseOnlineMode.Show();
		}

		//锦标赛
		public void OnBtnChampionClick() {
			DataDef.modetype = ModeType.Season;
			SetBtnTag(DataDef.modetype);

			if (Client.Online.CheckNetwork()) {
				if (Client.User.UserInfo.Setting.UserId == -1) {//未注册用户id
					if (Client.User.UserInfo.Setting.Nickname == Client.User.InitNickName) {//如果是初始昵称，要求用户修改昵称
						ChangeNickNameDialog.Show();
					} else {//可能从sdk获取了昵称,直接尝试注册
						ChangeNickNameDialog.CheckNickName(Client.User.UserInfo.Setting.Nickname, b => {
							switch (b) {
								case ChangeNickBackType.Success:
									ShowChampionshipBoard();
									break;
								case ChangeNickBackType.Fail:
									ChangeNickNameDialog.Show();
									break;
								case ChangeNickBackType.AlreadyExist:
									ChangeNickNameDialog.Show();
									break;
								case ChangeNickBackType.NetworkFail:
									break;
							}
						});

					}

					return;
				}
				ShowChampionshipBoard();
			} else {
				CommonDialog.Show((LString.GAMEUI_MAINMENUBOARD_ONBTNNETMATCHCLICK_3).ToLocalized(), (LString.GAMEUI_MAINMENUBOARD_ONBTNNETMATCHCLICK_4).ToLocalized(), (LString.GAMEUI_ANDROIDRETURNKEY_UPDATE_2).ToLocalized(), null, null, null);
			}
		}

		private void ShowChampionshipBoard() {
			WaittingTip.Show((LString.GAMEUI_TOPFUNCLASS_SHOWCHAMPIONSHIPBOARD).ToLocalized());
			Client.Championship.HaveChampionship((b) => {
				WaittingTip.Hide();
				if (b) {
					OnModeClick();
					ChampionshipBoard.Show();
				} else {
					CommonTip.Show((LString.GAMEUI_MAINMENUBOARD_SHOWCHAMPIONSHIPBOARD_1).ToLocalized());
				}
			});
		}

		//挑战模式
		public void OnBtnChallageClick() {
			DataDef.modetype = ModeType.Challenge;
			SetBtnTag(DataDef.modetype);
			OnModeClick();
			this.DelayInvoke(ChallengeBoard.Show, 0.4f);
		}
		#endregion

	}

}

