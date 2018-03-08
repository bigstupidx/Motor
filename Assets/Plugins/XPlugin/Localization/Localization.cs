// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XPlugin.Data.Json;
using XPlugin.Update;

namespace XPlugin.Localization {

	public delegate void LanguageChange(LanguageEnum old, LanguageEnum newer);

	public static class Localization {

		public readonly static Dictionary<LanguageEnum, string> LanguagePath = new Dictionary<LanguageEnum, string>{
				{LanguageEnum.Unkonwn, "" },
				{ LanguageEnum.zh_CN, "zh-CN/" },
				{ LanguageEnum.zh_TW, "zh-TW/" },
				{ LanguageEnum.en_US, "en-US/" },
				{ LanguageEnum.fr_FR, "fr-FR/" },
			};

		public static LanguageChange OnLanguageChanged;

		private static LanguageEnum _language = LanguageEnum.zh_CN;
		/// <summary>
		/// 当前语言
		/// </summary>
		public static LanguageEnum Language
		{
			get { return _language; }
			set
			{
				Debug.Log("设置语言:" + value);
				var oldLanguage = _language;
				_language = value;
				CurrentPath = LanguagePath[value];
				if (OnLanguageChanged != null) {
					OnLanguageChanged(oldLanguage, value);
				}
			}
		}

		/// <summary>
		/// 当前语言资源路径
		/// </summary>
		public static string CurrentPath { private set; get; }

		[RuntimeInitializeOnLoadMethod]
		public static void InitLanguage() {
			//预置语言
			var www = UResources.LoadStreamingAsset("language.txt");
			if (string.IsNullOrEmpty(www.error)) {
				var txt = www.text;
				txt = txt.Replace("-", "_");//避免把_写成了-
				Language = (LanguageEnum)Enum.Parse(typeof(LanguageEnum), txt);
				Debug.Log("使用StreamingAsset中的语言:" + www.text + " => " + Language);
			} else {
				var systemLanguage = Application.systemLanguage;
				switch (systemLanguage) {
					case SystemLanguage.Chinese:
					case SystemLanguage.ChineseSimplified:
						Language = LanguageEnum.zh_CN;
						break;
					case SystemLanguage.ChineseTraditional:
						Language = LanguageEnum.zh_TW;
						break;
					case SystemLanguage.English:
						Language = LanguageEnum.en_US;
						break;
					case SystemLanguage.French:
						Language = LanguageEnum.fr_FR;
						break;
				}
				Debug.Log("没有找到StreamingAsset中的语言,使用系统语言:" + systemLanguage + " => " + Language);
			}
		}

		/// <summary>
		/// 将资源路径转换为本地化资源路径
		/// 例如简体中文下asset变为zh-CN/asset
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string ConvertAssetPath(string path) {
			return CurrentPath + path;
		}
	}
}