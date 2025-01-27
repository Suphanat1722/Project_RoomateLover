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
    public GameObject legsCollider;

    [Header("UI Button")]
    public GameObject bodyUpperButton;
    public GameObject bodyUnderButton;
    public GameObject breastButton;
    public GameObject legsButton;

    private int shirtLevel;
    private int shortsLevel;
    private string currentLayerName;
    private string highlightLayerName;

    private int beforeStatus; // 1 = standard
    private int currentStatus;

    private void Update()
    {
        HandleMouseInput();
        CheckClothingLevels();
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

        // Activate the appropriate button based on the current layer
        switch (currentLayerName)
        {
            case "Body Upper":
                bodyUpperButton.SetActive(true);
                break;

            case "Body Under":
                bodyUnderButton.SetActive(true);
                break;

            case "Breast":
                breastButton.SetActive(true);
                break;

            case "Legs":
                legsButton.SetActive(true);
                break;
            case "cancel":
                bodyUpperButton.SetActive(false);
                bodyUnderButton.SetActive(false);
                breastButton.SetActive(false);
                legsButton.SetActive(false);
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
        hip1.SetActive(false);
        hip2.SetActive(true);

        leg1.SetActive(false);
        leg2.SetActive(true);
    }
    
    public void FoldLegsBack() 
    {
        hip1.SetActive(true);
        hip2.SetActive(false);

        leg1.SetActive(true);
        leg2.SetActive(false);
    }
    #endregion

    private void CheckClothingLevels()
    {
        shirtLevel = Mathf.Clamp(shirtLevel, 0, 3);
        shortsLevel = Mathf.Clamp(shortsLevel, 0, 6);

        //เสื้อ
        if (shirtLevel == 3)
        {
            shirt.SetActive(false);

            bodyUpperWearShirtCollider.SetActive(false);
            bodyUpperNotWearShirtCollider.SetActive(true);
            
            breastCollider.SetActive(true);
        }
        else
        {
            shirt.SetActive(true);

            bodyUpperWearShirtCollider.SetActive(true);
            bodyUpperNotWearShirtCollider.SetActive(false);
            
            breastCollider.SetActive(false);
        }

        //กางเกง และ กางเกงใน
        if (shortsLevel >= 6)
        {
            bodyUnderWearShortsCollider.SetActive(false);
            bodyUnderNotWearShortsCollider.SetActive(true);         
            legsCollider.SetActive(true);

            shorts.SetActive(false);
            underwear.SetActive(false);
        }
        else if (shortsLevel >= 3)
        {
            bodyUnderWearShortsCollider.SetActive(true);
            bodyUnderNotWearShortsCollider.SetActive(false);
            legsCollider.SetActive(false);

            shorts.SetActive(false);
            underwear.SetActive(true);
        }
        else
        {
            bodyUnderWearShortsCollider.SetActive(true);
            bodyUnderNotWearShortsCollider.SetActive(false);
            legsCollider.SetActive(false);

            shorts.SetActive(true);
            underwear.SetActive(true);
        }
    }
}
