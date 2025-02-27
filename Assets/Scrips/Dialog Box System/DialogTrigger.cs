using UnityEngine;
using System;
using Live2D.Cubism.Core;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] GameObject canvasDialogBoxActive;

    DialogManager dialogManager; 
    Action onDialogEnded;

    // เพิ่ม Event สำหรับแจ้งเตือนเมื่อไดอะล็อกจบลง
    public event Action OnDialogEndedEvent;

    public void TriggerDialog(string characterName, string titleName, Action onDialogEnd)
    {
        if (dialogManager == null)
        {
            Debug.LogError("DialogManager is not assigned!");
            return;
        }

        canvasDialogBoxActive.SetActive(true);
        dialogManager.StartDialog(characterName, titleName);
        this.onDialogEnded = onDialogEnd;
    }

    public void EndDialogue()
    {
        canvasDialogBoxActive.SetActive(false);
        onDialogEnded?.Invoke();
        OnDialogEndedEvent?.Invoke(); // แจ้งเตือนว่าไดอะล็อกจบลงแล้ว
    }

    void Awake()
    {
        dialogManager = FindFirstObjectByType<DialogManager>();
        if (dialogManager == null)
        {
            Debug.LogError("DialogManager not found in the scene!");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canvasDialogBoxActive.activeSelf)
        {
            if (DialogManager.isTyping)
            {
                dialogManager.DisplayNextSentence();
            }
            else
            {
                DialogManager.isLeftClickedToSkip = true;
            }
        }
    }
}