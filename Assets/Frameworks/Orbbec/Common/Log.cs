//#define _NO_LOG_

#define _MUL_THREAD_

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if _MUL_THREAD_
using System.Threading; 
#endif

public class Log
{
	public enum LogType
	{
		ToLog,
		ToBuffer,
	}

	public enum Level
	{
		Log,
		Warning,
		Error,
	}

	public static int MaxBufferLogNum = 20;
	public static LinkedList<string> LogBufferList = null;
	public static bool HasNewLogInBuffer = false;

	public static LogType msType = LogType.ToLog;

#if _MUL_THREAD_
	static System.Object LockObject = new System.Object();
#endif

	public static void PrintToLog( Level eFlag, string Info)
	{
#if _NO_LOG_
		return;
#endif

#if _MUL_THREAD_
		
		lock(LockObject)
		{
#endif
		switch (eFlag)
		{
			case Level.Error:
				Debug.LogError(Info);
				break;
			case Level.Warning:
				Debug.LogWarning(Info);
				break;
			case Level.Log:
				Debug.Log(Info);
				break;
		}

#if _MUL_THREAD_
		}
#endif
	}

	public static string GetBufferLogTotal()
	{
		if (LogBufferList == null)
			return "";

		System.Text.StringBuilder builder = new System.Text.StringBuilder(4096);

		LinkedListNode<string> node = LogBufferList.First;
		while (node != null)
		{
			builder.Append(node.Value);
			node = node.Next;
		}

		HasNewLogInBuffer = false;
		return builder.ToString(0, builder.Length);
	}

	public static void PrintToBuffer( Level eFlag, string Info)
	{
#if _NO_LOG_
		return;
#endif

		if (LogBufferList == null)
			return;

#if _MUL_THREAD_

		lock (LockObject)
		{
#endif
			string NewLog = "";
			switch (eFlag)
			{
				case Level.Error:
					NewLog += "Error:";
					break;
				case Level.Warning:
					NewLog += "Warning:";
					break;
				case Level.Log:
					NewLog += "Info:";
					break;
			}

			NewLog += Info + "\n";

			LogBufferList.AddLast(NewLog);
			HasNewLogInBuffer = true;

			if (LogBufferList.Count > MaxBufferLogNum)
			{
				LogBufferList.RemoveFirst();
			}
#if _MUL_THREAD_
		}
#endif
	}

	public static void Print( Level eFlag, string Info)
	{
		switch(msType)
		{
			case LogType.ToLog:
				PrintToLog(eFlag, Info);
				break;
			case LogType.ToBuffer:
				PrintToBuffer(eFlag, Info);
				PrintToLog(eFlag, Info);
				break;
		}
		
	}

	public static void Print(Level eFlag, string format, params object[] args)
	{
		string LogInfo = string.Format(format, args);
		Print(eFlag, LogInfo);
	}
}