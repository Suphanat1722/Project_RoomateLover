using UnityEngine;
using UnityEngine.EventSystems;

public class PlayDialogue : MonoBehaviour
{
    public DialogManager dialogManager;
    public DialogTrigger dialogTrigger;
    public GameObject dialogBox;
    public GameObject character;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // เมื่อคลิกเมาส์ซ้าย
        {
            RaycastInteraction();
        }

        ShowCanvasDialogBox();
    }

    void RaycastInteraction()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("Clicked on: " + hit.collider.name);
            // เพิ่มการกระทำที่คุณต้องการ
            dialogTrigger.TriggerDialog();
        }
    }
    void ShowCanvasDialogBox()
    {
        if (DialogTrigger.isShowCanvasDialogBox)
        {
            dialogBox.SetActive(true);
            character.SetActive(false);

            if (Input.GetMouseButtonDown(0))
            {
                dialogManager.DisplayNextSentence();
            }
        }
        else
        {
            dialogBox.SetActive(false);
            character.SetActive(true);
        }
    }
}
