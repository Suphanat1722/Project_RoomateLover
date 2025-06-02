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

        public void UpdateState(bool isLegsPiece = false, bool isLegsOpen = false, ClothingPiece pairedPiece = null, InteractionManager manager = null)
        {
            if (levels == null || levels.Length == 0)
            {
                Debug.LogError($"Levels array for {name} is not set or empty!");
                return;
            }

            if (isLegsPiece)
            {
                if (levels.Length < 2)
                {
                    Debug.LogError($"Levels array for {name} must have at least 2 elements for legs (closed and open states)!");
                    return;
                }

                levels[0].SetActive(!isLegsOpen); // ขาปิด
                levels[1].SetActive(isLegsOpen);  // ขาเปิด
            }
            else if (name == "Shirt" || name == "ShirtSpread")
            {
                // เรียกฟังก์ชันที่จัดการทั้ง Shirt และ ShirtSpread
                if (manager != null)
                {
                    manager.UpdateShirtStates(isLegsOpen);
                }
            }
            else
            {
                // สำหรับชิ้นส่วนอื่น (เช่น pants, underwear): ใช้ currentLevel
                for (int i = 0; i < levels.Length; i++)
                {
                    if (levels[i] != null)
                    {
                        levels[i].SetActive(i == currentLevel);
                    }
                    else
                    {
                        Debug.LogWarning($"Level {i} for {name} is null!");
                    }
                }
            }
        }

        public bool IsFullyRemoved()
        {
            return currentLevel >= maxLevel;
        }
    }

    [Header("Interaction Areas")]
    public ClothingPiece shirt;
    public ClothingPiece shirtSpread;
    public ClothingPiece pants;
    public ClothingPiece underwear;
    public ClothingPiece legs;

    [Header("UI Elements")]
    public GameObject actionPanel;
    public GameObject buttonPrefab;
    public TextMeshProUGUI actionText;

    private ClothingPiece currentClothingPiece;
    private Vector2 lastClickPosition;
    private List<Button> spawnedButtons = new List<Button>();
    private bool isLegsOpen = false;

    private void Start()
    {
        // ทำให้ shirt และ shirtSpread ใช้ currentLevel ร่วมกัน
        shirtSpread.currentLevel = shirt.currentLevel;

        // อัปเดตสถานะเริ่มต้น
        shirt.UpdateState(false, isLegsOpen, null, this);
        pants.UpdateState();
        underwear.UpdateState();
        legs.UpdateState(true, isLegsOpen);

        // ตรวจสอบเพิ่มเติมเพื่อให้แน่ใจว่า shirtSpread ปิดในตอนเริ่มต้น
        if (!isLegsOpen)
        {
            for (int i = 0; i < shirtSpread.levels.Length; i++)
            {
                if (shirtSpread.levels[i] != null)
                {
                    shirtSpread.levels[i].SetActive(false);
                }
            }
        }

        if (actionPanel != null)
        {
            actionPanel.SetActive(false);
        }
    }

    private void UpdateShirtStates(bool isLegsOpen)
    {
        // บังคับปิดทุก levels ของทั้ง Shirt และ ShirtSpread ก่อน
        for (int i = 0; i < shirt.levels.Length; i++)
        {
            if (shirt.levels[i] != null)
            {
                shirt.levels[i].SetActive(false);
            }
        }
        for (int i = 0; i < shirtSpread.levels.Length; i++)
        {
            if (shirtSpread.levels[i] != null)
            {
                shirtSpread.levels[i].SetActive(false);
            }
        }

        // อัปเดตตาม isLegsOpen
        if (isLegsOpen)
        {
            // เปิด ShirtSpread ตาม currentLevel
            for (int i = 0; i < shirtSpread.levels.Length; i++)
            {
                if (shirtSpread.levels[i] != null)
                {
                    bool shouldBeActive = (i == shirtSpread.currentLevel);
                    shirtSpread.levels[i].SetActive(shouldBeActive);
                }
            }
        }
        else
        {
            // เปิด Shirt ตาม currentLevel
            for (int i = 0; i < shirt.levels.Length; i++)
            {
                if (shirt.levels[i] != null)
                {
                    bool shouldBeActive = (i == shirt.currentLevel);
                    shirt.levels[i].SetActive(shouldBeActive);
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                lastClickPosition = mousePos;
                string layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
                ShowActionPanel(layerName);
            }
            else
            {
                if (actionPanel != null && actionPanel.activeSelf && !IsPointerOverUI())
                {
                    Debug.Log("Hiding action panel due to click outside");
                    actionPanel.SetActive(false);
                    ClearButtons();
                }
            }
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

    private void ShowActionPanel(string layerName)
    {
        ClearButtons();
        currentClothingPiece = null;

        if (layerName == shirt.interactionLayer)
            currentClothingPiece = shirt;
        else if (layerName == pants.interactionLayer)
            currentClothingPiece = pants;
        else if (layerName == underwear.interactionLayer)
            currentClothingPiece = underwear;
        else if (layerName == legs.interactionLayer)
            currentClothingPiece = legs;

        if (currentClothingPiece != null)
        {
            actionPanel.SetActive(true);
            actionText.text = currentClothingPiece.name;

            int buttonIndex = 0;
            foreach (string action in currentClothingPiece.availableActions)
            {
                if (ShouldShowAction(action))
                {
                    GameObject buttonObj = Instantiate(buttonPrefab, actionPanel.transform);
                    Button button = buttonObj.GetComponent<Button>();
                    TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

                    button.onClick.AddListener(() => OnActionButtonClick(action));

                    buttonText.text = GetActionDisplayText(action);

                    buttonObj.GetComponent<RectTransform>().localPosition = new Vector3(0, -100 * buttonIndex, 0);
                    buttonIndex++;

                    spawnedButtons.Add(button);
                }
            }
        }
    }

    private bool ShouldShowAction(string action)
    {
        if (currentClothingPiece == null) return false;

        switch (action)
        {
            case "TakeOff":
                return !currentClothingPiece.IsFullyRemoved();
            case "Dress":
                return currentClothingPiece.currentLevel > 0;
            case "SpreadLegs":
                return !isLegsOpen;
            case "CloseLegs":
                return isLegsOpen;
            case "FingerInside":
            case "JerkOff":
                return true;
            default:
                return false;
        }
    }

    private string GetActionDisplayText(string action)
    {
        switch (action)
        {
            case "TakeOff": return "ถอนชุด";
            case "Dress": return "สวมกลับ";
            case "SpreadLegs": return "กางขา";
            case "CloseLegs": return "หุบขา";
            case "FingerInside": return "สอดนิ้ว";
            case "JerkOff": return "ชักว่าว";
            default: return action;
        }
    }

    public void OnActionButtonClick(string action)
    {
        switch (action)
        {
            case "TakeOff":
                if (currentClothingPiece.currentLevel < currentClothingPiece.maxLevel)
                {
                    currentClothingPiece.currentLevel++;
                    if (currentClothingPiece == shirt)
                    {
                        shirtSpread.currentLevel = currentClothingPiece.currentLevel;
                    }
                    currentClothingPiece.UpdateState(currentClothingPiece == legs, isLegsOpen, null, this);
                    if (currentClothingPiece == shirt)
                    {
                        UpdateShirtStates(isLegsOpen); // อัปเดตทั้ง Shirt และ ShirtSpread
                    }
                }
                else
                {
                    Debug.LogWarning($"{currentClothingPiece.name} is already fully removed!");
                }
                break;
            case "Dress":
                if (currentClothingPiece.currentLevel > 0)
                {
                    currentClothingPiece.currentLevel = 0;
                    if (currentClothingPiece == shirt)
                    {
                        shirtSpread.currentLevel = currentClothingPiece.currentLevel;
                    }
                    Debug.Log($"Reset {currentClothingPiece.name} level to {currentClothingPiece.currentLevel}");
                    currentClothingPiece.UpdateState(currentClothingPiece == legs, isLegsOpen, null, this);
                    if (currentClothingPiece == shirt)
                    {
                        UpdateShirtStates(isLegsOpen); // อัปเดตทั้ง Shirt และ ShirtSpread
                    }
                }
                else
                {
                    Debug.LogWarning($"{currentClothingPiece.name} is already fully dressed!");
                }
                break;
            case "SpreadLegs":
                isLegsOpen = true;
                legs.UpdateState(true, isLegsOpen);
                UpdateShirtStates(isLegsOpen);
                Debug.Log("กางขา และเปลี่ยนไปใช้ ShirtSpread");
                break;
            case "CloseLegs":
                isLegsOpen = false;
                legs.UpdateState(true, isLegsOpen);
                UpdateShirtStates(isLegsOpen);
                Debug.Log("หุบขา และเปลี่ยนกลับไปใช้ Shirt ปกติ");
                break;
            case "FingerInside":
                Debug.Log("สอดนิ้วเข้าจิ๋ม");
                break;
            case "JerkOff":
                Debug.Log("ชักว่าว");
                break;
        }

        ShowActionPanel(currentClothingPiece.interactionLayer);
    }

    private void ClearButtons()
    {
        foreach (Button button in spawnedButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }
        spawnedButtons.Clear();
    }
}