using UnityEngine;
using System.Collections;
using GameClient;

namespace GameUI
{
	public class PropItemInfo
	{
		public PropInfo PropInfo;
		
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

		public PropItemInfo(PropInfo info)
		{
			this.PropInfo = info;
		}
	}

}

