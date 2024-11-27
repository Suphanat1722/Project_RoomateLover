using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public Dialog dialog; // บทสนทนาที่ต้องการแสดง

    public static bool isShowCanvasDialogBox = false; 

    public void TriggerDialog()
    {
        isShowCanvasDialogBox = true;

        if (isShowCanvasDialogBox)
        {
            FindFirstObjectByType<DialogManager>().StartDialog(dialog);
        }
    }
}


