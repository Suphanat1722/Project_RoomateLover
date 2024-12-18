using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    public void SaveGame(GameTime gameTime, PlayerStats playerStats)
    {
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
        Debug.Log("Game saved to " + savePath);
    }

    public void LoadGame(GameTime gameTime, PlayerStats playerStats)
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            // โหลดค่ากลับเข้าสู่ระบบ
            gameTime.AddTime(saveData.hour - gameTime.Hour, saveData.minute - gameTime.Minute);
            playerStats.actionPoint = saveData.actionPoint;
            playerStats.sexualPoint = saveData.sexualPoint;
            playerStats.money = saveData.money;

            Debug.Log("Game loaded from " + savePath);
        }
        else
        {
            Debug.LogWarning("Save file not found!");
        }
    }
    private void Update()
    {

        if (Input.GetMouseButton(1)) // คลิ้กซ้ายย้อนกลับ
        {
            NotActiveUiSave();
            NotActiveUiLoad();
        }
    }
    public GameObject uiSave;
    public GameObject uiLoad;
    public void ActiveUiSave()
    {
        uiSave.SetActive(true);
    }
    public void ActiveUiLoad()
    {
        uiLoad.SetActive(true);
    }

    public void NotActiveUiSave()
    {
        uiSave.SetActive(false);
    }
    public void NotActiveUiLoad()
    {
        uiLoad.SetActive(false);
    }

    public void ButtonSave()
    {
        SaveGame(FindAnyObjectByType<GameTime>(), FindAnyObjectByType<PlayerStats>());
        NotActiveUiSave();
    }
    public void ButtonLoad()
    {
        LoadGame(FindAnyObjectByType<GameTime>(), FindAnyObjectByType<PlayerStats>());
        NotActiveUiLoad();
    }
}
