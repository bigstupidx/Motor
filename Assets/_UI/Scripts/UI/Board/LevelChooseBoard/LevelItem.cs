
using System.Collections;
using GameClient;
using Joystick;
using Joystick.Other;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameUI {
	public class LevelItem : MonoBehaviour, IPointerClickHandler {
		public Image Bg;
		public Image Outside;
		public Image Board;
		public Image Icon;
		public Image OutLine;
		public GameObject AllStar;
		public Text UnlockStarText;
		public Image[] Stars;
		public MatchInfo Info;
		public GameObject BoardPos;
		public int Index { get; set; }

		public void SetData(int index, MatchInfo info) {
			this.Index = index;
			OutLine.color = UIDataDef.OutBlue;
			Info = info;
			//处理星星
			var count = 0;
			for (var i = 0; i < this.Stars.Length; i++) {
				this.Stars[i].gameObject.SetActive(false);
				this.Stars[i].color = Color.black;
				if (info.TaskResults[i]) count++;
			}
			for (var i = 0; i < count; i++) {
				this.Stars[i].gameObject.SetActive(true);
				this.Stars[i].color = Color.white;
			}

			bool unlock = info.IsUnlocked();
			foreach (var graphic in this.Stars) {
				graphic.color = UIDataDef.StarBlue;
			}

			if (unlock) {
				Stars[0].gameObject.SetActive(true);
				Stars[1].gameObject.SetActive(true);
				Stars[2].gameObject.SetActive(true);
				this.UnlockStarText.gameObject.SetActive(false);
				Bg.transform.parent.gameObject.SetActive(false);
				for (int i = 0; i < info.TaskResults.Count; i++) {
					this.Stars[i].color = info.TaskResults[i] ? UIDataDef.StarYellow : Color.grey;
				}
				switch (info.GetStarCount()) {
					case 0:
						Stars[0].gameObject.SetActive(false);
						Stars[1].gameObject.SetActive(false);
						Stars[2].gameObject.SetActive(false);
						Bg.transform.parent.gameObject.SetActive(true);
						Outside.gameObject.SetActive(false);
						Board.gameObject.SetActive(false);
						Bg.sprite = AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Icon_Level_Open");
						IsCurrentMatch();
						break;
					case 1:
						AllStar.GetComponent<RectTransform>().localPosition = new Vector3(0, -28f, 0);
						SetBoard(false);
						break;
					case 2:
						AllStar.GetComponent<RectTransform>().localPosition = new Vector3(0, -28f, 0);
						SetBoard(false);
						break;
					case 3:
						SetBoard(false);
						AllStar.GetComponent<RectTransform>().localPosition = new Vector3(0, -28f, 0);
						break;
				}
			} else {
				Stars[0].gameObject.SetActive(false);
				Stars[1].gameObject.SetActive(false);
				Stars[2].gameObject.SetActive(false);
				this.UnlockStarText.gameObject.SetActive(true);
				Outside.gameObject.SetActive(false);
				Board.gameObject.SetActive(false);
				Bg.transform.parent.gameObject.SetActive(true);
				Bg.sprite = AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Icon_Level_Lock");
				this.UnlockStarText.text = info.Data.UnlockStarCount.ToString() + (LString.GAMEUI_LEVELITEM_SETDATA).ToLocalized();
			}
			//levelName.text = info.Data.Name;
		}

		public void OnPointerClick(PointerEventData eventData) {
			if (this.Info.IsUnlocked()) {
				GamePrepareBoard.Show(Info, false);
				//Client.Spree.OnArriveShowPoint(ShowPoint.LevelClick, (state) => { LevelDialog.Show(Info); });
			} else {
				CommonTip.Show((LString.GAMEUI_LEVELITEM_ONPOINTERCLICK).ToLocalized());
			}
		}

		public void SetBoard(bool IsCurrent) {
			Board.gameObject.SetActive(true);
			SetIcon();
			if (IsCurrent) {
				Board.sprite = AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Icon_Board_Normal");
				Outside.gameObject.SetActive(true);
				GetComponent<LevelItemFocusable>().Focus();
			} else {
				Board.sprite = AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Icon_Board_Daoju");
				Outside.gameObject.SetActive(false);
			}
		}

		public void SetIcon() {
			switch (Info.Data.GameMode) {
				case GameMode.Timing:
				case GameMode.Racing:
				case GameMode.Elimination:
					Icon.sprite = AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Icon_Normal");
					break;
				case GameMode.RacingProp:
				case GameMode.EliminationProp:
					Icon.sprite = AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Icon_Daoju");
					break;
			}
		}

		public void IsCurrentMatch() {
			int Index = Info.Data.Index;
			foreach (var match in LevelChooseBoard.Ins.matchList) {
				if (match.IsUnlocked()) {
					if (match.GetStarCount() == 0) {
						if (Index >= match.Data.Index) {
							Index = match.Data.Index;
						}
					}
				}
			}
			if (Index == Info.Data.Index) {
				Bg.transform.parent.gameObject.SetActive(false);
				OutLine.color = UIDataDef.OutYello;
				SetBoard(true);
			}
		}


	}


}
