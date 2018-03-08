using UnityEngine;
using Game;
using UnityEngine.EventSystems;

public class BtnDriftWithDirection : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{

	public bool isLeft;

	public void OnPointerDown(PointerEventData eventData)
	{
		//游戏中才响应按钮事件
		if (GameModeBase.Ins == null || GameModeBase.Ins.State != GameState.Gaming)
		{
			return;
		}

		BikeInputTouch.Ins.bikeInput.OnDrift();
		if (isLeft)
		{
			BikeInputTouch.Ins.TurnRight = false;
			BikeInputTouch.Ins.TurnLeft = true;
		}
		else
		{
            BikeInputTouch.Ins.TurnLeft = false;
			BikeInputTouch.Ins.TurnRight = true;
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		//游戏中才响应按钮事件
		if (GameModeBase.Ins == null || GameModeBase.Ins.State != GameState.Gaming)
		{
			return;
		}

		if (isLeft)
		{
			BikeInputTouch.Ins.TurnLeft = false;
		} else
		{
			BikeInputTouch.Ins.TurnRight = false;
		}
	}
}
