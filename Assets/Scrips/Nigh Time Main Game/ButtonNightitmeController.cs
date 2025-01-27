using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonNighttimeController : MonoBehaviour
{
    [Header("Cloth")]
    public GameObject shirt;
    public GameObject shorts;
    public GameObject underwear;
    public GameObject hip1,hip2;
    public GameObject leg1,leg2;

    [Header("Part Collider")]
    public GameObject bodyUpperWearShirtCollider;
    public GameObject bodyUpperNotWearShirtCollider;
    public GameObject bodyUnderWearShortsCollider;
    public GameObject bodyUnderNotWearShortsCollider;
    public GameObject breastCollider;
    public GameObject leg1Collider, leg2Collider;
    public GameObject pussy1Collider, pussy2Collider;


    [Header("UI Button")]
    public GameObject bodyUpperButton;
    public GameObject bodyUnderButton;
    public GameObject breastButton;
    public GameObject legsButton;
    public GameObject pussy1Button, pussy2Button;

    [Header("Text")]
    public TextMeshProUGUI textSelectLayerName;

    private int shirtLevel;
    private int shortsLevel;
    private string currentLayerName;
    private string currentSelectLayerName;
    private bool IsSpreadlegs;

    private int beforeStatus; // 1 = standard
    private int currentStatus;

    private void Update()
    {
        HandleMouseInput();
        CheckClothingLevels();

        textSelectLayerName.text = currentSelectLayerName;
    }

    #region Interaction Methods

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            DetectHoveredLayer();
            ShowClothingButtons();
        }
        if (Input.GetMouseButtonDown(1))
        {
            currentLayerName = "cancel";
            ShowClothingButtons();
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

    private void ShowClothingButtons()
    {
        // Reset all buttons to inactive
        bodyUpperButton.SetActive(false);
        bodyUnderButton.SetActive(false);
        breastButton.SetActive(false);
        legsButton.SetActive(false);
        pussy1Button.SetActive(false);
        pussy2Button.SetActive(false);

        // Activate the appropriate button based on the current layer
        switch (currentLayerName)
        {
            case "Body Upper":

                bodyUpperButton.SetActive(true);

                currentSelectLayerName = "เสื้อ";
                break;

            case "Body Under":

                bodyUnderButton.SetActive(true);

                if (shortsLevel < 3)
                {
                    currentSelectLayerName = "กางเกง";
                }
                else
                {
                    currentSelectLayerName = "กางเกงใน";
                }
                break;

            case "Breast":
                
                breastButton.SetActive(true);

                currentSelectLayerName = "หน้าอก";
                break;

            case "Legs":
                
                legsButton.SetActive(true);

                currentSelectLayerName = "ขา";
                break;

            case "Pussy":

                if (!IsSpreadlegs)
                {
                    pussy1Button.SetActive(true);
                }
                else
                {
                    pussy2Button.SetActive(true);
                }

                currentSelectLayerName = "จิ๋ม";
                break;
            case "cancel":

                currentSelectLayerName = "เลือก";

                bodyUpperButton.SetActive(false);
                bodyUnderButton.SetActive(false);
                breastButton.SetActive(false);
                legsButton.SetActive(false);
                pussy1Button.SetActive(false);
                pussy2Button.SetActive(false);
                break;

            default:
                // No layer selected; keep all buttons inactive
                break;
        }
    }
    #endregion
    #region Button Methods

    public void TakeOffClothes()
    {
        if (currentLayerName == "Body Upper")
        {
            shirtLevel++;
        }
        else if (currentLayerName == "Body Under")
        {
            shortsLevel++;
        }
    }

    public void DressClothes()
    {
        if (currentLayerName == "Body Upper")
        {
            shirtLevel--;
        }
        else if (currentLayerName == "Body Under")
        {
            shortsLevel--;
        }
    }

    public void SpreadLegs()
    {
        IsSpreadlegs = true;

        hip1.SetActive(false);
        hip2.SetActive(true);

        leg1.SetActive(false);
        leg2.SetActive(true);

        leg1Collider.SetActive(false);
        leg2Collider.SetActive(true);
    }
    
    public void FoldLegsBack() 
    {
        IsSpreadlegs = false;

        hip1.SetActive(true);
        hip2.SetActive(false);

        leg1.SetActive(true);
        leg2.SetActive(false);

        leg1Collider.SetActive(true);
        leg2Collider.SetActive(false);
    }
    #endregion

    private void CheckClothingLevels()
    {
        shirtLevel = Mathf.Clamp(shirtLevel, 0, 3);
        shortsLevel = Mathf.Clamp(shortsLevel, 0, 6);

        //เสื้อ
        if (shirtLevel == 3)
        {
            //ถอดเสื้อ

            //ส่วนเสื้อผ้า
            shirt.SetActive(false);
            //ส่วนร่างกาย
            bodyUpperWearShirtCollider.SetActive(false);
            bodyUpperNotWearShirtCollider.SetActive(true);          
            breastCollider.SetActive(true);
        }
        else
        {
            //ไม่ถอดเสื้อ

            //ส่วนเสื้อผ้า
            shirt.SetActive(true);

            //ส่วนร่างกาย
            bodyUpperWearShirtCollider.SetActive(true);
            bodyUpperNotWearShirtCollider.SetActive(false);        
            breastCollider.SetActive(false);
        }

        //กางเกง และ กางเกงใน
        if (shortsLevel >= 6)
        {
            //ถอดกางเกงใน

            //ส่วนร่างกาย
            bodyUnderWearShortsCollider.SetActive(false);
            bodyUnderNotWearShortsCollider.SetActive(true);         
            leg1Collider.SetActive(true);
            if (!IsSpreadlegs)
            {
                pussy1Collider.SetActive(true);
            }
            else
            {
                pussy2Collider.SetActive(true);
            }        

            //ส่วนเสื้อผ้า
            shorts.SetActive(false);
            underwear.SetActive(false);
        }
        else if (shortsLevel >= 3)
        {
            //ถอดกางเกง

            //ส่วนร่างกาย
            bodyUnderWearShortsCollider.SetActive(true);
            bodyUnderNotWearShortsCollider.SetActive(false);
            leg1Collider.SetActive(false);
            pussy1Collider.SetActive(false);
            pussy2Collider.SetActive(false);

            //ส่วนเสื้อผ้า
            shorts.SetActive(false);
            underwear.SetActive(true);
        }
        else
        {
            //ไม่ถอดส่วนล่าง
            bodyUnderWearShortsCollider.SetActive(true);
            bodyUnderNotWearShortsCollider.SetActive(false);
            leg1Collider.SetActive(false);
            pussy1Collider.SetActive(false);
            pussy2Collider.SetActive(false);


            //ส่วนเสื้อผ้า
            shorts.SetActive(true);
            underwear.SetActive(true);
        }
    }
}
