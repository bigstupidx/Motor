using UnityEngine;
using UnityEngine.UI;

public class RuleDialog : MonoBehaviour {

	public Text Content;

	public void Init(string s) {
		Content.text = s;
	}
}
