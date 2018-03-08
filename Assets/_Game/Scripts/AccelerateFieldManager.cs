using HeavyDutyInspector;
using UnityEngine;

namespace Game {
	public class AccelerateFieldManager : MonoBehaviour {
		public AccelerateField[] AccelerateFields;


		[SerializeField]
		[Button("Auto find", "__Find", true)]
		private bool _btn_find;
		void __Find() {
			this.AccelerateFields = FindObjectsOfType<AccelerateField>();
		}


		void Start()
		{
			if (RaceManager.Ins != null) {
				if (RaceLineManager.Ins.Current.IsReverse) {
					if (this.AccelerateFields == null || this.AccelerateFields.Length == 0) {
						this.AccelerateFields = transform.GetComponentsInChildren<AccelerateField>();
					}

					foreach (var a in this.AccelerateFields) {
						a.Reverse();
					}
				}
			}
		}
	}
}