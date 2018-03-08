using GameClient;
using Orbbec;

public class BikeOrbecController {

	private static BikeOrbecController ins;

	public static BikeOrbecController Ins {
		get {
			if (ins == null) {
				ins = new BikeOrbecController();
			}
			return ins;
		}
	}

	public void Init() {
		OrbbecSensingManager.Init();
		OrbbecSensingManager.instance.showTrackingUI = true;
		OrbbecSensingManager.instance.playerMode = OrbbecSensingManager.PlayerMode.single;
		//		OrbbecSensingManager.instance.deviceInitAction = OnDeviceInit;
		//				OrbbecSensingManager.instance.trackedAction = OnTracked;
		//		OrbbecSensingManager.instance.unTrackedAction = OnUnTrackedAction;
		//		OrbbecSensingManager.instance.leftAtkAction = OnLeftAtkAction;
		//		OrbbecSensingManager.instance.rightAtkAction = OnRightAtkAction;
		OrbbecSensingManager.instance.InitOrbbecDevice();
	}

	public static bool HasOrbbecDevice() {
		return Client.Config.CanUseSensor && OrbbecManager.HasOrbbecDevice();
	}

}
