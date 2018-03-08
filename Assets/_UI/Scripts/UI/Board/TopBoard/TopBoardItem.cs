using System;
using System.Collections;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class TopBoardItem : MonoBehaviour {
		public IAPType Type;
		public Text Count;
		public Button AddButton;
		public Button Button;//整个Item区域
		public UITweener Tweener;

		protected virtual void Start() {
			Init();
		}

		protected virtual void OnDestroy() {
			switch (Type) {
				case IAPType.Coin:
					Client.Item.Coin.OnAmountChange -= OnAmountChange;
					break;
				case IAPType.Diamond:
					Client.Item.Diamond.OnAmountChange -= OnAmountChange;
					break;
			}
		}

		public virtual void Init() {
			AddButton.onClick.AddListener(OnBtnClick);
			Button.onClick.AddListener(OnBtnClick);
			switch (Type) {
				case IAPType.Coin:
					Client.Item.Coin.OnAmountChange += OnAmountChange;
					break;
				case IAPType.Diamond:
					Client.Item.Diamond.OnAmountChange += OnAmountChange;
					break;
			}
		}

		private void OnAmountChange(ItemInfo itemInfo) {

			this.Count.text = itemInfo.Amount.ToString();
			this.Tweener.ResetToBeginning();
			this.Tweener.PlayForward();
		}

		public virtual void Refresh() {
			int amount = 0;
			switch (Type) {
				case IAPType.Diamond:
					amount = Client.Item.Diamond.Amount;
					break;
				case IAPType.Coin:
					amount = Client.Item.Coin.Amount;
					break;
			}
			this.Count.text = amount.ToString();
		}

		public void OnBtnClick() {
			ShopBoard.Show(Type);
		}
	}

}

