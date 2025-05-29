using UnityEngine;

public class ClothingManager : MonoBehaviour
{
    [System.Serializable]
    public class ClothingPiece
    {
        public string name; // ชื่อของชิ้นชุด (เช่น "Shirt", "Pants")
        public GameObject[] levels; // GameObject สำหรับแต่ละระดับ (0 = สวมเต็ม, 1 = ร่น 1, 2 = ร่น 2, 3 = ถอนออก)
        public int currentLevel = 0; // ระดับปัจจุบัน (0-3)
        public int maxLevel = 3; // จำนวนระดับสูงสุด (3 = ถอนออก)
        public string interactionLayer; // Layer สำหรับการโต้ตอบ (เช่น "ShirtArea", "PantsArea")

        public void UpdateState()
        {
            // เปิด/ปิด GameObject ตามระดับ
            for (int i = 0; i < levels.Length; i++)
            {
                if (levels[i] != null)
                {
                    levels[i].SetActive(i == currentLevel);
                }
            }
        }
    }

    public ClothingPiece shirt;
    public ClothingPiece pants;

    private void Start()
    {
        // อัปเดตสถานะเริ่มต้นของชุด
        shirt.UpdateState();
        pants.UpdateState();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                string layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
                HandleClothingInteraction(layerName);
            }
        }
    }

    private void HandleClothingInteraction(string layerName)
    {
        if (layerName == shirt.interactionLayer)
        {
            if (shirt.currentLevel < shirt.maxLevel)
            {
                // ถอนชุด (เพิ่มระดับ)
                shirt.currentLevel++;
            }
            else
            {
                // ใส่ชุดกลับ (รีเซ็ตระดับ)
                shirt.currentLevel = 0;
            }
            shirt.UpdateState();
            Debug.Log($"Shirt Level: {shirt.currentLevel}");
        }
        else if (layerName == pants.interactionLayer)
        {
            if (pants.currentLevel < pants.maxLevel)
            {
                // ถอนชุด (เพิ่มระดับ)
                pants.currentLevel++;
            }
            else
            {
                // ใส่ชุดกลับ (รีเซ็ตระดับ)
                pants.currentLevel = 0;
            }
            pants.UpdateState();
            Debug.Log($"Pants Level: {pants.currentLevel}");
        }
    }
}
