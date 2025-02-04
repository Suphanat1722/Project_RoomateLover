using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonNighttimeController : MonoBehaviour
{
    [System.Serializable]
    public class ClothingItem
    {
        public GameObject clothing;
        public GameObject colliderWorn;
        public GameObject colliderNotWorn;
        public int maxLevel;
        public int currentLevel = 0;

        public void UpdateState()
        {
            clothing.SetActive(currentLevel < maxLevel);
            colliderWorn.SetActive(currentLevel < maxLevel);
            colliderNotWorn.SetActive(currentLevel >= maxLevel);
        }
    }

    [Header("Clothing")]
    public ClothingItem shirt;
    public ClothingItem shorts;
    public ClothingItem underwear;

    [Header("Legs State")]
    public GameObject hipClosed, hipOpened;
    public GameObject legClosed, legOpened;
    public GameObject legClosedCollider, legOpenedCollider;
    public GameObject pussyClosedCollider, pussyOpenedCollider;

    [Header("Part Collider")]
    public GameObject breastRightCollider, breastLeftCollider;

    [Header("UI Button")]
    public GameObject bodyUpperButton;
    public GameObject bodyUnderButton;
    public GameObject breastLbutton, breastRbutton;
    public GameObject legsButton;
    public GameObject closedLegsPussyLeftButton, closedLegsPussyRightButton;
    public GameObject openLegsPussyLeftButton, openLegsPussyRightButton;

    [Header("Text")]
    public TextMeshProUGUI textLeftHand;
    public TextMeshProUGUI textRightHand;

    private string currentLayerName;
    private string selectedLayerLeft = "Left Hand";
    private string selectedLayerRight = "Right Hand";
    private bool isSpreadLegs = false;

    private void Update()
    {
        HandleMouseInput();
        CheckClothingLevels();

        textLeftHand.text = selectedLayerLeft;
        textRightHand.text = selectedLayerRight;
    }

    #region Interaction Methods

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            selectedLayerLeft = "Left Hand";
            selectedLayerRight = "Right Hand";

            DetectHoveredLayer();
            ShowUiButtons();
        }
        if (Input.GetMouseButtonDown(1))
        {
            currentLayerName = "cancel";
            ShowUiButtons();
        }
    }

    private void DetectHoveredLayer()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            if (IsLayerValid(hit.collider.gameObject.layer))
            {
                currentLayerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
                Debug.Log("Current Layer Name: " + currentLayerName);
            }
        }
    }

    private bool IsLayerValid(int layer)
    {
        string[] validLayers = { "Body Upper", "Body Under", "Breast L", "Breast R", "Legs", "Pussy" };
        string layerName = LayerMask.LayerToName(layer);
        return System.Array.IndexOf(validLayers, layerName) != -1;
    }

    private void ShowUiButtons()
    {
        ResetAllButtons();

        switch (currentLayerName)
        {
            case "Breast L":
                breastLbutton.SetActive(true);
                selectedLayerLeft = "Left Breast";
                break;
            case "Breast R":
                breastRbutton.SetActive(true);
                selectedLayerRight = "Right Breast";
                break;
            case "Body Upper":
                bodyUpperButton.SetActive(true);
                selectedLayerRight = "เสื้อ";
                break;
            case "Body Under":
                bodyUnderButton.SetActive(true);
                selectedLayerRight = shorts.currentLevel < shorts.maxLevel ? "กางเกง" : "กางเกงใน";
                break;
            case "Legs":
                legsButton.SetActive(true);
                selectedLayerRight = "Legs";
                break;
            case "Pussy":
                if (isSpreadLegs)
                {
                    openLegsPussyLeftButton.SetActive(true);
                    openLegsPussyRightButton.SetActive(true);
                }
                else
                {
                    closedLegsPussyLeftButton.SetActive(true);
                    closedLegsPussyRightButton.SetActive(true);
                }
                selectedLayerLeft = selectedLayerRight = "Pussy";
                break;
            case "cancel":
                selectedLayerLeft = "Left Hand";
                selectedLayerRight = "Right Hand";
                break;
        }
    }

    private void ResetAllButtons()
    {
        bodyUpperButton.SetActive(false);
        bodyUnderButton.SetActive(false);
        breastLbutton.SetActive(false);
        breastRbutton.SetActive(false);
        legsButton.SetActive(false);
        closedLegsPussyLeftButton.SetActive(false);
        closedLegsPussyRightButton.SetActive(false);
        openLegsPussyLeftButton.SetActive(false);
        openLegsPussyRightButton.SetActive(false);
    }

    #endregion

    #region Button Methods

    public void TakeOffClothes()
    {
        switch (currentLayerName)
        {
            case "Body Upper":
                shirt.currentLevel = Mathf.Clamp(shirt.currentLevel + 1, 0, shirt.maxLevel);
                break;
            case "Body Under":
                // เพิ่มการตรวจสอบว่ากางเกงถูกถอดหมดแล้วหรือยังก่อนจะถอดกางเกงใน
                if (shorts.currentLevel >= shorts.maxLevel)
                {
                    underwear.currentLevel = Mathf.Clamp(underwear.currentLevel + 1, 0, underwear.maxLevel);
                }
                else
                {
                    shorts.currentLevel = Mathf.Clamp(shorts.currentLevel + 1, 0, shorts.maxLevel);
                }
                break;
        }
    }

    public void DressClothes()
    {
        switch (currentLayerName)
        {
            case "Body Upper":
                shirt.currentLevel = Mathf.Clamp(shirt.currentLevel - 1, 0, shirt.maxLevel);
                break;
            case "Body Under":
                // เพิ่มการตรวจสอบว่ากางเกงในถูกถอดแล้วหรือยังก่อนจะใส่กลับ
                if (underwear.currentLevel > 0)
                {
                    underwear.currentLevel = Mathf.Clamp(underwear.currentLevel - 1, 0, underwear.maxLevel);
                }
                else if (shorts.currentLevel > 0)
                {
                    shorts.currentLevel = Mathf.Clamp(shorts.currentLevel - 1, 0, shorts.maxLevel);
                }
                break;
        }
    }

    private void SetLegsState(bool spread)
    {
        isSpreadLegs = spread;

        hipClosed.SetActive(!spread);
        hipOpened.SetActive(spread);

        legClosed.SetActive(!spread);
        legOpened.SetActive(spread);

        legClosedCollider.SetActive(!spread);
        legOpenedCollider.SetActive(spread);

        pussyClosedCollider.SetActive(!spread);
        pussyOpenedCollider.SetActive(spread);
    }

    public void OpenLegs() => SetLegsState(true);
    public void ClosedLegs() => SetLegsState(false);

    #endregion

    private void CheckClothingLevels()
    {
        shirt.UpdateState();
        shorts.UpdateState();
        underwear.UpdateState();

        bool isUnderwearRemoved = underwear.currentLevel >= underwear.maxLevel;
        bool isShortsRemoved = shorts.currentLevel >= shorts.maxLevel;

        breastLeftCollider.SetActive(shirt.currentLevel >= shirt.maxLevel);
        breastRightCollider.SetActive(shirt.currentLevel >= shirt.maxLevel);

        legClosedCollider.SetActive(!isShortsRemoved);
        legOpenedCollider.SetActive(isUnderwearRemoved);

        pussyClosedCollider.SetActive(!isSpreadLegs && isUnderwearRemoved);
        pussyOpenedCollider.SetActive(isSpreadLegs && isUnderwearRemoved);
    }
}