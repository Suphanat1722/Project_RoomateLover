using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class InteractionManager : MonoBehaviour
{
    #region Data Structures
    [System.Serializable]
    public class ClothingPiece
    {
        [Header("Basic Settings")]
        public string name;
        public GameObject[] levels;
        public int currentLevel = 0;
        public int maxLevel = 3;
        public string interactionLayer;
        public List<string> availableActions;

        [Header("Legs Open/Close Support")]
        public bool hasLegsVariant = false; // มีรูปแบบขาเปิด/ปิดไหม

        public bool IsFullyRemoved() => currentLevel >= maxLevel;
        public bool CanBeRemovedFurther() => currentLevel < maxLevel;
        public bool CanBeDressed() => currentLevel > 0;

        public void ResetToFullyDressed()
        {
            currentLevel = 0;
        }

        public void RemoveOneLevel()
        {
            if (CanBeRemovedFurther())
                currentLevel++;
        }

        // อัปเดตสถานะตามว่าขาเปิดหรือปิด
        public void UpdateDisplayState(bool isLegsOpen, InteractionManager manager = null)
        {
            if (hasLegsVariant)
            {
                // ถ้ามีรูปแบบขาเปิด/ปิด ให้ใช้ manager จัดการ
                if (manager != null)
                {
                    manager.UpdateClothingWithLegsState(this, isLegsOpen);
                }
            }
            else
            {
                // ถ้าไม่มีรูปแบบขาเปิด/ปิด ใช้ levels ปกติ
                UpdateNormalState();
            }
        }

        private void UpdateNormalState()
        {
            for (int i = 0; i < levels.Length; i++)
            {
                if (levels[i] != null)
                {
                    levels[i].SetActive(i == currentLevel);
                }
            }
        }
    }

    public enum ActionType
    {
        TakeOff,
        Dress,
        SpreadLegs,
        CloseLegs,
        FingerInside,
        JerkOff
    }
    #endregion

    #region Inspector Fields
    [Header("Clothing Pieces")]
    public ClothingPiece shirt;
    public ClothingPiece shirtSpread;    // เสื้อแบบขาเปิด
    public ClothingPiece pants;
    public ClothingPiece pantsSpread;    // กางเกงแบบขาเปิด
    public ClothingPiece underwear;
    public ClothingPiece underwearSpread; // กางเกงในแบบขาเปิด
    public ClothingPiece legs;

    [Header("UI Elements")]
    public GameObject actionPanel;
    public GameObject buttonPrefab;
    public TextMeshProUGUI actionText;
    #endregion

    #region Private Fields
    private ClothingPiece currentClothingPiece;
    private List<Button> spawnedButtons = new List<Button>();
    private bool isLegsOpen = false;

    private readonly Dictionary<string, ActionType> actionTypeMap = new Dictionary<string, ActionType>
    {
        { "TakeOff", ActionType.TakeOff },
        { "Dress", ActionType.Dress },
        { "SpreadLegs", ActionType.SpreadLegs },
        { "CloseLegs", ActionType.CloseLegs },
        { "FingerInside", ActionType.FingerInside },
        { "JerkOff", ActionType.JerkOff }
    };

    private readonly Dictionary<ActionType, string> actionDisplayTexts = new Dictionary<ActionType, string>
    {
        { ActionType.TakeOff, "ถอนชุด" },
        { ActionType.Dress, "สวมกลับ" },
        { ActionType.SpreadLegs, "กางขา" },
        { ActionType.CloseLegs, "หุบขา" },
        { ActionType.FingerInside, "สอดนิ้ว" },
        { ActionType.JerkOff, "ชักว่าว" }
    };
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        InitializeGame();
    }

    private void Update()
    {
        HandleMouseInput();
    }
    #endregion

    #region Initialization
    private void InitializeGame()
    {
        // ตั้งค่าว่าชิ้นไหนมีรูปแบบขาเปิด/ปิด
        SetupClothingVariants();

        // ซิงค์ level ของชิ้นที่เป็นคู่กัน
        SynchronizeClothingLevels();

        // อัปเดตการแสดงผลทั้งหมด
        UpdateAllClothingDisplay();

        HideActionPanel();
    }

    private void SetupClothingVariants()
    {
        // กำหนดว่าชิ้นไหนมีรูปแบบขาเปิด/ปิด
        shirt.hasLegsVariant = true;
        pants.hasLegsVariant = true;
        underwear.hasLegsVariant = true;
        legs.hasLegsVariant = true;
    }

    private void SynchronizeClothingLevels()
    {
        // ให้ level ของชิ้นปกติกับชิ้นขาเปิดเท่ากัน
        shirtSpread.currentLevel = shirt.currentLevel;
        pantsSpread.currentLevel = pants.currentLevel;
        underwearSpread.currentLevel = underwear.currentLevel;
    }

    private void UpdateAllClothingDisplay()
    {
        UpdateClothingDisplay(shirt, shirtSpread);
        UpdateClothingDisplay(pants, pantsSpread);
        UpdateClothingDisplay(underwear, underwearSpread);
        legs.UpdateDisplayState(isLegsOpen, this);
    }

    private void UpdateClothingDisplay(ClothingPiece normalPiece, ClothingPiece spreadPiece)
    {
        // ปิดทุก levels ของทั้งสองชิ้นก่อน
        SetAllLevelsInactive(normalPiece);
        SetAllLevelsInactive(spreadPiece);

        if (isLegsOpen)
        {
            // แสดง spread version ตาม currentLevel
            SetLevelActive(spreadPiece, spreadPiece.currentLevel);
        }
        else
        {
            // แสดง normal version ตาม currentLevel
            SetLevelActive(normalPiece, normalPiece.currentLevel);
        }
    }

    private void SetAllLevelsInactive(ClothingPiece piece)
    {
        for (int i = 0; i < piece.levels.Length; i++)
        {
            if (piece.levels[i] != null)
            {
                piece.levels[i].SetActive(false);
            }
        }
    }

    private void SetLevelActive(ClothingPiece piece, int levelIndex)
    {
        if (levelIndex < piece.levels.Length && piece.levels[levelIndex] != null)
        {
            piece.levels[levelIndex].SetActive(true);
        }
    }
    #endregion

    #region Input Handling
    private void HandleMouseInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // ใช้ RaycastAll เพื่อได้ collider ทั้งหมดที่โดนคลิก
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);

        if (hits.Length > 0)
        {
            // เรียงตาม SortingOrder หรือ Z-position (ชิ้นที่อยู่ด้านหน้าสุดก่อน)
            System.Array.Sort(hits, (hit1, hit2) => {
                // เปรียบเทียบ SortingOrder ของ SpriteRenderer
                SpriteRenderer sr1 = hit1.collider.GetComponent<SpriteRenderer>();
                SpriteRenderer sr2 = hit2.collider.GetComponent<SpriteRenderer>();

                if (sr1 != null && sr2 != null)
                {
                    // SortingOrder สูงกว่า = อยู่ด้านหน้า = ควรได้รับการคลิกก่อน
                    return sr2.sortingOrder.CompareTo(sr1.sortingOrder);
                }

                // ถ้าไม่มี SpriteRenderer ให้เปรียบเทียบ Z position
                return hit2.transform.position.z.CompareTo(hit1.transform.position.z);
            });

            // เลือก hit แรก (ชิ้นที่อยู่ด้านหน้าสุด)
            HandleWorldClick(hits[0].collider.gameObject.layer);
        }
        else
        {
            HandleEmptySpaceClick();
        }
    }

    private void HandleWorldClick(int layerIndex)
    {
        string layerName = LayerMask.LayerToName(layerIndex);
        ShowActionPanel(layerName);
    }

    private void HandleEmptySpaceClick()
    {
        if (actionPanel != null && actionPanel.activeSelf && !IsPointerOverUI())
        {
            Debug.Log("[InteractionManager] ซ่อนเมนู - คลิกข้างนอก");
            HideActionPanel();
        }
    }

    private bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
    #endregion

    #region UI Management
    private void ShowActionPanel(string layerName)
    {
        ClothingPiece clothingPiece = GetClothingPieceByLayer(layerName);
        if (clothingPiece == null) return;

        currentClothingPiece = clothingPiece;
        DisplayActionPanel();
        CreateActionButtons();
    }

    private ClothingPiece GetClothingPieceByLayer(string layerName)
    {
        // ตรวจสอบทุกชิ้นเสื้อผ้า (ใช้ชิ้นปกติเป็นตัวแทน)
        if (layerName == shirt.interactionLayer) return shirt;
        if (layerName == pants.interactionLayer) return pants;
        if (layerName == underwear.interactionLayer) return underwear;
        if (layerName == legs.interactionLayer) return legs;
        return null;
    }

    private void DisplayActionPanel()
    {
        actionPanel.SetActive(true);
        actionText.text = currentClothingPiece.name;
        ClearExistingButtons();
    }

    private void CreateActionButtons()
    {
        int buttonIndex = 0;

        foreach (string actionString in currentClothingPiece.availableActions)
        {
            if (ShouldShowAction(actionString))
            {
                CreateActionButton(actionString, buttonIndex);
                buttonIndex++;
            }
        }
    }

    private void CreateActionButton(string actionString, int index)
    {
        GameObject buttonObj = Instantiate(buttonPrefab, actionPanel.transform);
        Button button = buttonObj.GetComponent<Button>();
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        button.onClick.AddListener(() => OnActionButtonClick(actionString));
        buttonText.text = GetActionDisplayText(actionString);

        PositionButton(buttonObj, index);
        spawnedButtons.Add(button);
    }

    private void PositionButton(GameObject buttonObj, int index)
    {
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, -100 * index, 0);
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
        foreach (Button button in spawnedButtons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }
        spawnedButtons.Clear();
    }
    #endregion

    #region Action Logic
    private bool ShouldShowAction(string actionString)
    {
        if (currentClothingPiece == null || !actionTypeMap.ContainsKey(actionString))
            return false;

        ActionType actionType = actionTypeMap[actionString];

        return actionType switch
        {
            ActionType.TakeOff => currentClothingPiece.CanBeRemovedFurther(),
            ActionType.Dress => currentClothingPiece.CanBeDressed(),
            ActionType.SpreadLegs => !isLegsOpen,
            ActionType.CloseLegs => isLegsOpen,
            ActionType.FingerInside or ActionType.JerkOff => true,
            _ => false
        };
    }

    private string GetActionDisplayText(string actionString)
    {
        if (actionTypeMap.ContainsKey(actionString))
        {
            ActionType actionType = actionTypeMap[actionString];
            return actionDisplayTexts.GetValueOrDefault(actionType, actionString);
        }
        return actionString;
    }

    public void OnActionButtonClick(string actionString)
    {
        if (!actionTypeMap.ContainsKey(actionString))
        {
            Debug.LogWarning($"[InteractionManager] ไม่รู้จักคำสั่ง: {actionString}");
            return;
        }

        ActionType actionType = actionTypeMap[actionString];
        ExecuteAction(actionType);
        RefreshActionPanel();
    }

    private void ExecuteAction(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.TakeOff:
                HandleTakeOffAction();
                break;
            case ActionType.Dress:
                HandleDressAction();
                break;
            case ActionType.SpreadLegs:
                HandleSpreadLegsAction();
                break;
            case ActionType.CloseLegs:
                HandleCloseLegsAction();
                break;
            case ActionType.FingerInside:
                HandleFingerInsideAction();
                break;
            case ActionType.JerkOff:
                HandleJerkOffAction();
                break;
        }
    }

    private void HandleTakeOffAction()
    {
        if (!currentClothingPiece.CanBeRemovedFurther())
        {
            Debug.LogWarning($"[InteractionManager] {currentClothingPiece.name} ถอดหมดแล้ว");
            return;
        }

        currentClothingPiece.RemoveOneLevel();
        SynchronizeWithSpreadVersion();
        UpdateRelatedClothing();

        Debug.Log($"[InteractionManager] ถอด {currentClothingPiece.name} level: {currentClothingPiece.currentLevel}");
    }

    private void HandleDressAction()
    {
        if (!currentClothingPiece.CanBeDressed())
        {
            Debug.LogWarning($"[InteractionManager] {currentClothingPiece.name} ใส่เต็มที่แล้ว");
            return;
        }

        currentClothingPiece.ResetToFullyDressed();
        SynchronizeWithSpreadVersion();
        UpdateRelatedClothing();

        Debug.Log($"[InteractionManager] ใส่ {currentClothingPiece.name} กลับ");
    }

    private void SynchronizeWithSpreadVersion()
    {
        // ซิงค์ level กับชิ้นขาเปิด
        if (currentClothingPiece == shirt)
            shirtSpread.currentLevel = shirt.currentLevel;
        else if (currentClothingPiece == pants)
            pantsSpread.currentLevel = pants.currentLevel;
        else if (currentClothingPiece == underwear)
            underwearSpread.currentLevel = underwear.currentLevel;
    }

    private void UpdateRelatedClothing()
    {
        if (currentClothingPiece == shirt)
            UpdateClothingDisplay(shirt, shirtSpread);
        else if (currentClothingPiece == pants)
            UpdateClothingDisplay(pants, pantsSpread);
        else if (currentClothingPiece == underwear)
            UpdateClothingDisplay(underwear, underwearSpread);
    }

    private void HandleSpreadLegsAction()
    {
        isLegsOpen = true;
        UpdateAllClothingDisplay();
        Debug.Log("[InteractionManager] กางขา - เปลี่ยนเป็นโหมดขาเปิด");
    }

    private void HandleCloseLegsAction()
    {
        isLegsOpen = false;
        UpdateAllClothingDisplay();
        Debug.Log("[InteractionManager] หุบขา - เปลี่ยนเป็นโหมดขาปิด");
    }

    private void HandleFingerInsideAction()
    {
        Debug.Log("[InteractionManager] สอดนิ้วเข้าหี");
    }

    private void HandleJerkOffAction()
    {
        Debug.Log("[InteractionManager] ชักว่าว");
    }

    private void RefreshActionPanel()
    {
        if (currentClothingPiece != null)
        {
            ShowActionPanel(currentClothingPiece.interactionLayer);
        }
    }
    #endregion

    #region Clothing State Management
    public void UpdateClothingWithLegsState(ClothingPiece piece, bool isLegsOpen)
    {
        if (piece == legs)
        {
            UpdateLegsState(isLegsOpen);
        }
        else
        {
            // สำหรับชิ้นอื่นๆ ใช้ระบบปกติ
            piece.UpdateDisplayState(isLegsOpen, this);
        }
    }

    private void UpdateLegsState(bool isLegsOpen)
    {
        if (legs.levels.Length < 2)
        {
            Debug.LogError("[InteractionManager] ขาต้องมีอย่างน้อย 2 levels (ปิด/เปิด)");
            return;
        }

        legs.levels[0].SetActive(!isLegsOpen); // ขาปิด
        legs.levels[1].SetActive(isLegsOpen);   // ขาเปิด
    }
    #endregion
}