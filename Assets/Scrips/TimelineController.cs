using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public GameObject obj_Blur;
    public GameObject obj_Frame;
    public PlayableDirector pl_GoSchool;

    void Start()
    {
        pl_GoSchool.stopped += OnPlayableDirectorStopped;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // ถ้ากด Spacebar ให้เริ่มเล่น Timeline
        {
            PlayTimelineGoToSchool();
        }
    }

    public void PlayTimelineGoToSchool()
    {
        obj_Frame.SetActive(true); // แสดง Frame ที่ซ่อนไว้
        obj_Blur.SetActive(true); // แสดง Blur ที่ซ่อนไว้
        pl_GoSchool.Play(); // ใช้ฟังก์ชันนี้เพื่อสั่งเล่น Timeline
    }

    void OnPlayableDirectorStopped(PlayableDirector director)
    {
        obj_Frame.SetActive(false);
        obj_Blur.SetActive(false);

        Debug.Log("Timeline has stopped.");
    }

    void OnDestroy()
    {
        // ลบ Event Listener เมื่อ Object ถูกทำลาย
        pl_GoSchool.stopped -= OnPlayableDirectorStopped;
    }
}
