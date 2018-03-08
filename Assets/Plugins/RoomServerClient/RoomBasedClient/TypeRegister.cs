using System;
using System.IO;
using RoomServerModel;
using SilentOrbit.ProtocolBuffers;
using Protocol;
using UnityEngine;

namespace RoomBasedClient {
	public static class TypeRegister {

		enum SerilizeType {

			HostPort = 50,
			LobbyRoom = 51,
			GameRoom = 52,
			Player = 53,

			Vector3 = 60,
			Vector2 = 61,
			Quaternion = 62,
		}


		[RuntimeInitializeOnLoadMethod]
		static void Register() {
			SerializableTypeRegister.RegisterType(typeof(Vector3), (byte)SerilizeType.Vector3, Vector3SerializeFunction, Vector3DeserializeFunction);
			SerializableTypeRegister.RegisterType(typeof(Vector2), (byte)SerilizeType.Vector2, Vector2SerializeFunction, Vector2DeserializeFunction);
			SerializableTypeRegister.RegisterType(typeof(Quaternion), (byte)SerilizeType.Quaternion, QuaternionSerializeFunction, QuaternionDeserializeFunction);

			SerializableTypeRegister.RegisterType(typeof(HostPort), (byte)SerilizeType.HostPort, (stream, customObject) => {
				HostPort obj = (HostPort)customObject;
				obj.Serilize(stream);
			}, stream => {
				HostPort ret = new HostPort();
				ret.Deserilize(stream);
				return ret;
			});


			SerializableTypeRegister.RegisterType(typeof(LobbyRoom), (int)SerilizeType.LobbyRoom, (stream, customObject) => {
				LobbyRoom obj = (LobbyRoom)customObject;
				obj.Serilize(stream);
			}, stream => {
				LobbyRoom ret = new LobbyRoom();
				ret.Deserilize(stream);
				return ret;
			});


			SerializableTypeRegister.RegisterType(typeof(GameRoom), (int)SerilizeType.GameRoom, (stream, customObject) => {
				GameRoom obj = (GameRoom)customObject;
				obj.Serilize(stream);
			}, stream => {
				GameRoom ret = new GameRoom();
				ret.Deserilize(stream);
				return ret;
			});

			SerializableTypeRegister.RegisterType(typeof(RoomPlayer), (int)SerilizeType.Player, (stream, customObject) => {
				RoomPlayer obj = (RoomPlayer)customObject;
				obj.Serilize(stream);
			}, stream => {
				RoomPlayer ret = new RoomPlayer();
				ret.Deserilize(stream);
				return ret;
			});
		}

		private static object Vector3DeserializeFunction(Stream stream) {
			Vector3 obj = new Vector3();
			obj.x = ProtocolParser.ReadFloat(stream);
			obj.y = ProtocolParser.ReadFloat(stream);
			obj.z = ProtocolParser.ReadFloat(stream);
			return obj;
		}

		private static void Vector3SerializeFunction(Stream stream, object customObject) {
			Vector3 obj = (Vector3)customObject;
			ProtocolParser.WriteFloat(stream, obj.x);
			ProtocolParser.WriteFloat(stream, obj.y);
			ProtocolParser.WriteFloat(stream, obj.z);
		}

		private static object Vector2DeserializeFunction(Stream stream) {
			Vector2 obj = new Vector2();
			obj.x = ProtocolParser.ReadFloat(stream);
			obj.y = ProtocolParser.ReadFloat(stream);
			return obj;
		}

		private static void Vector2SerializeFunction(Stream stream, object customObject) {
			Vector2 obj = (Vector2)customObject;
			ProtocolParser.WriteFloat(stream, obj.x);
			ProtocolParser.WriteFloat(stream, obj.y);
		}

		private static object QuaternionDeserializeFunction(Stream stream) {
			Quaternion obj = new Quaternion();
			obj.x = ProtocolParser.ReadFloat(stream);
			obj.y = ProtocolParser.ReadFloat(stream);
			obj.z = ProtocolParser.ReadFloat(stream);
			obj.w = ProtocolParser.ReadFloat(stream);
			return obj;
		}

		private static void QuaternionSerializeFunction(Stream stream, object customObject) {
			Quaternion obj = (Quaternion)customObject;
			ProtocolParser.WriteFloat(stream, obj.x);
			ProtocolParser.WriteFloat(stream, obj.y);
			ProtocolParser.WriteFloat(stream, obj.z);
			ProtocolParser.WriteFloat(stream, obj.w);
		}
	}
}