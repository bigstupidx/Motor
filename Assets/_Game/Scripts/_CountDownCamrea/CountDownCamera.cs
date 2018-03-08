using System;
using System.Collections;
using UnityEngine;
using GameUI;

namespace Game {
	public class CountDownCamera : Singleton<CountDownCamera> {
		public NormalPrefab CountDownCameraPos;

		private CountDownCameraPos countDownCameraPos;
		private bool showMovie;

		public AnimationCurve MovieFadeCurve;

		public Action OnShowMovieFinish = delegate { };
		public Action OnCountDownGo = delegate { };

		public void Init() {
			RaceManager.Ins.OnWaitingStart += () => {
				CountDownBoard.Show();
				showMovie = true;
				StartCoroutine(ShowMovie());
			};

			RaceManager.Ins.OnCountDownStart += () => {
				showMovie = false;
				SetCountDownTarget(BikeManager.Ins.CurrentBike.transform);
				StopAllCoroutines();
				StartCoroutine(ShowCountDown());
			};

			RaceManager.Ins.OnMatchStart += () => {
				enabled = false;
			};
		}

		public void SetCountDownTarget(Transform target) {
			countDownCameraPos = CountDownCameraPos.Spawn().GetComponent<CountDownCameraPos>();
			countDownCameraPos.transform.position = target.position;
			countDownCameraPos.transform.rotation = target.rotation;
			ShowNum(0);
		}

		IEnumerator ShowMovie() {
			float time = 4f;
			float timer = 0;
			int i = 0;
			while (showMovie) {
				while (timer < time) {
					timer += Time.deltaTime;
					var t = timer / time;
					SetCountDownCameraPos(CountDownMoviePos.Ins.MoviePos[i], CountDownMoviePos.Ins.MoviePos[i + 1], t);
					CountDownBoard.Ins.Dark.alpha = MovieFadeCurve.Evaluate(t);
					yield return null;
				}
				timer = 0;
				i += 2;
				if (i >= CountDownMoviePos.Ins.MoviePos.Count - 1) {//回到第一个继续播
					i = 0;
				}
				OnShowMovieFinish();
			}
		}

		IEnumerator ShowCountDown() {
			float time = 1f;
			float timer = 0;

			int count = 3;
			while (count >= 0) {
				CountDown(count);
				while (timer < time) {
					timer += Time.deltaTime;
					if (count >= 0) {
						var t = timer / time;
						CountDownBoard.Ins.Dark.alpha = MovieFadeCurve.Evaluate(t);
						switch (count) {
							case 3:
								SetCountDownCameraPos(countDownCameraPos.CameraPos[0], countDownCameraPos.CameraPos[1], t);
								break;
							case 2:
								SetCountDownCameraPos(countDownCameraPos.CameraPos[2], countDownCameraPos.CameraPos[3], t);
								break;
							case 1:
								SetCountDownCameraPos(countDownCameraPos.CameraPos[4], countDownCameraPos.CameraPos[5], t);
								break;
						}

					}
					yield return null;
				}
				timer = 0;
				count--;
			}
		}

		/// <summary>
		/// 倒计时
		/// 3-1 为数字倒计时，0 为显示GO提示
		/// </summary>
		public void CountDown(int second) {
			ShowNum(second);
			switch (second) {
				case 0:
					SfxManager.Ins.PlayOneShot(SfxType.SFX_CountDown_Go);
					OnCountDownGo();
					break;
				case 1:
					SfxManager.Ins.PlayOneShot(SfxType.SFX_CountDown);
					StartLine.Ins.ShowGo();
					break;
				case 2:
					SfxManager.Ins.PlayOneShot(SfxType.SFX_CountDown);
					break;
				case 3:
					SfxManager.Ins.PlayOneShot(SfxType.SFX_CountDown);
					// 启动音效
					SfxManager.Ins.PlayOneShot(BikeManager.Ins.CurrentBike.bikeSound.StartAudio.Clip);
					break;
			}
		}

		private void ShowNum(int num) {
			for (var i = 0; i < countDownCameraPos.NumPos.Count; i++) {
				countDownCameraPos.NumPos[i].gameObject.SetActive(i + 1 == num);
			}
		}

		private void SetCountDownCameraPos(Transform from, Transform to, float t) {
			this.transform.position = Vector3.Lerp(from.position, to.position, t);
			this.transform.rotation = Quaternion.Lerp(from.rotation, to.rotation, t);
		}

	}

}
