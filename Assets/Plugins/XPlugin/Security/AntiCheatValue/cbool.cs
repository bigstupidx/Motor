//
// cbool.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace XPlugin.Security.AnitiCheatValue {

	public struct cbool {
		/// <summary>
		/// 获取值（也可以直接用bool转换符）
		/// </summary>
		public bool Value {
			get {
				return this._value == 4867;
			}
			set {
				this._value= value ? 4867 : 7684;
			}
		}

		private int _value;

		#region 类型转换符及一些运算符

		public static implicit operator cbool(bool value) {
			cbool ret=new cbool();
			ret.Value = value;
			return ret;
		}

		public static implicit operator bool (cbool obj) {
			return obj.Value;
		}

		public static bool operator ==(cbool lhs, cbool rhs) {
			return lhs._value == rhs._value;
		}

		public static bool operator !=(cbool lhs, cbool rhs) {
			return lhs._value != rhs._value;
		}

		#endregion


		#region 重写一些从object派生的方法

		public bool Equals(bool obj) {
			return this.Value == obj;
		}

		public bool Equals(cbool obj) {
			return this == obj;
		}

		public override bool Equals(object obj) {
			if (!(obj is cbool))
				return false;

			return this == (cbool)obj;
		}

		public override string ToString() {
			return this.Value.ToString();
		}

		public override int GetHashCode() {
			return (bool)this ? 1 : 0;
		}

		#endregion
	}
}
