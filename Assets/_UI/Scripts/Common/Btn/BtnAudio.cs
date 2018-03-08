using UnityEngine;
using UnityEngine.EventSystems;
using XPlugin.Update;

namespace GameUI {

	public class BtnAudio :MonoBehaviour,IPointerClickHandler {

		public SfxType SfxType=SfxType.SFX_Btn;

		public void OnPointerClick(PointerEventData eventData) {
			SfxManager.Ins.PlayOneShot(UResources.Load<AudioClip>(this.SfxType.ToString()));
		}
	}
}