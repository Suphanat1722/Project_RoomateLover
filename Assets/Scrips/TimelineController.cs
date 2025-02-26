using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
     public PlayableDirector pl_GoSchool;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) // ถ้ากด Spacebar ให้เริ่มเล่น Timeline
        {
            PlayTimelineGoToSchool();
        }     
    }

    public void PlayTimelineGoToSchool()
    {
        pl_GoSchool.Play(); // ใช้ฟังก์ชันนี้เพื่อสั่งเล่น Timeline
    }
}
