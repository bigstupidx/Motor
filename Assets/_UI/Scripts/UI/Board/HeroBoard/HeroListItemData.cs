
using GameClient;

namespace GameUI
{
	public class HeroListItemData
	{
		public HeroInfo Info;

		public SelectedChangedDelegate selectedChanged;
		public LockStateChangedDelegate LockStateChanged;

		private bool _selected;
		public bool Selected {
			get { return _selected; }
			set
			{
				if (_selected != value)
				{
					_selected = value;
					if(selectedChanged != null)
					{
						selectedChanged(_selected);
					}
				}
			}
		}

		private bool _isUnLock;
		public bool IsUnlock {
			get { return _isUnLock; }
			set {
				if (_isUnLock != value)
				{
					_isUnLock = value;
					if (LockStateChanged != null)
					{
						LockStateChanged(_isUnLock);
					}
				}
			}
		}

		public HeroListItemData(HeroInfo info)
		{
			Info = info;
		}

	}


}
