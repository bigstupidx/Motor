using System.Collections.Generic;
using HeavyDutyInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Joystick.UGUI {

	public class FocusItemSlider : FocusItemButton {

		public enum Dir {
			Hor, Ver
		}

		public Dir dir;
		public Transform Handle;


		protected override void Reset() {
			base.Reset();
			Handle = GetComponent<Slider>().handleRect;
		}


		public override FocusItemBase Get(DirType dir, List<FocusItemBase> list) {
			if (this.dir == Dir.Hor) {
				if (dir == DirType.Left || dir == DirType.Right) {
					return this;
				}
			} else if (this.dir == Dir.Ver) {
				if (dir == DirType.Up || dir == DirType.Down) {
					return this;
				}
			}
			return base.Get(dir, list);
		}

		public override Vector3 GetFocusEffectPos() {
			if (Handle != null) {
				return Handle.position;
			}
			return base.GetFocusEffectPos();
		}
	}
}