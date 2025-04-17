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

    private void Start()
    {      
        gameTime = FindFirstObjectByType<GameTime>();

        objScensMainRoom.SetActive(true);
        objScensBathRoom.SetActive(false);
        objScensBedRoom.SetActive(false);
        objScensOutsideRoom.SetActive(false);
    }
    /*
     // เปลี่ยน Scene โดยใช้ Index (จาก Build Settings)
     public void LoadSceneByIndex(int sceneIndex)
     {
         SceneManager.LoadScene(sceneIndex);
     }  
    */

    private void Update()
    {
        //ช่วงเย็นหลัง 19.00
        if (gameTime.GetHourCurrentTime() >= 19)
        {
            Debug.Log("ช่วงเย็นหลัง 19.00");
        }
    }
}
