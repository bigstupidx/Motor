using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameClient;

namespace GameUI
{
	public class TopBoardItemStar :TopBoardItem  {
		void OnEnable ()
		{
			if (this.Type == IAPType.Star) {
				//StartCoroutine (UpdateStar ());
				Init ();
			}
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
		}

		public override void Init ()
		{
			//base.Init ();
			SetStarAmount (Client.Match.GetTotalOwnedStar ());
		}

	

		public override void Refresh ()
		{
			SetStarAmount (Client.Match.GetTotalOwnedStar ());
		}

		private void SetStarAmount (int amount)
		{
			this.Count.text = amount.ToString () + "/" + Client.Match.GetTotalStar().ToString ();
		}
	}
}
