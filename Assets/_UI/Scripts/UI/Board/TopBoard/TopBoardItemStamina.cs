using System.Collections;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class TopBoardItemStamina : TopBoardItem {
		public GameObject RestoreBoard;
		public Text RestoreTime;
		public Color FullStaminaColor = Color.green;
		public Color EmptyStaminaColor = Color.red;

		void OnEnable() {
			if (this.Type == IAPType.Stamina) {
				StartCoroutine(UpdateStanima());
			}
		}

		IEnumerator UpdateStanima() {
			while (true) {
				yield return new WaitForSecondsRealtime(1);
				SetStaminaAmount(Client.User.UserInfo.Stamina.NowValue);
			}
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			Client.User.UserInfo.Stamina.OnChange -= OnStaminaChange;
		}

		public override void Init() {
			base.Init();
			Client.User.UserInfo.Stamina.OnChange += OnStaminaChange;
		}

		private void OnStaminaChange(int oldValue, int newValue) {
			SetStaminaAmount(newValue);
			this.Tweener.ResetToBeginning();
			this.Tweener.PlayForward();
		}

		public override void Refresh() {
			SetStaminaAmount(Client.User.UserInfo.Stamina.NowValue);
		}

		private void SetStaminaAmount(int amount) {
			this.Count.text = amount.ToString() + "/" + Client.User.UserInfo.Stamina.MaxValue.ToString();
			if (Client.User.UserInfo.Stamina.IsReachMax) {
				this.Count.color = this.FullStaminaColor;
				this.RestoreBoard.gameObject.SetActive(false);
			} else {
				if (Client.User.UserInfo.Stamina.NowValue == 0) {
					this.Count.color = this.EmptyStaminaColor;
				} else {
					this.Count.color = Color.white;
				}
				this.RestoreBoard.gameObject.SetActive(true);
				this.RestoreTime.text = CommonUtil.GetFormatTimeMinSec(Client.User.UserInfo.Stamina.RestoreNextTime);
			}
		}
	}
}