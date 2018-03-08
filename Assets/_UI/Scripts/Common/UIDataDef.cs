
using EnhancedUI.EnhancedScroller;
using GameClient;
using UnityEngine;
using XPlugin.Update;

namespace GameUI {
	/// <summary>
	/// This delegate handles the UI's button click
	/// </summary>
	/// <param name="cellView">The cell view that had the button click</param>
	public delegate void SelectedDelegate(EnhancedScrollerCellView cellView);

	/// <summary>
	/// This delegate handles any changes to the selection state of the data
	/// </summary>
	/// <param name="val">The state of the selection</param>
	public delegate void SelectedChangedDelegate(bool val);

	public delegate void AmountChangeDelegate(int val);

	public delegate void LockStateChangedDelegate(bool val);

	public enum NumPicType {
		White,
		Yellow,
		WhiteWithBlackOutline
	}

	public class UIDataDef {
		public const string Atlas_UI_Name = "UI";
		public const string Atlas_Icon_Name = "Icon";
		public const string Atlas_Flag_Name = "Flag";
		public const string Atlas_Spree_Name = "Spree";

		public static string NetMatchItem_Random_Name = (LString.GAMEUI_UIDATADE).ToLocalized();

		#region btn
		public static Sprite Sprite_Button_Yellow {
			get { return AtlasManager.GetSprite(Atlas_UI_Name, "Button_BG_Common_Active"); }
		}

		public static Sprite Sprite_Button_Green {
			get { return AtlasManager.GetSprite(Atlas_UI_Name, "Button_BG_Common_Normal"); }
		}

		public static Sprite Sprite_Button_Grey {
			get { return AtlasManager.GetSprite(Atlas_UI_Name, "Button_BG_Common_Disable"); }
		}

		public static Sprite Sprite_Button_Blue {
			get { return AtlasManager.GetSprite(Atlas_UI_Name, "Button_BG_Common_Blue_45"); }
		}
		#endregion

		#region normal item
		public static Sprite Sprite_Frame_BG_Item_Selected {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_TaskInfo_Active_40"); }
		}

		public static Sprite Sprite_Frame_BG_Item_Normal {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_TaskInfo_Normal_41"); }
		}
		#endregion

		#region hero/bike/prop/weapon item
		public static Sprite Sprite_Frame_BG_HeroItem_Selected {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_Block_52"); }
		}

		public static Sprite Sprite_Frame_BG_HeroItem_Normal {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_Prop_53"); }
		}

		public static Sprite Sprite_Frame_BG_HeroItemName_Selected {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_HeroItemName_Selected"); }
		}

		public static Sprite Sprite_Frame_BG_HeroItemName_Normal {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_HeroItemName_Normal"); }
		}
		#endregion

		#region task tag
		public static Sprite Sprite_Frame_BG_TaskTag_Yellow {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Button_Task_Tag_Select"); }
		}

		public static Sprite Sprite_Frame_BG_TaskTag_Blue {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Button_Task_Tag_Normal"); }
		}
		#endregion

		#region netMatch player item
		public static Sprite Sprite_Frame_BG_NetMatch_PlayerItem_Selected {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_NetMatch_PlayerItem_Selected"); }
		}

		public static Sprite Sprite_Frame_BG_NetMatch_PlayerItem_Normal {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_NetMatch_PlayerItem"); }
		}
		#endregion

		#region sign item
		public static Sprite Sprite_Frame_BG_Sign_Item_Normal {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_Sign_Item_Normal"); }
		}

		public static Sprite Sprite_Frame_BG_Sign_Item_Current {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_Sign_Item_Current"); }
		}

		public static Sprite Sprite_Frame_BG_Sign_Item_Already {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_Sign_Item_Already"); }
		}

		public static Sprite Sprite_Frame_Sign_Item_Normal {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_Sign_Item_Normal"); }
		}

		public static Sprite Sprite_Frame_Sign_Item_Current {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_Sign_Item_Current"); }
		}
		#endregion

		#region chapter item
		public static Sprite Sprite_Frame_BG_Chapter_Item_Purple {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_Chapter_Item_Purple"); }
		}
		public static Sprite Sprite_Frame_BG_Chapter_Item_Blue {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_Chapter_Item_Blue"); }
		}

