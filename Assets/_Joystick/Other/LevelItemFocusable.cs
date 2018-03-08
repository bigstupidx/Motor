using System.Collections;
using System.Collections.Generic;
using GameUI;
using Joystick.UGUI;

namespace Joystick.Other {
	public class LevelItemFocusable : FocusItemPointerClick {

		public static Dictionary<int, int> LastFocusLevelItem = new Dictionary<int, int>();

		public LevelItem LevelItem;

		public void Focus() {
			if (FocusManager.TVMode) {
				StartCoroutine(DelayFocus());
			}
		}

		IEnumerator DelayFocus() {
			yield return null;
			yield return null;
			int chapter = LevelChooseBoard.Ins.CurrentChapterID;
			//如果有记录，则聚焦在记录上，没有则聚焦在当前关卡
			if (LastFocusLevelItem.ContainsKey(chapter)) {
				FocusManager.Ins.Focus(LevelChooseBoard.Ins.Items[LastFocusLevelItem[chapter]].GetComponent<LevelItemFocusable>());
			} else {
				FocusManager.Ins.Focus(this);
			}
		}

		public override void OnFocused(FocusItemBase lastFocus) {
			base.OnFocused(lastFocus);
			int totalCount = LevelChooseBoard.Ins.Items.Count - 1;
			if (totalCount == 0) {
				LevelChooseBoard.Ins.LerpTo = 0;
			} else {
				LevelChooseBoard.Ins.LerpTo = (float)(this.LevelItem.Index) / totalCount;
			}

			int chapter = LevelChooseBoard.Ins.CurrentChapterID;
			int index = this.LevelItem.Index;
			LastFocusLevelItem.AddOrReplace(chapter, index);
		}

		public override FocusItemBase Get(DirType dir, List<FocusItemBase> list) {
			this.FirstFocus = false;//释放优先聚焦
			if (dir == DirType.Left) {
				if (this.LevelItem.Index != 0) {
					return LevelChooseBoard.Ins.Items[this.LevelItem.Index - 1].GetComponent<FocusItemBase>();
				}
			} else if (dir == DirType.Right) {
				int totalCount = LevelChooseBoard.Ins.Items.Count - 1;
				if (this.LevelItem.Index != totalCount) {
					return LevelChooseBoard.Ins.Items[this.LevelItem.Index + 1].GetComponent<FocusItemBase>();
				}
			}
			return base.Get(dir, list);
		}
	}
}