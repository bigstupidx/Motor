namespace RoomServerModel {
	public enum CreateRoomResult {
		None = 0,
		Success = 1,
		RoomAlreadyExist = 2,
		NoAvailableRoomServer = 3,
		ConnectToRoomServerFail=4,
		Fail = 10,
	}
}