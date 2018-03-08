using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game {

	public class BikeCameraEffect : Singleton<BikeCameraEffect> {

		public RadialBlurAdvanced RadialBlurEffect;
		private float _radialBlurStrength;

		public GlassBroken GlassBroken;

		public MFColorCorrectionEffect ColorCorrection;

		public DirtLens DirtLens;


		private LevelOverrideArg levelOverrideArg;

		void Start() {
			this._radialBlurStrength = this.RadialBlurEffect.Strength;
			this.RadialBlurEffect.Strength = 0;
			this.RadialBlurEffect.enabled = false;
			this.levelOverrideArg = LevelOverrideArg.Ins;
		}

		void Update() {
			var bike = MainCamera.Ins.BikeCamera.Bike;
			if (bike == null) {
				return;
			}
			float targetStrength = bike.Boosting ? this._radialBlurStrength : 0f;
			this.RadialBlurEffect.Strength = Mathf.Lerp(this.RadialBlurEffect.Strength, targetStrength, 10 * Time.deltaTime);
			this.RadialBlurEffect.enabled = Mathf.Abs(this.RadialBlurEffect.Strength) > 0.01f;

			this.GlassBroken.enabled = bike.Crashed;


			if (this.levelOverrideArg != null) {
				if (bike.Crashed) {
					this.ColorCorrection.Arg =
						MFColorCorrectionEffect.ColorCorrectArg.MoveToward(this.ColorCorrection.Arg,
							this.levelOverrideArg.CrashColorCorrectArg, 1f * Time.deltaTime);
				} else if (bike.Boosting) {
					this.ColorCorrection.Arg =
						MFColorCorrectionEffect.ColorCorrectArg.MoveToward(this.ColorCorrection.Arg,
							this.levelOverrideArg.NitroColorCorrectArg, 0.1f * Time.deltaTime);
				} else {
					this.ColorCorrection.Arg =
						MFColorCorrectionEffect.ColorCorrectArg.MoveToward(this.ColorCorrection.Arg,
							this.levelOverrideArg.ColorCorrectArg, 0.1f * Time.deltaTime);
				}
			}
		}


	}
}
