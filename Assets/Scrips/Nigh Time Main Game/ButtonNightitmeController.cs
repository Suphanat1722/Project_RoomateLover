using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Feeling System")]
    public FeelingSystem feelingSystem;

    private string currentLayerName;
    private string selectedLayerLeft = "Left Hand";
    private string selectedLayerRight = "Right Hand";
    private bool isSpreadLegs = false;

    [Header("UI Elements for Feelings")]
    public Slider feelGoodSlider;
    public Slider feelBadSlider;

    private void Update()
    {
        HandleMouseInput();
        CheckClothingLevels();

        textLeftHand.text = selectedLayerLeft;
        textRightHand.text = selectedLayerRight;

        // อัปเดตค่าของ Slider ตามค่าความรู้สึกดี
        if (feelGoodSlider != null)
        {
            feelGoodSlider.value = feelingSystem.GetFeelGoodValue() / feelingSystem.feelGood.maxValue;
        }

        // อัปเดตค่าของ Slider สำหรับความรู้สึกแย่
        if (feelBadSlider != null)
        {
            feelBadSlider.value = feelingSystem.GetFeelBadValue() / feelingSystem.feelBad.maxValue;
        }
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
            currentLayerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
        }
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
                currentLayerName = null; // รีเซ็ต currentLayerName
                StartCoroutine(performAction(0.5f));
                break;
            case "Body Under":
                // เพิ่มการตรวจสอบว่ากางเกงถูกถอดหมดแล้วหรือยังก่อนจะถอดกางเกงใน
                if (shorts.currentLevel >= shorts.maxLevel)
                {
                    underwear.currentLevel = Mathf.Clamp(underwear.currentLevel + 1, 0, underwear.maxLevel);
                    currentLayerName = null;
                }
                else
                {
                    shorts.currentLevel = Mathf.Clamp(shorts.currentLevel + 1, 0, shorts.maxLevel);
                }
                currentLayerName = null; // รีเซ็ต currentLayerName
                StartCoroutine(performAction(0));
                break;
        }
    }

    public void DressClothes()
    {
        switch (currentLayerName)
        {
            case "Body Upper":
                shirt.currentLevel = Mathf.Clamp(shirt.currentLevel - 1, 0, shirt.maxLevel);
                currentLayerName = null;
                StartCoroutine(performAction(0));
                break;
            case "Body Under":
                // เพิ่มการตรวจสอบว่ากางเกงในถูกถอดแล้วหรือยังก่อนจะใส่กลับ
                if (underwear.currentLevel > 0)
                {
                    underwear.currentLevel = Mathf.Clamp(underwear.currentLevel - 1, 0, underwear.maxLevel);
                    currentLayerName = null;
                    StartCoroutine(performAction(0));
                }
                else if (shorts.currentLevel > 0)
                {
                    shorts.currentLevel = Mathf.Clamp(shorts.currentLevel - 1, 0, shorts.maxLevel);
                    currentLayerName = null;
                    StartCoroutine(performAction(0));
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

    public void OpenLegs()
    {
        SetLegsState(true);
        currentLayerName = null;
        StartCoroutine(performAction(0)); // เปลี่ยนสถานะขาเป็นกาง และจับเวลา 3 วินาที
    }

    public void ClosedLegs()
    {
        SetLegsState(false);
        currentLayerName = null;
        StartCoroutine(performAction(0)); // เปลี่ยนสถานะขาเป็นหุบ และจับเวลา 3 วินาที
    }

    public void OnGrabBreastButtonClick()
    {
        // เพิ่มค่าความรู้สึกดีและแย่เมื่อจับนม
        feelingSystem.CalculateFeelings();      
        StartCoroutine(performAction(3f)); // ตัวอย่างการใช้งาน Coroutine
    }

    public void OnLickBreastButtonClick()
    {
        // เพิ่มค่าความรู้สึกดีและแย่เมื่อเลียนม
        feelingSystem.CalculateFeelings();
        StartCoroutine(performAction(3f));
    }
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

    private IEnumerator performAction(float duration)
    {
        // ปิดปุ่มที่กำลังเปิดอยู่ทั้งหมด
        ResetAllButtons();

        // รอเวลาที่กำหนด
        yield return new WaitForSeconds(duration);

        // หลังจากเวลาผ่านไปแล้ว, สามารถเปิดปุ่มกลับมาได้หรือทำอย่างอื่นตามที่ต้องการ
        // ตรงนี้คุณอาจต้องการเรียก ShowUiButtons() หรือทำการอัปเดต UI อื่นๆ
    }


}