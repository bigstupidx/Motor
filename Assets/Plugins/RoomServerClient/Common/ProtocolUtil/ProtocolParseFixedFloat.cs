using System.IO;

namespace SilentOrbit.ProtocolBuffers {
	public static partial class ProtocolParser {

		private static byte[] _floatBuffer = new byte[4];

		public static float ReadFloat(Stream stream) {
			lock (_floatBuffer) {
				stream.Read(_floatBuffer, 0, 4);
				int tmpIndex = 0;
				return BitConverterNoAlloc.ToSingle(_floatBuffer, ref tmpIndex);
			}
		}

		public static void WriteFloat(Stream stream, float val) {
			lock (_floatBuffer) {
				int tmpIndex = 0;
				BitConverterNoAlloc.Include(val, _floatBuffer, ref tmpIndex);
				stream.Write(_floatBuffer, 0, 4);
			}
		}

		private static byte[] _doubleBuffer = new byte[8];

		public static double ReadDouble(Stream stream) {
			lock (_doubleBuffer) {
				stream.Read(_doubleBuffer, 0, 8);
				int tmpIndex = 0;
				return BitConverterNoAlloc.ToDouble(_doubleBuffer, ref tmpIndex);
			}
		}

		public static void WriteDouble(Stream stream, double val) {
			lock (_doubleBuffer) {
				int tmpIndex = 0;
				BitConverterNoAlloc.Include(val, _doubleBuffer, ref tmpIndex);
				stream.Write(_doubleBuffer, 0, 8);
			}
		}

	}
}