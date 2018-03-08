using System;
using UnityEngine.Events;

namespace UnityEngine.UI {
	public class NumSpin : MonoBehaviour {

		public Text NumText;
		public int Value;
		public int Min;
		public int Max;

		public UnityEvent<int> OnValueChanged;

		public void Init(int initValue, int min, int max, UnityEvent<int> onValueChanged) {
			this.Min = min;
			this.Max = max;
			this.NumText.text = this.Value.ToString();
			this.OnValueChanged = onValueChanged;
			SetValue(initValue);
		}


		public void Increse() {
			SetValue(this.Value + 1);
		}

		public void Descrese() {
			SetValue(this.Value - 1);
		}

		public void SetValue(int value) {
			this.Value = value;
			this.Value = Mathf.Clamp(this.Value, this.Min, this.Max);
			this.NumText.text = this.Value.ToString();
			if (this.OnValueChanged != null) {
				this.OnValueChanged.Invoke(this.Value);
			}
		}



	}
}