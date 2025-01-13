using UnityEngine;

public class ButtonNighttimeController : MonoBehaviour
{
    [Header("Character Components")]
    public GameObject shirt;
    public GameObject shorts;
    public GameObject underwear;

    public GameObject bodyUpperWearShirtCollider;
    public GameObject bodyUpperNotWearShirtCollider;
    public GameObject bodyUnderCollider;
    public GameObject breastCollider;

    public GameObject bodyUpperButton;
    public GameObject bodyUnderButton;
    public GameObject breastButton;

    private int shirtLevel;
    private int shortsLevel;
    private string currentLayerName;
    private string highlightLayerName;

    private int beforeStatus; // 1 = standard
    private int currentStatus;

    private void Update()
    {
        HandleMouseInput();
        HoverClothingShowUiButton();
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

            case "cancel":
                bodyUpperButton.SetActive(false);
                bodyUnderButton.SetActive(false);
                breastButton.SetActive(false);
                break;

            default:
                // No layer selected; keep all buttons inactive
                break;
        }
    }

    private void HoverClothingShowUiButton()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            highlightLayerName = LayerMask.LayerToName(hit.collider.gameObject.layer);

            if (highlightLayerName == "Body Upper")
            {
                if (!bodyUpperNotWearShirtCollider.activeSelf)
                {
                    ToggleSprite(bodyUpperWearShirtCollider, true);
                }
                else
                {
                    ToggleSprite(bodyUpperNotWearShirtCollider, true);
                }              
            }
            else if (highlightLayerName == "Body Under")
            {
                ToggleSprite(bodyUnderCollider, true);
            }
            else if (highlightLayerName == "Breast")
            {
                ToggleSprite(breastCollider, true);
            }
        }
        else
        {
            ToggleSprite(bodyUpperWearShirtCollider, false);
            ToggleSprite(bodyUpperNotWearShirtCollider, false);
            ToggleSprite(bodyUnderCollider, false);
            ToggleSprite(breastCollider, false);
        }
    }

    #endregion
    #region Clothing Methods

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

    private void CheckClothingLevels()
    {
        shirtLevel = Mathf.Clamp(shirtLevel, 0, 3);
        shortsLevel = Mathf.Clamp(shortsLevel, 0, 6);

        if (shirtLevel == 3)
        {
            shirt.SetActive(false);
            bodyUpperNotWearShirtCollider.SetActive(true);
            bodyUpperWearShirtCollider.SetActive(false);
            breastCollider.SetActive(true);
        }
        else
        {
            shirt.SetActive(true);
            bodyUpperNotWearShirtCollider.SetActive(false);
            bodyUpperWearShirtCollider.SetActive(true);
            breastCollider.SetActive(false);
        }

        if (shortsLevel >= 6)
        {
            shorts.SetActive(false);
            underwear.SetActive(false);
        }
        else if (shortsLevel >= 3)
        {
            shorts.SetActive(false);
            underwear.SetActive(true);
        }
        else
        {
            shorts.SetActive(true);
            underwear.SetActive(true);
        }
    }

    #endregion

    public void ToggleSprite(GameObject gameObj, bool isVisible)
    {
        SpriteRenderer spriteRenderer = gameObj.GetComponent<SpriteRenderer>();

        // ตรวจสอบว่ามี SpriteRenderer หรือไม่
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = isVisible; // true เพื่อเปิด, false เพื่อปิด
        }
    }
}
