// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections.Generic;
using System.Text;
using XPlugin.Localization;

namespace XPlugin {

	/// <summary>
	/// 昵称库
	/// </summary>
	public class NicknameLibrary {
		protected List<List<string>> WordList;

		public NicknameLibrary() {
			TextAsset textAsset = LResources.Load<TextAsset>("NickNameLibrary");
			this.WordList = new List<List<string>>();
			string[] lines = textAsset.text.Split("\n".ToCharArray());
			string[] fistLineWord = lines[0].Split(',');
			for (int index = 0; index < fistLineWord.Length; index++) {
				this.WordList.Add(new List<string>());
			}

			foreach (var line in lines) {
				if (string.IsNullOrEmpty(line)) {
					continue;
				}
				string[] word = line.Split(',');
				for (int i = 0; i < word.Length; i++) {
					if (!string.IsNullOrEmpty(word[i])) {
						this.WordList[i].Add(word[i]);
					}
				}
			}

		}

		/// <summary>
		/// 获取一个随机昵称
		/// </summary>
		public string Random {
			get {
				string result = "";
				foreach (var wordList in this.WordList) {
					if (wordList.Count > 0) {
						var word = wordList[UnityEngine.Random.Range(0, wordList.Count)];
						if (word.Contains("$$")) {//$$表示替换已经组合的词
							result = word.Replace("$$", result);
						} else {
							result += word;
						}
					}
				}
				return result;
			}
		}
	}
}
