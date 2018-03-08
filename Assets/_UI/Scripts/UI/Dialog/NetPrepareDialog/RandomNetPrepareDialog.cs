using UnityEngine;
using System.Collections;
using GameClient;
using GameUI;
using UnityEngine.UI;
using XUI;

public class RandomNetPrepareDialog : Singleton<RandomNetPrepareDialog> {

	#region base

	public const string PrefabPath = "UI/Dialog/NetPrepareDialog/RandomNetPrepareDialog";

	public static void Show(MatchInfo matchInfo) {
		string[] PrefabNames ={
			UICommonItem.MENU_BACKGROUND,
			PrefabPath,
			UICommonItem.TOP_BOARD_BACK
		};
		RandomNetPrepareDialog ins = ModMenu.Ins.Cover(PrefabNames)[1].Instance.GetComponent<RandomNetPrepareDialog>();
		ins.CurrentMode = matchInfo.Data.GameMode;
		ins.matchInfo = matchInfo;
		ins.Init();
	}

	#endregion

	public GameMode CurrentMode;

	public Text Mode;
	public Text Tip;
	public CommonBtn BtnChangeRoom;
	public NetPlayerItem[] Players;
	public float StartDelayTime = 1;
	public float MatchingTimeRandomMin = 0;
	public float MatchingTimeRandomMax = 2;

	public MatchInfo matchInfo;
	private bool isMatching;

	private int _NowPlayerCount = 0;

	public Coroutine MatchingCoro;
	public Coroutine TipCoro;

	public void Init() {
		this.Mode.text = DataDef.GameModeName(this.CurrentMode);
		DoReset();
		this.BtnChangeRoom.gameObject.SetActive(false);//暂时不允许换房间
	}

	public void DoReset() {
		if (this.MatchingCoro != null) {
			StopCoroutine(this.MatchingCoro);
		}

		if (this.TipCoro != null) {
			StopCoroutine(this.TipCoro);
		}

		for (int i = 0; i < this.Players.Length; i++) {
			this.Players[i].SetActive(false);
		}

		this.BtnChangeRoom.SetEnable(true);

		//已经存在房间里的人
		int alreayIn = Client.Online.GetAlreadyInPlayerCount();
		for (int i = 0; i < alreayIn; i++) {
			var enemyInfo = matchInfo.Enemys[i];
			this.Players[i].SetData(enemyInfo.ChoosedHero.Data.Icon.Sprite, enemyInfo.NickName, false);
			this.Players[i].SetActive(true);
		}
		this._NowPlayerCount = alreayIn;

		//玩家自己Client.User.UserInfo.Setting.Avatar
		this.Players[this._NowPlayerCount].SetData(Client.User.ChoosedHeroInfo.Data.Icon.Sprite, Client.User.UserInfo.Setting.Nickname, true);
		this.Players[this._NowPlayerCount].SetActive(true);
		this._NowPlayerCount++;

		isMatching = true;
		MatchingCoro = StartCoroutine(Matching());
		this.Tip.text = (LString.NETPREPAREDIALOG_SHOWTIP).ToLocalized();
	}

	IEnumerator Matching() {
		while (this._NowPlayerCount < matchInfo.PlayerNum) {
			yield return new WaitForSeconds(Random.Range(2, 7));
			int enemyIndex = this._NowPlayerCount - 1;
			if (enemyIndex < 0) {
				enemyIndex = 0;
			}
			PlayerInfo info = matchInfo.Enemys[enemyIndex];
			this.Players[this._NowPlayerCount].SetData(info.ChoosedHero.Data.Icon.Sprite, info.NickName, false);
			this.Players[this._NowPlayerCount].SetActive(true);
			this._NowPlayerCount++;
		}
		StartGame();
	}

	public void StartGame() {
		this.isMatching = false;
		this.BtnChangeRoom.SetEnable(false);
		if (Client.Stamina.ChangeStamina(-matchInfo.Data.NeedStamina)) {
			var info = matchInfo;
			this.DelayInvoke(() => {
				SfxManager.Ins.PlayOneShot(SfxType.sfx_menu_start_race);
				Client.Game.StartGame(info);
			}, this.StartDelayTime);

		}
	}

	public void OnBtnChangeRoomClick() {
		StopCoroutine(TipCoro);
		StopCoroutine(MatchingCoro);
		WaittingTip.Show((LString.NETPREPAREDIALOG_ONBTNCHANGEROOMCLICK).ToLocalized());
		StartCoroutine(DelayChangeRoom(Random.Range(0.2f, 0.5f)));
	}

	private IEnumerator DelayChangeRoom(float time) {
		ResetPlayer();
		yield return new WaitForSeconds(time);
		WaittingTip.Hide();
		DoReset();
	}

	private void ResetPlayer() {
		for (int i = 0; i < matchInfo.Enemys.Count; i++) {
			matchInfo.Enemys[i].NickName = Client.Nickname.Lib.Random;
			matchInfo.Enemys[i].ChoosedHero = Client.Online.GetRandomHeroInfo();
		}
	}

}
