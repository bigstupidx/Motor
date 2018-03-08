using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameClient;
using XUI;

namespace GameUI {
	public class GamePrepareBoard : UIStackBehaviour {

		[System.Serializable]
		public class LevelDialogTask {
			public Image Star;
			public Text TaskDesc;
		}

		#region base

		public const string UIPrefabPath = "UI/Board/GamePrepareBoard/GamePrepareBoard";

		public static void Show(MatchInfo info, bool destroyBefore = false) {
			string[] UIPrefabNames ={
				UIPrefabPath,
				UICommonItem.TOP_BOARD,
			};

			ModelShow.Ins.ChangeCameraPos(ModelShow.CameraPos.PrepareBoard);
			GroupIns = ModMenu.Ins.Cover(UIPrefabNames, "GamePrepareBoard", destroyBefore);
			GamePrepareBoard ins = (GroupIns[0]).Instance.GetComponent<GamePrepareBoard>();
			ins.CurrentLevelInfo = info;
			ins.Init();//这边需要赋值后才调用Init
		}

		public override void OnUISpawned() {
			base.OnUISpawned();
			if (this.CurrentLevelInfo != null) {
				RefreshEquip();
			}
		}

		public override void OnUILeaveStack() {
			this.CurrentLevelInfo = null;
			GroupIns = null;
		}

		#endregion

		public static UIGroup GroupIns { get; private set; }

		public MatchInfo CurrentLevelInfo;

		public Text levelName;
		public Text LevelMode;
		public Text StationName;
		public Image selectHeroIcon;
		public Image selectWeaponIcon;
		public Image selectPropIcon;
		public Image selectBikeIcon;

		public Text weaponCount;
		public Text propCount;

		public Text LevelName2;
		public RawImage BG;
		public RawImage BG2;
		public Text TargetDes;

		public Text staminaCount;
		public ReduceStamina ReduceStaminaEffect;
		public Transform Mask;
		public OnlineBoard Boards;
		public LevelDialogTask[] Task;
		public GameObject NormalInfo;
		public GameObject OtherInfo;

		public void Init() {
			ReduceStaminaEffect.Init();
			Mask.gameObject.SetActive(false);
			MatchData data = this.CurrentLevelInfo.Data;
			this.levelName.text = this.CurrentLevelInfo.Data.Name;
			this.LevelName2.text = this.CurrentLevelInfo.Data.Name;
			this.LevelMode.text = DataDef.GameModeName(this.CurrentLevelInfo.Data.GameMode);
			RefreshEquip();
			Boards.SetData(CurrentLevelInfo);
			TargetDes.text = (LString.GAMEUI_GAMEPREPAREBOARD_INIT).ToLocalized();
			switch (CurrentLevelInfo.MatchMode) {
				case MatchMode.Normal:
					this.StationName.gameObject.SetActive(true);
					this.StationName.text = CurrentLevelInfo.Data.Chapter.Description;
					NormalInfo.SetActive(true);
					OtherInfo.SetActive(false);
					for (int i = 0; i < data.LevelTasks.Length; i++) {
						LevelDialogTask ui = this.Task[i];
						ui.Star.color = this.CurrentLevelInfo.TaskResults[i] ? UIDataDef.StarYellow : Color.grey;
						ui.TaskDesc.text = data.LevelTasks[i].Description;
						if (i == 0) {
                            ui.TaskDesc.text += (LString.GamePrepareBoard_Goal_TEXT).ToLocalized();// "  (首要目标)";
						}
					}
					this.staminaCount.text = this.CurrentLevelInfo.Data.NeedStamina.ToString();
					break;
				case MatchMode.Online:
				case MatchMode.OnlineRandom:
					NormalInfo.SetActive(false);
					OtherInfo.SetActive(true);
					this.staminaCount.text = DataDef.GameModestaminaCount(CurrentLevelInfo.Data.GameMode).ToString();
					break;
				case MatchMode.Challenge:
					this.StationName.gameObject.SetActive(false);
					NormalInfo.SetActive(true);
					OtherInfo.SetActive(false);
					for (int i = 0; i < data.LevelTasks.Length; i++) {
						LevelDialogTask ui = this.Task[i];
						ui.TaskDesc.text = data.LevelTasks[i].Description;
					}
					this.staminaCount.text = this.CurrentLevelInfo.Data.NeedStamina.ToString();
					break;
				case MatchMode.Championship:
					NormalInfo.SetActive(false);
					OtherInfo.SetActive(true);
					this.staminaCount.text = this.CurrentLevelInfo.Data.NeedStamina.ToString();
					break;
			}
		}

