using System.Collections.Generic;
using UnityEngine;


namespace Game {

	public class BuffOverlayInfo {
		public GameObject effect;
		public float time;
		public float timer;
		public float value;
		public bool Infinite;
		public object UserData;

		public void Clean() {
			if (effect != null) {
				Object.Destroy(effect);
			}
		}
	}

	public class BuffBaseOverlayWithEffect : BuffBase {
		/// <summary>
		/// 总和的值
		/// </summary>
		public float value;
		public List<BuffOverlayInfo> overlayInfos = new List<BuffOverlayInfo>();

		public BuffOverlayInfo Start(float time, float value, string effect, object userData = null) {
			GameObject effectGameObject = null;
			if (!string.IsNullOrEmpty(effect)) {
				effectGameObject = GameObjectUtility.LoadAndIns(effect);
//				BuffEffect buffEffect = effectGameObject.GetComponent<BuffEffect>();
				// GameObjectUtility.SetGameObjectParent(effectGameObject, character.characterSkill.mountPointsDic[buffEffect.mpType], true);
				GameObjectUtility.SetGameObjectParent(effectGameObject, bike.transform, true);
				//				effectGameObject.transform.SetUniformLocalScale(character.characterBuff.scaleFactor);
			}

			BuffOverlayInfo overlayInfo = new BuffOverlayInfo() {
				effect = effectGameObject,
				time = time,
				value = value,
				UserData = userData,
			};
			overlayInfos.Add(overlayInfo);
			bool startSuccess = base.Start(GetRemainTime());
			if (startSuccess) {
				UpdateValues();
			}
			return overlayInfo;
		}

		/// <summary>
		/// 调用这个方法可以手动在叠加类buff中添加一个buff
		/// </summary>
		/// <param name="time"></param>
		/// <param name="value"></param>
		/// <param name="effect"></param>
		/// <returns>所添加的buff,这个引用在手动停止这个buff时会用到</returns>
		public BuffOverlayInfo StartManual(float value, string effect, object userData = null) {
			BuffOverlayInfo overlayInfo = Start(Mathf.Infinity, value, effect, userData);
			overlayInfo.Infinite = true;
			return overlayInfo;
		}
		/// <summary>
		/// 手动停止一个手动开启的buff
		/// </summary>
		/// <param name="overlayInfo"></param>
		public void StopManual(BuffOverlayInfo overlayInfo) {
			overlayInfo.Clean();
			overlayInfos.Remove(overlayInfo);
			UpdateValues();
		}

		public override void UpdateWhenAffect() {
			for (int i = 0; i < overlayInfos.Count;) {
				BuffOverlayInfo overlayInfo = overlayInfos[i];
				if (overlayInfo.Infinite) {
					i++;
					continue;
				}
				overlayInfo.timer += Time.deltaTime;
				if (overlayInfo.timer > overlayInfo.time) {
					overlayInfo.Clean();
					overlayInfos.Remove(overlayInfo);
					UpdateValues();
				} else {
					i++;
				}
			}
		}

		protected virtual void UpdateValues() {
			value = 0f;
			foreach (var buffOverlayInfo in overlayInfos) {
				value += buffOverlayInfo.value;
			}
			OnUpdateValues();
		}

		protected virtual void OnUpdateValues() {
		}

		float GetRemainTime() {
			float ret = 0f;
			foreach (var buffOverlayInfo in overlayInfos) {
				float r = buffOverlayInfo.time - buffOverlayInfo.timer;
				if (r > ret) {
					ret = r;
				}
			}
			return ret;
		}

		/// <summary>
		/// stop all overlay buff
		/// </summary>
		public override void OnBuffStop() {
			base.OnBuffStop();

			foreach (var overlayInfo in overlayInfos) {
				overlayInfo.Clean();
			}
			overlayInfos.Clear();
			UpdateValues();
		}


		//默认小于等于0为debuff，如果要更改请重写
		public override void OnRemoveDebuff() {
			for (int i = 0; i < overlayInfos.Count;) {
				if (overlayInfos[i].value <= 0) {
					overlayInfos[i].Clean();
					overlayInfos.RemoveAt(i);
				} else {
					i++;
				}
			}
			UpdateValues();
		}
	}
}
