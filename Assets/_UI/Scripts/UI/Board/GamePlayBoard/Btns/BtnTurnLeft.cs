using UnityEngine;
using System.Collections;
using Game;
using UnityEngine.EventSystems;

public class BtnTurnLeft : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public void OnPointerDown(PointerEventData eventData)
    {
		//游戏中才响应按钮事件
		if (GameModeBase.Ins == null || GameModeBase.Ins.State != GameState.Gaming)
		{
			return;
		}

		BikeInputTouch.Ins.TurnLeft = true;
    }

	public void OnPointerUp(PointerEventData eventData)
	{
		//游戏中才响应按钮事件
		if (GameModeBase.Ins == null || GameModeBase.Ins.State != GameState.Gaming)
		{
			return;
		}

		BikeInputTouch.Ins.TurnLeft = false;
	}
}
