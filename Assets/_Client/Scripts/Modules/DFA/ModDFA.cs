using UnityEngine;
using XPlugin.Data.SQLite;
using XPlugin.Update;

namespace GameClient {
	public class ModDFA : ClientModule {

		public string DFAFileName = "DFA";


		public override void InitData(DbAccess db) {
			base.InitData(db);
			TextAsset text = UResources.Load<TextAsset>(DFAFileName);
			DFAUtil.Init(text.text);
		}

		/// <summary>
		/// 敏感字过滤，需先初始化
		/// </summary>
		/// <returns>过滤后的字符串.</returns>
		/// <param name="input">输入的字符串.</param>
		/// <param name="isFilter">是否被过滤.</param>
		public string Filter(string input, out bool isFilter) {
			return DFAUtil.FilterWords(input, out isFilter);
		}
	}

}

