using System;
using TMPro;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    public int Hour { get; private set; }
    public int Minute { get; private set; }

    public TextMeshProUGUI timeText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddTime(0, 30); // เพิ่มครึ่งชั่วโมง
        }
        UpdateTimeUI();
    }

    public GameTime(int hour, int minute)
    {
        Hour = hour;
        Minute = minute;
    }

    public void AddTime(int hours, int minutes)
    {
        Minute += minutes;
        Hour += hours + Minute / 60;
        Minute %= 60;

        if (Hour >= 24)
        {
            Hour %= 24;
        }
    }

    public int GetHourCurrentTime() => Hour;
    public int GetMinuteCurrentTime() => Minute;

    public void SetTimeCurrentTime(int hour, int minute)
    {
        Hour = hour;
        Minute = minute;
    }

    private void UpdateTimeUI()
    {
        // แสดงเวลาในรูปแบบ 24 ชั่วโมง
        timeText.text = $"{Hour:00}:{Minute:00}";
    }

}
