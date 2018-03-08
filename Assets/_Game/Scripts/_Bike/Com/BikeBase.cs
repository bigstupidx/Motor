using UnityEngine;
using System.Collections.Generic;
using GameClient;

namespace Game {
	public class BikeBase : MonoBehaviour {
		public PlayerInfo info { get; set; }
		public BikeControl bikeControl { get; private set; }
		public BikeDriver bikeDriver { get; private set; }
		public BikeInput bikeInput { get; private set; }
		public BikeSound bikeSound { get; private set; }
		public BikeFx bikeFx { get; private set; }
		public BikeState bikeState { get; private set; }
		public BikeAttack bikeAttack { get; private set; }
		public BikeHealth bikeHealth { get; private set; }
		public BikeBuff bikeBuff { get; private set; }
		public BikeProp bikeProp { get; private set; }
		public RacerInfo racerInfo { get; private set; }

		public BikeNetwork bikeNetwork { get; private set; }

		public virtual void Awake() {
			this.bikeControl = GetComponent<BikeControl>();
			this.bikeInput = GetComponent<BikeInput>();
			this.bikeSound = GetComponent<BikeSound>();
			this.bikeFx = GetComponent<BikeFx>();
			this.bikeDriver = GetComponent<BikeDriver>();
			this.bikeState = GetComponent<BikeState>();
			this.bikeAttack = GetComponent<BikeAttack>();
			this.bikeHealth = GetComponent<BikeHealth>();
			this.bikeBuff = GetComponent<BikeBuff>();
			this.bikeProp = GetComponent<BikeProp>();
			this.racerInfo = GetComponent<RacerInfo>();
			this.bikeNetwork = GetComponent<BikeNetwork>();
		}

		public void InitInfo(PlayerInfo playerInfo) {
			foreach (var component in bikeState.ComponentList) {
				component.info = playerInfo;
				component.Init(playerInfo);
			}
		}

		public virtual void Init(PlayerInfo playerInfo) {
		}
	}
}