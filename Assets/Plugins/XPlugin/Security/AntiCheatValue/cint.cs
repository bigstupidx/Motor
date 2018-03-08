//
// cint.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace XPlugin.Security.AnitiCheatValue {

	public struct cint {
		/// <summary>
		/// 获取值（也可以直接用int转换符）
		/// </summary>
		public int Value {
			get { return ~this._value; }
			set { this._value = ~value; }
		}

		private int _value;

		#region 类型转换符及一些运算符

		public static implicit operator cint(int value) {
			cint ret=new cint();
			ret.Value = value;
			return ret;
		}

		public static implicit operator int (cint obj) {
			return obj.Value;
		}

		public static cint operator ++(cint lhs) {
			return lhs.Value + 1;
		}

		public static cint operator --(cint lhs) {
			return lhs.Value - 1;
		}

		public static bool operator ==(cint lhs, cint rhs) {
			return lhs._value == rhs._value;
		}

		public static bool operator !=(cint lhs, cint rhs) {
			return lhs._value != rhs._value;
		}

		#endregion


		#region 重写一些从object派生的方法

		public bool Equals(int obj) {
			return this.Value == obj;
		}

		public bool Equals(cint obj) {
			return this == obj;
		}

		public override bool Equals(object obj) {
			if (!(obj is cint))
				return false;

			return this == (cint)obj;
		}

		public override string ToString() {
			return this.Value.ToString();
		}

		public override int GetHashCode() {
			return this;
		}

		#endregion
	}
}