using UnityEngine;
using System.Collections;
using GameUI;
using UnityEngine.EventSystems;

public class MotorView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector2 first = Vector2.zero;
    private Vector2 second = Vector2.zero;
    private bool isLeft;
    private bool isRight;

    public void OnPointerDown(PointerEventData eventData)
    {
        first = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        second = Input.mousePosition;
        if (first.x - second.x > 100f)
        {
            isLeft = true;
            isRight = false;
            GarageBoard.Ins.OnBtnNextClick();
        }
        else if (second.x - first.x > 100f)
        {
            isLeft = false;
            isRight = true;
            GarageBoard.Ins.OnBtnPreClick();
        }
        else if (Mathf.Abs(second.x - first.x) < 30f)
        {
            ModelShow.Ins.OnBikeClick();
        }
        else
        {
            isLeft = false;
            isRight = false;
        }
    }

    void Update()
    {
    }
}