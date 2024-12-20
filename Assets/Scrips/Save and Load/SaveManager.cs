using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public Button[] buttonsSaveSlot;
    public Button[] buttonsLoadSlot;

    private GameTime gameTime;
    private PlayerStats playerStats;

    private void Awake()
    {
        for (int i = 0; i < buttonsSaveSlot.Length; i++)
        {
            int capturedSlotID = i; // Capture i by value
            buttonsSaveSlot[i].onClick.AddListener(() => OnSaveSlotClicked(capturedSlotID));
        }

        for (int i = 0; i < buttonsLoadSlot.Length; i++)
        {
            int capturedSlotID = i; // Capture i by value
            buttonsLoadSlot[i].onClick.AddListener(() => OnLoadedSlotClicked(capturedSlotID));
        }
    }

    private void Start()
    {
        gameTime = FindAnyObjectByType<GameTime>();
        playerStats = FindAnyObjectByType<PlayerStats>();
    }

    private string GetSavePath(int slotID)
    {
        return Path.Combine(Application.persistentDataPath, $"saveData_slot_{slotID}.json");
    }

    public void SaveGame(GameTime gameTime, PlayerStats playerStats, int slotID)
    {
        string savePath = GetSavePath(slotID);

        SaveData saveData = new SaveData
        {
            hour = gameTime.Hour,
            minute = gameTime.Minute,
            actionPoint = playerStats.actionPoint,
            sexualPoint = playerStats.sexualPoint,
            money = playerStats.money
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Game saved to {savePath}");
    }

    public void LoadGame(GameTime gameTime, PlayerStats playerStats, int slotID)
    {
        string savePath = GetSavePath(slotID);

        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            // โหลดค่ากลับเข้าสู่ระบบ
            gameTime.AddTime(saveData.hour - gameTime.Hour, saveData.minute - gameTime.Minute);
            playerStats.SetActionPoint(saveData.actionPoint);
            playerStats.SetSexualPoint(saveData.sexualPoint);
            playerStats.SetMoney(saveData.money);
        }
        else
        {
            Debug.LogWarning($"Save file not found at Slot {slotID}!");
        }
    }

    // จัดการการคลิกปุ่ม Save Slot
    public void OnSaveSlotClicked(int slotID)
    {
        SaveGame(gameTime, playerStats, slotID);
        DeactivateUiSave();
        Debug.Log($"Saved to Slot {slotID}");
    }

    public void OnLoadedSlotClicked(int slotID)
    {
        LoadGame(gameTime, playerStats, slotID);
        DeactivateUiLoad();
        Debug.Log($"Loaded to Slot {slotID}");
    }

    // UI Management
    public GameObject uiSave;
    public GameObject uiLoad;

    public void ActivateUiSave() => uiSave.SetActive(true);
    public void ActivateUiLoad() => uiLoad.SetActive(true);
    public void DeactivateUiSave() => uiSave.SetActive(false);
    public void DeactivateUiLoad() => uiLoad.SetActive(false);

    private void Update()
    {
        if (Input.GetMouseButton(1)) // คลิกขวาเพื่อปิด UI Save/Load
        {
            DeactivateUiSave();
            DeactivateUiLoad();
        }
    }
}
