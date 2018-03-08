namespace UnityEngine.UI {

	using UnityEngine;
	using System.Collections.Generic;

	public class GradientLegacy : BaseMeshEffect {
		[Header("这个已经不用了，使用xplugin里面的渐变替代")]
		public Color32 topColor = Color.white;
		public Color32 bottomColor = Color.black;

		public override void ModifyMesh(VertexHelper helper) {
			if (!IsActive() || helper.currentVertCount == 0) {
				return;
			}

			List<UIVertex> vertices = new List<UIVertex>();
			helper.GetUIVertexStream(vertices);

			for (int i = 0; i < vertices.Count && vertices.Count - i >= 6;) {
				ChangeColor(ref vertices, i, topColor);
				ChangeColor(ref vertices, i + 1, topColor);
				ChangeColor(ref vertices, i + 2, bottomColor);
				ChangeColor(ref vertices, i + 3, bottomColor);
				ChangeColor(ref vertices, i + 4, bottomColor);
				ChangeColor(ref vertices, i + 5, topColor);
				i += 6;
			}

			helper.Clear();
			helper.AddUIVertexTriangleStream(vertices);

		}


		private void ChangeColor(ref List<UIVertex> verList, int index, Color color) {
			UIVertex temp = verList[index];
			temp.color *= color;
			verList[index] = temp;
		}
	}
}