using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int Energy { get; private set; }
    public int Money { get; private set; }

    public PlayerStats(int energy, int money)
    {
        Energy = energy;
        Money = money;
    }

    public bool UseEnergy(int amount)
    {
        if (Energy >= amount)
        {
            Energy -= amount;
            return true; // ใช้พลังงานสำเร็จ
        }
        return false; // พลังงานไม่พอ
    }

    public void AddMoney(int amount) => Money += amount;
    public void DeductMoney(int amount) => Money = Mathf.Max(0, Money - amount);

    /*วิธีใช้งาน
        PlayerStats playerStats = new PlayerStats(100, 500);
        if (playerStats.UseEnergy(20))
            {
                Debug.Log("พลังงานถูกใช้!");
            }
        else
            {
                Debug.Log("พลังงานไม่พอ!");
            }
    */
}