		public static Sprite Sprite_Frame_Top_Chapter_Item_Purple {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_Top_Chapter_Item_Purple"); }
		}
		public static Sprite Sprite_Frame_Top_Chapter_Item_Blue {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_Top_Chapter_Item_Blue"); }
		}

		public static Sprite Sprite_Frame_Chapter_Item_Purple {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_Chapter_Item_Purple"); }
		}
		public static Sprite Sprite_Frame_Chapter_Item_Blue {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_Chapter_Item_Blue"); }
		}
		#endregion

		#region championship rank item
		public static Sprite Sprite_Frame_Championship_Rank_Normal {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_Championship_Rank_Normal"); }
		}

		public static Sprite Sprite_Frame_Championship_Rank_Selected {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_Championship_Rank_Selected"); }
		}
		#endregion

		#region championship Title
		public static Sprite Sprite_Frame_BG_Championship_Title_Green {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_Championship_Title_Green"); }
		}

		public static Sprite Sprite_Frame_BG_Championship_Title_Yellow {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_Championship_Title_Yellow"); }
		}
		#endregion

		#region gameover txt

		public static Sprite Sprite_Text_GXHS {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Text_gxhs"); }
		}

		public static Sprite Sprite_Text_ZJZL {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Text_zjzl"); }
		}

		#endregion

		#region rank icon bg
		public static Sprite Sprite_Frame_BG_Avatar_Blue {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_Avatar_Blue"); }
		}

