using UnityEngine;
using UnityEngine.Playables;
using static SceneController;
using UnityEngine.SceneManagement;

public class TimelineController : MonoBehaviour
{
    public GameObject obj_Blur;
    public GameObject obj_Frame;
    public GameObject obj_GoSchool;
    public GameObject obj_GoHome;
    public PlayableDirector pl_GoSchool;
    public PlayableDirector pl_GoHome;

    GameTime gameTime;
    SceneController sceneController;


    DialogTrigger dialogTrigger;

    void Start()
    {
        pl_GoSchool.stopped += OnPlayableDirectorStopped;
        pl_GoHome.stopped += OnPlayableDirectorStopped;

        gameTime = FindFirstObjectByType<GameTime>();
        sceneController = FindFirstObjectByType<SceneController>();
        dialogTrigger = FindFirstObjectByType<DialogTrigger>();
    }

    void Update()
    {
        // ตื่นนอน ทานข้าว
        if (gameTime.GetHourCurrentTime() >= 8.00 && gameTime.GetHourCurrentTime() <= 9.00)
        {
            Debug.Log("ช่วงเย็นหลัง 19.00");
        }
    }

    public void PlayTimelineGoToSchool()
    {
        obj_Frame.SetActive(true); // แสดง Frame ที่ซ่อนไว้
        obj_Blur.SetActive(true); // แสดง Blur ที่ซ่อนไว้
        obj_GoSchool.SetActive(true);
        obj_GoHome.SetActive(false);

        pl_GoSchool.Play(); // ใช้ฟังก์ชันนี้เพื่อสั่งเล่น Timeline
    }

    public void PlayTimelineGoToHome()
    {
        if (pl_GoHome == null)
        {
            Debug.LogError("pl_GoHome is not assigned!");
            return;
        }

        obj_Frame.SetActive(true); // แสดง Frame ที่ซ่อนไว้
        obj_Blur.SetActive(true); // แสดง Blur ที่ซ่อนไว้
        obj_GoHome.SetActive(true);
        obj_GoSchool.SetActive(false);

        pl_GoHome.Play(); // ใช้ฟังก์ชันนี้เพื่อสั่งเล่น Timeline
    }

    void OnPlayableDirectorStopped(PlayableDirector director)
    {
        //obj_Frame.SetActive(false);
        //obj_Blur.SetActive(false);

        if (director == pl_GoSchool)
        {
            dialogTrigger.TriggerDialog("นีน่า", "ถึงมหาลัย", () =>
            {
                gameTime.AddTime(0, 30);
                PlayTimelineGoToHome();
                
                Debug.Log("pl_GoSchool จบการเล่นไดอะล็อก");
            });
        }
        if (director == pl_GoHome)
        {
            gameTime.AddTime(7, 30);

            sceneController.SwitchScene(SceneController.SceneType.OutsideRoom);
            obj_Frame.SetActive(false);
            obj_Blur.SetActive(false); 
            obj_GoHome.SetActive(false);
            obj_GoSchool.SetActive(false);
        }
    }


    void OnDestroy()
    {
        // ลบ Event Listener เมื่อ Object ถูกทำลาย
        pl_GoSchool.stopped -= OnPlayableDirectorStopped;
        pl_GoHome.stopped -= OnPlayableDirectorStopped;
    }

    public void GoHome()
    {
        sceneController.SwitchScene(SceneController.SceneType.MainRoom);
    }

    public void GoStore()
    {
        sceneController.SwitchScene(SceneController.SceneType.StoreRoom);
    }
}
