using UnityEngine;
using System.Collections;
using GameClient;

namespace GameUI
{
	public class WeaponItemInfo
	{
		public WeaponInfo WeaponInfo;
		
		public SelectedChangedDelegate selectedChanged;
		public AmountChangeDelegate amountChanged;

		private bool _selected;
		public bool Selected
		{
			get { return _selected; }
			set
			{
				// if the value has changed
				if (_selected != value)
				{
					// update the state and call the selection handler if it exists
					_selected = value;
					if (selectedChanged != null) selectedChanged(_selected);
				}
			}
		}

		public WeaponItemInfo(WeaponInfo info)
		{
			this.WeaponInfo = info;
		}
	}

}