		public static Sprite Sprite_Frame_BG_Avatar_Yellow {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_UI_Name, "Frame_BG_Avatar_Yellow"); }
		}
		#endregion


		#region case icon
		public static Sprite Sprite_Icon_Case_Close {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_Icon_Name, "Icon_Treasure_Closed"); }
		}

		public static Sprite Sprite_Icon_Case_Open {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_Icon_Name, "Icon_Treasure_open"); }
		}
		#endregion

		#region spreedialog btnclose
		public static Sprite Sprite_Btn_Close {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_Spree_Name, "Btn_Close"); }
		}

		public static Sprite Sprite_Btn_Close_Blurry {
			get { return AtlasManager.GetSprite(UIDataDef.Atlas_Spree_Name, "Btn_Close_Blurry"); }
		}
		#endregion


		public static Sprite Get_Bike_Rank_Icon(BikeRank rank) {
			return AtlasManager.GetLocalizedSprite(Atlas_UI_Name, "Icon_Motor_Grade_" + rank);
		}

		public static Sprite Get_Hero_Show_Icon(int id) {
			return AtlasManager.GetSprite(Atlas_Icon_Name, "Icon_Hero_Large_" + id);
		}

		public static Sprite Get_Player_Avatar(string index) {
			if (string.IsNullOrEmpty(index)) {
				index = "1";
			}
			return AtlasManager.GetSprite(Atlas_Icon_Name, "Icon_Avatar_" + index);
		}

		public static Sprite Get_RankBG_Icon(bool isSelect) {
			return AtlasManager.GetSprite(Atlas_UI_Name, "" + (isSelect ? "Button_BG_Common_46" : "Button_BG_Common_Blue"));
		}

		public static Color Green = new Color(0.01f, 0.35f, 0);
		public static Color LightGreen = new Color(0.44f, 0.76f, 0.08f);
		public static Color Brown = new Color(0.6f, 0.28f, 0);
		public static Color LightBrown = new Color(0.68f, 0.43f, 0);
		public static Color DarkBrown = new Color(0.36f, 0.19f, 0.01f);
		public static Color Blue = new Color(0.12f, 0.44f, 0.58f);
		public static Color LightBlue = new Color(0.05f, 0.64f, 0.89f);
		public static Color DarkBlue = new Color(0f, 0.24f, 0.42f);
		public static Color Purple = new Color(0.57f, 0.27f, 0.8f);
		public static Color Tan = new Color(0.59f, 0.47f, 0.11f);
		public static Color RankFirst = new Color(229f / 256f, 192f / 256f, 102f / 256f);
		public static Color RankLast = new Color(120f / 256f, 135f / 256f, 156f / 256f);
		public static Color RankPlayer = new Color(63f / 256f, 75f / 256f, 85f / 256f);
		public static Color RankOther = new Color(2f / 256f, 2f / 256f, 4f / 256f);
		public static Color OnlineName = new Color(78f / 256f, 40f / 256f, 0);

		public static Color SelfText = new Color(87f / 256f, 155f / 256f, 241f / 256f);
		public static Color OtherText = new Color(174f / 256f, 188f / 256f, 211f / 256f);
		public static Color RankTypeSelected = new Color(40f / 256f, 65f / 256f, 97f / 256f);
		public static Color RankTypeUnselected = new Color(182f / 256f, 226f / 256f, 234f / 256f);

		public static Color RankBG_Normal = new Color(1 / 256f, 2 / 256f, 2 / 256f);
		public static Color RankBG_Select = new Color(61 / 256f, 75 / 256f, 83 / 256f);

		public static Color StarYellow = new Color(248 / 256f, 207 / 256f, 110 / 256f);
		public static Color StarBlue = new Color(18 / 256f, 79 / 256f, 123 / 256f);
		public static Color OutBlue = new Color(90 / 256f, 165 / 256f, 223 / 256f);
		public static Color OutYello = new Color(255 / 256f, 175 / 256f, 42 / 256f);

		public static Color MotorUnlock = new Color(136 / 256f, 136 / 256f, 136 / 256f);
		public static Texture GetModeTexture(GameMode mode) {
			return UResources.Load<Texture>("Mode/" + mode);
		}

		public static Texture GetModeLinTexture(string mode) {
			return UResources.Load<Texture>("Mode/" + mode);
		}

		public static Sprite GetNumPic(int num, NumPicType type) {
			switch (type) {
				case NumPicType.White:
					return AtlasManager.GetSprite(Atlas_UI_Name, "Text_White_" + num);
				case NumPicType.Yellow:
					return AtlasManager.GetSprite(Atlas_UI_Name, "Text_" + num);
				case NumPicType.WhiteWithBlackOutline:
					return AtlasManager.GetSprite(Atlas_UI_Name, "Text_WhiteB_" + num);
			}

			return null;
		}

		public static Sprite GetRankMedal(int num) {
			return AtlasManager.GetSprite(Atlas_UI_Name, "Icon_Rank_" + num);
		}

		public static Texture ChampionshipDefaultImg {
			get { return UResources.Load<Texture>("Mode/net1"); }
		}

		public static Sprite GetSpreeTitle(int paycode) {
			return AtlasManager.GetSprite(Atlas_Spree_Name, "Spree_Title_" + paycode);
		}

		public static Sprite GetSpreeDesc(int paycode) {
			Sprite ret = AtlasManager.GetSprite(Atlas_Spree_Name, "Spree_Desc_" + paycode);
			return ret ?? AtlasManager.GetSprite(Atlas_Spree_Name, "Spree_Desc_3");
		}
		//获取头像
		public static Sprite GetPortrait(int index) {
			Sprite ret = AtlasManager.GetSprite(Atlas_Icon_Name, "Icon_Portrait_" + index);
			return ret ?? AtlasManager.GetSprite(Atlas_Icon_Name, "Icon_Portrait_0");
		}

		public static Sprite GetCountryOrRegionFlag(RegionEnum region) {
			string spriteName = (int)region + "--" + region.ToString();
			if (!string.IsNullOrEmpty(spriteName)) {
				return AtlasManager.GetSprite(Atlas_Flag_Name, spriteName);
			}
			return null;
		}

		/// <summary>
		/// Convert a decimal value to its hex representation.
		/// It's coded because num.ToString("X6") syntax doesn't seem to be supported by Unity's Flash. It just silently crashes.
		/// string.Format("{0,6:X}", num).Replace(' ', '0') doesn't work either. It returns the format string, not the formatted value.
		/// </summary>
		[System.Diagnostics.DebuggerHidden]
		[System.Diagnostics.DebuggerStepThrough]
		static public string DecimalToHex24(int num) {
			num &= 0xFFFFFF;
			return num.ToString("X6");
		}

		[System.Diagnostics.DebuggerHidden]
		[System.Diagnostics.DebuggerStepThrough]
		static public string DecimalToHex8(int num) {
			num &= 0xFF;
			return num.ToString("X2");
		}
	}

}

