using GameClient;
using UnityEngine;
using GameUI;
using UnityEngine.UI;
using XUI;

public class HideSettingBoard : UIStackBehaviour {

	public InputField TcpInputField;
	public InputField WebInputField;
	public Button close;
	public Button save;
	public Button cleanSave;

	public const string Path = "UI/HideSettingBoard";

	public static void Show() {
		ModMenu.Ins.Overlay(new string[] { Path });
	}

	public override void OnUISpawned() {
		base.OnUISpawned();
		this.TcpInputField.text = Lobby.Ins.Host;
	}


	void Update() {
		Lobby.Ins.Host = this.TcpInputField.text;
		Client.Config.WebHost = this.WebInputField.text;
		Interface.Ins.FullApiUrl = this.WebInputField.text + Interface.Ins.ApiPath;
	}

	void Start() {
		this.TcpInputField.text = Lobby.Ins.Host;
		this.WebInputField.text = Client.Config.WebHost;

		this.close.onClick.AddListener(() => {
			ModMenu.Ins.Back(true);
		});

		this.save.onClick.AddListener(() => {
			PlayerPrefs.SetString("TcpServer", this.TcpInputField.text);
			PlayerPrefs.SetString("WebServer", this.WebInputField.text + "/index.php/Api/Index/index");
		});

		this.cleanSave.onClick.AddListener(() => {
			PlayerPrefs.DeleteKey("TcpServer");
			PlayerPrefs.DeleteKey("WebServer");
		});

	}

}
