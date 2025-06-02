using UnityEngine;

// คลาสช่วยเหลือสำหรับ ClothingPiece เพื่อให้อ่านง่ายขึ้น
public static class ClothingPieceExtensions
{
    public static void UpdateState(this InteractionManager.ClothingPiece piece,
        bool isLegsPiece = false,
        bool isLegsOpen = false,
        InteractionManager.ClothingPiece pairedPiece = null,
        InteractionManager manager = null)
    {
        // ตรวจสอบว่ามี levels หรือไม่
        if (piece.levels == null || piece.levels.Length == 0)
        {
            Debug.LogError($"[ClothingPiece] ไม่มี levels array สำหรับ {piece.name}!");
            return;
        }

        // ถ้าเป็นชิ้นขา
        if (isLegsPiece)
        {
            UpdateLegsClothing(piece, isLegsOpen);
        }
        // ถ้าเป็นเสื้อ
        else if (IsShirtClothing(piece))
        {
            if (manager != null)
            {
                // ให้ InteractionManager จัดการเสื้อเอง
                // เพราะมันซับซ้อนและต้องจัดการทั้งเสื้อปกติและเสื้อแบบขาเปิด
            }
        }
        // ถ้าเป็นชิ้นอื่น ๆ
        else
        {
            UpdateNormalClothing(piece);
        }
    }

    private static void UpdateLegsClothing(InteractionManager.ClothingPiece piece, bool isLegsOpen)
    {
        // ขาต้องมีอย่างน้อย 2 levels (ปิด/เปิด)
        if (piece.levels.Length < 2)
        {
            Debug.LogError($"[ClothingPiece] ชิ้นขา {piece.name} ต้องมีอย่างน้อย 2 levels (ปิด/เปิด)!");
            return;
        }

        piece.levels[0].SetActive(!isLegsOpen); // ขาปิด (index 0)
        piece.levels[1].SetActive(isLegsOpen);   // ขาเปิด (index 1)
    }

    private static void UpdateNormalClothing(InteractionManager.ClothingPiece piece)
    {
        // วนลูปทุก levels
        for (int i = 0; i < piece.levels.Length; i++)
        {
            if (piece.levels[i] != null)
            {
                // แสดงเฉพาะ level ที่ตรงกับ currentLevel
                bool shouldShow = (i == piece.currentLevel);
                piece.levels[i].SetActive(shouldShow);
            }
            else
            {
                Debug.LogWarning($"[ClothingPiece] Level {i} สำหรับ {piece.name} เป็น null!");
            }
        }
    }

    private static bool IsShirtClothing(InteractionManager.ClothingPiece piece)
    {
        // ตรวจสอบว่าเป็นเสื้อหรือไม่
        if (piece.name == "Shirt") return true;
        if (piece.name == "ShirtSpread") return true;

        return false;
    }
}