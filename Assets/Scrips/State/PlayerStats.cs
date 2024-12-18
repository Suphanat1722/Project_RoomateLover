using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int actionPoint { get; private set; }
    public int money { get; private set; }
    public int sexualPoint { get; private set; }

    public PlayerStats(int energy,int sexual, int money)
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
            return true; // ใช้พลังงานสำเร็จ
        }
        return false; // พลังงานไม่พอ
    }

    public bool UseSexualPoint(int amount)
    {
        if (sexualPoint >= amount)
        {
            sexualPoint -= amount;
            return true; // ใช้พลังงานสำเร็จ
        }
        return false; // พลังงานไม่พอ
    }

    public void AddMoney(int amount) => money += amount;
    public void DeductMoney(int amount) => money = Mathf.Max(0, money - amount);

    public TextMeshProUGUI actionPointText;
    public TextMeshProUGUI sexualPointText;
    public TextMeshProUGUI moneyText;

    private void Start()
    {
        actionPoint = 25;
        sexualPoint = 3;
        money = 3000;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            AddMoney(500);
            UseActionPoint(10);
            UseSexualPoint(1);
        }
        UpdateStateUI();
    }

    void UpdateStateUI()
    {
        actionPointText.text = $"{actionPoint}";
        sexualPointText.text = $"{sexualPoint}";
        moneyText.text = $"{money}";
    }
    
}
