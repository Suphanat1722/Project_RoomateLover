using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int actionPoint { get; private set; }
    public int sexualPoint { get; private set; }
    public int money { get; private set; }


    public PlayerStats(int energy, int sexual, int money)
    {
        actionPoint = energy;
        sexualPoint = sexual;
        this.money = money;
    }

    public bool UseActionPoint(int amount)
    {
        if (actionPoint >= amount)
        {
            actionPoint -= amount;
            UpdateStateUI();
            return true; // ใช้พลังงานสำเร็จ
        }
        UpdateStateUI();
        return false; // พลังงานไม่พอ
    }

    public bool UseSexualPoint(int amount)
    {
        if (sexualPoint >= amount)
        {
            sexualPoint -= amount;
            UpdateStateUI();
            return true; // ใช้พลังงานสำเร็จ
        }
        UpdateStateUI();
        return false; // พลังงานไม่พอ
    }

    public void AddMoney(int amount) => money += amount;
    public void DeductMoney(int amount) => money = Mathf.Max(0, money - amount);

    // ฟังก์ชันสำหรับตั้งค่าคุณสมบัติ
    public void SetActionPoint(int value) => actionPoint = value;
    public void SetSexualPoint(int value) => sexualPoint = value;
    public void SetMoney(int value) => money = value;

    public TextMeshProUGUI actionPointText;
    public TextMeshProUGUI sexualPointText;
    public TextMeshProUGUI moneyText;

    private void Start()
    {
        actionPoint = 50;
        sexualPoint = 2;
        money = 5000;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            AddMoney(500);
            UseActionPoint(10);
            UseSexualPoint(1);
        }
        
    }

    public void UpdateStateUI()
    {
        actionPointText.text = $"{actionPoint}/50";
        sexualPointText.text = $"{sexualPoint}/2";
        moneyText.text = $"{money}";
    }

}
