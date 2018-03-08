using System.Collections.Generic;
using GameUI;
using Joystick.UGUI;

namespace Joystick.Other {
	public class TaskListBtnFocusable : FocusEnhancedCell {


		//第一个任务按钮向上时去到每日/成就一栏
		public override FocusItemBase Get(DirType dir, List<FocusItemBase> list) {
			if (dir != DirType.Up) {
				return base.Get(dir, list);
			} else {
				var item = transform.GetComponentInParent<TaskListItem>();
				if (item.DataIndex == 0) {
					return TaskBoard.Ins.AchieveTaskBtn.GetComponent<FocusItemBase>();
				} else {
					return base.Get(dir, list);
				}
			}
		}
	}
}