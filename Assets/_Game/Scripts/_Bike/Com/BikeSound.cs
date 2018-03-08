using System.Collections;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace Game {

	public class BikeSound : BikeBase {
		private static readonly float[] _gears = new float[] { 0.3f, 0.4f, 0.5f, 0.7f, 0.8f, 2f };
		private static readonly float[] a = new float[] { 5f, 3.5f, 3f, 2f, 1.5f, 1f };

		public int currentGear = 0;
		public BikeAudioClip EngineAudio;
		public BikeAudioClip StartAudio;

		public BikeAudioClip DriftAudio;
		public BikeAudioClip ImpactAudio;
		public BikeAudioClip RubAudio;

		public BikeAudioClip BoostStartAudio;
		public BikeAudioClip BoostStopAudio;

		public BikeAudioClip LandAudio;

		public BikeAudioClip CrashAudio;

		public override void Init(PlayerInfo info) {
			base.Init(info);
		}


		void Start() {
			if (!gameObject.CompareTag(Tags.Ins.Enemy)) {
				this.EngineAudio.Init(this);
				this.EngineAudio.Play();
			}
			this.StartAudio.Init(this);
			this.DriftAudio.Init(this);
			this.ImpactAudio.Init(this);
			this.RubAudio.Init(this);
			this.BoostStartAudio.Init(this);
			this.BoostStopAudio.Init(this);
			this.LandAudio.Init(this);
			this.CrashAudio.Init(this);

			this.bikeControl.OnGround += onAirTime => {
				this.LandAudio.Volume = Mathf.Clamp01(onAirTime);
				this.LandAudio.Play();
			};

			this.bikeControl.OnBoost += b => {
				if (b) {
					this.BoostStartAudio.Play();
				} else {
					this.BoostStopAudio.Play();
				}
			};

			this.bikeControl.OnDrift += drifting => {
				if (drifting) {
					this.DriftAudio.Play();
				} else {
					this.DriftAudio.Stop();
				}
			};

			this.bikeState.Fsm.OnCrashIn += o => {
				this.CrashAudio.Play();
			};

			this.bikeControl.OnImpact += (collison) => {
				this.ImpactAudio.Volume = collison.relativeVelocity.magnitude * 0.01f;
				this.ImpactAudio.Play();
			};

			this.bikeControl.OnRub += (collison, b) => {
				this.RubAudio.Volume = collison.relativeVelocity.magnitude*0.02f;
				if (b) {
					this.RubAudio.Play();
				} else {
					this.RubAudio.Stop();
				}
			};
			this.bikeControl.OnRubStay += collision => {
				this.RubAudio.Volume = collision.relativeVelocity.magnitude*0.01f;
			};
		}


		private float rpm;
		void Update() {
			if (this.EngineAudio.Source != null) {
				rpm = Mathf.MoveTowards(rpm, this.bikeControl.Speed / this.bikeControl.bikeSetting.LimitBoostSpeed, 1 * Time.deltaTime);
				if (!bikeControl.Grounded) {
					//				this.currentGear = 0;
					rpm = rpm * 0.7f;//这里把转速调整一下让空中的声音和地面声音有点变化
				}
				float newPitch = 0.5f + a[this.currentGear] * rpm * rpm;//y=ax^2
				float currentPitch = this.EngineAudio.Source.pitch;
				if (newPitch > currentPitch) {
					if (rpm > _gears[this.currentGear]) { //该换挡了
						this.currentGear++;
					}
				} else {
					if (this.currentGear > 0) {
						if (rpm < _gears[this.currentGear - 1]) {
							this.currentGear--;
						}
					}
				}

				this.EngineAudio.Source.pitch = Mathf.Lerp(currentPitch, newPitch, 20 * Time.deltaTime);
				if (RubAudio.Source != null) {
					this.RubAudio.Source.pitch = Mathf.Lerp(currentPitch, newPitch, 20 * Time.deltaTime);
				}
			}
		}

	}
}