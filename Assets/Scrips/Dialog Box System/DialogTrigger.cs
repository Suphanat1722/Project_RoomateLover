using UnityEngine;
using System;
using Live2D.Cubism.Core;

public class DialogTrigger : MonoBehaviour
{
    public static bool isDialogActive = false;
    [SerializeField]GameObject canvasDialogBoxActive;

    DialogManager dialogManager; 
    Action onDialogEnded;

    public void TriggerDialog(string characterName, string titleName, Action onDialogEnd)
    {
        isDialogActive = true;
        canvasDialogBoxActive.SetActive(true);
        dialogManager.StartDialog(characterName, titleName);
        this.onDialogEnded = onDialogEnd;
    }

    public void EndDialogue()
    {
        isDialogActive = false;
        canvasDialogBoxActive.SetActive(false);
        onDialogEnded?.Invoke();
    }

    void Start()
    {
        dialogManager = FindFirstObjectByType<DialogManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0)) // เมื่อคลิกเมาส์ซ้าย
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider == null && isDialogActive)
            {
                dialogManager.DisplayNextSentence();
            }
        }
    }
}