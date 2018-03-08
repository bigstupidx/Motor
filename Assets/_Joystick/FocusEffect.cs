using UnityEngine;

namespace Joystick {
	/// <summary>
	/// 手指光标效果显示
	/// </summary>
	public class FocusEffect : MonoBehaviour {

		public FocusItemBase item;

		public GameObject Renderer;

		public void Focus(FocusItemBase item) {
			gameObject.SetActive(true);
			this.item = item;
		}

		void LateUpdate() {
			if (this.item != null) {
				if (DisableFocusEffect.Ins != null) {
					this.Renderer.SetActive(false);
					FocusManager.Ins.CurrentFocusItem = null;
					return;
				}
				this.Renderer.SetActive(true);
				var targetPos = this.item.GetFocusEffectPos();
				if (item.useLerpEffect) {
					this.transform.position = Vector3.Lerp(this.transform.position, targetPos,
														   15 * Time.unscaledDeltaTime);
				} else {
					this.transform.position = targetPos;
					item.useLerpEffect = true;
				}

			}
		}

	}
}