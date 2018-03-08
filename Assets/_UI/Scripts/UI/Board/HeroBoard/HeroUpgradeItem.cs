using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class HeroUpgradeItem : MonoBehaviour {
		public HeroUpgradeType Type;
		public Text Current;
		//public Text Next;
		public HeroBoard HeroBoard;
		public RectTransform barRect;
		private RectTransform parentRect;
		private float _timer;
		private float _animTime = 0.4f;
		private float barValue;
		void Awake() {
			parentRect = (RectTransform)barRect.parent;
		}

		public void Refresh() {
			this._timer = 0f;
			var heroInfo = this.HeroBoard.CurrentShowHero.Info;
			if (heroInfo.IsMaxLv) {
				this.Current.text = heroInfo.Data.UpgradeDatas[this.Type].GetValue(heroInfo.Level).ToString();
			} else {
				this.Current.text = heroInfo.Data.UpgradeDatas[this.Type].GetValue(heroInfo.Level).ToString() + "------" + heroInfo.Data.UpgradeDatas[this.Type].GetValue(heroInfo.Level + 1).ToString();
			}
			float baseValue = heroInfo.Data.UpgradeDatas[this.Type].Base;
			//计算当前值在区内内的位置
			barValue = (heroInfo.Data.UpgradeDatas[this.Type].GetValue(heroInfo.Level) - baseValue) /
				(heroInfo.Data.UpgradeDatas[this.Type].GetValue(Client.Hero.MaxLevel - 1) - baseValue);
			//Bar.SetValue(barValue, 0.2f);
			if (barValue >= 1.0f) {
				barValue = 1.0f;
			}
			if (barValue <= 0.0f) {
				barValue = 0.0f;
			}
		}

		void Update() {
			if (this._timer < this._animTime) {
				this._timer += Time.deltaTime;
				var size = this.barRect.sizeDelta;
				size.x = Mathf.Lerp(this.barRect.sizeDelta.x, barValue * (parentRect.sizeDelta.x - 10), this._timer / this._animTime);
				barRect.sizeDelta = new Vector2(size.x, barRect.sizeDelta.y);
			}
		}

	}
}