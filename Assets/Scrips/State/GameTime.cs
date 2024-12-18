using System;
using TMPro;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    public int Hour { get; private set; }
    public int Minute { get; private set; }

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

    /* วิธีเรียกใช้
        GameTime gameTime = new GameTime(8, 0, 1, 1, 2024);
        gameTime.AddTime(0, 30); // เพิ่มครึ่งชั่วโมง
        Debug.Log($"Time: {gameTime.Hour}:{gameTime.Minute}, Day: {gameTime.Day}");
    */


    public TextMeshProUGUI timeText;
    public TextMeshProUGUI energyText;

    private void Start()
    {
        // ตั้งค่าเวลาเริ่มต้น
        Hour = 8;
        Minute = 0;

        // อัปเดต UI เริ่มต้น
        UpdateTimeUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddTime(0, 30); // เพิ่มครึ่งชั่วโมง
            UpdateTimeUI(); // อัปเดตเวลาใน UI
        }


    }

    private void UpdateTimeUI()
    {
        // แปลงเวลาเป็นระบบ 12 ชั่วโมง
        string period = Hour >= 12 ? "pm" : "am";
        int displayHour = Hour % 12;
        if (displayHour == 0) displayHour = 12; // แก้ไข 0 ให้เป็น 12

        // อัปเดตข้อความแสดงผล
        timeText.text = $"{displayHour:00}:{Minute:00} {period}";
    }

}
