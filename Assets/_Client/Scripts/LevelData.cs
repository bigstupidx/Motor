namespace GameClient {
	/// <summary>
	/// 数值=基数+成长*等级 模型
	/// </summary>
	public class FloatLevelData {
		public float Base;
		public float Grow;


		public FloatLevelData(float @base, float grow) {
			this.Base = @base;
			this.Grow = grow;
		}

		public FloatLevelData(string[] s) {
			this.Base = float.Parse(s[0]);
			this.Grow = float.Parse(s[1]);
		}

		public float GetValue(int lv) {
			return this.Base + this.Grow*lv;
		}
	}

	public class IntLevelData {
		public int Base;
		public int Grow;


		public IntLevelData(int @base, int grow) {
			this.Base = @base;
			this.Grow = grow;
		}

		public IntLevelData(string[] s) {
			this.Base = int.Parse(s[0]);
			this.Grow = int.Parse(s[1]);
		}

		public int GetValue(int lv) {
			return this.Base + this.Grow * lv;
		}
	}
}