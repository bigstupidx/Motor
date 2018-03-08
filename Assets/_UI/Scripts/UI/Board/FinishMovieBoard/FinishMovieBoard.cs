using UnityEngine;
using System.Collections;
using GameUI;

public class FinishMovieBoard : MonoBehaviour {

	#region base

	public const string UIPrefabPath = "UI/Board/FinishMovieBoard";

	public static void Show() {
		string[] UIPrefabNames ={
			UIPrefabPath,
		};
		ModMenu.Ins.Cover(UIPrefabNames, "FinishMovieBoard");
	}

	#endregion

	public void OnBtnNextClick() {
		GameOverBoard.Show();
	}
}
