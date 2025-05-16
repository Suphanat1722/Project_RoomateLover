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

    float selectCumCountDown = 5f;

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

        // การการปุ่มเลือกน้ำแตก
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
        if (currentLayerName == "Head"|| currentLayerName == "Breast L" || currentLayerName == "Pussy" || currentLayerName == "Toy"  || currentLayerName == "Cancel")
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
            case "Toy":
                useToyButton.SetActive(true);
                selectedLayerRight = "Toy";
                break;
            case "cancel":
                ResetAllButtons(0);
                selectedLayerLeft = "Left Hand";
                break;
        }
    }
    private void ShowUiButtonRight()
    {
        // ตรวจสอบว่า isInsertDickInPussy เป็นจริงหรือไม่
        if (isInsertDickInPussy)
        {
            if (currentLayerName != "Fuck" && currentLayerName != "Cum" && currentLayerName != "Cancel")
            {
                currentLayerName = previousLayerNameWhenInserted; // กลับไปใช้ค่าเดิม
            }
            else
            {
                previousLayerNameWhenInserted = currentLayerName; // อัปเดตค่า previousLayerNameWhenInserted ถ้าเป็นค่าที่อนุญาต
            }
            ResetAllButtons(2);
            // ถ้าเป็นจริง, เราจำกัดให้ currentLayerName เป็นเพียง "Fuck", "Cum", หรือ "Cancel" เท่านั้น
            switch (currentLayerName)
            {
                case "Fuck":
                    fuckButton.SetActive(true);
                    selectedLayerRight = "Pussy";
                    break;
                case "Cum":
                    cumButton.SetActive(true);
                    selectedLayerRight = "Select Cum";
                    break;
                case "Cancel":
                    ResetAllButtons(2);
                    selectedLayerRight = "Right Hand";
                    break;
                // ถ้าไม่ใช่ "Fuck", "Cum", หรือ "Cancel", เราจะไม่ทำอะไรเลย
                default:
                    return; // หยุดการทำงานของฟังก์ชันทันที
            }
        }
        else
        {
            // ถ้า isInsertDickInPussy เป็นเท็จ, ทำงานตามปกติ
            if (currentLayerName == "Head" || currentLayerName == "Breast R" || currentLayerName == "Body Upper" ||
                currentLayerName == "Body Under" || currentLayerName == "Legs" || currentLayerName == "Pussy" ||
                currentLayerName == "Fuck" || currentLayerName == "Cum" || currentLayerName == "Cancel")
            {
                ResetAllButtons(2);
            }
            switch (currentLayerName)
            {
                case "Head":
                    headRubRightButton.SetActive(true);
                    selectedLayerRight = "Head";
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
                case "Cum":
                    cumButton.SetActive(true);
                    selectedLayerRight = "Select Cum";
                    break;
                case "Cancel":
                    ResetAllButtons(2);
                    selectedLayerRight = "Right Hand";
                    break;
            }
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
            cumButton.SetActive(false);
        }
        else if (index == 1)
        {
            headRubLeftButton.SetActive(false);
            breastLbutton.SetActive(false);
            closedLegsPussyLeftButton.SetActive(false);
            openLegsPussyLeftButton.SetActive(false);
            useToyButton.SetActive(false);
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
            fuckButton.SetActive(false);
            cumButton.SetActive(false);
        }
        
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
    }
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
    private void HandleCumSelection()
    {
        if (isInsertDickInPussy && !isMoveMoveFast)
        {
            feelingSystem.CalculatePlayerArousal(100);

            // เมื่อถึงจุดสุดยอด จะให้เวลา 5 วิในการเลือกจุดที่ต้องการเสร็จ
            if (feelingSystem.playerArousal.currentValue >= feelingSystem.playerArousal.maxValue)
            {
                if (!isSelectCumInside && !isSelectCumOutside)
                {
                    currentLayerName = "Cum";
                }
                else if (isSelectCumInside && !isSelectCumOutside)
                {
                    currentLayerName = "Fuck";
                }
                else
                {
                    currentLayerName = "Cancel";
                }

                ShowUiButtonLeft();
                ShowUiButtonRight();
                feelingSystem.SetPlayerSexEnergy(1);

                selectCumCountDown -= Time.deltaTime;
                //เมื่อเลือกไม่ทันจะแตกใน
                if (selectCumCountDown <= 0 && !isSelectCumInside && !isSelectCumOutside)
                {
                    Debug.Log("แตกใน เอาออกไม่ทัน");
                    OnCumInsideButtonClick();
                    currentLayerName = "Fuck";
                    ResetCumSelection();
                }
                else if (selectCumCountDown <= 0 && isSelectCumInside)
                {
                    Debug.Log("แตกใน");
                    ResetCumSelection();
                }
                else if (selectCumCountDown <= 0 && isSelectCumOutside)
                {
                    Debug.Log("แตกนอก");
                    isInsertDickInPussy = false;
                    ResetCumSelection();
                }
            }
        }
    }
    private void ResetCumSelection()
    {
        selectCumCountDown = 5;
        feelingSystem.ResetFeelingPlayerValues();
        isSelectCumInside = false;
        isSelectCumOutside = false;
        ShowUiButtonLeft();
        ShowUiButtonRight();
    }

    // ถอดเสื้อ
    public void TakeOffClothes()
    {
        StartCoroutine(ContinuousActionWhileTakeClothes());
        switch (currentLayerName)
        {
            case "Body Upper":
                shirt.currentLevel = Mathf.Clamp(shirt.currentLevel + 1, 0, shirt.maxLevel);
                currentLayerName = null;
                StartCoroutine(ContinueusResetButtons(0,2));
                break;
            case "Body Under":
                // กำหนดให้ เมื่อแหวกขาแล้ว จะถอดกางเกงได้ในครั้งเดียว
                if (isOpenLegs)
                {
                    if (shorts.currentLevel >= shorts.maxLevel)
                    {
                        underwear.currentLevel = underwear.maxLevel;
                        currentLayerName = null;
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
                        underwear.currentLevel = Mathf.Clamp(underwear.currentLevel + 1, 0, underwear.maxLevel);
                        currentLayerName = null;
                    }
                    else
                    {
                        shorts.currentLevel = Mathf.Clamp(shorts.currentLevel + 1, 0, shorts.maxLevel);
                    }
                }               
                currentLayerName = null;
                StartCoroutine(ContinueusResetButtons(0,2));
                break;
        }
    }
    // สวมเสื้อ
    public void DressClothes()
    {       
        switch (currentLayerName)
        {
            case "Body Upper":
                shirt.currentLevel = Mathf.Clamp(shirt.currentLevel - 1, 0, shirt.maxLevel);
                currentLayerName = null;
                StartCoroutine(ContinueusResetButtons(0,2));
                break;
            case "Body Under":
                // เพิ่มการตรวจสอบว่ากางเกงในถูกถอดแล้วหรือยังก่อนจะใส่กลับ
                if (underwear.currentLevel > 0)
                {
                    underwear.currentLevel = Mathf.Clamp(underwear.currentLevel - 1, 0, underwear.maxLevel);
                    currentLayerName = null;
                    StartCoroutine(ContinueusResetButtons(0, 2));
                }
                else if (shorts.currentLevel > 0)
                {
                    shorts.currentLevel = Mathf.Clamp(shorts.currentLevel - 1, 0, shorts.maxLevel);
                    currentLayerName = null;
                    StartCoroutine(ContinueusResetButtons(0, 2));
                }
                break;
        }
    }
    //ถ่างขา
    public void OpenLegs()
    {
        if (feelingSystem.feelGood.currentValue >= 30f)
        {
           // StartCoroutine(ContinuousActionWhileInserted());
            SetLegsState(true);
            currentLayerName = null;
            StartCoroutine(ContinueusResetButtons(0, 2)); 
        }
        else
        {
            Debug.Log("ต้องทำให้ผู้หญิงรู้สึกดีก่อน");
        }
    }
    // หุบขา
    public void ClosedLegs()
    {
        SetLegsState(false);
        currentLayerName = null;
        StartCoroutine(ContinueusResetButtons(0, 2)); // เปลี่ยนสถานะขาเป็นหุบ และจับเวลา 3 วินาที
    }
    // ลูบหัว
    public void OnHeadRubButtonClick()
    {
        // ลดค่าความรู้สึกแย่
        feelingSystem.CalculateFeelings(0f, -5f); // ค่าตัวอย่าง, ปรับตามความเหมาะสม ที่นี่ใช้ 0 เพื่อไม่เพิ่มค่าความรู้สึกดี      
    }
    // บีบ นม
    public void OnGrabBreastButtonClick()
    {     
        feelingSystem.CalculateFeelings(10f, 2f); // ค่าตัวอย่าง, ปรับตามความเหมาะสม     
    }
    //เลีย หัวนม
    public void OnLickBreastButtonClick()
    {    
        feelingSystem.CalculateFeelings(10f, 2f);        
    }
    // ถูๆจิมิ
    public void OnRubPussyButtonClick()
    {
        feelingSystem.CalculateFeelings(10f, 2f);
    }
    // ชักว่าว
    public void OnJerkOffButtonClick()
    {      
        feelingSystem.CalculateFeelings(10f, 2f); 
    }
    // สอดนิ้วเข้าไป
    public void OnFingerInsidePussyButtonClick()
    {        
        feelingSystem.CalculateFeelings(10f, 2f);    
    }
    // เลีย จิมิ
    public void OnLickPussyButtonClick()
    {       
        feelingSystem.CalculateFeelings(10f, 2f);     
    }
    // เอา ควย สอดใส่
    public void OnInsertDickInPussyButtonClick()
    {      
        currentLayerName = "Fuck";
        isInsertDickInPussy = true;
    }
    // เลือก ของเล่น
    public void OnUseToyButtonClick()
    {       
        currentLayerName = "Toy";
    }
    // ขยับ ควย ช้าๆ
    public void OnMoveSlowlyButtonClick()
    {       
        isMoveMoveFast = false;      
        //StartCoroutine(ContinuousActionWhileInserted()); // เริ่ม Coroutine      
    }
    //ขยับ ควย ไวๆ
    public void OnMoveFastButtonClick()
    {       
        isMoveMoveFast = true;       
        //StartCoroutine(ContinuousActionWhileInserted()); // เริ่ม Coroutine
    }
    //ดัน ควย เข้าแค่หัว
    public void OnPushShallowButtonClick()
    {   
        isPushPushDeep = false;
        feelingSystem.CalculateFeelings(8f, 1.5f);
    }
    //ดัน ควย เข้าลึกๆ
    public void OnPushDeepButtonClick()
    {
        isPushPushDeep = true;      
        feelingSystem.CalculateFeelings(20f, 4f);
    }
    //เอาควยออก
    public void OnPullOutButtonClick()
    {       
        currentLayerName = "Cancel";
        isInsertDickInPussy = false;
        StartCoroutine(ContinueusResetButtons(0f, 2));
    }
    //แตกนอก
    public void OnCumOutsideButtonClick()
    {        
        isSelectCumOutside = true;       
        Debug.Log("เลือก แตกนอก");
        currentLayerName = "Cancel";
    }
    //แตกใน
    public void OnCumInsideButtonClick()
    {      
        isSelectCumInside = true;      
        Debug.Log("เลือก แตกใน");
        currentLayerName="Cancel";
    }
    // เลือกใช้ Vibrator
    public void OnUseVibratorButtonClick()
    {
        feelingSystem.CalculateFeelings(15f, 3f); 
        StartCoroutine(ContinueusResetButtons(0f, 2));
    }
    // เลือกใช้ Egg Vibrator
    public void OnUseEggVibratorButtonClick()
    {
        feelingSystem.CalculateFeelings(12f, 2.5f); 
        StartCoroutine(ContinueusResetButtons(0f, 2));
    }

    private IEnumerator ContinueusResetButtons(float duration,int indexReset)
    {
        // ปิดปุ่มที่กำลังเปิดอยู่ทั้งหมด
        ResetAllButtons(indexReset);

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

            if (isTakeClothes)
            {
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }      
    }
    
}