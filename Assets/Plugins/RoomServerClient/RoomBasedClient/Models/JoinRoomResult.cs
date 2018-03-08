namespace RoomServerModel {
	public enum JoinRoomResult {
		None = 0,
		Success = 1,
		RoomFull = 2,
		RoomNotExist = 3,
		RoomNotOpen = 4,
		AuthFail = 5,
		ConnectToRoomServerFail = 6,
	}
}