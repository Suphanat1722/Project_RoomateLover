using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClothingInteractionManager : MonoBehaviour
{
    public GameObject button;
    public Transform Tbutton;

    private void Start()
    {
        // ตรวจสอบว่ามีการ assign references หรือไม่
        if (button == null)
            Debug.LogError("Button prefab is not assigned!");
        if (Tbutton == null)
            Debug.LogError("Transform Tbutton is not assigned!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            CreateNewButton();
        }
    }

    private void CreateNewButton()
    {
        // ตรวจสอบก่อนสร้าง
        if (button != null && Tbutton != null)
        {
            GameObject newButton = Instantiate(button, Tbutton); // ไม่ต้อง .transform
            Button buttonComponent = newButton.GetComponent<Button>();

            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(() => Key("Button clicked!"));
            }
            else
            {
                Debug.LogError("Button component not found on instantiated object!");
            }
        }
    }

    void Key(string message)
    {
        Debug.Log(message);
    }
}