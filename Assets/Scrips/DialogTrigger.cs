using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public Dialog dialog; // บทสนทนาที่ต้องการแสดง
    public GameObject dialogBox;
    public GameObject character;

    public static bool isShow = false;

    public void TriggerDialog()
    {

        dialogBox.SetActive(true);
        character.SetActive(false);
        isShow = true;
        if (isShow)
        {
            FindFirstObjectByType<DialogManager>().StartDialog(dialog);
        }
    }
}


