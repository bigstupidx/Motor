
using System;
using System.Text;

public class CommonUtil {

	public static string GetFormatTime(float time)
	{
		//		var  dt = new DateTime((long)(time * 10000000) , DateTimeKind.Utc);
		//		return dt.ToString("mm:ss:fff");

		//		var sb = new StringBuilder();
		//		var min = (int) time/60;
		//		var sec = (int) time%60;
		//		var msec = (int)((time - (int)time) * 1000);
		//		if (min < 10) sb.Append("0");
		//		sb.Append(min);
		//		sb.Append(":");
		//		if (sec < 10) sb.Append("0");
		//		sb.Append(sec);
		//		sb.Append(".");
		//		if (msec < 10)
		//			sb.Append("00");
		//		else if (msec < 100)
		//			sb.Append("0");
		//		sb.Append(msec);
		//		return sb.ToString();

		var min = ((int) time/60).ToString("d2");
		var sec = ((int) time%60).ToString("d2");
		var msec = ((int) ((time - (int) time)*1000)).ToString("d3");
		return min + ":" + sec + "." + msec;

		//				string min = (int) time/60 + "";
		//				string sec = (int) time%60 + "";
		//				string msec = (int) ((time - (int) time)*1000) + "";
		//				if (msec.Length < 2)
		//				{
		//					msec = "00" + msec;
		//				}
		//				else if (msec.Length < 3)
		//				{
		//					msec = "0" + msec;
		//				}
		//				if (min.Length < 2)
		//				{
		//					min = "0" + min;
		//				}
		//				if (sec.Length < 2)
		//				{
		//					sec = "0" + sec;
		//				}
		//		
		//				return min + ":" + sec + "." + msec;
	}

	public static string GetFormatTimeMinSec(float time) {
		string min = (int)time / 60 + "";
		string sec = (int)time % 60 + "";
		if (min.Length < 2) {
			min = "0" + min;
		}
		if (sec.Length < 2) {
			sec = "0" + sec;
		}
		return min + ":" + sec;
	}

	public static string GetFormatTimeDHMS(long timeStamp)
	{
		string s = "";
		var oneDay = 60*60*24;
		var oneHour = 60*60;
		var oneMin = 60;
		int day = (int)(timeStamp/oneDay);
		int hour = (int)((timeStamp - day*oneDay)/ oneHour);
		int min = (int)((timeStamp - day*oneDay - hour*oneHour)/oneMin);
		int sec = (int)(timeStamp - day*oneDay - hour*oneHour-min*oneMin);

		s = day + (LString.COMMONUTIL_GETFORMATTIMEDHMS).ToLocalized() + hour + (LString.COMMONUTIL_GETFORMATTIMEDHMS_1).ToLocalized() + min + (LString.COMMONUTIL_GETFORMATTIMEDHMS_2).ToLocalized() + sec + (LString.COMMONUTIL_GETFORMATTIMEDHMS_3).ToLocalized();
		return s;
	}
}
