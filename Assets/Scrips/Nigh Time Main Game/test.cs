using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    [System.Serializable]
    public class ClothingItem
    {
        public GameObject shirtLevel_0, shirtLevel_1, shirtLevel_2;
        public GameObject clothing;
        public GameObject colliderWorn;
        public GameObject colliderNotWorn;
        public int maxLevel;
        public int currentLevel = 0;

        public void UpdateState(bool isShirt = false)
        {
            clothing.SetActive(currentLevel < maxLevel);
            colliderWorn.SetActive(currentLevel < maxLevel);
            colliderNotWorn.SetActive(currentLevel >= maxLevel);

            if (isShirt)
            {
                shirtLevel_0.SetActive(currentLevel == 0);
                shirtLevel_1.SetActive(currentLevel == 1);
                shirtLevel_2.SetActive(currentLevel == 2);
                Debug.Log($"shirt.currentLevel == {currentLevel}");
            }
        }
    }

    [Header("Clothing")]
    public ClothingItem shirt;
    public ClothingItem shorts;
    public ClothingItem underwear;

    [Header("Legs State")]
    public GameObject legClosed, legOpened;
    public GameObject legClosedCollider, legOpenedCollider;
    public GameObject pussyClosed, pussyOpened;
    public GameObject pussyClosedCollider, pussyOpenedCollider;

    [Header("Other body Collider")]
    public GameObject breastRightCollider;
    public GameObject breastLeftCollider;

    [Header("UI Button")]
    public GameObject headRubLeftButton;
    public GameObject headRubRightButton;
    public GameObject bodyUpperButton;
    public GameObject bodyUnderButton;
    public GameObject breastLbutton, breastRbutton;
    public GameObject legsButton;
    public GameObject closedLegsPussyLeftButton, closedLegsPussyRightButton;
    public GameObject openLegsPussyLeftButton, openLegsPussyRightButton;
    public GameObject useToyButton;
    public GameObject fuckButton;
    public GameObject cumButton;

    [Header("Text")]
    public TextMeshProUGUI textLeftHand;
    public TextMeshProUGUI textRightHand;

    [Header("Feeling System")]
    public FeelingSystem feelingSystem;
    public GameTime gameTime;
    public SceneController sceneController;

    [Header("UI Elements for Feelings")]
    public Slider feelGoodSlider;
    public Slider feelBadSlider;

    private string currentLayerName;
    private string selectedLayerLeft = "Left Hand";
    private string selectedLayerRight = "Right Hand";
    private bool isOpenLegs = false;
    private bool isMoveMoveFast = false;
    private bool isPushPushDeep = false;
    private bool isInsertDickInPussy;
    private bool isTakeClothes;
    private bool isSelectCumInside;
    private bool isSelectCumOutside;
    private string previousLayerNameWhenInserted = "Cancel";
    private float selectCumCountDown = 5f;

    private Dictionary<string, (GameObject button, string label, bool isLeft)> buttonMap;

    private void Awake()
    {
        buttonMap = new Dictionary<string, (GameObject, string, bool)>
        {
            { "HeadLeft", (headRubLeftButton, "Head", true) },
            { "HeadRight", (headRubRightButton, "Head", false) },
            { "BreastL", (breastLbutton, "Left Breast", true) },
            { "BreastR", (breastRbutton, "Right Breast", false) },
            { "BodyUpper", (bodyUpperButton, "เสื้อ", false) },
            { "BodyUnder", (bodyUnderButton, null, false) },
            { "Legs", (legsButton, "Legs", false) },
            { "PussyClosedLeft", (closedLegsPussyLeftButton, "Pussy", true) },
            { "PussyClosedRight", (closedLegsPussyRightButton, "Pussy", false) },
            { "PussyOpenLeft", (openLegsPussyLeftButton, "Pussy", true) },
            { "PussyOpenRight", (openLegsPussyRightButton, "Pussy", false) },
            { "Toy", (useToyButton, "Toy", true) },
            { "Fuck", (fuckButton, "Pussy", false) },
            { "Cum", (cumButton, "Select Cum", false) }
        };
    }

    private void Update()
    {
        HandleMouseInput();
        CheckClothingLevels();

        textLeftHand.text = selectedLayerLeft;
        textRightHand.text = selectedLayerRight;

        if (feelGoodSlider != null)
        {
            feelGoodSlider.value = feelingSystem.GetFeelGoodValue() / feelingSystem.feelGood.maxValue;
        }

        if (feelBadSlider != null)
        {
            feelBadSlider.value = feelingSystem.GetFeelBadValue() / feelingSystem.feelBad.maxValue;
        }

        HandleCumSelection();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            DetectHoveredLayer();
            ShowUiButtonLeft();
            ShowUiButtonRight();
            Debug.Log(currentLayerName);
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (!isInsertDickInPussy)
            {
                selectedLayerLeft = "Left Hand";
                selectedLayerRight = "Right Hand";
                ResetAllButtons(0);
            }
            else
            {
                selectedLayerLeft = "Left Hand";
                ResetAllButtons(1);
            }
        }
    }

    private void DetectHoveredLayer()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            currentLayerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
        }
    }

    private void ShowUiButtonLeft()
    {
        ResetLeftButtons();

        if (string.IsNullOrEmpty(currentLayerName) || new[] { "Head", "Breast L", "Pussy", "Toy", "Cancel" }.Contains(currentLayerName))
        {
            switch (currentLayerName)
            {
                case "Head":
                    ActivateButton("HeadLeft");
                    break;
                case "Breast L":
                    ActivateButton("BreastL");
                    break;
                case "Pussy":
                    ActivateButton(isOpenLegs ? "PussyOpenLeft" : "PussyClosedLeft");
                    break;
                case "Toy":
                    ActivateButton("Toy");
                    break;
                case "Cancel":
                    selectedLayerLeft = "Left Hand";
                    break;
            }
        }
    }

    private void ShowUiButtonRight()
    {
        ResetRightButtons();

        if (isInsertDickInPussy)
        {
            if (!new[] { "Fuck", "Cum", "Cancel" }.Contains(currentLayerName))
            {
                currentLayerName = previousLayerNameWhenInserted;
            }
            else
            {
                previousLayerNameWhenInserted = currentLayerName;
            }

            switch (currentLayerName)
            {
                case "Fuck":
                    ActivateButton("Fuck");
                    break;
                case "Cum":
                    ActivateButton("Cum");
                    break;
                case "Cancel":
                    selectedLayerRight = "Right Hand";
                    break;
                default:
                    return;
            }
        }
        else
        {
            if (string.IsNullOrEmpty(currentLayerName) || new[] { "Head", "Breast R", "Body Upper", "Body Under", "Legs", "Pussy", "Fuck", "Cum", "Cancel" }.Contains(currentLayerName))
            {
                switch (currentLayerName)
                {
                    case "Head":
                        ActivateButton("HeadRight");
                        break;
                    case "Breast R":
                        ActivateButton("BreastR");
                        break;
                    case "Body Upper":
                        ActivateButton("BodyUpper");
                        break;
                    case "Body Under":
                        buttonMap["BodyUnder"].button.SetActive(true);
                        selectedLayerRight = shorts.currentLevel < shorts.maxLevel ? "กางเกง" : "กางเกงใน";
                        break;
                    case "Legs":
                        ActivateButton("Legs");
                        break;
                    case "Pussy":
                        ActivateButton(isOpenLegs ? "PussyOpenRight" : "PussyClosedRight");
                        break;
                    case "Fuck":
                        ActivateButton("Fuck");
                        break;
                    case "Cum":
                        ActivateButton("Cum");
                        break;
                    case "Cancel":
                        selectedLayerRight = "Right Hand";
                        break;
                }
            }
        }
    }

    private void ActivateButton(string key)
    {
        var buttonData = buttonMap[key];
        buttonData.button.SetActive(true);
        if (buttonData.isLeft)
            selectedLayerLeft = buttonData.label;
        else
            selectedLayerRight = buttonData.label;
    }

    private void ResetAllButtons(int index)
    {
        if (index == 0 || index == 1)
        {
            ResetLeftButtons();
        }
        if (index == 0 || index == 2)
        {
            ResetRightButtons();
        }
    }

    private void ResetLeftButtons()
    {
        headRubLeftButton.SetActive(false);
        breastLbutton.SetActive(false);
        closedLegsPussyLeftButton.SetActive(false);
        openLegsPussyLeftButton.SetActive(false);
        useToyButton.SetActive(false);
    }

    private void ResetRightButtons()
    {
        headRubRightButton.SetActive(false);
        bodyUpperButton.SetActive(false);
        bodyUnderButton.SetActive(false);
        breastRbutton.SetActive(false);
        legsButton.SetActive(false);
        closedLegsPussyRightButton.SetActive(false);
        openLegsPussyRightButton.SetActive(false);
        fuckButton.SetActive(false);
        cumButton.SetActive(false);
    }

    private void SetLegsState(bool spread)
    {
        isOpenLegs = spread;

        legClosed.SetActive(!spread);
        legOpened.SetActive(spread);
        legClosedCollider.SetActive(!spread);
        legOpenedCollider.SetActive(spread);
        pussyClosed.SetActive(!spread);
        pussyOpened.SetActive(spread);
        pussyClosedCollider.SetActive(!spread);
        pussyOpenedCollider.SetActive(spread);

        currentLayerName = null;
        StartCoroutine(ContinueusResetButtons(0, 2));
    }

    private void CheckClothingLevels()
    {
        shirt.UpdateState(true);
        shorts.UpdateState();
        underwear.UpdateState();

        bool isUnderwearRemoved = underwear.currentLevel >= underwear.maxLevel;
        bool isShortsRemoved = shorts.currentLevel >= shorts.maxLevel;

        breastLeftCollider.SetActive(shirt.currentLevel >= shirt.maxLevel);
        breastRightCollider.SetActive(shirt.currentLevel >= shirt.maxLevel);

        legClosedCollider.SetActive(!isShortsRemoved);
        legOpenedCollider.SetActive(isUnderwearRemoved);

        pussyClosedCollider.SetActive(!isOpenLegs && isUnderwearRemoved);
        pussyOpenedCollider.SetActive(isOpenLegs && isUnderwearRemoved);

        isTakeClothes = shirt.currentLevel > 0;
    }

    private void HandleCumSelection()
    {
        if (isInsertDickInPussy && !isMoveMoveFast)
        {
            feelingSystem.CalculatePlayerArousal(100);

            if (feelingSystem.playerArousal.currentValue >= feelingSystem.playerArousal.maxValue)
            {
                if (!isSelectCumInside && !isSelectCumOutside)
                {
                    currentLayerName = "Cum";
                }
                else
                {
                    currentLayerName = isSelectCumInside ? "Fuck" : "Cancel";
                }

                ShowUiButtonLeft();
                ShowUiButtonRight();
                feelingSystem.SetPlayerSexEnergy(1);

                selectCumCountDown -= Time.deltaTime;
                if (selectCumCountDown <= 0)
                {
                    if (!isSelectCumInside && !isSelectCumOutside)
                    {
                        Debug.Log("แตกใน เอาออกไม่ทัน");
                        OnCumInsideButtonClick();
                    }
                    else if (isSelectCumInside)
                    {
                        Debug.Log("แตกใน");
                    }
                    else if (isSelectCumOutside)
                    {
                        Debug.Log("แตกนอก");
                        isInsertDickInPussy = false;
                    }
                    ResetCumSelection();
                }
            }
        }
    }

    private void ResetCumSelection()
    {
        selectCumCountDown = 5f;
        feelingSystem.ResetFeelingPlayerValues();
        isSelectCumInside = false;
        isSelectCumOutside = false;
        ShowUiButtonLeft();
        ShowUiButtonRight();
    }

    private void UpdateClothingLevel(ClothingItem item, int delta, bool resetButtons = true)
    {
        item.currentLevel = Mathf.Clamp(item.currentLevel + delta, 0, item.maxLevel);
        if (resetButtons)
        {
            StartCoroutine(ContinueusResetButtons(0, 2));
        }
    }

    public void SetShirtLevel(int level)
    {
        shirt.currentLevel = Mathf.Clamp(level, 0, shirt.maxLevel);
        shirt.UpdateState(true);
    }

    public void AddShirtLevel(int amountToAdd)
    {
        SetShirtLevel(shirt.currentLevel + amountToAdd);
    }

    public void TakeOffClothes()
    {
        StartCoroutine(ContinuousActionWhileTakeClothes());
        switch (currentLayerName)
        {
            case "Body Upper":
                UpdateClothingLevel(shirt, 1);
                break;
            case "Body Under":
                if (isOpenLegs)
                {
                    if (shorts.currentLevel >= shorts.maxLevel)
                    {
                        underwear.currentLevel = underwear.maxLevel;
                    }
                    else
                    {
                        shorts.currentLevel = shorts.maxLevel;
                    }
                }
                else
                {
                    if (shorts.currentLevel >= shorts.maxLevel)
                    {
                        UpdateClothingLevel(underwear, 1);
                    }
                    else
                    {
                        UpdateClothingLevel(shorts, 1);
                    }
                }
                break;
        }
        currentLayerName = null;
    }

    public void DressClothes()
    {
        switch (currentLayerName)
        {
            case "Body Upper":
                UpdateClothingLevel(shirt, -1);
                break;
            case "Body Under":
                if (underwear.currentLevel > 0)
                {
                    UpdateClothingLevel(underwear, -1);
                }
                else if (shorts.currentLevel > 0)
                {
                    UpdateClothingLevel(shorts, -1);
                }
                break;
        }
        currentLayerName = null;
    }

    public void OpenLegs()
    {
        if (feelingSystem.feelGood.currentValue >= 30f)
        {
            SetLegsState(true);
        }
        else
        {
            Debug.Log("ต้องทำให้ผู้หญิงรู้สึกดีก่อน");
        }
    }

    public void ClosedLegs()
    {
        SetLegsState(false);
    }

    private void ApplyFeeling(float good, float bad)
    {
        feelingSystem.CalculateFeelings(good, bad);
    }

    public void OnHeadRubButtonClick()
    {
        ApplyFeeling(0f, -5f);
    }

    public void OnGrabBreastButtonClick()
    {
        ApplyFeeling(10f, 2f);
    }

    public void OnLickBreastButtonClick()
    {
        ApplyFeeling(10f, 2f);
    }

    public void OnRubPussyButtonClick()
    {
        ApplyFeeling(10f, 2f);
    }

    public void OnJerkOffButtonClick()
    {
        ApplyFeeling(10f, 2f);
    }

    public void OnFingerInsidePussyButtonClick()
    {
        ApplyFeeling(10f, 2f);
    }

    public void OnLickPussyButtonClick()
    {
        ApplyFeeling(10f, 2f);
    }

    public void OnInsertDickInPussyButtonClick()
    {
        currentLayerName = "Fuck";
        isInsertDickInPussy = true;
    }

    public void OnUseToyButtonClick()
    {
        currentLayerName = "Toy";
    }

    public void OnMoveSlowlyButtonClick()
    {
        isMoveMoveFast = false;
    }

    public void OnMoveFastButtonClick()
    {
        isMoveMoveFast = true;
    }

    public void OnPushShallowButtonClick()
    {
        isPushPushDeep = false;
        ApplyFeeling(8f, 1.5f);
    }

    public void OnPushDeepButtonClick()
    {
        isPushPushDeep = true;
        ApplyFeeling(20f, 4f);
    }

    public void OnPullOutButtonClick()
    {
        currentLayerName = "Cancel";
        isInsertDickInPussy = false;
        StartCoroutine(ContinueusResetButtons(0f, 2));
    }

    public void OnCumOutsideButtonClick()
    {
        isSelectCumOutside = true;
        Debug.Log("เลือก แตกนอก");
        currentLayerName = "Cancel";
    }

    public void OnCumInsideButtonClick()
    {
        isSelectCumInside = true;
        Debug.Log("เลือก แตกใน");
        currentLayerName = "Cancel";
    }

    public void OnUseVibratorButtonClick()
    {
        ApplyFeeling(15f, 3f);
        StartCoroutine(ContinueusResetButtons(0f, 2));
    }

    public void OnUseEggVibratorButtonClick()
    {
        ApplyFeeling(12f, 2.5f);
        StartCoroutine(ContinueusResetButtons(0f, 2));
    }

    private IEnumerator ContinueusResetButtons(float duration, int indexReset)
    {
        ResetAllButtons(indexReset);
        yield return new WaitForSeconds(duration);
    }

    private IEnumerator ContinuousActionWhileTakeClothes()
    {
        while (isTakeClothes)
        {
            feelingSystem.CalculateFeelings(2f, 5f);

            if (isTakeClothes)
            {
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}