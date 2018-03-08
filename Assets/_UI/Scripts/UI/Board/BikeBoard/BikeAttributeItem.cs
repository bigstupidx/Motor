using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameClient;
namespace GameUI
{
	public class BikeAttributeItem : MonoBehaviour
	{

		public BikeUpgradeType UpgradeType;
		public Image barImage;
		public Text Current;
        private float _timer;
        private float _animTime = 0.4f;
        private float barValue;
		public void Refresh (BikeInfo bikeInfo)
		{
            _timer = 0f;
            BikeUpgradeItemData itemData = bikeInfo.Data.UpgradeItemDatas [this.UpgradeType];
			int lv = bikeInfo.UpgradeItemLv [this.UpgradeType];
            if (Current != null)
            {
                this.Current.text = itemData.GetValue(lv).ToString();
            }

            //计算当前值在区内内的位置
            barValue = (itemData.GetValue (lv) - Client.Bike.GetUpgradeBase (this.UpgradeType)) /
							 (Client.Bike.GetUpgradeMax (this.UpgradeType) - Client.Bike.GetUpgradeBase (this.UpgradeType));
        }

        void Update()
        {
            if (this._timer < this._animTime)
            {
                this._timer += Time.deltaTime;
                barImage.fillAmount = Mathf.Lerp(barImage.fillAmount, barValue, this._timer / this._animTime);
            }
        }
	}
}
