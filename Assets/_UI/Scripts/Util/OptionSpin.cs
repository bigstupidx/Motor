using UnityEngine.Events;

namespace UnityEngine.UI {
	public class OptionSpin : MonoBehaviour {
		public string[] Options;
		public Text OptText;
		public int Index;

		public bool Loop;

		public string Value {
			get { return this.Options[this.Index]; }
		}

		public UnityEvent<int, string> OnOptionChanged;

		public void Init(string[] options, int initIndex, bool Loop, UnityEvent<int, string> onOptionChanged) {
			this.Index = initIndex;
			this.Options = options;
			this.Loop = Loop;
			this.OnOptionChanged = onOptionChanged;
			SetIndex(this.Index);
		}

		public void Left() {
			SetIndex(this.Index - 1);
		}

		public void Right() {
			SetIndex(this.Index + 1);
		}

		private void SetIndex(int i) {
			this.Index = i;
			if (this.Index < 0) {
				if (this.Loop) {
					this.Index = this.Options.Length - 1;
				} else {
					this.Index = 0;
				}
			} else if (this.Index >= this.Options.Length) {
				if (this.Loop) {
					this.Index = 0;
				} else {
					this.Index = this.Options.Length - 1;
				}
			}
			var text = this.Options[this.Index];
			this.OptText.text = text;
			if (this.OnOptionChanged != null) {
				this.OnOptionChanged.Invoke(this.Index, text);
			}
		}
	}
}