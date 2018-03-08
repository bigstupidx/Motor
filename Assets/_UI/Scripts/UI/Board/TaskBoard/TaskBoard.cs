using System.Collections.Generic;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using GameClient;
using GameUI;
using Joystick;
using UnityEngine.UI;
using XUI;

public class TaskBoard : SingleUIStackBehaviour<TaskBoard>, IEnhancedScrollerDelegate {
	#region base
	public const string UIPrefabPath = "UI/Board/TaskBoard/TaskBoard";

	public static void Show(bool destroyBefore = false) {
		string[] UINames ={
			UICommonItem.MENU_BACKGROUND,
			UIPrefabPath,
			UICommonItem.TOP_BOARD_BACK
		};
		TaskBoard ins = ModMenu.Ins.Cover(UINames, "TaskBoard", destroyBefore)[1].Instance.GetComponent<TaskBoard>();
		ins.CurrentShowMode = TaskMode.DailyTask;
		ins.Init();
		Client.EventMgr.SendEvent(EventEnum.UI_EnterMenu, "TaskBoard");
	}

	private bool _inStack = false;

	public override void OnUISpawned() {
		base.OnUISpawned();
		if (_inStack) {
			Init();
			Client.EventMgr.SendEvent(EventEnum.UI_EnterMenu, "TaskBoard");
		}
	}

	public override void OnUIEnterStack() {
		base.OnUIEnterStack();
		this._inStack = true;
	}

	public override void OnUILeaveStack() {
		base.OnUILeaveStack();
		_inStack = false;
	}

	public override void OnUIDeOverlay() {
		base.OnUIDeOverlay();
		Reload();
	}
	#endregion

	public RewardItem RewardPrefab;

	public CommonBtn DailyTask;
	public CommonBtn Achieve;

	public SmallList<TaskInfo> _data;
	public EnhancedScroller Scroller;
	public EnhancedScrollerCellView CellViewPrefab;
	public float CellSize;

	public Button DailyTaskBtn;
	public Button AchieveTaskBtn;

	public TaskMode CurrentShowMode = TaskMode.DailyTask;

	public void Init() {
		Scroller.Delegate = this;
		if (Client.Task.GetDisplayTask(TaskMode.DailyTask)[0].State != TaskState.Completed &&
			Client.Task.GetDisplayTask(TaskMode.AchieveTask)[0].State == TaskState.Completed) {
			OnBtnAchieveClick();
		} else {
			OnBtnDailyClick();
		}
	}

	public void SetTag() {
		switch (CurrentShowMode) {
			case TaskMode.DailyTask:
				DailyTask.Bg.sprite = UIDataDef.Sprite_Frame_BG_TaskTag_Yellow;
				Achieve.Bg.sprite = UIDataDef.Sprite_Frame_BG_TaskTag_Blue;
				break;
			case TaskMode.AchieveTask:
				DailyTask.Bg.sprite = UIDataDef.Sprite_Frame_BG_TaskTag_Blue;
				Achieve.Bg.sprite = UIDataDef.Sprite_Frame_BG_TaskTag_Yellow;
				break;
		}
	}

	public void OnBtnDailyClick() {
		CurrentShowMode = TaskMode.DailyTask;
		SetTag();
		Reload();
	}

	public void OnBtnAchieveClick() {
		CurrentShowMode = TaskMode.AchieveTask;
		SetTag();
		Reload();
	}

	#region Scroller
	public void Reload() {
		_data = new SmallList<TaskInfo>();

		List<TaskInfo> list = Client.Task.GetDisplayTask(CurrentShowMode);
		for (int i = 0; i < list.Count; i++) {
			_data.Add(list[i]);
		}
		Scroller.ReloadData();

		if (FocusManager.TVMode) {
			var items = this.Scroller.transform.GetComponentsInChildren<BtnTask>();
			FocusItemBase focus = null;
			if (items != null && items.Length > 0&&items[0].Btn.enabled) {
				focus = items[0].GetComponent<FocusItemBase>();
				focus.FirstFocus = true;
			} else {
				focus = this.CurrentShowMode == TaskMode.DailyTask
					? this.DailyTaskBtn.GetComponent<FocusItemBase>()
					: this.AchieveTaskBtn.GetComponent<FocusItemBase>();
				focus.FirstFocus = true;
			}
			if (focus != null) {
				this.DelayInvoke(() => {
					FocusManager.Ins.Focus(focus);
				}, 0.05f);
			}
		}
	}

	public int GetNumberOfCells(EnhancedScroller scroller) {
		return _data.Count;

	}

	public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
		return CellSize;
	}

	public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
		TaskListItem item = scroller.GetCellView(CellViewPrefab) as TaskListItem;
		item.SetData(dataIndex, _data[dataIndex]);
		item.gameObject.name = "taskItem" + dataIndex;
		return item;
	}
	#endregion
}
