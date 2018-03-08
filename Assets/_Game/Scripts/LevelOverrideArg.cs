using UnityEngine;
using System.Collections;

namespace Game {


	public class LevelOverrideArg : Singleton<LevelOverrideArg> {

		public MFColorCorrectionEffect.ColorCorrectArg ColorCorrectArg = new MFColorCorrectionEffect.ColorCorrectArg(1, 1, 1, 0, 0, 0);
		public MFColorCorrectionEffect.ColorCorrectArg NitroColorCorrectArg = new MFColorCorrectionEffect.ColorCorrectArg(1, 1, 1, 0, 0, 0);
		public MFColorCorrectionEffect.ColorCorrectArg CrashColorCorrectArg = new MFColorCorrectionEffect.ColorCorrectArg(1, 1, 1, 0, 0, 0);

		[Header("dirt lens")]
		public float intensity = 1.0f;
		public Texture2D texture = null;
		public Color Color;
		public Transform Light;

		[Header("shadow")]
		public Vector3 Offset = new Vector3(11, 10, -1);
		public Vector3 Rot = new Vector3(43.72f, -81.85f, -69.63f);


		// Use this for initialization
		void Start() {
			if (DeviceLevel.Score < 20) {
				RenderSettings.fog = false;
			}
			BikeCameraEffect.Ins.ColorCorrection.Arg = this.ColorCorrectArg;
			var dirtLens = BikeCameraEffect.Ins.DirtLens;
			dirtLens.Color = this.Color;
			dirtLens.Light = this.Light;
			dirtLens.texture = this.texture;
			dirtLens.intensity = this.intensity;

			ProjectorShadow.ProjectorShadow.Ins.Offset = Offset;
			ProjectorShadow.ProjectorShadow.Ins.GetComponent<Transform>().rotation = Quaternion.Euler(Rot);
		}

	}
}

