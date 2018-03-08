using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

namespace OrbbecGestures
{

	public class OGList<T>
	{
		public int ItemNum
		{
			get
			{
				return _ItemNum;
			}
		}

		public int MaxSize
		{
			get
			{
				if (_Items == null)
					return 0;

				return _Items.Length;
			}
		}

		public OGList()
		{

		}

		public OGList(int Size)
		{

		}

		public IntPtr GetItemHeadPtr()
		{
			if (_Items == null)
				return IntPtr.Zero;
			return Marshal.UnsafeAddrOfPinnedArrayElement( _Items, 0);
		}

		public void Resize(int Size)
		{
			if (_Items != null && _Items.Length >= Size)
			{
				_ItemNum = Size;
				return;
			}

			T[] newItems = new T[Size];
			if (_Items != null)
				Array.Copy(_Items, newItems, _Items.Length);
			_Items = newItems;
			_ItemNum = Size;
		}

		public T this[int index]
		{ 
			get 
			{
				if (index >= ItemNum)
				{
					Log.Print(Log.Level.Error, string.Format("OGList Error! Index:{0} is out of range.",index));
					return default(T);
				}
				return _Items[index];
			} 

			set 
			{
				if (index >= ItemNum)
				{
					Log.Print(Log.Level.Error, string.Format("OGList Error! Index:{0} is out of range.", index));
					return;
				}

				_Items[index] = value;
			}
		}

		T[] _Items = null;
		int _ItemNum = 0;
	}
}
