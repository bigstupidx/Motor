using System.Collections.Generic;
using System.IO;
using System.Text;
using Protocol;

namespace SilentOrbit.ProtocolBuffers {
	public static class DictionaryExtensions {

		/// <summary>
		/// 没有则加入，有了则更新
		/// </summary>
		public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue value) {
			if (dic.ContainsKey(key)) {
				dic[key] = value;
			} else {
				dic.Add(key, value);
			}
		}

		/// <summary>
		/// 没有则加入，有了则更新，如果新值为null，则移除
		/// </summary>
		public static void AddOrUpdateOrRemove<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue value) {
			if (dic.ContainsKey(key)) {
				if (value == null) {
					dic.Remove(key);
				} else {
					dic[key] = value;
				}
			} else {
				if (value != null) {
					dic.Add(key, value);
				}
			}
		}

		public static void AddOrUpdateOrRemove<TKey, TValue>(this IDictionary<TKey, TValue> origin, IDictionary<TKey, TValue> changed) {
			foreach (var changedKv in changed) {
				AddOrUpdateOrRemove(origin, changedKv.Key, changedKv.Value);
			}
		}

		public static string ToStringFull<TKey, TValue>(this IDictionary<TKey, TValue> dict) {
			StringBuilder builder = new StringBuilder();
			builder.Append("{");
			foreach (var kv in dict) {
				builder.Append(string.Format("{0}={1},", kv.Key, kv.Value));
			}
			builder.Append("}");
			return builder.ToString();
		}

		public static Dictionary<object, object> BytesDictToProp(Dictionary<object, object> bytesDict, bool allowNull = true) {
			if (bytesDict == null) {
				return new Dictionary<object, object>();
			}
			Dictionary<object, object> ret = new Dictionary<object, object>();
			MemoryStream tmpStream = new MemoryStream();
			foreach (var kv in bytesDict) {
				byte[] valueData = (byte[])kv.Value;
				tmpStream.Clean();
				tmpStream.Write(valueData, 0, valueData.Length);//TODO 这里是简单实现，GC很大
				tmpStream.Position = 0;
				object realValue = SerializableTypeRegister.Deserialize(tmpStream);
				if (allowNull || realValue != null) {
					ret.Add(kv.Key, realValue);
				}
			}
			return ret;
		}

		public static Dictionary<object, object> PropToBytesDict(Dictionary<object, object> originDict) {
			if (originDict == null) {
				return new Dictionary<object, object>();
			}
			MemoryStream tmpStream = new MemoryStream();
			Dictionary<object, object> bytesDict = new Dictionary<object, object>();//TODO 这里是简单实现，GC很大
			foreach (var kv in originDict) {
				SerializableTypeRegister.Serialize(kv.Value, tmpStream);
				bytesDict.Add(kv.Key, tmpStream.ToArray());
				tmpStream.Clean();
			}
			return bytesDict;
		}


	}
}