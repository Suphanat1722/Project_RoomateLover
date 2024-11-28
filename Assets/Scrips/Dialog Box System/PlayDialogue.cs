using UnityEngine;
using UnityEngine.EventSystems;

public class PlayDialogue : MonoBehaviour
{
    [SerializeField] private DialogTrigger dialogTrigger;
    [SerializeField] private GameObject endTesat;

    void Update()
    {
        RaycastInteraction();  
    }

    void RaycastInteraction()
    {
        if (Input.GetMouseButtonUp(0)) // เมื่อคลิกเมาส์ซ้าย
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Character"))
            {
                dialogTrigger.TriggerDialog("นีน่า", "นีน่าในห้อง", EndTest);
                
            }
        }
    }

    void EndTest()
    {
        endTesat.SetActive(true);
    }
}
