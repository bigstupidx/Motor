using System.Collections.Generic;
using GameClient;
using UnityEngine;

namespace Game {
	public class BikeState : BikeBase {

		public BikeFSM Fsm;

#if !UNITY_EDITOR
		[System.NonSerialized]
#endif
		public BikeFSM.State state;
		public List<BikeBase> ComponentList { get; private set; }

		public override void Init(PlayerInfo info)
		{
			base.Init(info);			
		}

		public override void Awake() {
			base.Awake();
			this.Fsm = new BikeFSM();
			this.Fsm.OnAttackIn += FsmOnOnAttackIn;
			this.Fsm.OnAttackOut += FsmOnOnAttackOut;
			this.Fsm.OnBackIn += FsmOnOnBackIn;
			this.Fsm.OnBackOut += FsmOnOnBackOut;
			this.Fsm.OnCrashIn += FsmOnOnCrashIn;
			this.Fsm.OnCrashOut += FsmOnOnCrashOut;
			this.Fsm.OnRideIn += FsmOnOnRideIn;
			this.Fsm.OnRideOut += FsmOnOnRideOut;
			this.Fsm.OnStandIn += FsmOnOnStandIn;
			this.Fsm.OnStandOut += FsmOnOnStandOut;

			ComponentList = new List<BikeBase>
			{
				bikeControl,
				bikeInput,
				bikeSound,
				bikeDriver,
				bikeState,
				bikeAttack,
				bikeHealth,
				bikeFx,
				bikeBuff,
				bikeProp,
				racerInfo
			};
		}

		void Update() {
#if UNITY_EDITOR
			this.state = this.Fsm.currentState();
#endif
			// if(bikeShapeshift.IsShapeshifted) return;
			if (bikeControl.SpeedWithNeg > 10) {
				bikeState.ProcessEvent(BikeFSM.Event.Ride);
			} else if (bikeControl.SpeedWithNeg < -1) {
				bikeState.ProcessEvent(BikeFSM.Event.Back);
			} else {
				bikeState.ProcessEvent(BikeFSM.Event.Stand);
			}
			//判断车祸
//			RaycastHit hit;
//			if (Physics.Raycast(transform.position, transform.forward, out hit, 3.0f, Layers.Ins.Wall) && this.bikeControl.Speed > 30) {
//				if (hit.collider.transform.root != transform.root) {
//					
//					ProcessEvent(BikeFSM.Event.Crash, BikeDeathType.Crash);
//				}
//			}
		}

		public bool IsCurrentState(BikeFSM.State state) {
			return this.Fsm.currentState() == state;
		}

		public void ProcessEvent(BikeFSM.Event e, object o = null) {
			// if(gameObject.CompareTag(Tags.Ins.Player)) Debug.Log("State" + state +  " Evemt" + e);
			this.Fsm.processEvent(e, o);
		}

		private void FsmOnOnStandOut(object o) {
		}

		private void FsmOnOnStandIn(object o) {
		}

		private void FsmOnOnRideOut(object o) {
		}

		private void FsmOnOnRideIn(object o) {
		}

		private void FsmOnOnCrashOut(object o) {
			this.bikeControl.Crashed = false;
		}

		private void FsmOnOnCrashIn(object o) {
			this.bikeControl.Crashed = true;
		}

		private void FsmOnOnBackOut(object o) {
		}

		private void FsmOnOnBackIn(object o) {
		}

		private void FsmOnOnAttackOut(object o) {
		}

		private void FsmOnOnAttackIn(object o) {
		}
	}
}