using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public static bool isShowCanvasDialogBox = false;

    public DialogManager dialogManager;
    public GameObject dialogBox;

    void Update()
    {
        if (DialogTrigger.isShowCanvasDialogBox)
        {
            dialogBox.SetActive(true);

            if (Input.GetMouseButtonDown(0))
            {
                dialogManager.DisplayNextSentence();
            }
        }
        else
        {
            dialogBox.SetActive(false);
        }
    }

    public void TriggerDialog(string characterName, string titleName)
    {
        isShowCanvasDialogBox = true;
        dialogManager.StartDialog(characterName, titleName);
    }

    /*public void TriggerDialog()
    {
        isShowCanvasDialogBox = true;
        FindFirstObjectByType<DialogManager>().StartDialog(characterName);
    }*/
}