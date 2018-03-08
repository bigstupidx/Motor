using System;
using System.Collections.Generic;
using UnityEditor;
using System.Runtime.InteropServices;
using Orbbec;

namespace Assets.Tools.Editor
{
	static class ConsoleController
	{
#if UNITY_EDITOR_64
		private const string dllName = "OrbbecNative64";
		private const string gestureDllName = "GestureNative64";
#else
		private const string dllName = "OrbbecNative";
		private const string gestureDllName = "GestureNative";
#endif

#if UNITY_EDITOR
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void InitConsole();

		[DllImport(gestureDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetGestureLogOutPut();
#endif

		[MenuItem("ConsoleController/OpenConsole")]
		static void OpenConsole()
		{
			InitConsole();
			SetGestureLogOutPut();
		}
	}
}
