using UnityEngine;

namespace Joystick.UGUI {
	/// <summary>
	/// 提供了UGUI组件中心点的计算
	/// </summary>
	public abstract class FocusItemUGUI : FocusItemBase {
		Vector3[] worldCorners = new Vector3[4];

		private void OnDrawGizmosSelected() {
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(GetFocusEffectPos(), 0.02f);
		}

		public override Vector3 GetFocusEffectPos() {
			var rectTrans = (RectTransform)transform;
			rectTrans.GetWorldCorners(worldCorners);
			return (worldCorners[0] + worldCorners[2]) / 2f + (Vector3)Offset;//取其中一条对角线的中点作为中心
		}
	}
}