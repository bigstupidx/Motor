using UnityEngine;
using UnityEngine.UI;

public class UIRaceTime : MonoBehaviour {
	public Text raceTime;

	public void SetTime(float time) {
		int m = (int)(time / 60);
		int s = (int)(time % 60);
		int ms = (int)((time - (int)time) * 1000);
		raceTime.text = m.ToStringFast() + ":" + s.ToStringFast() + "." + ms;
	}

	public void SetColor(Color color) {
		raceTime.color = color;
	}

}
