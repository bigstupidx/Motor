using UnityEngine;

namespace Game {
	public class WeaponNormal :WeaponBase {

		public GameObject[] entities;

		public override void Hide() {
			base.Hide();
			foreach (var entity in this.entities) {
				entity.SetActive(false);
			}
		}

		public override void Show() {
			base.Show();
			foreach (var entity in this.entities) {
				entity.SetActive(true);
			}
		}
	}
}