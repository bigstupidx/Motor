// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using System.IO;


public class TcpPacketHandler {
	public delegate void OnPacket(Stream stream);
	private enum PacketState {
		ReadHead, ReadBody,
	}
	private OnPacket _onPacket;
	private Action<Exception> _onError;

	private int _needLenth;
	private int _bodyLength;
	private MemoryStream _readStream = new MemoryStream();
	private MemoryStream _packetSwapStream = new MemoryStream();//用于临时交换的流
	PacketState _packetState;

	private byte[] headBytes = new byte[4];


	public TcpPacketHandler(OnPacket onPacket, Action<Exception> onError) {
		if (onPacket == null) {
			throw new ArgumentNullException("onPacket");
		}
		this._onPacket = onPacket;
		if (onError == null) {
			throw new ArgumentNullException("onError");
		}
		this._onError = onError;
		ResetPacket();
	}

	void ResetPacket() {
		this._needLenth = headBytes.Length;
		this._packetState = PacketState.ReadHead;
	}


	public void OnData(byte[] data, int offset, int length) {
		try {
			this._readStream.Write(data, offset, length);
			while (this._readStream.Length >= this._needLenth) {
				if (this._packetState == PacketState.ReadHead) {
					if (this._readStream.Length >= this._needLenth) {//头部读取完毕
						long originPos = this._readStream.Position;
						this._readStream.Position = 0;
						this._readStream.Read(this.headBytes, 0, this.headBytes.Length);//从头部取出内容长度
						int tmpIndex = 0;
						this._bodyLength = BitConverterNoAlloc.ToInt32(this.headBytes, ref tmpIndex);
						this._needLenth = this._bodyLength + this.headBytes.Length;//头部仍然在流中，所以加上头部的长度
						this._packetState = PacketState.ReadBody;
						this._readStream.Position = originPos;//恢复流位置
					}
				}
				if (this._packetState == PacketState.ReadBody) {
					if (this._readStream.Length >= this._needLenth) {//内容读取完毕
						ArraySegment<byte> buffer;
						if (!this._readStream.TryGetBuffer(out buffer)) {
							throw new Exception("Get Buffer From Stream Error");
						}
						//先将剩余部分写在交换流中
						int remainLength = (int)(this._readStream.Length - this._needLenth);
						this._packetSwapStream.Write(buffer.Array, this._needLenth, remainLength);

						//定位到内容部分并调用回调
						this._readStream.SetLength(this._needLenth);
						this._readStream.Position = this.headBytes.Length;
						this._onPacket(this._readStream);

						//重置包接收流并将剩余部分写入包接收流
						this._readStream.Position = 0;
						this._readStream.SetLength(0);
						this._packetSwapStream.WriteTo(this._readStream);
						//重置交换流
						this._packetSwapStream.Position = 0;
						this._packetSwapStream.SetLength(0);
						ResetPacket();
					}
				}
			}
		} catch (Exception e) {
			this._onError(e);
		}
	}

	private byte[] headBytesForSend = new byte[4];
	private MemoryStream _addHeaderStream = new MemoryStream();
	public ArraySegment<byte> AddPacketHead(byte[] data, int offset, int length) {
		if (data == null) {
			throw new ArgumentNullException("data");
		}
		//获取包长作为包头
		lock (_addHeaderStream) {
			_addHeaderStream.Position = 0;
			_addHeaderStream.SetLength(0);
			lock (headBytesForSend) {
				int tmpIndex = 0;
				BitConverterNoAlloc.Include(length, headBytesForSend, ref tmpIndex);
				_addHeaderStream.Write(headBytesForSend, 0, headBytesForSend.Length);
			}
			_addHeaderStream.Write(data, offset, length);
			ArraySegment<byte> ret;
			this._addHeaderStream.TryGetBuffer(out ret);
			return ret;
		}
	}




}