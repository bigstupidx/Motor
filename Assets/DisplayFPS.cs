using UnityEngine;
using System.Collections;

public class DisplayFPS : MonoBehaviour {
	float deltaTime = 0.0f;
	private GUIStyle style;

	private float _responseTime = 3f;
	private float _timeCounter = 3f;
	private bool _isUp = false;
	private bool _isDown = false;
	private bool _isLeft = false;
	private bool _isRight = false;
	private bool _isStart = false;
	private bool _isShow = false;

	void Start() {
		int w = Screen.width, h = Screen.height;
		style = new GUIStyle();
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 40;
		style.normal.textColor = new Color(1.0f, 1.0f, 0.5f, 1.0f);
	}

	void Update() {
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

		if (_isStart) {
			_timeCounter -= Time.deltaTime;
			if (_timeCounter <= 0) {
				_timeCounter = _responseTime;
				_isUp = false;
				_isDown = false;
				_isLeft = false;
				_isRight = false;
			}
		}

		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			_isUp = true;
			_isDown = false;
			_isLeft = false;
			_isRight = false;

			_isStart = false;
			_timeCounter = _responseTime;
			_isStart = true;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			if (_isUp) {
				_isDown = true;
			}
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			if (_isUp && _isDown) {
				_isLeft = true;
			}
		}
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			if (_isUp && _isDown && _isLeft) {
				_isRight = true;
				_isShow = !_isShow;
			}
		}
	}

	void OnGUI() {
		if (_isShow) {
			string text;
			if (Time.timeScale != 0) {
				text = ((int)(1.0f / deltaTime)).ToString();
			} else {
				text = "60";
			}
			// string text = string.Format("{0:0.0} ms ({1:0.} fps)", deltaTime * 1000.0f, 1.0f / deltaTime);
			GUILayout.Label("fps:" + text, style);
			if (Lobby.Ins != null && Lobby.Ins.RoomClient.ToRoomClient.Connected) {
				GUILayout.Label("Ping:" + Lobby.Ins.RoomClient.ToRoomClient.Ping, this.style);
			}
			if (Lobby.Ins != null && Lobby.Ins.RoomClient.ToLobbyClient.Connected) {
				GUILayout.Label("Ping:" + Lobby.Ins.RoomClient.ToLobbyClient.Ping, this.style);
			}

		}
	}
}
