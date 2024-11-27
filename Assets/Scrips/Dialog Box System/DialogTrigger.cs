using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public string characterName; // ชื่อตัวละครที่ต้องการแสดงบทสนทนา

    public static bool isShowCanvasDialogBox = false;

    public void TriggerDialog()
    {
        isShowCanvasDialogBox = true;
        FindFirstObjectByType<DialogManager>().StartDialog(characterName);
    }
}