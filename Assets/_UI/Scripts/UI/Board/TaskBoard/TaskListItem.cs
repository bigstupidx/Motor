using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class TaskListItem : EnhancedScrollerCellView {
		public Image Bg;
		public Image Icon;
		public Text Name;
		public Text Desc;
		public Text Schedule;
		public BtnTask Btn;
		public RectTransform RewardListRt;
		public float RewardItemWidth;
		public GameObject Reward;
		public int DataIndex { get; private set; }
		public GameObject Mask;
		public TweenAlpha tweenalpha;
		public TweenAlpha HasGetalpha;
		public TweenAlpha Textalpha;
		private TaskInfo info;
		private List<RewardItem> items = new List<RewardItem>();
		public TweenCanvasGroupAlpha tweenalphaall;

		public void SetData(int index, TaskInfo info) {
			tweenalphaall.ResetToBeginning();
			tweenalphaall.PlayForward();
			this.info = info;
			DataIndex = index;
			Init();
		}

		private void Init() {
			Icon.sprite = info.Data.Icon.Sprite;
			Name.text = info.Data.Name;
			Desc.text = info.Data.Description;
			Schedule.text = info.ProgressStr;
			Btn.SetData(info, this);

			switch (info.State) {
				case TaskState.Completed:
					Bg.sprite = UIDataDef.Sprite_Frame_BG_Item_Selected;
					Btn.gameObject.SetActive(true);
					this.Mask.SetActive(false);
					Reward.SetActive(true);
					this.Schedule.gameObject.SetActive(true);
					break;
				case TaskState.Rewarded:
					Bg.sprite = UIDataDef.Sprite_Frame_BG_Item_Normal;
					Btn.gameObject.SetActive(false);
					this.Mask.SetActive(true);
					Reward.SetActive(false);
					this.Schedule.gameObject.SetActive(false);
					break;
				default:
					Btn.gameObject.SetActive(true);
					this.Mask.SetActive(false);
					Reward.SetActive(true);
					this.Schedule.gameObject.SetActive(true);
					Bg.sprite = UIDataDef.Sprite_Frame_BG_Item_Normal;
					break;
			}

			//奖励列表
			for (int i = 0; i < items.Count; i++) {
				Destroy(items[i].gameObject);
			}
			items.Clear();
			//RewardListRt.sizeDelta = new Vector2(RewardItemWidth * info.Data.RewardList.Count, RewardListRt.sizeDelta.y);
			for (int i = 0; i < info.Data.RewardList.Count; i++) {
				RewardItem item = Instantiate(TaskBoard.Ins.RewardPrefab);
				item.SetData(info.Data.RewardList[i].Data, info.Data.RewardList[i].Amount);
				item.transform.SetParent(RewardListRt);
				item.transform.localScale = Vector3.one;
				items.Add(item);
			}
			if (info.State != TaskState.Rewarded) {
				tweenalpha.ResetToBeginning();
				HasGetalpha.ResetToBeginning();
				Textalpha.ResetToBeginning();
			} else {
				tweenalpha.alpha = 1;
				HasGetalpha.alpha = 1;
				Textalpha.alpha = 1;
			}
		}

		public void Click() {
			Btn.gameObject.SetActive(false);
			this.Mask.SetActive(true);
			this.Reward.SetActive(false);
			ShowTweener(this.tweenalpha, 0);
			ShowTweener(this.HasGetalpha, 0.5f);
			ShowTweener(this.Textalpha, 0.5f);
			GetReward();
//			this.DelayInvoke(GetReward, 0.7f);
		}

		private void ShowTweener(UITweener tweener, float delay) {
			this.DelayInvoke(() => {
				tweener.ResetToBeginning();
				tweener.PlayForward();
			}, delay);
		}

		public void GetReward() {
			info.GetReward((b, list) => {
				if (b) {
					SfxManager.Ins.PlayOneShot(SfxType.SFX_Blink);
					RewardDialog.Show(list);
				} else {
					CommonTip.Show((LString.GAMEUI_TASKLISTITEM_GETREWARD).ToLocalized());
				}
			});
		}
	}


}
