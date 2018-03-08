using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using GameUI;

namespace GameUI {
	public class BuySuccessTip : MonoBehaviour {

		public const string PREFAB_PATH = "UI/Tip/BuySuccessTip";

		public static void Show(List<RewardItemInfo> datas) {
			BuySuccessTip tip = ModTip.Ins.Spawn(PREFAB_PATH).GetComponent<BuySuccessTip>();
			tip.ShowTips(datas);
		}

		public TipItem TipItemPrefab;
		public List<TipItem> Items = new List<TipItem>();
		public float DelayTime = 0.5f;

		public void ShowTips(List<RewardItemInfo> datas) {
			List<RewardItemInfo> items = new List<RewardItemInfo>();
			foreach (var i in datas) {
				items.Add(i);
			}
			StartCoroutine(DelayShowTips(items));
		}

		IEnumerator DelayShowTips(List<RewardItemInfo> datas) {
			for (int i = 0; i < datas.Count; i++) {
				ShowSingleTip(datas[i]);
				yield return new WaitForSeconds(DelayTime);
			}

		}

		private void ShowSingleTip(RewardItemInfo data) {
			foreach (TipItem item in Items) {
				if (item.tweAlpha.alpha <= 0) {
					item.Show(data);
					return;
				}
			}

			TipItem tipItem = Instantiate(TipItemPrefab);
			tipItem.transform.SetParent(gameObject.transform);
			tipItem.transform.localPosition = Vector3.zero;
			tipItem.transform.localScale = Vector3.one;
			tipItem.Show(data);
			Items.Add(tipItem);
		}
	}

}
