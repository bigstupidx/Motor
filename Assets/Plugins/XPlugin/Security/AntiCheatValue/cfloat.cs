//
// cfloat.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System.Runtime.InteropServices;

namespace XPlugin.Security.AnitiCheatValue {


	public struct cfloat {
		/// <summary>
		/// 获取值（也可以直接用float转换符）
		/// </summary>
		public float Value {
			get {
				FloatIntBytesUnion v = new FloatIntBytesUnion();
				v.i = ~this._value.i;
				return v.f;
			}
			set {
				FloatIntBytesUnion v = new FloatIntBytesUnion();
				v.f = value;
				this._value.i = ~v.i;
			}
		}

		#region 加解密

		[StructLayout(LayoutKind.Explicit)]
		private struct FloatIntBytesUnion {
			[FieldOffset(0)]
			public float f;

			[FieldOffset(0)]
			public int i;

			[FieldOffset(0)]
			public byte b1;

			[FieldOffset(1)]
			public byte b2;

			[FieldOffset(2)]
			public byte b3;

			[FieldOffset(3)]
			public byte b4;
		}

		private FloatIntBytesUnion _value;

		#endregion


		#region 类型转换符及一些运算符

		public static implicit operator cfloat(float value) {
			cfloat ret=new cfloat();
			ret.Value = value;
			return ret;
		}

		public static implicit operator float (cfloat obj) {
			return obj.Value;
		}

		public static cfloat operator ++(cfloat lhs) {
			return lhs.Value+ 1;
		}

		public static cfloat operator --(cfloat lhs) {
			return lhs.Value - 1;
		}

		public static bool operator ==(cfloat lhs, cfloat rhs) {
			return lhs._value.f == rhs._value.f;
		}

		public static bool operator !=(cfloat lhs, cfloat rhs) {
			return lhs._value.f != rhs._value.f;
		}

		#endregion


		#region 重写一些从object派生的方法

		public bool Equals(float obj) {
			return this.Value == obj;
		}

		public bool Equals(cfloat obj) {
			return this == obj;
		}

		public override bool Equals(object obj) {
			if (!(obj is cfloat))
				return false;

			return this == (cfloat)obj;
		}

		public override string ToString() {
			return this.Value.ToString();
		}

		public string ToString(string format) {
			return this.Value.ToString(format);
		}

		public override int GetHashCode() {
			return ~this._value.i;
		}

		#endregion
	}
}