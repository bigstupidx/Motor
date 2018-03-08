using UnityEngine;
using EnhancedUI.EnhancedScroller;
using GameClient;
using GameUI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BikeListItem : EnhancedScrollerCellView, IPointerClickHandler {

	public Image BG;
	public Image Icon;
	public GameObject Lock;
	public RedPoint RedPoint;

	public BikeListItemData Data;

	public int DataIndex { get; private set; }
	public SelectedDelegate selected;

	public void SetData(int index, BikeListItemData data) {
		if (Data != null)
		{
			Data.selectedChanged -= ChooseToShow;
			Data.LockStateToGameChanged -= ChangeLockState;
		}
		Data = data;
		DataIndex = index;
		
		Icon.sprite = Data.Info.Data.Icon.Sprite;
		RedPoint.SetState(data.Info.RedPointState == RedPointState.ShouldShow);

		Data.selectedChanged -= ChooseToShow;
		Data.LockStateToGameChanged -= ChangeLockState;
		Data.selectedChanged += ChooseToShow;
		Data.LockStateToGameChanged += ChangeLockState;

		ChooseToShow(data.Selected);
		data.isUnLock = data.Info.isUnLock;
		ChangeLockState(data.Info.isUnLock);
	}

	public void ChooseToShow(bool isShow) {
		if (isShow)
		{
			BG.sprite = UIDataDef.Sprite_Frame_BG_HeroItem_Selected;
		} else
		{
			BG.sprite = UIDataDef.Sprite_Frame_BG_HeroItem_Normal;
		}
	}

	public void ChangeLockState(bool isUnlock) {
		Lock.SetActive(!isUnlock);
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (Data.Info.RedPointState == RedPointState.ShouldShow)
		{
			Client.Bike.SetRedPointShowed(Data.Info.Data.ID);
			RedPoint.SetState(false);
		}

		if (selected != null)
		{
			selected(this);
		}
	}
}
