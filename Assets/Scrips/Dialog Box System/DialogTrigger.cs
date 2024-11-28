using UnityEngine;
using System;
using Live2D.Cubism.Core;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField]GameObject canvasDialogBoxActive;

    DialogManager dialogManager; 
    Action onDialogEnded;

    public void TriggerDialog(string characterName, string titleName, Action onDialogEnd)
    {
        Debug.Log("Strat");
        canvasDialogBoxActive.SetActive(true);
        dialogManager.StartDialog(characterName, titleName);
        this.onDialogEnded = onDialogEnd;
    }

    public void EndDialogue()
    {
        canvasDialogBoxActive.SetActive(false);
        onDialogEnded?.Invoke();
    }

    void Start()
    {
        dialogManager = FindFirstObjectByType<DialogManager>();
    }

    void Update()
    {
        if (canvasDialogBoxActive.activeSelf)
        {
            if (Input.GetMouseButtonUp(0)) // เมื่อคลิกเมาส์ซ้าย
            {
                dialogManager.DisplayNextSentence();
                Debug.Log("Next");
            }
        }

        
    }
}