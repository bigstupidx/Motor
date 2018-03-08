//
// LevelModel.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System;
using System.Collections;
using XPlugin.Security.AnitiCheatValue;

namespace GameClient {
	/// <summary>
	/// 等级经验状态
	/// </summary>
	public struct ExpState {
		public int Level;
		public int Exp;
		public int NowLevelExp;
		public int NowLevelMaxExp;

		public void Reset () {
			Level = 1;
			Exp = 0;
			NowLevelExp = 0;
			NowLevelMaxExp = 0;
		}
	}

	/// <summary>
	/// 等级模型
	/// </summary>
	public abstract class LevelModel {
		/// <summary>
		/// 等级
		/// </summary>
		public cint Level;

		/// <summary>
		/// 累计经验
		/// </summary>
		public cint Exp;

		/// <summary>
		/// 升级事件代理
		/// </summary>
		public event Action<int, int> OnLevelChange = null;

		public LevelModel (int level, int exp = 0) {
			Level = level;
			Exp = exp;

			if (Level > MinLevel && Exp < GetExpByLevel (Level - 1)) {
				Exp = GetExpByLevel (Level - 1);
			}
		}


		/// <summary>
		/// 直接转换成等级数字
		/// </summary>
		/// <param name="levelModel"></param>
		/// <returns></returns>
		public static implicit operator int (LevelModel levelModel) {
			return levelModel != null ? (int)levelModel.Level : 0;
		}

		public override string ToString () {
			return Level.ToString ();
		}

		/// <summary>
		/// 获取等级对应的经验值
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public abstract int GetExpByLevel (int level);

		/// <summary>
		/// 最大等级
		/// </summary>
		public abstract int MaxLevel { get; }

		/// <summary>
		/// 最小等级
		/// </summary>
		public virtual int MinLevel {
			get {
				return 1;
			}
		}


		/// <summary>
		/// 是否满级
		/// </summary>
		public bool IsMaxLevel {
			get {
				return Level >= MaxLevel;
			}
		}

		/// <summary>
		/// 是否经验值已满
		/// </summary>
		public bool IsMaxExp {
			get {
				return IsMaxLevel && NowLevelExp == NowLevelMaxExp;
			}
		}

		/// <summary>
		/// 当前等级经验值
		/// </summary>
		public virtual int NowLevelExp {
			get {
				if (Level == MinLevel) {
					return Exp;
				} else {
					return Exp - GetExpByLevel (Level - 1);
				}
			}
		}

		/// <summary>
		/// 当前等级最大经验值
		/// </summary>
		public virtual int NowLevelMaxExp {
			get {
				if (Level == MinLevel) {
					return GetExpByLevel (Level);
				} else if (Level == MaxLevel && GetExpByLevel (MaxLevel) == 0) {
					return 0;
				} else {
					return GetExpByLevel (Level) - GetExpByLevel (Level - 1);
				}
			}
		}

		/// <summary>
		/// 升级所需经验值
		/// </summary>
		public int NextLevelExp {
			get {
				return GetExpByLevel (Level);
			}
		}

		/// <summary>
		/// 设置值
		/// </summary>
		/// <param name="level"></param>
		public void SetLevel (int level) {
			int exp = 0;
			if (level > MinLevel) {
				exp = GetExpByLevel (level - 1);
			}
			SetValue (level, exp);
		}

		/// <summary>
		/// 设置经验值
		/// </summary>
		/// <param name="exp"></param>
		public void SetExp (int exp) {
			int max = MaxLevel;
			for (int i = MinLevel; i <= max; i++) {
				if (i == max) {
					SetValue (i, exp);
					break;
				} else if (exp < GetExpByLevel (i)) {
					SetValue (i, exp);
					break;
				}
			}
		}

		/// <summary>
		/// 设置值
		/// </summary>
		/// <param name="level"></param>
		/// <param name="exp"></param>
		public void SetValue (int level, int exp) {
			int line1 = 0, line2 = 0;
			if (level > MinLevel) {
				line1 = GetExpByLevel (level - 1);
			}
			line2 = GetExpByLevel (level);

			if (level == MaxLevel && line2 == 0) {
				exp = line1;
			} else {
				if (exp < line1) {
					exp = line1;
				} else if (exp > line2) {
					exp = line2;
				}
			}

			int oldLevel = Level;
			Level = level;
			Exp = exp;
			if (oldLevel != Level && OnLevelChange != null) {
				OnLevelChange (oldLevel, Level);
			}
		}

		/// <summary>
		/// 设置值
		/// </summary>
		/// <param name="model"></param>
		public void SetValue (LevelModel model) {
			SetValue (model.Level, model.Exp);
		}

		/// <summary>
		/// 增加经验
		/// </summary>
		/// <returns>升级次数</returns>
		/// <param name="expAmount">经验数量</param>
		public virtual int AddExp (int expAmount) {
			if (IsMaxExp) {
				// 经验已满
				return 0;
			}

			Exp += expAmount;
			int newLevel = Level;
			int maxLevel = MaxLevel;
			for (int i = Level; i <= maxLevel; i++) {
				if (i == maxLevel) {
					if (Exp > GetExpByLevel (i)) {
						Exp = GetExpByLevel (i);
					}
					newLevel = i;
					break;
				} else if (Exp < GetExpByLevel (i)) {
					newLevel = i;
					break;
				}
			}
			if (Level != newLevel) {
				int oldLevel = Level;
				Level = newLevel;
				if (OnLevelChange != null) {
					OnLevelChange (oldLevel, Level);
				}
				return Level - oldLevel;
			} else {
				return 0;
			}
		}

		/// <summary>
		/// 缓存经验及等级状态
		/// </summary>
		/// <returns></returns>
		public ExpState DumpState () {
			ExpState state = new ExpState ();
			state.Level = Level;
			state.Exp = Exp;
			state.NowLevelExp = NowLevelExp;
			state.NowLevelMaxExp = NowLevelMaxExp;
			return state;
		}
	}
}
