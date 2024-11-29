using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText; // ช่องแสดงชื่อของตัวละคร
    [SerializeField] private TextMeshProUGUI dialogText; // ช่องแสดงข้อความสนทนา

    public static string selectedLanguage = "th"; // ภาษาเลือก

    string jsonFilePath = $"dialogues_{selectedLanguage}";

    Queue<string> sentences; // คิวสำหรับเก็บประโยคสนทนา
    DialogData dialogData; // ข้อมูลบทสนทนา

    void Start()
    {
        sentences = new Queue<string>();
        LoadDialogData(); // โหลดข้อมูลบทสนทนาเมื่อเริ่มต้น
    }

    void LoadDialogData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFilePath);
        if (jsonFile != null)
        {
            dialogData = JsonUtility.FromJson<DialogData>(jsonFile.text);
            //Debug.Log("Dialog data loaded successfully.");
        }
        else
        {
            Debug.LogError("Failed to load JSON file at " + jsonFilePath);
        }
    }

    public void StartDialog(string characterName, string title)
    {
        Dialog dialog = FindDialogByNameAndDialog(characterName, title);
        if (dialog == null)
        {
            Debug.LogError("Dialog not found for character: " + characterName);
            return;
        }

        nameText.text = dialog.characterName;
        sentences.Clear();

        foreach (string sentence in dialog.sentences)
        {
            sentences.Enqueue(sentence);
            //Debug.Log($"Enqueued: {sentence}");
        }
        DisplayNextSentence();
    }

    Dialog FindDialogByNameAndDialog(string characterName, string dialogueTitle)
    {
        foreach (var dialog in dialogData.dialogs)
        {
            if (dialog.characterName == characterName && dialog.title == dialogueTitle)
            {
                return dialog;
            }
        }
        return null;
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

    public static bool isTypingFinished;
    public static bool isLeftClickedToSkip;

    IEnumerator TypeSentence(string sentence)
    {
        dialogText.text = "";
        isTypingFinished = false;
        isLeftClickedToSkip = false; // รีเซ็ตค่าเมื่อเริ่มแสดงข้อความใหม่

        foreach (char letter in sentence.ToCharArray())
        {
            if (isLeftClickedToSkip)
            {
                dialogText.text = sentence;
                break; // ออกจากลูปเมื่อคลิกซ้าย
            }

            dialogText.text += letter;
            yield return new WaitForSeconds(0.05f); //ระยะเวลาระหว่างตัวอักษร
        }

        isTypingFinished = true;
    }

    void EndDialog()
    {
        FindFirstObjectByType<DialogTrigger>()?.EndDialogue();
    }
}
