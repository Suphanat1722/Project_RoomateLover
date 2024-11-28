using UnityEngine;
using UnityEngine.EventSystems;

public class PlayDialogue : MonoBehaviour
{
    public DialogTrigger dialogTrigger;

    void Update()
    {
        if (!DialogTrigger.isShowCanvasDialogBox)
        {
            if (Input.GetMouseButtonDown(0)) // เมื่อคลิกเมาส์ซ้าย
            {
                RaycastInteraction();
            }
        }
    }

    void RaycastInteraction()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            // เพิ่มการกระทำที่คุณต้องการ
            dialogTrigger.TriggerDialog("Nina", "test");
        }
    }
}
