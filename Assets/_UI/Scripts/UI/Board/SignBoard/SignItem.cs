using GameClient;
using Joystick;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class SignItem : MonoBehaviour {
		public Image Bg;
		public Text Day;
		public Text RewardList;
		public Image RewardImg;
		public Button Btn;
		public Image Flag;
		public MaskableGraphic[] NeedGrey;
		public LayoutElement layoutElement;

		public BtnAudio _btnAudio;

		void Start() {
			Btn.onClick.AddListener(OnBtnClick);
		}

		public void SetData(SignData data, SignState state) {
			if (data.Day == 6) {
				Day.text = (LString.GAMEUI_UIGAMEOVERINFOONLINE_INIT_2).ToLocalized();
			} else {
				Day.text = string.Format(LString.GAMEUI_UIGAMEOVERINFOONLINE_INIT.ToLocalized(), data.Day + 1);
			}
			if (data.Reward.Data.ID == 1001) {
				RewardImg.sprite = AtlasManager.GetSprite("Icon", "Icon_Coin_1");
			} else if (data.Reward.Data.ID == 1000) {
				RewardImg.sprite = AtlasManager.GetSprite("Icon", "Icon_Diamond_2");
			} else {
				RewardImg.sprite = data.Reward.Data.Icon.Sprite;
			}
			string str = "x" + data.Reward.Amount;
			RewardList.text = "<color=#FBD471FF>" + data.Reward.Data.Name + ":" + "</color>" + str;
			switch (state) {
				case SignState.Normal:
					this.transform.localScale = new Vector3(0.9018f, 0.9018f, 0.9018f);
					Bg.gameObject.SetActive(false);
					Btn.enabled = false;
					_btnAudio.SfxType = SfxType.SFX_Cant;
					Flag.gameObject.SetActive(false);
					foreach (var maskableGraphic in NeedGrey) {
						maskableGraphic.SetGreyMaterail(true);
					}
					layoutElement.flexibleWidth = 1;
					break;
				case SignState.Already:
					this.transform.localScale = new Vector3(0.9018f, 0.9018f, 0.9018f);
					Bg.gameObject.SetActive(true);
					Btn.enabled = false;
					_btnAudio.SfxType = SfxType.SFX_Cant;
					Flag.gameObject.SetActive(false);
					foreach (var maskableGraphic in NeedGrey) {
						maskableGraphic.SetGreyMaterail(false);
					}
					layoutElement.flexibleWidth = 1;
					break;
				case SignState.Current:
					Bg.gameObject.SetActive(false);
					layoutElement.flexibleWidth = 1.05f;
					foreach (var maskableGraphic in NeedGrey) {
						maskableGraphic.SetGreyMaterail(false);
					}
					this.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
					//Btn.transition = Selectable.Transition.None;
					if (Client.Sign.CanSign() && !Client.Sign.IsSignToday()) {
						Flag.gameObject.SetActive(true);
						Btn.enabled = true;
						if (FocusManager.TVMode) {
							var focus = this.Btn.GetComponent<FocusItemBase>();
							focus.FirstFocus = true;
							focus.ForceFirstFocus = true;
							FocusManager.Ins.Focus(focus);
						}
						_btnAudio.SfxType = SfxType.SFX_Btn;
					} else {
						Bg.gameObject.SetActive(true);
						Flag.gameObject.SetActive(false);
						Btn.enabled = false;
						_btnAudio.SfxType = SfxType.SFX_Cant;
					}
					break;
			}
		}

		public void OnBtnClick() {
			var list = Client.Sign.SignToday();
			if (list != null) {
				SfxManager.Ins.PlayOneShot(SfxType.SFX_Blink);
				Btn.enabled = false;
				RewardDialog.Show(list);
			} else {
				SfxManager.Ins.PlayOneShot(SfxType.SFX_Cant);
			}
		}

	}


}
