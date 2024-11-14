using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Date
{
    private const string AFTER_NOON = "오후";
    private const string AFTER_NOON_ANDROID = "PM";
    public const int HALF_MAX_HOUR = 12;
    private const string DATE_TIME_FORMAT = "dddd, MMMM d, yyyy kl. hh:mm:ss tt";
    private static readonly CultureInfo DEFAULT_CULTURE_INFO = CultureInfo.InvariantCulture;
    private static readonly Dictionary<string, string> MonthStrToNumDict = new Dictionary<string, string>()
    {
        { "January", "01" }, { "February", "02" }, { "March", "03" }, { "April", "04" }, { "May", "05" }, { "June", "06" },
        { "July", "07" }, { "August", "08" }, { "September", "09" }, { "October", "10" }, { "November", "11" }, { "December", "12" },
    };

    public static string GetDateTime()
    {
        return DateTime.Now.ToString(DATE_TIME_FORMAT, DEFAULT_CULTURE_INFO);
    }

    public static string GetMonthNum(string monthStr)
    {
        if (MonthStrToNumDict.ContainsKey(monthStr))
        {
            return MonthStrToNumDict[monthStr];
        }

        Debug.LogError("ParsingFailed");
        return "";
    }

    public static string[] GetTimeSplit(string currentTime)
    {
        string[] temp = currentTime.Split();
        string[] date = temp[temp.Length - 2].Split('-', '/', ':');
        if (temp[temp.Length - 1] == AFTER_NOON_ANDROID || temp[temp.Length - 1] == AFTER_NOON)
        {
            if (date[0] != HALF_MAX_HOUR.ToString())
            {
                date[0] = (int.Parse(date[0]) + Date.HALF_MAX_HOUR).ToString();
            }
        }
        return date;
    }

    public static string[] GetTimeSplit()
    {
        string currentTime = GetDateTime();
        return GetTimeSplit(currentTime);
    }

    public static string[] GetDaySplit(string currentTime)
    {
        string[] temp = currentTime.Split();
        string day = GetMonthNum(temp[1]);
        string[] date = new string[] { temp[3], day, temp[2].Split(',')[0] };
        return date;
    }

    public static string[] GetDaySplit()
    {
        string currentTime = GetDateTime();
        return GetDaySplit(currentTime);
    }
}