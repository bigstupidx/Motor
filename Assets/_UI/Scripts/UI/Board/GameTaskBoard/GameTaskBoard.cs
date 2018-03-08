using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Game;
using GameClient;

namespace GameUI {
	public class GameTaskBoard : MonoBehaviour {

		#region base

		public const string UIPrefabPath = "UI/Board/GameTaskBoard";

		public static string[] UINames =
		{
			UIPrefabPath
		};

		public static void Show(bool destroyBefore = false) {
			GameTaskBoard ins = ModMenu.Ins.Cover(UINames, "GameTaskBoard", destroyBefore)[0].Instance.GetComponent<GameTaskBoard>();
			ins.Init();
		}

		//		void OnUISpawned() {
		//			
		//		}
		//
		//		void OnUIDespawn() {
		//			
		//		}

		#endregion

		public Image[] TaskStars;
		public Text[] TaskDescs;
		public Text[] TaskValues;
		public UITweener BackTweener;
		public UITweener TitleTweener;
		public UITweener TagTweener;
		public UITweener[] TaskTweeners;
		//
		//		void Awake()
		//		{
		//			Tweeners = gameObject.GetComponentsInChildren<UITweener>();
		//		}

		public void Init() {
			SetTasks();
			// Reset;
			BackTweener.ResetToBeginning();
			TitleTweener.ResetToBeginning();
			TagTweener.ResetToBeginning();
			for (var i = 0; i < TaskTweeners.Length; i++) {
				TaskTweeners[i].ResetToBeginning();
			}
			// Play
			ShowTweener(BackTweener, 0f);
			ShowTweener(TitleTweener, 0f);
			ShowTweener(TagTweener, 0.25f);
			for (var i = 0; i < TaskTweeners.Length; i++) {
				ShowTweener(TaskTweeners[i], 0.25f * (i + 1));
			}
			StartCoroutine(DelayCountDownBoard());
		}

		private void ShowTweener(UITweener tweener, float delay) {
			this.DelayInvoke(() => {
				tweener.ResetToBeginning();
				tweener.PlayForward();
			}, delay);
		}

		IEnumerator DelayCountDownBoard() {
			yield return new WaitForSeconds(4f);
			CountDownBoard.Show();
		}

		public void SetTasks() {
			var tasks = GameModeBase.Ins.Tasks;
			for (var i = 0; i < tasks.Count; i++) {
				var task = tasks[i];
				TaskDescs[i].text = task.Data.Description;
				TaskValues[i].text = task.ProgressStr;
				TaskStars[i].SetGreyMaterail(true);
			}

			var tasksSave = Client.Game.MatchInfo.TaskResults;
			for (int i = 0; i < tasksSave.Count; i++) {
				if (tasksSave[i]) {
					TaskStars[i].SetGreyMaterail(false);
					TaskValues[i].text = (LString.GAMEUI_GAMETASKBOARD_SETTASKS).ToLocalized();
				}
			}
		}
	}
}

