using UnityEngine;

public class FeelingSystem : MonoBehaviour
{
    [System.Serializable]
    public class FeelingValues
    {
        public float currentValue = 0f;
        public float maxValue = 100f;
    }

    [Header("Feeling Girl System")]
    public FeelingValues feelGood = new FeelingValues();
    public FeelingValues feelBad = new FeelingValues();

    [Header("Feeling Player System")]
    public FeelingValues playerArousal = new FeelingValues();

    [Header("Status Girl")]
    public float trustValue = 50f;
    public float sexualInterest = 50f;
    public float mood = 50f;

    [Header("Player Stats")]
    public float playerEnergy = 50f; // ค่าพลังงานเริ่มต้นที่ 50
    public int playerSexEnergy = 1; // ค่าพลังงานด้าน sex เริ่มต้นที่ 1
    public int playerMoney = 1500;

    // ฟังก์ชันสำหรับการคำนวณค่าความรู้สึก
    public void CalculateFeelings(float baseGoodIncrease, float baseBadIncrease)
    {
        // คำนวณค่าความรู้สึกดี
        float goodMultiplier = (trustValue / 100f) * (sexualInterest / 100f) * (mood / 100f);
        feelGood.currentValue += ((baseGoodIncrease * goodMultiplier) / 10) * Time.deltaTime;
        feelGood.currentValue = Mathf.Clamp(feelGood.currentValue, 0f, feelGood.maxValue);

        // คำนวณค่าความรู้สึกแย่
        float badMultiplier = 1f - goodMultiplier;
        feelBad.currentValue += ((baseGoodIncrease * badMultiplier) / 10) * Time.deltaTime;
        feelBad.currentValue = Mathf.Clamp(feelBad.currentValue, 0f, feelBad.maxValue);
    }

    public void CalculatePlayerArousal(float basePlayerArousal)
    {
        playerArousal.currentValue += (basePlayerArousal / 10)* Time.deltaTime;
        playerArousal.currentValue = Mathf.Clamp(playerArousal.currentValue, 0f, playerArousal.maxValue);
    }

    // รีเซ็ตค่าความรู้สึก
    public void ResetFeelingGirlValues()
    {
        feelGood.currentValue = 0f;
        feelBad.currentValue = 0f;
    }
    public void ResetFeelingPlayerValues()
    {
        playerArousal.currentValue = 0f;
    }

    // ฟังก์ชันสำหรับการเปลี่ยนแปลงค่าสถานะที่มีผลต่อการคำนวณ
    public void UpdateStatus(float trustChange, float sexualInterestChange, float moodChange)
    {
        trustValue = Mathf.Clamp(trustValue + trustChange, 0f, 100f);
        sexualInterest = Mathf.Clamp(sexualInterest + sexualInterestChange, 0f, 100f);
        mood = Mathf.Clamp(mood + moodChange, 0f, 100f);
    }

    // ฟังก์ชันสำหรับการเพิ่มลดค่าสถานะต่างๆ
    public void AddPlayerSexEnergy(int value) => playerSexEnergy = Mathf.Max(playerSexEnergy + value, 0);
    public void SubtractPlayerSexEnergy(int value) => playerSexEnergy = Mathf.Max(playerSexEnergy - value, 0);
    public void AddPlayerEnergy(float value) => playerEnergy = Mathf.Max(playerEnergy + value, 0f);
    public void SubtractPlayerEnergy(float value) => playerEnergy = Mathf.Max(playerEnergy - value, 0f);
    public void AddPlayerMoney(int value) => playerMoney = Mathf.Max(playerMoney + value, 0);
    public void SubtractPlayerMoney(int value) => playerMoney = Mathf.Max(playerMoney - value, 0);
    // เพิ่มฟังก์ชันสำหรับ trustValue, sexualInterest, และ mood
    public void AddTrustValue(float value) => trustValue = Mathf.Clamp(trustValue + value, 0f, 100f);
    public void SubtractTrustValue(float value) => trustValue = Mathf.Clamp(trustValue - value, 0f, 100f);
    public void AddSexualInterest(float value) => sexualInterest = Mathf.Clamp(sexualInterest + value, 0f, 100f);
    public void SubtractSexualInterest(float value) => sexualInterest = Mathf.Clamp(sexualInterest - value, 0f, 100f);
    public void AddMood(float value) => mood = Mathf.Clamp(mood + value, 0f, 100f);
    public void SubtractMood(float value) => mood = Mathf.Clamp(mood - value, 0f, 100f);

    // ฟังก์ชันสำหรับการรับค่าปัจจุบันของความรู้สึกดีและแย่
    public float GetFeelGoodValue() => feelGood.currentValue;
    public float GetFeelBadValue() => feelBad.currentValue;
    public float GetPlayerEnergy() => playerEnergy;
    public int GetPlayerSexEnergy() => playerSexEnergy;
    public float GetPlayerArousalValue() => playerArousal.currentValue;
    public float GetPlayerMoney() => playerMoney;

    // ฟังก์ชันสำหรับการตั้งค่าความรู้สึกดีและแย่ (สำหรับการทดสอบหรือการเซ็ตค่าเริ่มต้น)
    public void SetFeelGoodValue(float value) => feelGood.currentValue = Mathf.Clamp(value, 0f, feelGood.maxValue);
    public void SetFeelBadValue(float value) => feelBad.currentValue = Mathf.Clamp(value, 0f, feelBad.maxValue);
    public void SetPlayerEnergy(float value) => playerEnergy = Mathf.Clamp(value, 0f, 100f);
    public void SetPlayerSexEnergy(int value) => playerSexEnergy = Mathf.Max(value, 0);
    public void SetPlayerArousalValue(float value) => playerArousal.currentValue = Mathf.Clamp(value, 0f, playerArousal.maxValue);
    public float SetPlayerMoney(int value) => playerMoney = Mathf.Max(value, 0);
}
