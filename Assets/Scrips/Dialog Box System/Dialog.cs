using UnityEngine;

[System.Serializable]
public class Dialog
{
    public string characterName; // ชื่อของตัวละครที่พูด
    [TextArea(3, 10)] // ปรับขนาดกล่องข้อความใน Inspector
    public string[] sentences; // ประโยคที่ตัวละครจะพูด
}