		public void Refresh() {
			ModelShow.Ins.ChangeCameraPos(ModelShow.CameraPos.PrepareBoard);
		}
		//从角色，车辆，武器等界面返回时应该刷新
		public void RefreshEquip() {
			ModelShow.Ins.ChangeCameraPos(ModelShow.CameraPos.PrepareBoard);
			//if (this.bikeInfo != null) {
			var heroPrefab = Client.Hero[Client.User.UserInfo.ChoosedHeroID].Prefab;
			var bikePrefab = Client.Bike[Client.User.UserInfo.ChoosedBikeID].Prefab;
			ModelShow.Ins.ShowMainMenuModel(heroPrefab, bikePrefab);
			//}
			UserInfo user = Client.User.UserInfo;
			this.selectHeroIcon.sprite = Client.Hero[user.ChoosedHeroID].Icon.Sprite;
			this.selectBikeIcon.sprite = Client.Bike[user.ChoosedBikeID].Icon.Sprite;
			if (user.EquipedPropList.Count > 0) {
				this.selectPropIcon.gameObject.SetActive(true);
				var propData = Client.Prop[user.EquipedPropList[0]];
				this.selectPropIcon.sprite = propData.Icon.Sprite;
				this.propCount.text = Client.Prop.GetPropInfo(user.EquipedPropList[0]).Amount.ToString();
			} else {
				this.selectPropIcon.gameObject.SetActive(false);
				this.propCount.text = "0";
			}

			var weaponData = Client.Weapon[user.ChoosedWeaponID];
			this.selectWeaponIcon.sprite = weaponData.Icon.Sprite;
			if (!weaponData.Consum) {
				this.weaponCount.transform.parent.gameObject.SetActive(false);
				this.weaponCount.text = "∞";
			} else {
				this.weaponCount.transform.parent.gameObject.SetActive(true);
				this.weaponCount.text = Client.Weapon.GetWeaponInfo(user.ChoosedWeaponID).Amount.ToString();
			}
		}

		#region Button
		//
		public void OnBtnStartGameClick() {
			//限定人物、车辆检测
			if (CurrentLevelInfo.Data.NeedHero != null && Client.User.UserInfo.ChoosedHeroID != CurrentLevelInfo.Data.NeedHero.ID) {
				SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
				CommonTip.Show((LString.GAMEUI_GAMEPREPAREBOARD_UPDATEMOTORINFO).ToLocalized() + CurrentLevelInfo.Data.NeedHero.Name + "</color>");
				return;
			}

			if (CurrentLevelInfo.Data.NeedBike != null && Client.User.UserInfo.ChoosedBikeID != CurrentLevelInfo.Data.NeedBike.ID) {
				SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
				CommonTip.Show((LString.GAMEUI_GAMEPREPAREBOARD_UPDATEMOTORINFO_1).ToLocalized() + CurrentLevelInfo.Data.NeedBike.Name + "</color>");
				return;
			}

			switch (CurrentLevelInfo.MatchMode) {
				case MatchMode.Challenge:
				case MatchMode.Normal:
					if (Client.Stamina.ChangeStamina(-this.CurrentLevelInfo.Data.NeedStamina)) {
						var info = CurrentLevelInfo;
						ReduceStaminaEffect.Show(this.CurrentLevelInfo.Data.NeedStamina);
						SfxManager.Ins.PlayOneShot(SfxType.sfx_menu_start_race);
						this.DelayInvoke(() => {

							if (this.CurrentLevelInfo.MatchMode == MatchMode.Challenge) {
								ChallengeInfo challenge = Client.Challenge.GetChallengeMatchInfo(this.CurrentLevelInfo.Data.ID);
								challenge.hasPlay++;
								challenge.LastTime = Client.System.TimeOnLocal;
								Client.Challenge.SaveData();
							}

							ModMenu.Ins.Back();
							Client.Match.GetMatchInfo(info.Data.ID).SetStoryPlayed();
							Client.Game.StartGame(info);
						}, 0.5f);
					} else {
						SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
						CommonTip.Show((LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_ONENTERPREPARE_3).ToLocalized());
					}
					break;
				case MatchMode.Championship:
				case MatchMode.Online:
				case MatchMode.OnlineRandom:
					break;
			}

		}


		public void OnBtnSelectHeroClick() {
			HeroBoard.Show(Client.User.UserInfo.ChoosedHeroID);
		}

		public void OnBtnSelectPropClick() {
			PropBoard.Show();
		}

		public void OnBtnSelectWeaponClick() {
			WeaponBoard.Show();
		}

		public void OnBtnChangeMotorClick() {
			GarageBoard.Show(Client.User.UserInfo.ChoosedBikeID);
		}
		#endregion
	}
}
