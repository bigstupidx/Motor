using System;
using System.Collections.Generic;
using CommandModel;
using UnityEngine;

#if COREFX
using System.Threading.Tasks;
#endif

namespace ClientEngine {
	public class RpcClient : CommandClient, IRpcSession {
		public Dictionary<string, RpcRequest> Rpcs = new Dictionary<string, RpcRequest>();

		public RpcClient(CommandExecuteMode commandExecuteMode) : base(commandExecuteMode) {
		}

		protected virtual string GetNewRpcId() {
			return Guid.NewGuid().ToString();
		}

#if COREFX
		public async Task<ICommand> RpcAsync(RpcRequest request, float timeOut = -1) {
			request.Id = GetNewRpcId();
			Rpcs.Add(request.Id, request);
			Send(request);
#pragma warning disable 4014
			if (timeOut > 0) {
				Task.Run(async () => {//处理超时
					await Task.Delay((int)(timeOut * 1000));
					lock (request) {
						if (!request.Tcs.Task.IsCompleted) {
							request.Error = RpcError.Timeout;
							this.RemoveRpcRequest(request.Id);
							request.Tcs.SetResult(null);
						}
					}
				});
			}
#pragma warning restore 4014
			return await request.Tcs.Task;
		}
#else
		public void RpcAsync(RpcRequest request, Action<ICommand> onDone, float timeOut = -1) {
			request.Id = GetNewRpcId();
			request.OnDone = onDone;
			Rpcs.Add(request.Id, request);
			Send(request);
			if (timeOut > 0) {
				throw new NotImplementedException("Time Out Not Support Yet");
			}
		}
#endif

		public RpcRequest GetRpcRequest(string id) {
			if (!this.Rpcs.ContainsKey(id)) {
				Debug.LogError(id + "  rpc not found");//TODO only for test
			}
			return this.Rpcs[id];
		}

		public void RemoveRpcRequest(string id) {
			this.Rpcs.Remove(id);
		}

	}
}
