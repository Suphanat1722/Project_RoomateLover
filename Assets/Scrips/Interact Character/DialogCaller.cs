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

        gameTime.SetTimeCurrentTime(7,30);

       
    }

    public void StartDialogSequence()
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

    private bool hasStartedDialog = false;

    void Update()
    {
        int hour = gameTime.GetHourCurrentTime();
        int minute = gameTime.GetMinuteCurrentTime();

        // เรียก dialog เมื่อถึง 7:30 และยังไม่ได้เรียก
        if (!hasStartedDialog && hour == 7 && minute == 30)
        {
            StartDialogSequence();
            hasStartedDialog = true;
        }

        // รีเซ็ตเมื่อเวลาหลุดจาก 7:30
        if (hasStartedDialog && (hour != 7 || minute != 30))
        {
            hasStartedDialog = false;
        }
    }
}
