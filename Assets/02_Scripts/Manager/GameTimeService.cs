using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeService : IGameTimeService
{
    private const string last_Time_Key = "Last_Session_Time";
    private const double max_OffLine_Seconds = 24 * 60 * 60; // 24시간 제한

    /// <summary>
    /// 오프라인 시간 계산기
    /// </summary>
    public double GetOfflineSeconds()
    {
        if (!PlayerPrefs.HasKey(last_Time_Key))
            return 0;

        long lastTicks = long.Parse(PlayerPrefs.GetString(last_Time_Key));
        DateTime lastTime = new DateTime(lastTicks, DateTimeKind.Utc);
        DateTime now = DateTime.UtcNow;

        double seconds = (now - lastTime).TotalSeconds;

        if (seconds < 0)
            return 0;

        return Math.Min(seconds, max_OffLine_Seconds);
    }

    /// <summary>
    /// 세이브 시점 저장
    /// </summary>
    public void SaveSessionTime()
    {
        PlayerPrefs.SetString(
            last_Time_Key,
            DateTime.UtcNow.Ticks.ToString()
            );
        PlayerPrefs.Save();
    }
}
