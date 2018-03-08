using UnityEngine;
using System.Collections;
using Game;
using UnityEngine.EventSystems;

public class BtnTurnRight : MonoBehaviour,IPointerDownHandler , IPointerUpHandler{
    
    public void OnPointerDown(PointerEventData eventData)
    {
		//游戏中才响应按钮事件
		if (GameModeBase.Ins == null || GameModeBase.Ins.State != GameState.Gaming)
		{
			return;
		}

		BikeInputTouch.Ins.TurnRight = true;
    }

	public void OnPointerUp(PointerEventData eventData)
	{
		//游戏中才响应按钮事件
		if (GameModeBase.Ins == null || GameModeBase.Ins.State != GameState.Gaming)
		{
			return;
		}

		BikeInputTouch.Ins.TurnRight = false;
	}
}
