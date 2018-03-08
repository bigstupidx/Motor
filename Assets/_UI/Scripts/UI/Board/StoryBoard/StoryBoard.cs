using System.Collections.Generic;
using GameClient;
using GameUI;
using UnityEngine;
using XUI;

public class StoryBoard : SingleUIStackBehaviour<StoryBoard> {
	public const string UIPrefabPath = "UI/Board/StoryBoard/StoryBoard";

	public static void Show(MatchInfo match) {
		string[] UIPrefabNames =
		{
			UIPrefabPath
		};
		StoryBoard ins = ModMenu.Ins.Overlay(UIPrefabNames)[0].Instance.GetComponent<StoryBoard>();
		ins.gameObject.SetActive(false);
		ins.Match = match;
		ins.Init();
	}

	public override void OnUIDespawn() {
		base.OnUIDespawn();
		for (int i = 0; i < items.Count; i++) {
			Destroy(items[i].gameObject);
		}
		items.Clear();
		currentShowItem = null;
	}

	public MatchInfo Match;
	public CommonBtn Btn;
	public RectTransform List;
	public StoryItem StoryItemPrefab;
	public Vector2 InitSize;


	private List<StoryData> storyList;
	private int schedule;
	private bool isPlay;
	private bool isOver;
	private List<StoryItem> items = new List<StoryItem>();
	public StoryItem currentShowItem;
	private bool canNxt;

	void Start() {
		Btn.Btn.onClick.AddListener(OnBtnClick);
	}

	public void Init() {
		schedule = 0;
		isPlay = false;
		isOver = false;
		canNxt = false;
		List.sizeDelta = InitSize;


		bool needPlayStory = false;
		storyList = new List<StoryData>();
		if (!Match.IsStoryPlayed) {
			storyList = Client.Story.GetStoryByMatchID(Match.Data.ID);
			needPlayStory = storyList.Count > 0;
		}

		if (needPlayStory) {
			gameObject.SetActive(true);
			//播放剧情
			Delay.DelayInvoke(this, () => { Play(0); canNxt = true; }, 0.8f);
		} else {
			//不需要播放剧情，游戏开始
			//TODO 来个动画吧
			Client.Game.StartGame(Match);
		}
	}

	public void Play(int index) {
		canNxt = false;
		StoryItem item = Instantiate(StoryItemPrefab);
		currentShowItem = item;
		items.Add(item);
		item.SetData(storyList[index]);
		List.sizeDelta = new Vector2(List.sizeDelta.x, List.sizeDelta.y + 100);

		RectTransform rt = item.gameObject.GetComponent<RectTransform>();
		rt.SetParent(List);
		rt.localScale = Vector3.one;
		isPlay = true;
		canNxt = true;
	}

	public void OnFinish(int index) {
		if (index < schedule) {
			Debug.LogError("==>>>>>【剧情】序号错误，请检出数值表中的对话顺序");
		}
		isPlay = false;
		schedule += 1;
		//		Debug.Log("==>>>>>>>>story schedule:" +schedule + ";sum:"+ storyList.Count);
		if (schedule >= storyList.Count) {
			isOver = true;
		}
	}

	public void OnBtnClick() {
		if (!canNxt) {
			return;
		}
		if (isPlay) {
			currentShowItem.Typewriter.Finish();
		} else if (isOver) {
			Client.Match.GetMatchInfo(Match.Data.ID).SetStoryPlayed();
			//TODO 来个动画吧
			Client.Game.StartGame(Match);
		} else {
			Play(schedule);
		}
	}

}
