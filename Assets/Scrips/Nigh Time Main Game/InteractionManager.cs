using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class InteractionManager : MonoBehaviour
{
    [System.Serializable]
    public class ClothingPiece
    {
        public string name;
        public GameObject[] levels;
        public int currentLevel = 0;
        public int maxLevel = 3;
        public string interactionLayer;
        public List<string> availableActions;
        public bool hasLegsVariant = false;

        public bool IsFullyRemoved()
        {
            return currentLevel >= maxLevel;
        }

        public bool CanBeRemovedFurther()
        {
            return currentLevel < maxLevel;
        }

        public bool CanBeDressed()
        {
            return currentLevel > 0;
        }

        public void ResetToFullyDressed()
        {
            currentLevel = 0;
        }

        public void RemoveOneLevel()
        {
            if (CanBeRemovedFurther())
                currentLevel++;
        }
    }

    [Header("Clothing Pieces")]
    public ClothingPiece shirt;
    public ClothingPiece shirtSpread;
    public ClothingPiece pants;
    public ClothingPiece pantsSpread;
    public ClothingPiece underwear;
    public ClothingPiece underwearSpread;
    public ClothingPiece legs;

    [Header("UI Elements")]
    public GameObject actionPanel;
    public GameObject buttonPrefab;
    public TextMeshProUGUI actionText;

    private ClothingPiece currentClothingPiece;
    private List<Button> spawnedButtons = new List<Button>();
    private bool isLegsOpen = false;

    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        HandleMouseInput();
    }

    private void InitializeGame()
    {
        // กำหนดว่าชิ้นไหนมีรูปแบบขาเปิด/ปิด
        shirt.hasLegsVariant = true;
        pants.hasLegsVariant = true;
        underwear.hasLegsVariant = true;
        legs.hasLegsVariant = true;

        // ซิงค์ level ของชิ้นที่เป็นคู่กัน
        shirtSpread.currentLevel = shirt.currentLevel;
        pantsSpread.currentLevel = pants.currentLevel;
        underwearSpread.currentLevel = underwear.currentLevel;

        UpdateAllClothingDisplay();
        HideActionPanel();
    }

    private void HandleMouseInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);

        if (hits.Length > 0)
        {
            RaycastHit2D bestHit = FindBestHit(hits);
            if (bestHit.collider != null)
            {
                string layerName = LayerMask.LayerToName(bestHit.collider.gameObject.layer);
                ShowActionPanel(layerName);
            }
        }
        else
        {
            // คลิกที่พื้นที่ว่าง
            if (actionPanel != null && actionPanel.activeSelf && !IsClickingOnUI())
            {
                HideActionPanel();
            }
        }
    }

    private RaycastHit2D FindBestHit(RaycastHit2D[] hits)
    {
        RaycastHit2D bestHit = new RaycastHit2D();

        // ตรวจสอบตามลำดับความสำคัญ (ชิ้นด้านหน้าก่อน)
        // 1. กางเกง (ด้านหน้าสุด)
        bestHit = CheckForLayer(hits, "Pants");
        if (bestHit.collider != null && CanClickClothing("Pants")) return bestHit;

        // 2. กางเกงใน
        bestHit = CheckForLayer(hits, "Underwear");
        if (bestHit.collider != null && CanClickClothing("Underwear")) return bestHit;

        // 3. เสื้อ
        bestHit = CheckForLayer(hits, "Shirt");
        if (bestHit.collider != null && CanClickClothing("Shirt")) return bestHit;

        // 4. ขา (ด้านหลังสุด)
        bestHit = CheckForLayer(hits, "Legs");
        if (bestHit.collider != null && CanClickClothing("Legs")) return bestHit;

        // ถ้าไม่เจออะไรเลย ส่งกลับ hit แรก
        return hits[0];
    }

    private RaycastHit2D CheckForLayer(RaycastHit2D[] hits, string layerName)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            string hitLayerName = LayerMask.LayerToName(hits[i].collider.gameObject.layer);
            if (hitLayerName == layerName)
            {
                return hits[i];
            }
        }
        return new RaycastHit2D(); // ไม่เจอ
    }

    private bool CanClickClothing(string layerName)
    {
        if (layerName == "Underwear")
        {
            // กางเกงในคลิกได้ก็ต่อเมื่อกางเกงถูกถอดแล้ว
            return pants.IsFullyRemoved();
        }
        else if (layerName == "Legs")
        {
            // ขาคลิกได้ก็ต่อเมื่อกางเกงในถูกถอดแล้ว
            return underwear.IsFullyRemoved();
        }
        else if (layerName == "Shirt" || layerName == "Pants")
        {
            // เสื้อและกางเกงคลิกได้เสมอ
            return true;
        }

        return true;
    }

    private bool IsClickingOnUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    private void ShowActionPanel(string layerName)
    {
        ClothingPiece clothingPiece = GetClothingPieceByLayer(layerName);
        if (clothingPiece == null) return;

        currentClothingPiece = clothingPiece;
        actionPanel.SetActive(true);
        actionText.text = currentClothingPiece.name;

        ClearExistingButtons();
        CreateActionButtons();
    }

    private ClothingPiece GetClothingPieceByLayer(string layerName)
    {
        if (layerName == shirt.interactionLayer) return shirt;
        if (layerName == pants.interactionLayer) return pants;
        if (layerName == underwear.interactionLayer) return underwear;
        if (layerName == legs.interactionLayer) return legs;
        return null;
    }

    private void CreateActionButtons()
    {
        int buttonIndex = 0;

        for (int i = 0; i < currentClothingPiece.availableActions.Count; i++)
        {
            string actionString = currentClothingPiece.availableActions[i];

            if (ShouldShowAction(actionString))
            {
                CreateSingleButton(actionString, buttonIndex);
                buttonIndex++;
            }
        }
    }

    private void CreateSingleButton(string actionString, int index)
    {
        GameObject buttonObj = Instantiate(buttonPrefab, actionPanel.transform);
        Button button = buttonObj.GetComponent<Button>();
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        button.onClick.AddListener(() => OnActionButtonClick(actionString));
        buttonText.text = GetActionDisplayText(actionString);

        // จัดตำแหน่งปุ่ม
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, -100 * index, 0);

        spawnedButtons.Add(button);
    }

    private bool ShouldShowAction(string actionString)
    {
        if (currentClothingPiece == null) return false;

        if (actionString == "TakeOff")
        {
            return currentClothingPiece.CanBeRemovedFurther();
        }
        else if (actionString == "Dress")
        {
            return currentClothingPiece.CanBeDressed();
        }
        else if (actionString == "SpreadLegs")
        {
            return !isLegsOpen;
        }
        else if (actionString == "CloseLegs")
        {
            return isLegsOpen;
        }
        else if (actionString == "FingerInside" || actionString == "JerkOff")
        {
            return true;
        }

        return false;
    }

    private string GetActionDisplayText(string actionString)
    {
        if (actionString == "TakeOff") return "ถอนชุด";
        if (actionString == "Dress") return "สวมกลับ";
        if (actionString == "SpreadLegs") return "กางขา";
        if (actionString == "CloseLegs") return "หุบขา";
        if (actionString == "FingerInside") return "สอดนิ้ว";
        if (actionString == "JerkOff") return "ชักว่าว";

        return actionString;
    }

    public void OnActionButtonClick(string actionString)
    {
        if (actionString == "TakeOff")
        {
            HandleTakeOff();
        }
        else if (actionString == "Dress")
        {
            HandleDress();
        }
        else if (actionString == "SpreadLegs")
        {
            HandleSpreadLegs();
        }
        else if (actionString == "CloseLegs")
        {
            HandleCloseLegs();
        }
        else if (actionString == "FingerInside")
        {
            HandleFingerInside();
        }
        else if (actionString == "JerkOff")
        {
            HandleJerkOff();
        }

        RefreshActionPanel();
    }

    private void HandleTakeOff()
    {
        if (!currentClothingPiece.CanBeRemovedFurther())
        {
            Debug.LogWarning($"{currentClothingPiece.name} ถอดหมดแล้ว");
            return;
        }

        currentClothingPiece.RemoveOneLevel();
        SyncWithSpreadVersion();
        UpdateRelatedClothing();

        Debug.Log($"ถอด {currentClothingPiece.name} level: {currentClothingPiece.currentLevel}");
    }

    private void HandleDress()
    {
        if (!currentClothingPiece.CanBeDressed())
        {
            Debug.LogWarning($"{currentClothingPiece.name} ใส่เต็มที่แล้ว");
            return;
        }

        currentClothingPiece.ResetToFullyDressed();
        SyncWithSpreadVersion();
        UpdateRelatedClothing();

        Debug.Log($"ใส่ {currentClothingPiece.name} กลับ");
    }

    private void HandleSpreadLegs()
    {
        isLegsOpen = true;
        UpdateAllClothingDisplay();
        Debug.Log("กางขา - เปลี่ยนเป็นโหมดขาเปิด");
    }

    private void HandleCloseLegs()
    {
        isLegsOpen = false;
        UpdateAllClothingDisplay();
        Debug.Log("หุบขา - เปลี่ยนเป็นโหมดขาปิด");
    }

    private void HandleFingerInside()
    {
        Debug.Log("สอดนิ้วเข้าหี");
    }

    private void HandleJerkOff()
    {
        Debug.Log("ชักว่าว");
    }

    private void SyncWithSpreadVersion()
    {
        // ซิงค์ level กับชิ้นขาเปิด
        if (currentClothingPiece == shirt)
        {
            shirtSpread.currentLevel = shirt.currentLevel;
        }
        else if (currentClothingPiece == pants)
        {
            pantsSpread.currentLevel = pants.currentLevel;
        }
        else if (currentClothingPiece == underwear)
        {
            underwearSpread.currentLevel = underwear.currentLevel;
        }
    }

    private void UpdateRelatedClothing()
    {
        if (currentClothingPiece == shirt)
        {
            UpdateClothingDisplay(shirt, shirtSpread);
        }
        else if (currentClothingPiece == pants)
        {
            UpdateClothingDisplay(pants, pantsSpread);
        }
        else if (currentClothingPiece == underwear)
        {
            UpdateClothingDisplay(underwear, underwearSpread);
        }
    }

    private void UpdateAllClothingDisplay()
    {
        UpdateClothingDisplay(shirt, shirtSpread);
        UpdateClothingDisplay(pants, pantsSpread);
        UpdateClothingDisplay(underwear, underwearSpread);
        UpdateLegsDisplay();
    }

    private void UpdateClothingDisplay(ClothingPiece normalPiece, ClothingPiece spreadPiece)
    {
        // ปิดทุก levels ของทั้งสองชิ้นก่อน
        TurnOffAllLevels(normalPiece);
        TurnOffAllLevels(spreadPiece);

        if (isLegsOpen)
        {
            // แสดง spread version
            TurnOnLevel(spreadPiece, spreadPiece.currentLevel);
        }
        else
        {
            // แสดง normal version
            TurnOnLevel(normalPiece, normalPiece.currentLevel);
        }
    }

    private void UpdateLegsDisplay()
    {
        if (legs.levels.Length < 2)
        {
            Debug.LogError("ขาต้องมีอย่างน้อย 2 levels (ปิด/เปิด)");
            return;
        }

        legs.levels[0].SetActive(!isLegsOpen); // ขาปิด
        legs.levels[1].SetActive(isLegsOpen);   // ขาเปิด
    }

    private void TurnOffAllLevels(ClothingPiece piece)
    {
        for (int i = 0; i < piece.levels.Length; i++)
        {
            if (piece.levels[i] != null)
            {
                piece.levels[i].SetActive(false);
            }
        }
    }

    private void TurnOnLevel(ClothingPiece piece, int levelIndex)
    {
        if (levelIndex < piece.levels.Length && piece.levels[levelIndex] != null)
        {
            piece.levels[levelIndex].SetActive(true);
        }
    }

    private void RefreshActionPanel()
    {
        if (currentClothingPiece != null)
        {
            ShowActionPanel(currentClothingPiece.interactionLayer);
        }
    }

    private void HideActionPanel()
    {
        if (actionPanel != null)
        {
            actionPanel.SetActive(false);
            ClearExistingButtons();
        }
    }

    private void ClearExistingButtons()
    {
        for (int i = 0; i < spawnedButtons.Count; i++)
        {
            if (spawnedButtons[i] != null)
                Destroy(spawnedButtons[i].gameObject);
        }
        spawnedButtons.Clear();
    }
}