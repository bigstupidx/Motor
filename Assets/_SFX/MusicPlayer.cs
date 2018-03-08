using GameClient;
using UnityEngine;
using UnityEngine.SceneManagement;
using XPlugin.Update;

public class MusicPlayer : MonoBehaviour {

	[Reorderable]
	public string[] MenuMusic;
	[Reorderable]
	public string[] GameMusic;

	void Start() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene arg0, LoadSceneMode loadSceneMode) {
		string randomMusic = null;
		if (DataDef.MenuBG_Scene == arg0.name) {
            if (Client.Guide.CheckMainGuideFinished(1))
            {
                randomMusic = this.MenuMusic[Random.Range(0, this.MenuMusic.Length)];
                SfxManager.Ins.Play(UResources.Load<AudioClip>(randomMusic), true);
            }
		} else {
			randomMusic = this.GameMusic[Random.Range(0, this.GameMusic.Length)];
            SfxManager.Ins.Play(UResources.Load<AudioClip>(randomMusic), true);
        }
	}

	void OnDestroy() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

}
