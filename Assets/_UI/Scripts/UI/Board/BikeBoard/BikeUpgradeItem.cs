using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class BikeUpgradeItem : MonoBehaviour {
		public BikeUpgradeType UpgradeType;
		public Text Current, Next, Cost;
		public Image CostIcon;
        private float _timer;
        private float _animTime = 0.4f;
        private float barValue;
        public Button UpgradeButton;
		public GameObject UpgradeButtonFullLv;
		public GameObject arrowIcon;

		public MotorUpdateBoard motorUpdateBoard;
		//public MaskBar Bar;
		public RectTransform barRect;
		private RectTransform parentRect;

		void Awake() {
			this.UpgradeButton.onClick.AddListener(OnUpgradeClick);
			parentRect = (RectTransform)barRect.parent;
		}

		public void Refresh() {
            this._timer = 0f;
            var bikeInfo = this.motorUpdateBoard.bikeInfo;
			bool locked = !bikeInfo.isUnLock;

			BikeUpgradeItemData itemData = bikeInfo.Data.UpgradeItemDatas[this.UpgradeType];
			int lv = bikeInfo.UpgradeItemLv[this.UpgradeType];
			this.Current.text = itemData.GetValue(lv).ToString();
			this.Next.text = itemData.GetValue(lv + 1).ToString();
			this.Cost.text = itemData.GetCost(lv).ToString();

			this.CostIcon.sprite = bikeInfo.Data.LvUpgradeCost.Icon.Sprite;

			//计算当前值在区内内的位置
			barValue = (itemData.GetValue(lv) - Client.Bike.GetUpgradeBase(this.UpgradeType)) /
							 (Client.Bike.GetUpgradeMax(this.UpgradeType) - Client.Bike.GetUpgradeBase(this.UpgradeType));
			//Bar.SetValue(barValue, 0.2f);
			if (barValue >= 1.0f) {
				barValue = 1.0f;
			}
			if (barValue <= 0.0f) {
				barValue = 0.0f;
			}

			//barRect.sizeDelta = new Vector2 (barValue * (parentRect.sizeDelta.x - 10),barRect.sizeDelta.y);
			if (lv >= Client.Bike.LevelMax) {
				//已满级
				this.Cost.gameObject.SetActive(false);
				this.UpgradeButtonFullLv.SetActive(true);
				this.arrowIcon.SetActive (false);
				this.UpgradeButton.interactable = false;
				this.Next.gameObject.SetActive(false);
				if (bikeInfo.IsAllUpgradeMax)
				{
					//UGUIGrey.SetGreyMaterail(motorUpdateBoard.AbilityBg,false);
					//motorUpdateBoard.Max.SetActive(true);
				}
			} else {
				this.UpgradeButton.interactable = !locked;
				this.Cost.gameObject.SetActive(!locked);
				this.UpgradeButtonFullLv.SetActive(false);
				this.Next.gameObject.SetActive (true);
				this.arrowIcon.SetActive (true);
			}
			this.UpgradeButton.targetGraphic.SetGreyMaterail(locked);
		}

		public void OnUpgradeClick() {
			var bikeInfo = this.motorUpdateBoard.bikeInfo;
			if (Client.Bike.UpgradeBike(bikeInfo.Data.ID, this.UpgradeType)) {
				ModelShow.Ins.ShowUpgradeEffect(false);
				SfxManager.Ins.PlayOneShot(SfxType.sfx_menu_upgrade);
				//this.motorUpdateBoard.UpdateBikeInfo();
				Refresh();
                motorUpdateBoard.SetBikeInfo();
                motorUpdateBoard.UpdatePowScore();
            } else {
				SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
				CommonTip.Show(bikeInfo.Data.LvUpgradeCost.Name + (LString.BIKEBOARD_BUY_1).ToLocalized());
				Client.IAP.ShowShopBoardForSupply(bikeInfo.Data.LvUpgradeCost.ID);
			}
		}

        void Update()
        {
            if (this._timer < this._animTime)
            {
                this._timer += Time.deltaTime;
                var size = this.barRect.sizeDelta;
                size.x = Mathf.Lerp(this.barRect.sizeDelta.x, barValue * (parentRect.sizeDelta.x - 10), this._timer / this._animTime);
                barRect.sizeDelta = new Vector2(size.x, barRect.sizeDelta.y);
            }
        }

    }
}
