using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XPlugin.Localization;
using XPlugin.Update;

namespace GameUI
{
    public class AtlasManager 
    {
        private static Dictionary<string, Atlas> atlasList = new Dictionary<string, Atlas>();

	    public static Sprite GetLocalizedSprite(string atlasName, string spriteName)
	    {
		    string localizedName = spriteName + "_" + Localization.Language;
			Sprite s = GetSprite(atlasName, localizedName);
		    if (s == null)
		    {
			    s = GetSprite(atlasName, spriteName);
		    }

		    return s;
	    }

	    public static Sprite GetSprite(string atlasName, string spriteName)
	    {
		    Atlas atlas = GetAtlas(atlasName);
			Sprite ret;
			atlas.Dics.TryGetValue(spriteName, out ret);
//			if (ret == null)
//            {
//                Debug.LogError("Sprite is not found：" + atlasName + ", " + spriteName);
//            }
            return ret;
        }

	    public static Atlas GetAtlas(string atlasName)
	    {
			Atlas atlas = null;
			if (!atlasList.ContainsKey(atlasName))
			{
				atlas = UResources.Load<Atlas>("Atlas/" + atlasName);
				atlasList.Add(atlasName, atlas);
			}
			if (!atlasList.TryGetValue(atlasName, out atlas))
			{
				Debug.LogError("没有找到Atlas：" + atlasName);
				return null;
			}
			if (atlas == null)
			{
				Debug.LogError("找到Atlas：" + atlasName + "！但它是空的");
				return null;
			}
			if (atlas.Dics == null)
			{
				Debug.LogError("Atlas:" + atlasName + " 中的Dics为空");
				return null;
			}

		    return atlas;
	    }
    }
}

