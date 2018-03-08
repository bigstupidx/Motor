
using GameClient;

namespace GameUI
{
	public class BikeListItemData {

		public BikeInfo Info;
		public SelectedChangedDelegate selectedChanged;
		public LockStateChangedDelegate LockStateToGameChanged;

		private bool _selected;
		public bool Selected
		{
			get { return _selected; }
			set
			{
				if (_selected != value)
				{
					_selected = value;
					if (selectedChanged != null)
					{
						selectedChanged(_selected);
					}
				}
			}
		}

		private bool _isUnLock;
		public bool isUnLock
		{
			get { return _isUnLock; }
			set
			{
				if (_isUnLock != value)
				{
					_isUnLock = value;
					if (LockStateToGameChanged != null)
					{
						LockStateToGameChanged(_isUnLock);
					}
				}
			}
		}

		public BikeListItemData(BikeInfo info) {
			Info = info;
		}
	}


}
