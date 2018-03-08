using System;
using UnityEngine;

namespace Game {

	public delegate void OnAttackDelegate(bool? isLeft);
	public class BikeInput : BikeBase {

		public Action OnBoost = delegate { };
		public Action OnBosstWithoutEnergy = delegate { };
		public Action OnBosstWithoutEnergyTiming = delegate { };
		public Action OnDrift = delegate { };
		public OnAttackDelegate OnAttack = delegate { };

		private float _horizontal;
		public float Horizontal {
			get { return _horizontal; }
			set {
				_horizontal = value;
				_horizontal = Mathf.Clamp(_horizontal, -1, 1);
			}
		}

		private float _vertical;
		public float Vertical {
			get { return _vertical; }
			set {
				_vertical = value;
				_vertical = Mathf.Clamp(_vertical, -1, 1);
			}
		}
	}
}