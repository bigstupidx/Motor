
using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;
using XPlugin.Data.SQLite;
using XPlugin.Data.Json;

namespace GameClient {

	public enum SpreeType {
		Free = 0,
		RMB = 1,
		First = 2//首充
	}

	public enum ShowType {
		None = 0,
		Shop = 1,
		BikeBoard = 2,
		HeroBoard = 3
	}

	public enum BtnCloseType {
		Normal = 0,//清晰
		Blurry = 1,//模糊
		Hide = 2//隐藏
	}

	public class SpreeData : BuyItemData {
		public string Desc;
		public new SpreeType Type;
		public int PayValue;//单位：分
		public bool IsBuyMore;
		public string BtnName;
		public int Sort;
		public List<RewardItemInfo> AwardList;
		public ShowType ShowType;
		public BtnCloseType BtnCloseType;


		public SpreeData(SqliteDataReader reader) : base(reader) {
			Desc = (string)reader.GetValue("Description");
			Type = (SpreeType)reader.GetValue("SpreeType");
			PayValue = (int)reader.GetValue("PayValue");
			IsBuyMore = (bool)reader.GetValue("IsBuyMore");
			BtnName = (string)reader.GetValue("BtnName");
			Sort = (int)reader.GetValue("Sort");
			ShowType = (ShowType)reader.GetValue("ShowType");
			BtnCloseType = (BtnCloseType)reader.GetValue("BtnCloseType");

			string str = (string)reader.GetValue("AwardList");
			JArray json = JArray.Parse(str);
			AwardList = new List<RewardItemInfo>();
			for (int i = 0; i < json.Count; i++) {
				int id = json[i][0].AsInt();
				int amount = json[i][1].AsInt();
				AwardList.Add(new RewardItemInfo(id, amount));
			}
		}

		public SpreeData(JObject json) {
			ID = int.Parse(json["id"].ToString());
			Name = (string)json["name"];
			Desc = (string)json["description"];
			Type = (SpreeType)int.Parse(json["type"].ToString());
			if (Type == SpreeType.RMB) {
				PayCode = int.Parse(json["pay_code"].ToString());
				PayValue = int.Parse(json["pay_value"].ToString());
			} else {
				PayCode = -1;
			}

			string s = json["icon"].ToString();
			if (!string.IsNullOrEmpty(s)) {
				int iconId = int.Parse(s);
				Icon = Client.Icon[iconId];
			}
			IsHot = int.Parse(json["hot"].ToString()) == 1 ? true : false;
			IsBuyMore = int.Parse(json["buy_more"].ToString()) == 1 ? true : false;
			BtnName = json["btn_name"].ToString();
			Sort = int.Parse(json["sort"].ToString());
			ShowType = (ShowType)int.Parse(json["show_type"].ToString());
			BtnCloseType = (BtnCloseType)int.Parse(json["close_type"].ToString());

			JArray awards = (JArray)json["award"];
			AwardList = new List<RewardItemInfo>();
			for (int i = 0; i < awards.Count; i++) {
				int id = int.Parse(awards[i][0].ToString());
				int amount = int.Parse(awards[i][1].ToString());
				AwardList.Add(new RewardItemInfo(id, amount));
			}
		}

		public override SDKPayInfo GetPayInfo() {
			return new SDKPayInfo() {
				Name = this.Name,
				Desc = this.Name,
				Id = this.ID,
				Amount = 1,
				Price = this.PayValue
			};
		}
	}

}
