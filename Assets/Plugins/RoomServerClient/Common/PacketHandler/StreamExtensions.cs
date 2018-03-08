using System;
using System.IO;

public static class StreamExtensions {

	public static bool TryGetBuffer(this MemoryStream stream, out ArraySegment<byte> buffer) {
		buffer = new ArraySegment<byte>(stream.GetBuffer(), 0, (int)stream.Length);
		return true;
	}


}
