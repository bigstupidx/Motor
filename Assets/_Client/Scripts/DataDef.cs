

namespace GameClient {
	public enum ModeType {
		Stage,
		Challenge,
		Season,
		Network
	}
	public class DataDef {
		public const string MenuBG_Scene = "Garage";//MenuBG
		public const string Menu_Scene = "MenuScene";
		public const string UidSaveKey = "SDKLoginUid";

		public static int DefaultHero {
			get { return Client.System.GetMiscValue<int>("User.InitHero"); }
		}

		public static int DefaultBike {
			get { return Client.System.GetMiscValue<int>("User.InitBike"); }
		}

		public static int DefalutWeapon {
			get { return Client.System.GetMiscValue<int>("User.InitWeapon"); }
		}

		public static string OnlineGameName = (LString.GAMECLIENT_DATADEF).ToLocalized();

		public static ModeType modetype = ModeType.Stage;

		public static string GameModeName(GameMode mode) {
			//TODO 从数据表读取
			switch (mode) {
				case GameMode.Elimination:
					return (LString.GAMECLIENT_DATADEF_GAMEMODENAME).ToLocalized();
				case GameMode.Timing:
					return (LString.GAMECLIENT_DATADEF_GAMEMODENAME_1).ToLocalized();
				case GameMode.RacingProp:
					return (LString.GAMECLIENT_DATADEF_GAMEMODENAME_2).ToLocalized();
				case GameMode.EliminationProp:
					return (LString.GAMECLIENT_DATADEF_GAMEMODENAME_3).ToLocalized();
				case GameMode.Racing:
					return (LString.GAMECLIENT_DATADEF_GAMEMODENAME_4).ToLocalized();
			}
			return (LString.GAMECLIENT_DATADEF_GAMEMODENAME_6).ToLocalized();
		}

		public static int GameModestaminaCount(GameMode mode) {
			return 0;
		}

		public static string GameModeDesc(GameMode mode) {
			//TODO 从数据表读取
			switch (mode) {
				case GameMode.Elimination:
					return (LString.GAMECLIENT_DATADEF_GAMEMODEDESC).ToLocalized();
				case GameMode.Timing:
					return (LString.GAMECLIENT_DATADEF_GAMEMODEDESC_1).ToLocalized();
				case GameMode.RacingProp:
					return (LString.GAMECLIENT_DATADEF_GAMEMODEDESC_2).ToLocalized();
				case GameMode.EliminationProp:
					return (LString.GAMECLIENT_DATADEF_GAMEMODEDESC_3).ToLocalized();
				case GameMode.Racing:
					return (LString.GAMECLIENT_DATADEF_GAMEMODEDESC_4).ToLocalized();
			}
			return (LString.GAMECLIENT_DATADEF_GAMEMODENAME_6).ToLocalized();
		}

	}

}

