using UnityEngine;

public class FeelingSystem : MonoBehaviour
{
    [System.Serializable]
    public class FeelingValues
    {
        public float currentValue = 0f;
        public float maxValue = 100f;
    }

    [Header("Feeling System")]
    public FeelingValues feelGood = new FeelingValues();
    public FeelingValues feelBad = new FeelingValues();
    public FeelingValues playerArousal = new FeelingValues();

    [Header("Status Influences")]
    public float trustValue = 50f;
    public float sexualInterest = 50f;
    public float mood = 50f;

    // เพิ่มตัวแปรสำหรับการปรับค่า baseGoodIncrease และ baseBadIncrease ใน Inspector
    [Header("Base Increase Values")]
    [SerializeField] private float baseGoodIncrease = 10f;
    [SerializeField] private float baseBadIncrease = 2f;

    // ฟังก์ชันสำหรับการคำนวณค่าความรู้สึก
    public void CalculateFeelings()
    {
        // คำนวณค่าความรู้สึกดี
        // ค่า multiplier จะเพิ่มขึ้นเมื่อ trustValue, sexualInterest, และ mood สูงขึ้น
        float goodMultiplier = (trustValue / 100f) * (sexualInterest / 100f) * (mood / 100f);
        feelGood.currentValue += baseGoodIncrease * goodMultiplier;
        feelGood.currentValue = Mathf.Clamp(feelGood.currentValue, 0f, feelGood.maxValue);

        // คำนวณค่าความรู้สึกแย่
        // ค่า multiplier จะเพิ่มขึ้นเมื่อ trustValue, sexualInterest, และ mood ต่ำลง
        // ใช้สูตร 1 - goodMultiplier เพื่อให้ค่าความรู้สึกแย่สูงขึ้นเมื่อค่าอื่นๆ ต่ำลง
        float badMultiplier = 1f - goodMultiplier;
        feelBad.currentValue += baseBadIncrease * badMultiplier;
        feelBad.currentValue = Mathf.Clamp(feelBad.currentValue, 0f, feelBad.maxValue);

        // ตรวจสอบผลลัพธ์
        CheckFeelingsResult();
    }


    // ตรวจสอบผลลัพธ์ของค่าความรู้สึก
    private void CheckFeelingsResult()
    {
        if (feelGood.currentValue >= feelGood.maxValue)
        {
            Debug.Log("The girl has an orgasm!");
            ResetFeelingValues();
            // ที่นี่คุณ
        }
        else if (feelBad.currentValue >= feelBad.maxValue)
        {
            Debug.Log("The night ends and the day changes!");
            ResetFeelingValues();
            // ที่นี่คุณสามารถเพิ่มการทำงานเพื่อเปลี่ยนวันในเกม
        }
    }

    // รีเซ็ตค่าความรู้สึก
    private void ResetFeelingValues()
    {
        feelGood.currentValue = 0f;
        feelBad.currentValue = 0f;
        playerArousal.currentValue = 0f;
    }

    // ฟังก์ชันสำหรับการเปลี่ยนแปลงค่าสถานะที่มีผลต่อการคำนวณ
    public void UpdateStatus(float trustChange, float sexualInterestChange, float moodChange)
    {
        trustValue = Mathf.Clamp(trustValue + trustChange, 0f, 100f);
        sexualInterest = Mathf.Clamp(sexualInterest + sexualInterestChange, 0f, 100f);
        mood = Mathf.Clamp(mood + moodChange, 0f, 100f);
    }

    // ฟังก์ชันสำหรับการเพิ่มค่าความเสียวของผู้เล่น
    public void IncreasePlayerArousal(float arousalIncrease)
    {
        playerArousal.currentValue += arousalIncrease;
        playerArousal.currentValue = Mathf.Clamp(playerArousal.currentValue, 0f, playerArousal.maxValue);
    }

    // ฟังก์ชันสำหรับการรับค่าปัจจุบันของความรู้สึกดีและแย่
    public float GetFeelGoodValue() => feelGood.currentValue;
    public float GetFeelBadValue() => feelBad.currentValue;
    public float GetPlayerArousalValue() => playerArousal.currentValue;

    // ฟังก์ชันสำหรับการตั้งค่าความรู้สึกดีและแย่ (สำหรับการทดสอบหรือการเซ็ตค่าเริ่มต้น)
    public void SetFeelGoodValue(float value) => feelGood.currentValue = Mathf.Clamp(value, 0f, feelGood.maxValue);
    public void SetFeelBadValue(float value) => feelBad.currentValue = Mathf.Clamp(value, 0f, feelBad.maxValue);
    public void SetPlayerArousalValue(float value) => playerArousal.currentValue = Mathf.Clamp(value, 0f, playerArousal.maxValue);

}
