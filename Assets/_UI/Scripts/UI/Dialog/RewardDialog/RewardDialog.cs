using UnityEngine;
using System.Collections.Generic;
using Game;
using GameClient;
using UnityEngine.UI;
using XUI;

namespace GameUI {
	public enum GoToSee {
		None,
		GarageBoard,
		HeroBoard,
		PropBoard,
		WeaponBoard
	}

	public class RewardDialog : UIStackBehaviour {

		public const string UIPrefabPath = "UI/Dialog/RewardDialog/RewardDialog";

		public static void Show(List<RewardItemInfo> list, GoToSee seeWhat = GoToSee.None) {
			string[] UINames ={
				UICommonItem.DIALOG_BLACKBG_NOCLICK,
				UIPrefabPath
			};
			RewardDialog dialog = ModMenu.Ins.Overlay(UINames)[1].Instance.gameObject.GetComponent<RewardDialog>();
			dialog.seeWhat = seeWhat;
			dialog.Init(list);
		}

		public override void OnUIDespawn() {
			foreach (var i in this.items) {
				Destroy(i.gameObject);
			}
			this.items.Clear();
		}

		public UITweener BtnTw;
		public GridLayoutGroup Content;
		public RewardItem ItemPrefab;
		public float ItemWidth, ItemHeight;
		public float DelayTime;
		public GameObject BtnOK;
		public GameObject BtnCancel;
		public GameObject BtnSee;
		public Text Tag;

		private List<RewardItem> items = new List<RewardItem>();
		private GoToSee seeWhat = GoToSee.None;
		private int seeId;

		public void Init(List<RewardItemInfo> list) {
			BtnTw.ResetToBeginning();

			float width, height;
			if (list.Count > 3) {
				width = ItemWidth * 4 + 3 * 20;
				height = ItemHeight * (Mathf.Ceil(list.Count / (4 * 1.0f)));
			} else {
				width = ItemWidth * list.Count + 20 * (list.Count - 1);
				height = ItemHeight;
			}
			Content.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
			Content.cellSize = new Vector2(ItemWidth, ItemHeight);

			bool bikeIn = false;
			bool heroIn = false;
			for (int i = 0; i < list.Count; i++) {
				RewardItem item = Instantiate(ItemPrefab);
				item.SetData(list[i].Data, list[i].Amount);
				if (list[i].Data.Type == ItemType.Bike) {
					seeId = list[i].Data.ID;
					bikeIn = true;
				} else if (list[i].Data.Type == ItemType.Hero) {
					if (!bikeIn) {
						seeId = list[i].Data.ID;
					}
					heroIn = true;
				}
				item.gameObject.transform.SetParent(Content.transform);
				item.gameObject.transform.localScale = Vector3.one;
				for (int j = 0; j < item.Tweeners.Length; j++) {
					item.Tweeners[j].delay = DelayTime * i;
				}
				items.Add(item);
			}
			BtnTw.delay = DelayTime * list.Count;
			BtnTw.PlayForward();

			if (seeWhat == GoToSee.None) {
				if (bikeIn) {
					seeWhat = GoToSee.GarageBoard;
				} else if (heroIn) {
					seeWhat = GoToSee.HeroBoard;
				}
			}

			SetSeeWhat();
		}

		private void SetSeeWhat() {
			switch (seeWhat) {
				case GoToSee.GarageBoard:
					Tag.text = (LString.GAMEUI_REWARDDIALOG_SETSEEWHAT).ToLocalized();
					BtnCancel.SetActive(true);
					BtnSee.SetActive(true);
					BtnOK.SetActive(false);
					break;
				case GoToSee.HeroBoard:
					Tag.text = (LString.GAMEUI_REWARDDIALOG_SETSEEWHAT_1).ToLocalized();
					BtnCancel.SetActive(true);
					BtnSee.SetActive(true);
					BtnOK.SetActive(false);
					break;
				case GoToSee.None:
					BtnCancel.SetActive(false);
					BtnSee.SetActive(false);
					BtnOK.SetActive(true);
					break;
			}
		}

		public void OnBtnSeeClick() {
			if (this.timer < this.time) {
				return;
			}
			var id = seeId;
			var see = seeWhat;
			if (GameModeBase.Ins != null) {
				Client.Guide.FinishGuide(GuideType.StoryGuide_21_TaskBoard);
				GameUIInterface.Ins.ExitGame(MainBoard.GroupIns, () => {
					GoTo(see, id);
				});
			} else {
				ModMenu.Ins.Back();
				GoTo(see, id);
			}

		}

		private void GoTo(GoToSee see, int id) {
			Debug.Log("==>seeid:" + id);
			if (see == GoToSee.GarageBoard) {
				Client.Guide.FinishGuide(GuideType.StoryGuide_18_BikeBoard);
				GarageBoard.Show(id);
			} else {
				Client.Guide.FinishGuide(GuideType.StoryGuide_17_HeroBoard);
				HeroBoard.Show(id);
			}
		}


		private float timer;
		private float time = 0.5f;

		void Update() {
			this.timer += Time.deltaTime;

		}

		public void __OnOkClick() {
			if (this.timer >= time) {//避免电视上连续点击ok跳过
				ModMenu.Ins.Back();
			}
		}
	}


}
