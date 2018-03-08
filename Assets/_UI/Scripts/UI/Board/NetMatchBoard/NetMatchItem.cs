using System;
using System.Collections;
using EnhancedUI.EnhancedScroller;
using GameClient;
using GameUI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NetMatchItem : EnhancedScrollerCellView {
	public Transform Renderer;
	public Button Btn;
	public Text Name;
	public RawImage Img;
	public Image[] RewardImg;
	public Text[] RewardNum;
	private GameMode mode;

	public TweenCanvasGroupAlpha tweenalpha;
	public int DataIndex { get; private set; }
	public SelectedDelegate selected;

	void Awake() {
		Btn.onClick.AddListener(OnBtnClick);
	}

	private EnhancedScroller scroller;
	private NetMatchBoard netMatchBoard;


	public void SetData(NetMatchBoard netMatchBoard, int index, GameMode gameMode) {
		tweenalpha.ResetToBeginning();
		tweenalpha.PlayForward();
		this.netMatchBoard = netMatchBoard;
		DataIndex = index;
		mode = gameMode;
		Name.text = DataDef.GameModeName(mode);
		if (mode == GameMode.Racing || mode == GameMode.Elimination) {
			Img.texture = UIDataDef.GetModeLinTexture("net2");
		} else {
			Img.texture = UIDataDef.GetModeLinTexture("net1");
		}
		SetReward();
	}



	public void OnBtnClick() {
		WaittingTip.Show((LString.GAMEUI_MAINMENUBOARD_ONBTNNETMATCHCLICK_2).ToLocalized());
		StartCoroutine(DelayShowNetPrepare(0.3f));
	}

	public void SetReward() {
		var list = Client.Online.GetReward(mode);
		int count = list.Count;
		for (int i = 0; i < count; i++) {
			RewardImg[i].sprite = Client.Item[list[i].ID].Icon.Sprite;
			RewardNum[i].text = "x" + list[i].Amount.ToString();
		}
		for (int i = RewardImg.Length; i > count; i--) {
			RewardImg[i - 1].gameObject.SetActive(false);
			RewardNum[i - 1].gameObject.SetActive(false);
		}
	}

	private IEnumerator DelayShowNetPrepare(float time) {
		yield return new WaitForSeconds(time);
		WaittingTip.Hide();
		RandomNetPrepareDialog.Show(Client.Online.GetMatchInfo(mode, MatchMode.OnlineRandom));
	}
}
