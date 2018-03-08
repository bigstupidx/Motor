using System;
using UnityEngine;
using XPlugin.Data.Json;
using XPlugin.Security.AnitiCheatValue;

namespace GameClient {
	public class ItemInfo {
		[JsonIgnore]
		public ItemData Data;

		private cint _amount;
		public cint Amount {
			get { return this._amount; }
			protected set {
				this._amount = value;
				if (this.OnAmountChange != null) {
					this.OnAmountChange(this);
				}
			}
		}

		public RedPointState RedPointState = RedPointState.NotYetShow;

		[JsonIgnore]
		public Action<ItemInfo> OnAmountChange;

		public ItemInfo() {
		}

		public ItemInfo(int itemID, int amount = 0)
			: this(Client.Item[itemID], amount) {
		}

		public ItemInfo(ItemData data, int amount = 0) {
			this.Data = data;
			this.Amount = amount;
		}

		public virtual bool ChangeAmount(int delta) {
			if (delta == 0) {
				return false;
			}
			if (Amount + delta < 0) {//数量不够
				return false;
			}
			this.Amount += delta;
			Client.EventMgr.SendEvent(delta > 0 ? EventEnum.Item_Gain : EventEnum.Item_Use, (int)Data.Type, Mathf.Abs(delta));
			Client.EventMgr.SendEvent(EventEnum.Item_ChangeAmount, Data.ID, delta);
			if (delta < 0)
			{
				AnalyticsMgrBase.Ins.Use(Data.ID.ToString(), Mathf.Abs(delta));
			}
			if (delta > 0 && RedPointState == RedPointState.NotYetShow)
			{
				RedPointState = RedPointState.ShouldShow;
			}
			SaveData();
			return true;
		}

		public virtual bool ChangeAmountWithoutSave(int delta) {
			if (delta == 0)
			{
				return false;
			}
			if (Amount + delta < 0)
			{
				return false;
			}
			this.Amount += delta;
			return true;
		}

		public virtual void SaveData() {
			Client.Item.SaveData();
		}


		public void InitData(ItemData data) {
			this.Data = data;
		}
	}
}
