using System;
using System.Collections.Generic;
using System.IO;
#if COREFX
using System.Reflection;
#endif
using SilentOrbit.ProtocolBuffers;


namespace Protocol {
	public partial class SerializableTypeRegister {

		internal static Dictionary<Type, SerializableTypeInfo> TypeDict = new Dictionary<Type, SerializableTypeInfo>();
		internal static Dictionary<byte, SerializableTypeInfo> IdDict = new Dictionary<byte, SerializableTypeInfo>();

		public static void RegisterType(Type type, byte id, SerializeMethod serializeFunction, DeserializeMethod deserializeFunction) {
			if (TypeDict.ContainsKey(type) || IdDict.ContainsKey(id)) {
				throw new Exception("Type Already Registed :" + type + "/" + id);
			}
			SerializableTypeInfo customType = new SerializableTypeInfo(type, id, serializeFunction, deserializeFunction);
			TypeDict.Add(type, customType);
			IdDict.Add(id, customType);
		}

		public static void RegisterType(Type type, byte id, SerializeStreamMethod serializeFunction, DeserializeStreamMethod deserializeFunction) {
			if (TypeDict.ContainsKey(type) || IdDict.ContainsKey(id)) {
				throw new Exception("Type Already Registed :" + type + "/" + id);
			}
			SerializableTypeInfo customType = new SerializableTypeInfo(type, id, serializeFunction, deserializeFunction);
			TypeDict.Add(type, customType);
			IdDict.Add(id, customType);
		}

		public static int GetId(Type type) {
			var info = TypeDict[type];
			if (info == null) {
				throw new Exception("Type " + type + " not register");
			}
			return info.Id;
		}

		public static object Deserialize(Stream stream) {
			byte id = (byte)stream.ReadByte();
			if (id == (byte)InternalTypeId.Null) {
				return null;
			}
			SerializableTypeInfo info;
			if (!IdDict.TryGetValue(id, out info)) {
				throw new Exception("Type Id " + id + " not register");
			}
			if (info.DeserializeStreamFunction != null) {
				return info.DeserializeStreamFunction(stream);
			} else if (info.DeserializeFunction != null) {
				return info.DeserializeFunction(ProtocolParser.ReadBytes(stream));
			} else {
				throw new Exception("DeserializeStreamFunction and DeserializeFunction can't be null together");
			}
		}

		public static T Deserialize<T>(Stream stream) {
			var obj = Deserialize(stream);
			if (obj == null) {
				return default(T);
			}
			return (T)obj;
		}


		public static void Serialize(object obj, Stream stream) {
			if (obj == null) {
				stream.WriteByte((byte)InternalTypeId.Null);
				return;
			}
			var type = obj.GetType();
			//enum is deal with int32
#if COREFX
			if (type.GetTypeInfo().IsEnum) {
				type = typeof(int);
			}
#else
			if (type.IsEnum) {
				type = typeof(int);
			}
#endif
			SerializableTypeInfo info;
			if (!TypeDict.TryGetValue(type, out info)) {
				throw new Exception("Type " + type + " not register");
			}
			if (info.SerializeStreamFunction != null) {
				stream.WriteByte(info.Id);
				info.SerializeStreamFunction(stream, obj);
			} else if (info.SerializeFunction != null) {
				stream.WriteByte(info.Id);
				ProtocolParser.WriteBytes(stream, info.SerializeFunction(obj));
			} else {
				throw new Exception("SerializeStreamFunction and SerializeFunction can't be null together");
			}
		}



	}
}