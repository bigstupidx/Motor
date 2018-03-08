
using System.Collections;
using Game;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class UIRaceInfoTiming : UIRaceInfoBase {

		public Text Turn;//圈数
		public Text CheckDis;

		protected Color TargetColor = Color.white;

		private int _currentPassCount;

		public override void Init() {
			base.Init();
			RaceTime.SetColor(Color.white);
			if (GamePlayBoard.Inited) {
				return;
			}
			TargetColor = Color.white;
			_currentPassCount = 0;
			Turn.text = _currentPassCount + "/" + GameModeTiming.Ins.CheckerCount;
			GamePlayBoard.Inited = true;
		}

		public override void UpdateRaceTime() {
			RaceTime.SetTime(GameModeTiming.Ins.TimeLeft);
		}

		public void UpdateTurn() {
			if (_currentPassCount != GameModeTiming.Ins.PassCount) {
				_currentPassCount = GameModeTiming.Ins.PassCount;
				Turn.text = _currentPassCount + "/" + GameModeTiming.Ins.CheckerCount;
			}
		}

		public override void Update() {
			base.Update();
			UpdateTurn();

			this.TargetColor = GameModeTiming.Ins.TimeLeft < 10 ? Color.red : Color.white;
			RaceTime.SetColor(Color.Lerp(RaceTime.raceTime.color, TargetColor, Time.deltaTime));
			CheckDis.text = (int)GameModeTiming.Ins.NextDis() + "";
		}

	}
}
