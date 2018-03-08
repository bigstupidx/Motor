using GameClient;

namespace GameUI {
	public class BtnTask : CommonBtn {
		public BtnAudio BtnAudio;

		private TaskInfo info;
		private TaskListItem item;
		void Start() {
			Btn.onClick.AddListener(OnBtnClick);
		}

		public void SetData(TaskInfo info, TaskListItem item) {
			this.info = info;
			this.item = item;
			if (!gameObject.activeSelf) {
				gameObject.SetActive(true);
			}
			switch (info.State) {
				case TaskState.Doing:
					SetEnable(true, (LString.GAMEUI_BTNTASK_SETDATA).ToLocalized());
					Bg.sprite = UIDataDef.Sprite_Button_Grey;
					Btn.enabled = false;
					BtnAudio.SfxType = SfxType.SFX_Cant;
					break;
				case TaskState.Completed:
					SetEnable(true, (LString.GAMEUI_BTNTASK_SETDATA_1).ToLocalized());
					Bg.sprite = UIDataDef.Sprite_Button_Yellow;
					Btn.enabled = true;
					BtnAudio.SfxType = SfxType.SFX_Btn;
					break;
				case TaskState.Rewarded:
					SetEnable(true, (LString.GAMEUI_BTNTASK_SETDATA_2).ToLocalized());
					Bg.sprite = UIDataDef.Sprite_Button_Green;
					Btn.enabled = false;
					BtnAudio.SfxType = SfxType.SFX_Cant;
					break;
				case TaskState.Locked:
					SetEnable(false, (LString.GAMEUI_BTNTASK_SETDATA_3).ToLocalized());
					Bg.sprite = UIDataDef.Sprite_Button_Grey;
					Btn.enabled = false;
					BtnAudio.SfxType = SfxType.SFX_Cant;
					break;
			}
		}

		public void OnBtnClick() {
			switch (info.State) {
				case TaskState.Doing:
					//TODO 跳转页面
					break;
				case TaskState.Completed:
					item.Click();
					break;
			}
		}
	}


}
