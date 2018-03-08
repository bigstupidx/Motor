//
// DateTimeExt.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using UnityEngine;
using System.Collections;

public static class DateTimeExt {
	// 以中国时区（+8）为基准
	private static DateTime BaseDataTime = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);

	/// <summary>
	/// 将Unix时间戳格式转换为C# DateTime时间格式
	/// </summary>
	/// <param name="timeStamp"></param>
	/// <returns></returns>
	public static DateTime ToDateTime (this long timeStamp) {
		return BaseDataTime.AddSeconds (timeStamp);
	}

	/// <summary>
	/// 获取从这个时间戳到现在经过了多少秒<para/>
	/// 当时间发生在之前，返回正数，时间发生在之后，会返回负数
	/// </summary>
	/// <param name="timeStamp"></param>
	/// <returns></returns>
//	public static long GetTimeSpan (this long timeStamp) {
//		return GameClient.Client.System.NowTimeStamp - timeStamp;
//	}


	/// <summary>
	/// 将C# DateTime时间格式转换为Unix时间戳格式
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	public static long ToTimeStamp (this DateTime dateTime) {
		return (long)(dateTime - BaseDataTime).TotalSeconds;
	}

	/// <summary>
	/// 获取星期几，返回1~7
	/// </summary>
	/// <param name="dateTime"></param>
	/// <returns></returns>
	public static int GetDayOfWeek (this DateTime dateTime) {
		DayOfWeek day = dateTime.DayOfWeek;
		return day == DayOfWeek.Sunday ? (int)DayOfWeek.Saturday + 1 : (int)day;
	}


    /// <summary>
    /// 获取日期的零点
    /// </summary>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static long GetDayStart(this long timestamp)
    {
        DateTime dateTime = timestamp.ToDateTime();
        DateTime newDateTime = new DateTime(dateTime.Year,dateTime.Month,dateTime.Day);
        return newDateTime.ToTimeStamp();
    }
}
