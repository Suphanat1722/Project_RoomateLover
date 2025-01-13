using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class MoodStatus : MonoBehaviour
{
    private int moodCharacter; // Range: -30 to 30
   
    private void Update()
    {
        HandleMoodStatus();
    }
   
    public void TouchCharacter()
    {
        moodCharacter += 3;
    }


    private void HandleMoodStatus()
    {
        if (moodCharacter >= 10 && moodCharacter < 20)
        {
            Debug.Log("Mood: เริ่มมีอารมณ์");
        }
        else if (moodCharacter >= 20 && moodCharacter < 25)
        {
            Debug.Log("Mood: มีอารมณ์ร่วม");
        }
        else if (moodCharacter >= 25 && moodCharacter < 30)
        {
            Debug.Log("Mood: ใกล้ถึงจุดสุดยอด");
        }
    }
}
