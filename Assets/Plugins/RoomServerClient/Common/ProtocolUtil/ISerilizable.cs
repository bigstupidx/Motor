using System.IO;

namespace SilentOrbit.ProtocolBuffers {
	public interface ISerilizable {
		void Serilize(Stream stream);

		void Deserilize(Stream stream);

	}
}