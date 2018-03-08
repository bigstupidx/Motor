#region ProtocolParser
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SilentOrbit.ProtocolBuffers {
	public static partial class ProtocolParser {

		private static byte[] _stringBuffer = new byte[1024];

		public static string ReadString(Stream stream) {
			int length = (int)ReadInt32(stream);
			if (length < 0) {
				return null;
			}
			if (length == 0) {
				return "";
			}
			lock (_stringBuffer) {
				if (_stringBuffer.Length < length) {//string缓存区太小，放大
					int needLength = _stringBuffer.Length * 2;
					while (needLength < length) {
						needLength *= 2;
					}
					_stringBuffer = new byte[needLength];
				}
				stream.Read(_stringBuffer, 0, length);
				return Encoding.UTF8.GetString(_stringBuffer, 0, length);
			}
		}

		public static void WriteString(Stream stream, string val) {
			if (val == null) {//用负数表示为null的string
				WriteInt32(stream, -1);
				return;
			}
			int maxLength = Encoding.UTF8.GetMaxByteCount(val.Length);
			lock (_stringBuffer) {
				if (_stringBuffer.Length < maxLength) {//string缓存区太小，放大
					int needLength = _stringBuffer.Length * 2;
					while (needLength < maxLength) {
						needLength *= 2;
					}
					_stringBuffer = new byte[needLength];
				}
				int length = Encoding.UTF8.GetBytes(val, 0, val.Length, _stringBuffer, 0);
				WriteUInt32(stream, (uint)length);
				stream.Write(_stringBuffer, 0, length);
			}
		}

		/// <summary>
		/// Reads a length delimited byte array
		/// </summary>
		public static byte[] ReadBytes(Stream stream) {
			//VarInt length
			int length = (int)ReadUInt32(stream);

			//Bytes
			byte[] buffer = new byte[length];
			int read = 0;
			while (read < length) {
				int r = stream.Read(buffer, read, length - read);
				if (r == 0)
					throw new ProtocolBufferException("Expected " + (length - read) + " got " + read);
				read += r;
			}
			return buffer;
		}

		/// <summary>
		/// Writes length delimited byte array
		/// </summary>
		public static void WriteBytes(Stream stream, byte[] val) {
			WriteUInt32(stream, (uint)val.Length);
			stream.Write(val, 0, val.Length);
		}

	}
}

#endregion
#region ProtocolParserExceptions
//
// Exception used in the generated code
//

namespace SilentOrbit.ProtocolBuffers {
	///<summary>>
	/// This exception is thrown when badly formatted protocol buffer data is read.
	///</summary>
	public class ProtocolBufferException : Exception {
		public ProtocolBufferException(string message) : base(message) {
		}
	}
}

#endregion

#region ProtocolParserVarInt

namespace SilentOrbit.ProtocolBuffers {
	public static partial class ProtocolParser {
		/// <summary>
		/// Reads past a varint for an unknown field.
		/// </summary>
		public static void ReadSkipVarInt(Stream stream) {
			while (true) {
				int b = stream.ReadByte();
				if (b < 0)
					throw new IOException("Stream ended too early");

				if ((b & 0x80) == 0)
					return; //end of varint
			}
		}

		public static byte[] ReadVarIntBytes(Stream stream) {
			byte[] buffer = new byte[10];
			int offset = 0;
			while (true) {
				int b = stream.ReadByte();
				if (b < 0)
					throw new IOException("Stream ended too early");
				buffer[offset] = (byte)b;
				offset += 1;
				if ((b & 0x80) == 0)
					break; //end of varint
				if (offset >= buffer.Length)
					throw new ProtocolBufferException("VarInt too long, more than 10 bytes");
			}
			byte[] ret = new byte[offset];
			Array.Copy(buffer, ret, ret.Length);
			return ret;
		}

		#region VarInt: int32, uint32, sint32

		//		[Obsolete("Use (int)ReadUInt64(stream); //yes 64")]
		/// <summary>
		/// Since the int32 format is inefficient for negative numbers we have avoided to implement it.
		/// The same functionality can be achieved using: (int)ReadUInt64(stream);
		/// </summary>
		public static int ReadInt32(Stream stream) {
			return (int)ReadUInt64(stream);
		}

		//		[Obsolete("Use WriteUInt64(stream, (ulong)val); //yes 64, negative numbers are encoded that way")]
		/// <summary>
		/// Since the int32 format is inefficient for negative numbers we have avoided to imlplement.
		/// The same functionality can be achieved using: WriteUInt64(stream, (uint)val);
		/// Note that 64 must always be used for int32 to generate the ten byte wire format.
		/// </summary>
		public static void WriteInt32(Stream stream, int val) {
			//signed varint is always encoded as 64 but values!
			WriteUInt64(stream, (ulong)val);
		}

