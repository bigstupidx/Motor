using GameClient;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SDKLogin : Singleton<SDKLogin> {

	void Start() {
		SDKManager.Instance.SetLoginCallBack(this, "_LoginCallback");
	}

	private void _LoginCallback(string result) {
		Debug.Log("==>>>SDK Login callback,result:" + result);

		char[] c = "|".ToCharArray();
		string[] info = result.Split(c);
		if (info[0] == "success") {
			Debug.Log("==>>>SDK Login succuess");
			SDKManager.Instance.UID = info[1];
			if (!string.IsNullOrEmpty(SDKManager.Instance.UID)) {
				PlayerPrefs.SetString(DataDef.UidSaveKey, SDKManager.Instance.UID);
				SceneManager.LoadScene(DataDef.Menu_Scene);
			}
		} else {
			Debug.Log("==>>>SDK Login fail,try again");
			SDKManager.Instance.Login();
		}
	}
}
