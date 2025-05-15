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
        if (GetHourCurrentTime() < 0 )
        {
            
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

    public void AddTimeUntilMidnight(int hoursToAdd, int minutesToAdd)
    {
        int currentTotalMinutes = Hour * 60 + Minute;
        int addedTotalMinutes = hoursToAdd * 60 + minutesToAdd;

        int minutesUntilMidnight = (24 * 60) - currentTotalMinutes;

        // ถ้าเวลาที่จะบวกเกินเที่ยงคืน ให้ตัดเหลือแค่เวลาที่เหลือก่อนเที่ยงคืน
        int finalAddedMinutes = Mathf.Min(addedTotalMinutes, minutesUntilMidnight);

        AddTime(0, finalAddedMinutes); // ใช้ AddTime เดิมเพราะมันจัดการ carry ชั่วโมงกับนาทีให้แล้ว
    }

    private void UpdateTimeUI()
    {
        // แสดงเวลาในรูปแบบ 24 ชั่วโมง
        timeText.text = $"{Hour:00}:{Minute:00}";
    }

}
