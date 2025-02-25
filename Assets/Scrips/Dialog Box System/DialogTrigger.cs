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

    void Start()
    {
        dialogManager = FindFirstObjectByType<DialogManager>();
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