using UnityEngine;
using Game;
using GameClient;
using GameUI;
using UnityEngine.UI;

public class BtnGameProp : MonoBehaviour {

	public Image Icon;

	public PropType PropType=PropType.None;

	void OnEnable() {
		if (!GamePlayBoard.Inited) {
			SetProp(PropType.None);
		}
	}

	public void OnClick() {
		//游戏中才响应按钮事件
		if (Time.timeScale == 0) {
			return;
		}
		if (GameModeBase.Ins == null || GameModeBase.Ins.State != GameState.Gaming) {
			return;
		}

		if (BikeManager.Ins.CurrentBike.bikeProp.Use()) {
			Icon.gameObject.SetActive(false);
		}
	}

	public void SetProp(PropType type) {
		this.PropType = type;
		if (type == PropType.None) {
			gameObject.SetActive(false);
			Icon.gameObject.SetActive(false);
		} else {
			gameObject.SetActive(true);
			Icon.gameObject.SetActive(true);
			Icon.sprite = Client.Prop.GetDataByType(type).Icon.Sprite;
		}

	}
}
