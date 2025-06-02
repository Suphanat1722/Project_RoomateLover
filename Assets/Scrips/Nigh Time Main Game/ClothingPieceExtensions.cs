using UnityEngine;

// Extension class สำหรับ ClothingPiece เพื่อให้โค้ดอ่านง่ายขึ้น
public static class ClothingPieceExtensions
{
    public static void UpdateState(this InteractionManager.ClothingPiece piece,
        bool isLegsPiece = false,
        bool isLegsOpen = false,
        InteractionManager.ClothingPiece pairedPiece = null,
        InteractionManager manager = null)
    {
        if (piece.levels == null || piece.levels.Length == 0)
        {
            Debug.LogError($"[ClothingPiece] Levels array for {piece.name} is not set or empty!");
            return;
        }

        if (isLegsPiece)
        {
            UpdateLegsState(piece, isLegsOpen);
        }
        else if (IsShirtPiece(piece))
        {
            if (manager != null)
            {
                // ให้ InteractionManager จัดการ shirt states
                // เพราะมันซับซ้อนและเกี่ยวข้องกับ shirt + shirtSpread
            }
        }
        else
        {
            UpdateNormalClothingState(piece);
        }
    }

    private static void UpdateLegsState(InteractionManager.ClothingPiece piece, bool isLegsOpen)
    {
        if (piece.levels.Length < 2)
        {
            Debug.LogError($"[ClothingPiece] Legs piece {piece.name} must have at least 2 levels (closed/open)!");
            return;
        }

        piece.levels[0].SetActive(!isLegsOpen); // ขาปิด
        piece.levels[1].SetActive(isLegsOpen);   // ขาเปิด
    }

    private static void UpdateNormalClothingState(InteractionManager.ClothingPiece piece)
    {
        for (int i = 0; i < piece.levels.Length; i++)
        {
            if (piece.levels[i] != null)
            {
                piece.levels[i].SetActive(i == piece.currentLevel);
            }
            else
            {
                Debug.LogWarning($"[ClothingPiece] Level {i} for {piece.name} is null!");
            }
        }
    }

    private static bool IsShirtPiece(InteractionManager.ClothingPiece piece)
    {
        return piece.name == "Shirt" || piece.name == "ShirtSpread";
    }
}