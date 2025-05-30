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

        public void UpdateState()
        {

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

        public bool IsFullyRemoved()
        {
            return currentLevel >= maxLevel;
        }
    }

    [Header("Interaction Areas")]
    public ClothingPiece shirt;
    public ClothingPiece pants;
    public ClothingPiece legs;
    public ClothingPiece pussy;

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
        shirt.UpdateState();
        pants.UpdateState();
        legs.UpdateState();
        pussy.UpdateState();

        if (actionPanel != null)
        {
            actionPanel.SetActive(false);
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
                // ตรวจสอบว่าคลิกที่ UI หรือไม่
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
        else if (layerName == legs.interactionLayer)
            currentClothingPiece = legs;
        else if (layerName == pussy.interactionLayer)
            currentClothingPiece = pussy;

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
                return currentClothingPiece.currentLevel > 0; // แก้ไข: ควรแสดงเมื่อ currentLevel > 0
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
                    currentClothingPiece.UpdateState();
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
                    Debug.Log($"Reset {currentClothingPiece.name} level to {currentClothingPiece.currentLevel}");
                    currentClothingPiece.UpdateState();
                }
                else
                {
                    Debug.LogWarning($"{currentClothingPiece.name} is already fully dressed!");
                }
                break;
            case "SpreadLegs":
                isLegsOpen = true;
                legs.UpdateState();
                Debug.Log("กางขา");
                break;
            case "CloseLegs":
                isLegsOpen = false;
                legs.UpdateState();
                Debug.Log("หุบขา");
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