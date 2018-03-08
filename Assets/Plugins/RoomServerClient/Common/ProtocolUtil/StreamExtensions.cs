using System.IO;
using Protocol;

namespace SilentOrbit.ProtocolBuffers {
	public static class StreamExtensions {

		public static void Clean(this Stream stream) {
			stream.Position = 0;
			stream.SetLength(0);
		}

		public static void Write(this Stream stream, int val) {
			ProtocolParser.WriteInt32(stream, val);
		}

		public static void Write(this Stream stream, float val) {
			ProtocolParser.WriteFloat(stream, val);
		}

		public static void Write(this Stream stream, double val) {
			ProtocolParser.WriteDouble(stream, val);
		}

		public static void Write(this Stream stream, bool val) {
			ProtocolParser.WriteBool(stream, val);
		}

		public static void Write(this Stream stream, string val) {
			ProtocolParser.WriteString(stream, val);
		}

		public static void Write(this Stream stream, byte[] bytes) {
			ProtocolParser.WriteBytes(stream, bytes);
		}

		public static void WriteObject(this Stream stream, object obj) {
			SerializableTypeRegister.Serialize(obj, stream);
		}

		public static int ReadInt(this Stream stream) {
			return ProtocolParser.ReadInt32(stream);
		}

		public static float ReadFloat(this Stream stream) {
			return ProtocolParser.ReadFloat(stream);
		}

		public static double ReadDouble(this Stream stream) {
			return ProtocolParser.ReadDouble(stream);
		}

		public static bool ReadBool(this Stream stream) {
			return ProtocolParser.ReadBool(stream);
		}

		public static string ReadString(this Stream stream) {
			return ProtocolParser.ReadString(stream);
		}

		public static byte[] ReadBytes(this Stream stream) {
			return ProtocolParser.ReadBytes(stream);
		}

		public static T ReadObject<T>(this Stream stream) {
			return SerializableTypeRegister.Deserialize<T>(stream);
		}

		public static byte[] GetRemainBytes(this Stream stream) {
			long remainLength = stream.Length - stream.Position;
			byte[] remainBytes = new byte[remainLength];
			stream.Read(remainBytes, 0, (int)remainLength);
			return remainBytes;
		}

	}
}