namespace GameClient {
	public class BikeUpgradeItemData {
		public FloatLevelData Value;
		public IntLevelData Cost;


		public BikeUpgradeItemData(string[] str) {
			Value=new FloatLevelData(float.Parse(str[0]), float.Parse(str[1]));
			Cost=new IntLevelData(int.Parse(str[2]), int.Parse(str[3]));
		}

		public int GetCost(int lv) {
			return this.Cost.GetValue(lv);
		}

		public float GetValue(int lv) {
			return this.Value.GetValue(lv);
		}

	}
}