using System.Collections.Generic;
using GameClient;
using UnityEngine;
using UnityEngine.UI;
using XUI;

namespace GameUI {
	public class LevelChooseBoard : SingleUIStackBehaviour<LevelChooseBoard> {

		public const string UIPrefabPath = "UI/Board/LevelChooseBoard/LevelChooseBoard";

		public static void Show(int chapterID, bool destroyBefore = false) {
			string[] UIPrefabNames ={
				UIPrefabPath,
				UICommonItem.TOP_BOARD
			};
			GroupIns = ModMenu.Ins.Cover(UIPrefabNames, "LevelChooseBoard", destroyBefore);
			LevelChooseBoard ins = GroupIns[0].Instance.GetComponent<LevelChooseBoard>();
			ins.CurrentChapterID = chapterID;
			ins.CurrentShowListIndex = 0;
			ins.Init();
			IsFirstEnter = false;
		}

		public static bool IsFirstEnter = true;

		public override void OnUILeaveStack() {
			GroupIns = null;
		}

		public override void OnUISpawned() {
			base.OnUISpawned();
			if (GroupIns != null) {//如果该界面已经显示了，说明是从关闭状态重新打开，这时需要初始化
				Init();
			}
		}


		public static UIGroup GroupIns { get; private set; }


		public int CurrentChapterID;
		public int CurrentShowListIndex;

		public Text ChapterName;
		public RawImage Bg;
		public RawImage LineBg;
		public ScrollRect Scroll;
		public LevelItem ItemPrefab;
		[System.NonSerialized]
		public List<LevelItem> Items = new List<LevelItem>();

		public Text StarState;
		public float ScrollTime = 0.5f;
		public RewardCase Case;

		public ChapterInfo Chapter;
		public List<MatchInfo> matchList;


		public float LerpTo = -1;
		void Update() {
			if (this.LerpTo >= 0) {
				this.Scroll.horizontalNormalizedPosition = Mathf.Lerp(this.Scroll.horizontalNormalizedPosition, this.LerpTo,
					10 * Time.deltaTime);
			}
		}

		public void Init() {
			LerpTo = -1;
			Chapter = Client.Match.GetChapterInfo(CurrentChapterID);
			matchList = Chapter.GetSortedMatches();
			ChapterName.text = Chapter.Data.Name;//章节名称设置
			Bg.texture = Chapter.Data.LevelBg.Texture;//背景图设置
			Case.Init(Chapter);//设置宝箱状态
			Reload();
			if (IsFirstEnter) {
				JumpToCurrent();
			}

			int own = Client.Match.GetChapterOwnedStar(this.CurrentChapterID);
			int total = Client.Match.GetChapterTotalStar(this.CurrentChapterID);
			this.StarState.text = own + "/" + total;
		}

		private void JumpToCurrent() {
			int current = 0;
			foreach (var info in matchList) {
				if (!info.IsUnlocked()) {
					current = info.Data.Index - 1;
					break;
				}

			}

			if (current < 6) {
				Scroll.horizontalNormalizedPosition = 0;
			} else if (current < 10) {
				Scroll.horizontalNormalizedPosition = 0.5f;
			} else {
				Scroll.horizontalNormalizedPosition = 1;
			}
		}


		private void Reload() {
			for (int i = 0; i < Items.Count; i++) {
				Items[i].gameObject.SetActive(false);
			}

			var totalCount = Chapter.Data.Matches.Count;
			if (Items.Count < totalCount) {
				int count = totalCount - Items.Count;
				for (int i = 0; i < count; i++) {
					LevelItem item = Instantiate(ItemPrefab);
					item.transform.SetParent(LineBg.rectTransform);
					item.gameObject.SetActive(false);
					item.gameObject.name = "levelItem" + i;
					Items.Add(item);
				}
			}
			for (int i = 0; i < totalCount; i++) {
				Items[i].gameObject.SetActive(true);
				Items[i].transform.localScale = Vector3.one;
				Items[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(Chapter.Data.UILine.PointList[i].PosX, Chapter.Data.UILine.PointList[i].PosY);
				Items[i].SetData(i, matchList[i]);
			}
		}

	}


}
