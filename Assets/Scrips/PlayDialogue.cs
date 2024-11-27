using UnityEngine;
using UnityEngine.EventSystems;

public class PlayDialogue : MonoBehaviour
{
    public DialogManager dialogManager;

    void Update()
    {      
        if (DialogTrigger.isShow)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dialogManager.DisplayNextSentence();
            }
        }
    } 
}
