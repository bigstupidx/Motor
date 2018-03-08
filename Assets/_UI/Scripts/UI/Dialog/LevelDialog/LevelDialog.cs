using UnityEngine;
using GameClient;
using UnityEngine.UI;

namespace GameUI {
	public class LevelDialog : MonoBehaviour {

		[System.Serializable]
		public class LevelDialogTask {
			public GameObject Star;
			public Text TaskDesc;
		}

		#region base

		public const string UIPrefabPath = "UI/Dialog/LevelDialog";
		public static string[] UINames =
		{
			UICommonItem.DIALOG_BLACKBG_NOCLICK,
			UIPrefabPath,
			UICommonItem.TOP_BOARD_BACK
		};

		public static void Show(MatchInfo info) {
			LevelDialog ins = (ModMenu.Ins.Overlay(UINames)[1]).Instance.GetComponent<LevelDialog>();
			ins.CurrentLevelInfo = info;
			ins.Init();//这边需要赋值后才调用Init
		}

		void OnUISpawned() {
			if (this.CurrentLevelInfo != null) {
				RefreshEquip();
			}
		}

		//		void OnUIDespawn() {
		//		}

		void OnUILeaveStack() {
			this.CurrentLevelInfo = null;
		}

		#endregion

		public MatchInfo CurrentLevelInfo;

		//TOP
		public RawImage Icon;//TODO 这个图还没有动态换
		public Text Name;
		public Text Mode;
		public Text Index;
		public Text ModeDesc;
		public Text NeedHero;
		public Text NeedBike;

		public Image ChoosedHero;
		public Image ChoosedBike;
		public Image ChoosedProp;
		public Image ChoosedWeapon;

		public Text PropCount;
		public Text WeaponCount;

		public ReduceStamina ReduceStaminaEffect;
		public Transform Mask;

		[Reorderable]
		public LevelDialogTask[] Task;

		public void Init()
		{
			ReduceStaminaEffect.Init();
			Mask.gameObject.SetActive(false);

			MatchData data = this.CurrentLevelInfo.Data;
			this.Name.text = this.CurrentLevelInfo.Data.Name;
			this.Mode.text = DataDef.GameModeName(this.CurrentLevelInfo.Data.GameMode);
			this.ModeDesc.text = DataDef.GameModeDesc(this.CurrentLevelInfo.Data.GameMode);
			this.Index.text = (data.Chapter.Index + 1).ToString("00") + "-" + (data.Index + 1).ToString("00");
			this.NeedHero.text = data.NeedHero == null ? (LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_INIT).ToLocalized() : data.NeedHero.Name;
			this.NeedBike.text = data.NeedBike == null ? (LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_INIT).ToLocalized() : data.NeedBike.Name;

			RefreshEquip();

			//task
			for (int i = 0; i < data.LevelTasks.Length; i++) {
				LevelDialogTask ui = this.Task[i];
				ui.Star.SetActive(this.CurrentLevelInfo.TaskResults[i]);
				ui.TaskDesc.text = data.LevelTasks[i].Description;
			}
		}

		//从角色，车辆，武器等界面返回时应该刷新
		public void RefreshEquip() {
			UserInfo user = Client.User.UserInfo;
			this.ChoosedHero.sprite = Client.Hero[user.ChoosedHeroID].Icon.Sprite;
			this.ChoosedBike.sprite = Client.Bike[user.ChoosedBikeID].Icon.Sprite;

			if (user.EquipedPropList.Count > 0) {
				this.ChoosedProp.gameObject.SetActive(true);
				var propData = Client.Prop[user.EquipedPropList[0]];
				this.ChoosedProp.sprite = propData.Icon.Sprite;
				this.PropCount.text = Client.Prop.GetPropInfo(user.EquipedPropList[0]).Amount.ToString();
			} else {
				this.ChoosedProp.gameObject.SetActive(false);
				this.PropCount.text = "0";
			}

			var weaponData = Client.Weapon[user.ChoosedWeaponID];
			this.ChoosedWeapon.sprite = weaponData.Icon.Sprite;
			if (!weaponData.Consum) {
				this.WeaponCount.text = "∞";
			} else {
				this.WeaponCount.text = Client.Weapon.GetWeaponInfo(user.ChoosedWeaponID).Amount.ToString();
			}

		}

		public void OnBtnOKClick() {
			//限定人物、车辆检测
			if (CurrentLevelInfo.Data.NeedHero != null && Client.User.UserInfo.ChoosedHeroID != CurrentLevelInfo.Data.NeedHero.ID)
			{
				CommonTip.Show((LString.GAMEUI_GAMEPREPAREBOARD_UPDATEMOTORINFO).ToLocalized() + CurrentLevelInfo.Data.NeedHero.Name + "</color>");
				return;
			}

			if (CurrentLevelInfo.Data.NeedBike != null && Client.User.UserInfo.ChoosedBikeID != CurrentLevelInfo.Data.NeedBike.ID)
			{
				CommonTip.Show((LString.GAMEUI_GAMEPREPAREBOARD_UPDATEMOTORINFO_1).ToLocalized() + CurrentLevelInfo.Data.NeedBike.Name + "</color>");
				return;
			}

			//消耗体力
			if (Client.Stamina.ChangeStamina(-this.CurrentLevelInfo.Data.NeedStamina))
			{
				var info = CurrentLevelInfo;
				Mask.gameObject.SetActive(true);
				ReduceStaminaEffect.Show(this.CurrentLevelInfo.Data.NeedStamina);
				
				this.DelayInvoke(() =>
				{
					ModMenu.Ins.Back();
					StoryBoard.Show(info);
				}, 0.5f);

			} else
			{
				CommonTip.Show((LString.GAMEUI_CHAMPIONSHIPDETAILBOARD_ONENTERPREPARE_3).ToLocalized());
			}
		}

		public void OnBtnHeroClick() {
			HeroBoard.Show(Client.User.UserInfo.ChoosedHeroID);
		}
		public void OnBtnBikeClick() {
			GarageBoard.Show(Client.User.UserInfo.ChoosedBikeID);
		}

		public void OnBtnWeaponClick() {
			WeaponBoard.Show();
		}
		public void OnBtnPropClick() {
			PropBoard.Show();
		}


	}


}
