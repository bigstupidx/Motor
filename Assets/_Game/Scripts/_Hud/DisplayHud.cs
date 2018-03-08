using System;
using GameClient;
using GameUI;
using UnityEngine;
using XPlugin.Update;

namespace Game {
	public class DisplayHud : MonoBehaviour {
		public Transform Target;
		[NonSerialized]
		public BikeState Bike;

		private HUDText _hud;
		private string _bikeName;
		private bool _isShow = false;


		public void SetBike(BikeBase bike) {
			this.Bike = bike.bikeState;
			Target = bike.bikeDriver.BikeDriverModel.HeadPoint;
			if (_hud != null) {
				_hud.GetComponent<UIFollowTarget>().target = Target;
			}
			_bikeName = Bike.info.NickName;
		}

		public void ShowHud() {
			_hud = HUDText.Spawn(UResources.Load<GameObject>("UI/Hud/HUD"), Target, _bikeName);
			//			_hud.SetNameActive(RaceManager.Ins.GameModeInstance.Match.MatchMode == MatchMode.Online);
			_hud.SetNameActive(false);
		}

		public void SetIsSelf(bool isSelf) {
			_hud.SetRankArrow(isSelf);
		}

		private void Update() {
			if (_hud == null) {
				enabled = false;
				return;
			}
			if (Time.frameCount % 2 != 0) {
				return;
			}
			if (RaceManager.Ins.GamePhase <= GamePhase.CountDown) {
				_hud.SetRankActive(false);
				_hud.SetNameActive(false);
				return;
			} else {
				_hud.SetRankActive(true);
				_hud.SetNameActive(false);
			}
			_hud.SetRank(Bike.racerInfo.Rank);
		}

		void OnDisable() {
			if (this._hud != null) {
				ModHUD.Ins.Despawn(this._hud.gameObject, false);
			}
		}
	}

}
