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
        if (Input.GetMouseButtonDown(0) && DialogManager.isTypingFinished && canvasDialogBoxActive.activeSelf)
        {
            dialogManager.DisplayNextSentence();
        }  
        else if (Input.GetMouseButtonDown(0) && !DialogManager.isTypingFinished && canvasDialogBoxActive.activeSelf)
        {
            DialogManager.isLeftClickedToSkip = true;
        }
    }
}