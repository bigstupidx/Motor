using System.Collections;
using EnhancedUI.EnhancedScroller;
using Joystick.UGUI;
using UnityEngine;

namespace Joystick.Other {

	public interface ISelectCell {
		bool Selected { get; set; }
		GameObject gameObject { get; }
	}

	public class SelectedFocusListCell : FocusEnhancedCell {


		public override void OnFocused(FocusItemBase lastFocus) {
			if (lastFocus != null) {
				if (lastFocus is SelectedFocusListCell) {
					base.OnFocused(lastFocus);
				} else {

					if (Scroller == null) {
						Scroller = GetComponentInParent<EnhancedScroller>();
					}
					var cells = this.Scroller.transform.GetComponentsInChildren<ISelectCell>();
					foreach (var cell in cells) {
						if (cell.Selected) {
							var focus = cell.gameObject.GetComponent<FocusItemBase>();
							//							StartCoroutine(DelayFocus());
							FocusManager.Ins.Focus(focus);
						}
					}
				}
			} else {
				base.OnFocused(lastFocus);
			}
		}

		private IEnumerator DelayFocus(FocusItemBase focus) {
			yield return null;
		}
	}
}