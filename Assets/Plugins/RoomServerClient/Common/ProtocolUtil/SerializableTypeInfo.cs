using System;
using System.IO;

namespace Protocol {
	public delegate byte[] SerializeMethod(object customObject);
	public delegate object DeserializeMethod(byte[] serializedCustomObject);
	public delegate void SerializeStreamMethod(Stream stream, object customObject);
	public delegate object DeserializeStreamMethod(Stream stream);

	public class SerializableTypeInfo {
		public readonly byte Id;
		public readonly Type Type;
		public readonly SerializeMethod SerializeFunction;
		public readonly DeserializeMethod DeserializeFunction;
		public readonly SerializeStreamMethod SerializeStreamFunction;
		public readonly DeserializeStreamMethod DeserializeStreamFunction;

		public SerializableTypeInfo(Type type, byte id, SerializeMethod serializeFunction, DeserializeMethod deserializeFunction) {
			this.Type = type;
			this.Id = id;
			this.SerializeFunction = serializeFunction;
			this.DeserializeFunction = deserializeFunction;
		}

		public SerializableTypeInfo(Type type, byte id, SerializeStreamMethod serializeFunction, DeserializeStreamMethod deserializeFunction) {
			this.Type = type;
			this.Id = id;
			this.SerializeStreamFunction = serializeFunction;
			this.DeserializeStreamFunction = deserializeFunction;
		}
	}


}