using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    public TextMeshProUGUI nameText; // ช่องแสดงชื่อของตัวละคร
    public TextMeshProUGUI dialogText; // ช่องแสดงข้อความสนทนา

    private Queue<string> sentences; // คิวสำหรับเก็บประโยคสนทนา

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialog(Dialog dialog)
    {
        nameText.text = dialog.characterName;
        
        sentences.Clear();

        foreach (string sentence in dialog.sentences)
        {
            sentences.Enqueue(sentence);
        }
        
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialog();
            return;
        }

        string sentence = sentences.Dequeue();
        
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(0.05f); // ระยะเวลาระหว่างตัวอักษร
        }
    }

    void EndDialog()
    {
        DialogTrigger.isShowCanvasDialogBox = false;
        Debug.Log("End");
    }
}