		/// <summary>
		/// Zig-zag signed VarInt format
		/// </summary>
		public static int ReadZInt32(Stream stream) {
			uint val = ReadUInt32(stream);
			return (int)(val >> 1) ^ ((int)(val << 31) >> 31);
		}

		/// <summary>
		/// Zig-zag signed VarInt format
		/// </summary>
		public static void WriteZInt32(Stream stream, int val) {
			WriteUInt32(stream, (uint)((val << 1) ^ (val >> 31)));
		}

		/// <summary>
		/// Unsigned VarInt format
		/// Do not use to read int32, use ReadUint64 for that.
		/// </summary>
		public static uint ReadUInt32(Stream stream) {
			int b;
			uint val = 0;

			for (int n = 0; n < 5; n++) {
				b = stream.ReadByte();
				if (b < 0)
					throw new IOException("Stream ended too early");

				//Check that it fits in 32 bits
				if ((n == 4) && (b & 0xF0) != 0)
					throw new ProtocolBufferException("Got larger VarInt than 32bit unsigned");
				//End of check

				if ((b & 0x80) == 0)
					return val | (uint)b << (7 * n);

				val |= (uint)(b & 0x7F) << (7 * n);
			}

			throw new ProtocolBufferException("Got larger VarInt than 32bit unsigned");
		}

		/// <summary>
		/// Unsigned VarInt format
		/// </summary>
		public static void WriteUInt32(Stream stream, uint val) {
			byte b;
			while (true) {
				b = (byte)(val & 0x7F);
				val = val >> 7;
				if (val == 0) {
					stream.WriteByte(b);
					break;
				} else {
					b |= 0x80;
					stream.WriteByte(b);
				}
			}
		}

		#endregion

		#region VarInt: int64, UInt64, SInt64

		//		[Obsolete("Use (long)ReadUInt64(stream); instead")]
		/// <summary>
		/// Since the int64 format is inefficient for negative numbers we have avoided to implement it.
		/// The same functionality can be achieved using: (long)ReadUInt64(stream);
		/// </summary>
		public static int ReadInt64(Stream stream) {
			return (int)ReadUInt64(stream);
		}

		//		[Obsolete("Use WriteUInt64 (stream, (ulong)val); instead")]
		/// <summary>
		/// Since the int64 format is inefficient for negative numbers we have avoided to implement.
		/// The same functionality can be achieved using: WriteUInt64 (stream, (ulong)val);
		/// </summary>
		public static void WriteInt64(Stream stream, int val) {
			WriteUInt64(stream, (ulong)val);
		}

		/// <summary>
		/// Zig-zag signed VarInt format
		/// </summary>
		public static long ReadZInt64(Stream stream) {
			ulong val = ReadUInt64(stream);
			return (long)(val >> 1) ^ ((long)(val << 63) >> 63);
		}

		/// <summary>
		/// Zig-zag signed VarInt format
		/// </summary>
		public static void WriteZInt64(Stream stream, long val) {
			WriteUInt64(stream, (ulong)((val << 1) ^ (val >> 63)));
		}

		/// <summary>
		/// Unsigned VarInt format
		/// </summary>
		public static ulong ReadUInt64(Stream stream) {
			int b;
			ulong val = 0;

			for (int n = 0; n < 10; n++) {
				b = stream.ReadByte();
				if (b < 0)
					throw new IOException("Stream ended too early");

				//Check that it fits in 64 bits
				if ((n == 9) && (b & 0xFE) != 0)
					throw new ProtocolBufferException("Got larger VarInt than 64 bit unsigned");
				//End of check

				if ((b & 0x80) == 0)
					return val | (ulong)b << (7 * n);

				val |= (ulong)(b & 0x7F) << (7 * n);
			}

			throw new ProtocolBufferException("Got larger VarInt than 64 bit unsigned");
		}

		/// <summary>
		/// Unsigned VarInt format
		/// </summary>
		public static void WriteUInt64(Stream stream, ulong val) {
			byte b;
			while (true) {
				b = (byte)(val & 0x7F);
				val = val >> 7;
				if (val == 0) {
					stream.WriteByte(b);
					break;
				} else {
					b |= 0x80;
					stream.WriteByte(b);
				}
			}
		}

		#endregion

		#region Varint: bool

		public static bool ReadBool(Stream stream) {
			int b = stream.ReadByte();
			if (b < 0)
				throw new IOException("Stream ended too early");
			if (b == 1)
				return true;
			if (b == 0)
				return false;
			throw new ProtocolBufferException("Invalid boolean value");
		}

		public static void WriteBool(Stream stream, bool val) {
			stream.WriteByte(val ? (byte)1 : (byte)0);
		}

		#endregion
	}
}
#endregion
