using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    private InteractionManager interactionManager;
    private string actionName;
    private Button buttonComponent;

    void Awake()
    {
        SetupButton();
    }

    void OnDestroy()
    {
        RemoveButtonListeners();
    }

    public void Initialize(InteractionManager manager, string action)
    {
        // ตรวจสอบว่าข้อมูลที่ส่งมาถูกต้องไหม
        if (manager == null || string.IsNullOrEmpty(action))
        {
            Debug.LogError($"[ActionButton] ข้อมูลไม่ครบ - Manager: {manager != null}, Action: {!string.IsNullOrEmpty(action)}");
            return;
        }

        // เก็บข้อมูล
        interactionManager = manager;
        actionName = action;
        Debug.Log($"[ActionButton] ตั้งค่า {gameObject.name} กับคำสั่ง: {actionName}");

        // เชื่อมโยงปุ่มกับฟังก์ชัน
        ConnectButtonToFunction();
    }

    private void SetupButton()
    {
        buttonComponent = GetComponent<Button>();

        if (buttonComponent == null)
        {
            Debug.LogError($"[ActionButton] ไม่พบ Button component ใน {gameObject.name}");
        }
        else
        {
            Debug.Log($"[ActionButton] พบ Button ใน {gameObject.name}, สามารถคลิกได้: {buttonComponent.interactable}");
        }
    }

    private void ConnectButtonToFunction()
    {
        if (buttonComponent == null)
        {
            Debug.LogError($"[ActionButton] ไม่สามารถเชื่อมโยงปุ่มได้ - Button เป็น null ใน {gameObject.name}");
            return;
        }

        // ลบ listener เก่าทั้งหมดก่อน
        buttonComponent.onClick.RemoveAllListeners();

        // เพิ่ม listener ใหม่
        buttonComponent.onClick.AddListener(OnButtonClicked);

        Debug.Log($"[ActionButton] เชื่อมโยงปุ่มสำเร็จ: {gameObject.name}");
    }

    private void OnButtonClicked()
    {
        Debug.Log($"[ActionButton] ปุ่มถูกคลิก: {actionName} ใน {gameObject.name}");

        if (interactionManager == null)
        {
            Debug.LogError($"[ActionButton] InteractionManager เป็น null ใน {gameObject.name}");
            return;
        }

        // ส่งคำสั่งไปยัง InteractionManager
        interactionManager.OnActionButtonClick(actionName);
    }

    private void RemoveButtonListeners()
    {
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();
            Debug.Log($"[ActionButton] ลบ listeners สำหรับ {gameObject.name}");
        }
    }
}