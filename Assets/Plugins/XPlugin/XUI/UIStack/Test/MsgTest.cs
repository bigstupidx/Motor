using UnityEngine;

namespace XUI {
	public class MsgTest : MonoBehaviour {


		void OnUIEnterStack() {
			Debug.Log(gameObject + " OnUIEnterStack");
		}

		void OnUILeaveStack() {
			Debug.Log(gameObject + " OnUILeaveStack");
		}

		void OnUIBeenOverlay() {
			Debug.Log(gameObject + " OnUIBeenOverlay");
		}

		void OnUIDeOverlay() {
			Debug.Log(gameObject + " OnUIDeOverlay");
		}

		void OnUIBeenCover() {
			Debug.Log(gameObject + " OnUIBeenCover");
		}

		void OnUIDeCover() {
			Debug.Log(gameObject + " OnUIDeCover");
		}

		void OnUISpawned() {
			Debug.Log(gameObject + " OnUISpawned");
		}

		void OnUIDespawn() {
			Debug.Log(gameObject + " OnUIDespawn");
		}


	}
}