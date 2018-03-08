using Game;
using UnityEngine;

namespace GameClient {
	public class StoryGuide_1_Game_Trigger : MonoBehaviour {
		void OnTriggerEnter(Collider other) {
			if (other.gameObject.CompareTag(Tags.Ins.Player)) {
				StoryGuide_1_Game.Ins.GoNext = true;
				gameObject.SetActive(false);
			}
		}
	}
}