using UnityEngine;
using System.Collections.Generic;

public class ActionHandler : MonoBehaviour
{
    private ClothingManager clothingManager;

    public void Initialize(ClothingManager manager)
    {
        clothingManager = manager;
    }

    public void ExecuteAction(ActionType actionType, ClothingData targetClothing)
    {
        switch (actionType)
        {
            case ActionType.TakeOff:
                HandleTakeOff(targetClothing);
                break;
            case ActionType.Dress:
                HandleDress(targetClothing);
                break;
            case ActionType.SpreadLegs:
                HandleSpreadLegs();
                break;
            case ActionType.CloseLegs:
                HandleCloseLegs();
                break;
            case ActionType.FingerInside:
                HandleFingerInside();
                break;
            case ActionType.JerkOff:
                HandleJerkOff();
                break;
            default:
                Debug.LogWarning($"[ActionHandler] Unknown action: {actionType}");
                break;
        }
    }

    private void HandleTakeOff(ClothingData clothing)
    {
        if (!clothing.CanBeRemovedFurther)
        {
            Debug.LogWarning($"[ActionHandler] {clothing.name} ถอดหมดแล้ว");
            return;
        }

        clothingManager.UpdateClothingLevel(clothing, true);
        Debug.Log($"[ActionHandler] ถอด {clothing.name} level: {clothing.currentLevel}");
    }

    private void HandleDress(ClothingData clothing)
    {
        if (!clothing.CanBeDressed)
        {
            Debug.LogWarning($"[ActionHandler] {clothing.name} ใส่เต็มที่แล้ว");
            return;
        }

        clothingManager.UpdateClothingLevel(clothing, false);
        Debug.Log($"[ActionHandler] ใส่กลับ {clothing.name} level: {clothing.currentLevel}");
    }

    private void HandleSpreadLegs()
    {
        if (!clothingManager.CanSpreadLegs())
        {
            Debug.LogWarning("[ActionHandler] ไม่สามารถกางขาได้ในขณะนี้");
            return;
        }

        clothingManager.ToggleLegsState();
        Debug.Log("[ActionHandler] กางขา - เปลี่ยนเป็นโหมดขาเปิด");
    }

    private void HandleCloseLegs()
    {
        if (!clothingManager.CanCloseLegs())
        {
            Debug.LogWarning("[ActionHandler] ขาปิดอยู่แล้ว");
            return;
        }

        clothingManager.ToggleLegsState();
        Debug.Log("[ActionHandler] หุบขา - เปลี่ยนเป็นโหมดขาปิด");
    }

    private void HandleFingerInside()
    {
        Debug.Log("[ActionHandler] สอดนิ้วเข้าหี");
        // Add your finger inside logic here
    }

    private void HandleJerkOff()
    {
        Debug.Log("[ActionHandler] ชักว่าว");
        // Add your jerk off logic here
    }

    // ในไฟล์ ActionHandler.cs

    public List<ActionType> GetAvailableActions(ClothingData clothing)
    {
        List<ActionType> finalActions = new List<ActionType>();
        if (clothing == null || clothing.availableActions == null)
        {
            return finalActions;
        }

        // 1. วนลูปเฉพาะ Action ที่ "ถูกกำหนดไว้" ใน ClothingData ชิ้นนั้นๆ ก่อน
        foreach (ActionType action in clothing.availableActions)
        {
            // 2. จากนั้นค่อยนำ Action นั้นมาเช็คเงื่อนไข ว่าควรแสดงในสถานการณ์ปัจจุบันหรือไม่
            if (ShouldShowAction(action, clothing))
            {
                finalActions.Add(action);
            }
        }

        return finalActions;
    }

    // ฟังก์ชัน ShouldShowAction และอื่นๆ ไม่ต้องแก้ไขครับ
    private bool ShouldShowAction(ActionType actionType, ClothingData clothing)
    {
        switch (actionType)
        {
            case ActionType.TakeOff:
                return clothing.CanBeRemovedFurther;

            case ActionType.Dress:
                return clothing.CanBeDressed;

            case ActionType.SpreadLegs:
                // ใช้เงื่อนไขที่ซับซ้อนเหมือนโค้ดเก่า (ถ้าต้องการ)
                bool isOldFullyDressedConditionMet = (clothingManager.pants.IsFullyDressed && clothingManager.underwear.IsFullyDressed) || (clothingManager.pants.IsFullyRemoved && clothingManager.underwear.IsFullyDressed);
                return clothingManager.CanCloseLegs() == false && (isOldFullyDressedConditionMet || clothingManager.IsFullyRemoved());

            case ActionType.CloseLegs:
                return clothingManager.CanCloseLegs();

            case ActionType.FingerInside:
            case ActionType.JerkOff:
                return true; // Always available for now

            default:
                return false;
        }
    }

    public string GetActionDisplayText(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.TakeOff: return "ถอดชุด";
            case ActionType.Dress: return "สวมกลับ";
            case ActionType.SpreadLegs: return "กางขา";
            case ActionType.CloseLegs: return "หุบขา";
            case ActionType.FingerInside: return "สอดนิ้ว";
            case ActionType.JerkOff: return "ชักว่าว";
            default: return actionType.ToString();
        }
    }
}