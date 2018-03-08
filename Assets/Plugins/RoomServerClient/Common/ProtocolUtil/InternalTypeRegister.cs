using System;
using System.Collections.Generic;
using System.IO;
using SilentOrbit.ProtocolBuffers;

namespace Protocol {
	public partial class SerializableTypeRegister {

		public enum InternalTypeId {
			Null = 0,
			Bool,
			Int,
			Float,
			Double,
			Long,

			String = 20,
			Bytes = 21,

			Array = 30,
			List = 31,
			Dict = 32,
		}

		public static bool IsInternalType(Type type) {
			return type == typeof(bool) ||
				   type == typeof(int) ||
				   type == typeof(float) ||
				   type == typeof(double) ||
				   type == typeof(long) ||

				   type == typeof(string) ||
				   type == typeof(byte[]) ||

				   type == typeof(object[]) ||
				   type == typeof(List<object>) ||
				   type == typeof(Dictionary<object, object>)
				   ;
		}


		static SerializableTypeRegister() {
			//register internal type
			RegisterType(typeof(bool), (byte)InternalTypeId.Bool, BoolSerializeFunction, BoolDeserializeFunction);
			RegisterType(typeof(int), (byte)InternalTypeId.Int, IntSerializeFunction, IntDeserializeFunction);
			RegisterType(typeof(float), (byte)InternalTypeId.Float, FloatSerializeFunction, FloatDeserializeFunction);
			RegisterType(typeof(double), (byte)InternalTypeId.Double, DoubleSerializeFunction, DoubleDeserializeFunction);
			RegisterType(typeof(long), (byte)InternalTypeId.Long, LongSerializeFunction, LongDeserializeFunction);

			RegisterType(typeof(string), (byte)InternalTypeId.String, StringSerializeFunction, StringDeserializeFunction);
			RegisterType(typeof(byte[]), (byte)InternalTypeId.Bytes, BytesSerializeFunction, BytesDeserializeFunction);

			RegisterType(typeof(object[]), (byte)InternalTypeId.Array, ArraySerializeFunction, ArrayDeserializeFunction);
			RegisterType(typeof(List<object>), (byte)InternalTypeId.List, ListSerializeFunction, ListDeserializeFunction);
			RegisterType(typeof(Dictionary<object, object>), (byte)InternalTypeId.Dict, DictSerializeFunction, DictDeserializeFunction);
		}

		private static object BytesDeserializeFunction(Stream stream) {
			return ProtocolParser.ReadBytes(stream);
		}

		private static void BytesSerializeFunction(Stream outStream, object customObject) {
			byte[] obj = (byte[])customObject;
			ProtocolParser.WriteBytes(outStream, obj);
		}

		private static object ArrayDeserializeFunction(Stream stream) {
			int length = ProtocolParser.ReadInt32(stream);
			object[] ret = new object[length];
			for (int i = 0; i < length; i++) {
				ret[i] = Deserialize(stream);
			}
			return ret;
		}

		private static void ArraySerializeFunction(Stream stream, object customObject) {
			object[] objs = (object[])customObject;
			int count = objs.Length;//先写入对象数量
			ProtocolParser.WriteInt32(stream, count);
			for (var i = 0; i < objs.Length; i++) {
				var o = objs[i];
				Serialize(o, stream);
			}
		}

		private static object ListDeserializeFunction(Stream stream) {
			int length = ProtocolParser.ReadInt32(stream);
			List<object> ret = new List<object>(length);
			for (int i = 0; i < length; i++) {
				ret.Add(Deserialize(stream));
			}
			return ret;
		}

		private static void ListSerializeFunction(Stream stream, object customObject) {
			List<object> objs = (List<object>)customObject;
			int count = objs.Count;//先写入对象数量
			ProtocolParser.WriteInt32(stream, count);
			for (var i = 0; i < objs.Count; i++) {
				var o = objs[i];
				Serialize(o, stream);
			}
		}

		private static object DictDeserializeFunction(Stream stream) {
			int length = ProtocolParser.ReadInt32(stream);
			Dictionary<object, object> ret = new Dictionary<object, object>(length);
			for (int i = 0; i < length; i++) {
				object key = Deserialize(stream);
				object value = Deserialize(stream);
				ret[key] = value;
			}
			return ret;
		}

		private static void DictSerializeFunction(Stream stream, object customObject) {
			Dictionary<object, object> dict = (Dictionary<object, object>)customObject;
			int count = dict.Count;//先写入对象数量
			ProtocolParser.WriteInt32(stream, count);
			foreach (var kv in dict) {
				Serialize(kv.Key, stream);
				Serialize(kv.Value, stream);
			}
		}

		private static object DoubleDeserializeFunction(Stream inStream) {
			return ProtocolParser.ReadDouble(inStream);
		}

		private static void DoubleSerializeFunction(Stream outStream, object customObject) {
			ProtocolParser.WriteDouble(outStream, (double)customObject);
		}

		private static object LongDeserializeFunction(Stream inStream) {
			return ProtocolParser.ReadInt64(inStream);
		}

		private static void LongSerializeFunction(Stream outStream, object customObject) {
			ProtocolParser.WriteInt64(outStream, (int)customObject);
		}

		private static object BoolDeserializeFunction(Stream inStream) {
			return ProtocolParser.ReadBool(inStream);
		}


		private static void BoolSerializeFunction(Stream outStream, object customObject) {
			ProtocolParser.WriteBool(outStream, (bool)customObject);
		}

		private static object StringDeserializeFunction(Stream inStream) {
			return ProtocolParser.ReadString(inStream);
		}

		private static void StringSerializeFunction(Stream outStream, object customObject) {
			ProtocolParser.WriteString(outStream, (string)customObject);
		}

		private static object FloatDeserializeFunction(Stream inStream) {
			return ProtocolParser.ReadFloat(inStream);
		}

		private static void FloatSerializeFunction(Stream outStream, object customObject) {
			ProtocolParser.WriteFloat(outStream, (float)customObject);
		}

		private static object IntDeserializeFunction(Stream inStream) {
			return ProtocolParser.ReadInt32(inStream);
		}
		private static void IntSerializeFunction(Stream outStream, object customObject) {
			ProtocolParser.WriteInt32(outStream, (int)customObject);
		}
	}
}