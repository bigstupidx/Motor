using System.Collections.Generic;
using System.Linq;
using GameClient;
using RoomServerModel;
using UnityEngine;

public partial class Lobby {
	public const string MapKey = "Map";
	public const string LapCountKey = "Lap";
	public const string ModeKey = "Mode";
	public const string MasterNameKey = "MasterName";
	public const string MasterIconKey = "MasterIcon";
	public const string RobotCountKey = "RobotsCount";
	public const string PlayerInfoKey = "PlayerInfo";
	public const string RobotsKey = "robots";
	public const string PlayerStateKey = "state";


	public static GameMode[] GameModeOpts;
	public static string[] GameModeOptions {
		get {
			if (GameModeOpts == null) {
				GameModeOpts = new GameMode[] { GameMode.Racing, GameMode.RacingProp };
			}
			return GameModeOpts.Select(mode => DataDef.GameModeName(mode)).ToArray();
		}
	}

	public static List<SceneData> SceneDataList;

	public static string[] MapOptions {
		get {
			if (SceneDataList == null) {
				SceneDataList = Client.Scene.SceneDatas.Values.ToList();
			}
			List<string> sceneName = SceneDataList.Select(data => data.Name).ToList();
            sceneName.Insert(0,LString.LobbyRoomListItem_MapOptions.ToLocalized());// "随机");//注意在头部添加了一个随机，取索引时应该减去
			return sceneName.ToArray();
		}
	}



	public static string GetGameModeStr(int index) {
		return GameModeOptions[index];
	}

	public static string GetMapName(int index) {
		return MapOptions[index];
	}

	public static GameMode GetGameMode(int index) {
		return GameModeOpts[index];
	}

	public static SceneData GetSceneData(int index) {
		SceneData sceneData;
		if (index == 0) {
			sceneData = Client.Scene.SceneDatas.Random();
		} else {
			sceneData = SceneDataList[index - 1];
		}
		return sceneData;
	}

	public static string GeneratePassword() {
		return Random.Range(1000, 9999).ToString();
	}

	public string GetCreateRoomFailReason(CreateRoomResult result) {
		switch (result) {
			case CreateRoomResult.None:
			case CreateRoomResult.Success:
				return "";
				break;
			case CreateRoomResult.ConnectToRoomServerFail:
				return "无法连接到房间服务器";
				break;
			case CreateRoomResult.NoAvailableRoomServer:
				return "无可用房间服务器";
				break;
			case CreateRoomResult.RoomAlreadyExist:
				return "房间已存在";
				break;
			case CreateRoomResult.Fail:
				return "未知错误";
				break;
		}
		return "其他错误";
	}

	public string GetJoinRoomFailReason(JoinRoomResult result) {
		switch (result) {
			case JoinRoomResult.None:
			case JoinRoomResult.Success:
				return "";
				break;
			case JoinRoomResult.ConnectToRoomServerFail:
				return "无法连接到房间服务器";
				break;
			case JoinRoomResult.AuthFail:
				return "密码错误";
				break;
			case JoinRoomResult.RoomFull:
				return "房间人数已满";
				break;
			case JoinRoomResult.RoomNotExist:
				return "房间不存在";
				break;
			case JoinRoomResult.RoomNotOpen:
				return "房间已关闭";
				break;
		}
		return "其他错误";
	}



}
