using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("Scrips")]
    [SerializeField] GameTime gameTime;
    [Header("Obj Scens")]
    [SerializeField] GameObject objScensMainRoom;
    [SerializeField] GameObject objScensBathRoom;
    [SerializeField] GameObject objScensBedRoom;
    [SerializeField] GameObject objScensOutsideRoom;
    [SerializeField] GameObject objScensStoreRoom;

    public enum SceneType
    {
        MainRoom,
        BathRoom,
        BedRoom,
        OutsideRoom,
        StoreRoom
    }

    private void Start()
    {      
        gameTime = FindFirstObjectByType<GameTime>();
    }


    // ฟังก์ชันสำหรับเปลี่ยนฉาก
    public void SwitchScene(SceneType scene)
    {
        // ปิดทุกฉากก่อน
        objScensMainRoom.SetActive(false);
        objScensBathRoom.SetActive(false);
        objScensBedRoom.SetActive(false);
        objScensOutsideRoom.SetActive(false);
        objScensStoreRoom.SetActive(false);

        // เปิดฉากที่ต้องการ
        switch (scene)
        {
            case SceneType.MainRoom:
                objScensMainRoom.SetActive(true);
                break;
            case SceneType.BathRoom:
                objScensBathRoom.SetActive(true);
                break;
            case SceneType.BedRoom:
                objScensBedRoom.SetActive(true);
                break;
            case SceneType.OutsideRoom:
                objScensOutsideRoom.SetActive(true);
                break;
            case SceneType.StoreRoom:
                objScensStoreRoom.SetActive(true);
                break;
        }
    }


}
