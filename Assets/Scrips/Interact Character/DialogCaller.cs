using UnityEngine;

public class DialogCaller : MonoBehaviour
{
    public DialogTrigger dialogTrigger;
    public GameTime gameTime;
    public TimelineController timelineController;

    void Start()
    {
        dialogTrigger = FindFirstObjectByType<DialogTrigger>();
        gameTime = FindFirstObjectByType<GameTime>();
        timelineController = FindFirstObjectByType<TimelineController>();

        gameTime.SetTimeCurrentTime(7, 30);

        StartDialogSequence();
    }

    void StartDialogSequence()
    {

        dialogTrigger.TriggerDialog("นีน่า", "ตื่นนอน", () =>
        {
            gameTime.AddTime(0,30);
            dialogTrigger.TriggerDialog("ผู้เล่น", "ทานอาหารเช้า", () =>
            {               
                dialogTrigger.TriggerDialog("นีน่า", "กำลังเตรียมตัว", () =>
                {
                    
                    timelineController.PlayTimelineGoToSchool();                  
                });
            });
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
