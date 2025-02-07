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

        //ค่าสถาณะต่างๆ ในทุกๆ 1 วิ
        if (shirt.currentLevel > 0)
        {
            isTakeClothes = true;
        }
        else
        {
            isTakeClothes = false;
        }
 
    }

    #region Interaction Methods
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonUp(0))
        { 
            DetectHoveredLayer();
            ShowUiButtonLeft();
            ShowUiButtonRight();
                   
        }
        if (Input.GetMouseButtonDown(1))
        {
            selectedLayerLeft = "Left Hand";
            selectedLayerRight = "Right Hand";
            ResetAllButtons(0);
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
        if (currentLayerName == "Head"|| currentLayerName == "Breast L" || currentLayerName == "Pussy")
        {
            ResetAllButtons(1);
        }
        switch (currentLayerName)
        {
            case "Head":
                headRubLeftButton.SetActive(true);
                selectedLayerLeft = "Head";
                break;
            case "Breast L":
                breastLbutton.SetActive(true);
                selectedLayerLeft = "Left Breast";
                break;
            case "Pussy":
                if (isOpenLegs)
                {
                    openLegsPussyLeftButton.SetActive(true);
                }
                else
                {
                    closedLegsPussyLeftButton.SetActive(true);
                }
                selectedLayerLeft = "Pussy";
                break;
            case "cancel":
                ResetAllButtons(0);
                selectedLayerLeft = "Left Hand";
                break;
        }
    }

    private void ShowUiButtonRight()
    {
        if (currentLayerName == "Head" || currentLayerName == "Breast R" || currentLayerName == "Body Upper" || 
            currentLayerName == "Body Under" || currentLayerName == "Legs" || currentLayerName == "Pussy" ||
            currentLayerName == "Fuck" || currentLayerName == "Toy" || currentLayerName == "Cum")
        {
            ResetAllButtons(2);
        }
        switch (currentLayerName)
        {
            case "Head":
                headRubRightButton.SetActive(true);
                selectedLayerLeft = "Head";
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
                if (isOpenLegs)
                {
                    openLegsPussyRightButton.SetActive(true);
                }
                else
                {
                    closedLegsPussyRightButton.SetActive(true);
                }
                selectedLayerRight = "Pussy";
                break;
            case "Fuck":
                fuckButton.SetActive(true);
                selectedLayerRight = "Pussy";
                break;
            case "Toy":
                useToyButton.SetActive(true);
                selectedLayerRight = "Toy";
                break;
            case "Cum":
                cumButton.SetActive(true);
                selectedLayerRight = "Select Cum";
                break;
            case "cancel":
                ResetAllButtons(0);
                selectedLayerRight = "Right Hand";
                break;
        }
    }

    private void ResetAllButtons(int index)
    {
        //0 = All, 1 = Left, 2 = Right
        if (index == 0)
        {
            headRubLeftButton.SetActive(false);
            headRubRightButton.SetActive(false);
            bodyUpperButton.SetActive(false);
            bodyUnderButton.SetActive(false);
            breastLbutton.SetActive(false);
            breastRbutton.SetActive(false);
            legsButton.SetActive(false);
            closedLegsPussyLeftButton.SetActive(false);
            closedLegsPussyRightButton.SetActive(false);
            openLegsPussyLeftButton.SetActive(false);
            openLegsPussyRightButton.SetActive(false);
            useToyButton.SetActive(false);
            fuckButton.SetActive(false);
        }
        else if (index == 1)
        {
            headRubLeftButton.SetActive(false);
            breastLbutton.SetActive(false);
            closedLegsPussyLeftButton.SetActive(false);
            openLegsPussyLeftButton.SetActive(false);
        }
        else if (index == 2)
        {
            headRubRightButton.SetActive(false);
            bodyUpperButton.SetActive(false);
            bodyUnderButton.SetActive(false);
            breastRbutton.SetActive(false);
            legsButton.SetActive(false);
            closedLegsPussyRightButton.SetActive(false);
            openLegsPussyRightButton.SetActive(false);
            useToyButton.SetActive(false);
            fuckButton.SetActive(false);
        }
        
    }
    #endregion

    #region Button Methods
    public void TakeOffClothes()
    {
        StartCoroutine(ContinuousActionWhileTakeClothes());
        switch (currentLayerName)
        {
            case "Body Upper":
                shirt.currentLevel = Mathf.Clamp(shirt.currentLevel + 1, 0, shirt.maxLevel);
                currentLayerName = null; // รีเซ็ต currentLayerName
                StartCoroutine(performAction(0.5f,2));
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
                StartCoroutine(performAction(0,2));
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
                StartCoroutine(performAction(0,2));
                break;
            case "Body Under":
                // เพิ่มการตรวจสอบว่ากางเกงในถูกถอดแล้วหรือยังก่อนจะใส่กลับ
                if (underwear.currentLevel > 0)
                {
                    underwear.currentLevel = Mathf.Clamp(underwear.currentLevel - 1, 0, underwear.maxLevel);
                    currentLayerName = null;
                    StartCoroutine(performAction(0, 2));
                }
                else if (shorts.currentLevel > 0)
                {
                    shorts.currentLevel = Mathf.Clamp(shorts.currentLevel - 1, 0, shorts.maxLevel);
                    currentLayerName = null;
                    StartCoroutine(performAction(0, 2));
                }
                break;
        }
    }

    private void SetLegsState(bool spread)
    {       
        isOpenLegs = spread;

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
        StartCoroutine(ContinuousActionWhileInserted());
        SetLegsState(true);
        currentLayerName = null;
        StartCoroutine(performAction(0, 2)); // เปลี่ยนสถานะขาเป็นกาง และจับเวลา 3 วินาที
    }

    public void ClosedLegs()
    {
        SetLegsState(false);
        currentLayerName = null;
        StartCoroutine(performAction(0, 2)); // เปลี่ยนสถานะขาเป็นหุบ และจับเวลา 3 วินาที
    }

    public void OnHeadRubButtonClick()
    {
        // ลดค่าความรู้สึกแย่
        feelingSystem.CalculateFeelings(0f, -5f); // ค่าตัวอย่าง, ปรับตามความเหมาะสม ที่นี่ใช้ 0 เพื่อไม่เพิ่มค่าความรู้สึกดี
                                                  // เพิ่มค่าความรู้สึกดีเล็กน้อยถ้าต้องการ
                                                  // feelingSystem.CalculateFeelings(2f, -5f);
        feelingSystem.IncreasePlayerArousal(2f); // เพิ่มค่าความเสียวของผู้เล่นเล็กน้อย
       
    }

    public void OnGrabBreastButtonClick()
    {
        feelingSystem.CalculateFeelings(10f, 2f); // ค่าตัวอย่าง, ปรับตามความเหมาะสม
        feelingSystem.IncreasePlayerArousal(5f); // เพิ่มค่าความเสียวของผู้เล่น        
    }

    public void OnLickBreastButtonClick()
    {
        feelingSystem.CalculateFeelings(10f, 2f);
        feelingSystem.IncreasePlayerArousal(5f); 
        
    }
    
    public void OnRubPussyButtonClick()
    {
        feelingSystem.CalculateFeelings(10f, 2f);
        feelingSystem.IncreasePlayerArousal(5f);
       
    }
    public void OnJerkOffButtonClick()
    {
        feelingSystem.CalculateFeelings(10f, 2f);
        feelingSystem.IncreasePlayerArousal(5f);
        
    }
    public void OnFingerInsidePussyButtonClick()
    {
        feelingSystem.CalculateFeelings(10f, 2f);
        feelingSystem.IncreasePlayerArousal(5f);
       
    }
    public void OnLickPussyButtonClick()
    {
        feelingSystem.CalculateFeelings(10f, 2f);
        feelingSystem.IncreasePlayerArousal(5f);
      
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
        // การทำงานเมื่อกดปุ่มขยับเบาๆ
        StartCoroutine(ContinuousActionWhileInserted()); // เริ่ม Coroutine      
    }

    public void OnMoveFastButtonClick()
    {
        isMoveMoveFast = true;
        // การทำงานเมื่อกดปุ่มขยับเร็วๆ
        StartCoroutine(ContinuousActionWhileInserted()); // เริ่ม Coroutine
    }

    public void OnPushShallowButtonClick()
    {
        isPushPushDeep = false;
        feelingSystem.CalculateFeelings(8f, 1.5f);
        feelingSystem.IncreasePlayerArousal(3f);
    }

    public void OnPushDeepButtonClick()
    {
        isPushPushDeep = true;      
        feelingSystem.CalculateFeelings(20f, 4f);
        feelingSystem.IncreasePlayerArousal(15f);
    }

    public void OnPullOutButtonClick()
    {
        isInsertDickInPussy = false;
        StartCoroutine(performAction(0f, 2));
    }

    public void OnCumOutsideButtonClick()
    {
        //แตกนอก
        isInsertDickInPussy = false;
        StartCoroutine(performAction(0f, 0));
        Debug.Log("แตกนอก");
    }

    public void OnCumInsideButtonClick()
    {
        //แตกใน
        isInsertDickInPussy = false;
        StartCoroutine(performAction(0f, 0));
        Debug.Log("แตกใน");
    }

    public void OnUseVibratorButtonClick()
    {
        feelingSystem.CalculateFeelings(15f, 3f); // ค่าความรู้สึกดีสูงกว่าเล็กน้อย
        feelingSystem.IncreasePlayerArousal(7f);
        StartCoroutine(performAction(0f, 2));
    }

    public void OnUseEggVibratorButtonClick()
    {
        feelingSystem.CalculateFeelings(12f, 2.5f); // ค่าความรู้สึกดีน้อยกว่าเล็กน้อย
        feelingSystem.IncreasePlayerArousal(6f);
        StartCoroutine(performAction(0f, 2));
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

        pussyClosedCollider.SetActive(!isOpenLegs && isUnderwearRemoved);
        pussyOpenedCollider.SetActive(isOpenLegs && isUnderwearRemoved);
    }

    private IEnumerator performAction(float duration,int index)
    {
        // ปิดปุ่มที่กำลังเปิดอยู่ทั้งหมด
        ResetAllButtons(index);

        // รอเวลาที่กำหนด
        yield return new WaitForSeconds(duration);

        // หลังจากเวลาผ่านไปแล้ว, สามารถเปิดปุ่มกลับมาได้หรือทำอย่างอื่นตามที่ต้องการ
        // ตรงนี้คุณอาจต้องการเรียก ShowUiButtons() หรือทำการอัปเดต UI อื่นๆ
    }

    private IEnumerator ContinuousActionWhileTakeClothes()
    {
        while (isTakeClothes)
        {
            feelingSystem.CalculateFeelings(2f, 5f);
            feelingSystem.IncreasePlayerArousal(0.5f);

            if (isTakeClothes)
            {
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }      
    }
    private IEnumerator ContinuousActionWhileInserted()
    {
        if (isInsertDickInPussy)
        {         
            int countDown = 5; // รีเซ็ตค่านับถอยหลัง
            while (!isMoveMoveFast)
            {
                feelingSystem.CalculateFeelings(10f, 2f);
                feelingSystem.IncreasePlayerArousal(5f);

                // ตรวจสอบว่าค่าความรู้สึกดีถึงขีดสูงสุดหรือยัง
                if (feelingSystem.GetFeelGoodValue() >= feelingSystem.feelGood.maxValue)
                {
                    selectedLayerRight = "Cum";
                    countDown--;
                    if (countDown <= 0 && selectedLayerRight == "Cum")
                    {
                        OnCumInsideButtonClick();
                        yield break; // ออกจาก Coroutine เมื่อน้ำแตก
                    }
                }

                yield return new WaitForSeconds(1f);
            }
            while (isMoveMoveFast)
            {
                feelingSystem.CalculateFeelings(15f, 10f);
                feelingSystem.IncreasePlayerArousal(10f);

                if (feelingSystem.GetFeelGoodValue() >= feelingSystem.feelGood.maxValue)
                {
                    OnCumInsideButtonClick();
                    yield break; // ออกจาก Coroutine เมื่อน้ำแตก
                }

                yield return new WaitForSeconds(1f);
            }
        }
    }
}