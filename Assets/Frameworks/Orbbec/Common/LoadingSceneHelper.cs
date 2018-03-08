using UnityEngine;
using System.Collections;

public class LoadingSceneHelper : MonoBehaviour
{
	AsyncOperation async = null;

	public static string LoadingSceneName = "Game";
	public static float progress = 0;

	public float ProcessSpeed = 1.0f;

	// Use this for initialization
	void Start () 
	{
		progress = 0;
		StartCoroutine(loadScene());
	}

	IEnumerator loadScene()
	{
		if (!Application.isLoadingLevel)
		{
			async = Application.LoadLevelAsync(LoadingSceneName);
			async.allowSceneActivation = false;

			while (progress < 0.99f)
			{
				float Length = async.progress / 0.9f - progress;
				float curSpeed = Length / Time.deltaTime;
				if (curSpeed > ProcessSpeed)
					progress += ProcessSpeed * Time.deltaTime;
				else
					progress += Length;

				yield return new WaitForEndOfFrame();
			}

			progress = 1.0f;
			yield return new WaitForEndOfFrame();
			async.allowSceneActivation = true;
		}
	}
}
