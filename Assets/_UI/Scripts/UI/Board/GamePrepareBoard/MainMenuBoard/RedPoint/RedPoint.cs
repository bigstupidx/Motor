using UnityEngine;

namespace GameUI {
	public class RedPoint : MonoBehaviour {

		public GameObject Mark;

		public void SetState(bool show) {
			Mark.SetActive(show);
		}

		public void SetState() {
			Mark.SetActive(CheckState());
		}

		public virtual bool CheckState() {
			return false;
		}
	}
}
