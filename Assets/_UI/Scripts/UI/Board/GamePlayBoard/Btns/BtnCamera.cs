using UnityEngine;
using Game;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace GameUI {
	public class BtnCamera : MonoBehaviour, IPointerClickHandler
	{

		public List<Sprite> Icon;
		public Image Image;

		void Awake()
		{
			SetIcon(GameClient.Client.User.UserInfo.Setting.CamaerMode);
		}

		public void OnPointerClick(PointerEventData eventData) {
			//游戏中才响应按钮事件
			if (GameModeBase.Ins == null || GameModeBase.Ins.State != GameState.Gaming)
			{
				return;
			}

			var mode = GameClient.Client.User.UserInfo.Setting.CamaerMode;
			switch (mode)
			{
				case CameraMode.Drift:
					mode = CameraMode.Fixed;
					break;
				case CameraMode.Fixed:
					mode = CameraMode.Drift;
					break;
			}
			BikeCamera.Ins.SetCameraMode(mode);
			SetIcon(mode);
		}

		private void SetIcon(CameraMode mode)
		{
			switch (mode)
			{
				case CameraMode.Drift:
					Image.sprite = Icon[0];
					break;
				case CameraMode.Fixed:
					Image.sprite = Icon[1];
					break;
			}
		}
	}


}
