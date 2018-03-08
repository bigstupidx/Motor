using UnityEngine;
using System.Collections;
using System.Text;
using XPlugin.Data.Json;

namespace GameClient {
	public class HeroInfo {
		[JsonIgnore]
		public HeroData Data;

		public int Level;
		public bool isUnLock = false;
		public RedPointState RedPointState = RedPointState.NotYetShow;

		/// <summary>
		/// 获取完整的特殊能力描述
		/// </summary>
		[JsonIgnore]
		public string AbilityDesc {
			get {
				string desc = Data.AbilityDesc;
				for (int i = 1; i <= Data.AbilityValue.Length; i++) {
					var value = Data.GetAbilityValue(i - 1, Level);
					desc = desc.Replace("#" + i, ((float)value).ToString("0.#"));
				}
				return desc;
			}
		}

		[JsonIgnore]
		public float[] AbilityValue {
			get {
				float[] ret = new float[this.Data.AbilityValue.Length];
				for (int i = 0; i < ret.Length; i++) {
					ret[i] = this.Data.AbilityValue[i].GetValue(this.Level);
				}
				return ret;
			}
		}

		[JsonIgnore]
		public bool IsMaxLv {
			get { return this.Level >= Client.Hero.MaxLevel - 1; }
		}

		public HeroInfo() {
		}

		public HeroInfo(HeroData data, int level = 0) {
			Data = data;
			Level = level;
		}

		public void InitData(HeroData data) {
			this.Data = data;
		}

		public JObject ToJson() {
			JObject ret = new JObject();
			ret["ID"] = this.Data.ID;
			ret["Level"] = this.Level;
			return ret;
		}

		public HeroInfo(JObject json) : this(Client.Hero[json["ID"].AsInt()], json["Level"].AsInt()) {
		}
	}
}

