using System;
using System.Collections.Generic;

namespace Protocol {
	public class Pool<T> {

		private Func<T> _newInsFunc;

		Stack<T> _stack = new Stack<T>();
		List<T> poped = new List<T>();

		public Pool(int initSize, Func<T> newInsFunc) {
			this._newInsFunc = newInsFunc;
			for (int i = 0; i < initSize; i++) {
				this.Push(newInsFunc());
			}
		}

		public T Pop() {
			lock (this._stack) {
				T ret;
				if (this._stack.Count == 0) {
					ret = this._newInsFunc();
				} else {
					ret = this._stack.Pop();
				}
				this.poped.Add(ret);
				return ret;
			}
		}

		public void Push(T obj) {
			lock (this._stack) {
				this._stack.Push(obj);
				if (this.poped.Contains(obj)) {
					this.poped.Remove(obj);
				}
			}
		}

		/// <summary>
		/// 回收所有分配出去的对象,只有当你确定所有对象都不需要时才使用
		/// </summary>
		public void RecycleAll() {
			lock (this._stack) {
				for (var i = 0; i < this.poped.Count; i++) {
					var obj = this.poped[i];
					this._stack.Push(obj);
				}
			}
			this.poped.Clear();
		}

	}
}