// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Protocol;
using SilentOrbit.ProtocolBuffers;

namespace ClientEngine {

	public class TcpPacketClient : PacketClient {

		private readonly byte[] _reciveBuffer = new byte[1024 * 4];

		private TcpPacketHandler _packetHandler;

		private Socket _socket;
		private object _closeLock = new object();

		public override bool Connected {
			get { return this._socket != null && this._socket.Connected; }
		}

		public override void Close(CloseCause cause) {
			lock (_closeLock) {
				if (this._socket != null) {
					OnClosed(cause);
				}
			}
		}

		protected override void OnClosed(CloseCause cause) {
			if (this._socket != null) {
				base.OnClosed(cause);
#if COREFX
				this._socket.Dispose();
#else
				if (this._socket.Connected) {
					this._socket.Shutdown(SocketShutdown.Both);
					this._socket.Close(1);
				}
#endif
				lock (this.toSendQueue) {
					this.toSendQueue.Clear();
				}
				this._socket = null;
			}
		}
		private static IPAddress GetIpAddress(string address) {
			IPAddress iPAddress = null;
			if (IPAddress.TryParse(address, out iPAddress)) {
				return iPAddress;
			}
#if COREFX
			var addressList = Dns.GetHostAddressesAsync(address).GetAwaiter().GetResult();
#else
			var addressList = Dns.GetHostEntry(address).AddressList;
#endif
			foreach (IPAddress iPAddress2 in addressList) {
				if (iPAddress2.AddressFamily == AddressFamily.InterNetworkV6) {
					return iPAddress2;
				}
				if (iPAddress2.AddressFamily == AddressFamily.InterNetwork) {
					return iPAddress2;
				}
			}
			throw new SocketException((int)SocketError.HostNotFound);
		}

		public override void Connect(string host, int port) {
			Log.Info("Connect "+host+":"+port);
			new Thread(() => {
				try {
					var ipAddress = GetIpAddress(host);
					this._socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
					IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
					this._socket.Connect(ipEndPoint);
					_packetHandler = new TcpPacketHandler(OnPacket, OnError);
					OnConnected();
					new Thread(ReciveLoop) { Name = "RecieveThread" }.Start();
					new Thread(SendLoop) { Name = "SendThread" }.Start();
				} catch (Exception e) {
					OnConnectFail(e);
				}
			}) {
				Name = "ConnectThread"
			}.Start();
		}

		private void ReciveLoop() {
			while (this.Connected) {
				try {
					int reciveCount = this._socket.Receive(this._reciveBuffer);
					if (reciveCount == 0) {
						Close(CloseCause.ReceiveException);
					} else {
						this._packetHandler.OnData(this._reciveBuffer, 0, reciveCount);
					}
				} catch (Exception e) {
					Close(CloseCause.ReceiveException);
				}
			}
		}

		private Pool<MemoryStream> sendPool = new Pool<MemoryStream>(2, () => new MemoryStream());
		private Queue<MemoryStream> toSendQueue = new Queue<MemoryStream>();
		private void SendLoop() {
			while (this.Connected) {
				while (this.toSendQueue.Count > 0) {
					MemoryStream tosend;
					lock (toSendQueue) {
						tosend = this.toSendQueue.Dequeue();
					}
					ArraySegment<byte> buff;
					tosend.TryGetBuffer(out buff);
					try {
						this._socket.Send(buff.Array, buff.Offset, buff.Count, SocketFlags.None);
					} catch (Exception) {
						Close(CloseCause.SendException);
					}
					this.sendPool.Push(tosend);
				}
#if COREFX
				SpinWait.SpinUntil(() => false, 1);
#else
				//				Thread.SpinWait(1);
				Thread.Sleep(1);//spin似乎在低端机上开销大
#endif
			}
		}

		public override void Send(byte[] data, int offset, int count) {
			if (!Connected) {
				Log.Error("Not connected");
				this.Close(CloseCause.SendException);
			}
			var packet = this._packetHandler.AddPacketHead(data, offset, count);
			MemoryStream toSend = this.sendPool.Pop();
			toSend.Clean();
			toSend.Write(packet.Array, packet.Offset, packet.Count);
			lock (toSendQueue) {
				this.toSendQueue.Enqueue(toSend);
			}
		}

	}
}