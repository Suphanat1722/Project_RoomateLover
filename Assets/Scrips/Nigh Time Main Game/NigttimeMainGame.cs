using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class NighttimeMainGame : MonoBehaviour
{
    #region Fields and Variables

    [Header("Character Components")]
    public GameObject shirt;
    public GameObject shorts;
    public GameObject underwear;
    public GameObject bodyUpper;
    public GameObject bodyUnder;
    public GameObject bodyUpperButton;
    public GameObject bodyUnderButton;

    private int moodCharacter; // Range: -30 to 30
    private int shirtLevel;
    private int shortsLevel;
    private string currentLayerName;
    private string highlightLayerName;

    private int beforeStatus; // 1 = standard
    private int currentStatus;

    #endregion

    #region Unity Methods

    private void Update()
    {
        HandleMoodStatus();
        HandleMouseInput();
        UpdateClothingHighlights();
        CheckClothingLevels();
    }

    #endregion

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

        // Activate the appropriate button based on the current layer
        switch (currentLayerName)
        {
            case "Body Upper":
                bodyUpperButton.SetActive(true);
                break;

            case "Body Under":
                bodyUnderButton.SetActive(true);
                break;

            case "cancel":
                bodyUpperButton.SetActive(false);
                bodyUnderButton.SetActive(false);
                break;

            default:
                // No layer selected; keep all buttons inactive
                break;
        }
    }

    private void UpdateClothingHighlights()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            highlightLayerName = LayerMask.LayerToName(hit.collider.gameObject.layer);

            if (highlightLayerName == "Body Upper")
            {
                ToggleBodyUpperSprite(bodyUpper,true);
            }
            else if (highlightLayerName == "Body Under")
            {
                ToggleBodyUpperSprite(bodyUnder, true);
            }
        }
        else
        {
            ToggleBodyUpperSprite(bodyUpper, false);
            ToggleBodyUpperSprite(bodyUnder, false);
        }
    }

    #endregion

    #region Clothing Methods

    public void TakeOffClothes()
    {
        moodCharacter -= 10;

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
        // Clamp shirtLevel and shortsLevel to valid range
        shirtLevel = Mathf.Clamp(shirtLevel, 0, 3);
        shortsLevel = Mathf.Clamp(shortsLevel, 0, 6);

        // Update shirt visibility based on shirtLevel
        shirt.SetActive(shirtLevel < 3);

        // Update shorts and underwear visibility based on shortsLevel
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

    public void TouchCharacter()
    {
        moodCharacter += 3;
    }

    #endregion

    #region Mood Handling

    private void HandleMoodStatus()
    {
        if (moodCharacter >= 10 && moodCharacter < 20)
        {
            Debug.Log("Mood: เริ่มมีอารมณ์");
        }
        else if (moodCharacter >= 20 && moodCharacter < 25)
        {
            Debug.Log("Mood: มีอารมณ์ร่วม");
        }
        else if (moodCharacter >= 25 && moodCharacter < 30)
        {
            Debug.Log("Mood: ใกล้ถึงจุดสุดยอด");
        }
    }

    #endregion


    public void ToggleBodyUpperSprite(GameObject gameObj, bool isVisible)
    {
        // เข้าถึง SpriteRenderer ของ bodyUpper
        SpriteRenderer spriteRenderer = gameObj.GetComponent<SpriteRenderer>();

        // ตรวจสอบว่ามี SpriteRenderer หรือไม่
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = isVisible; // true เพื่อเปิด, false เพื่อปิด
        }
        else
        {
            Debug.LogWarning("SpriteRenderer not found on bodyUpper!");
        }
    }

    #region Debugging

    public void DebugForTest(string action, string result, int before, int current)
    {
        beforeStatus = before;
        currentStatus = current;

        Debug.Log($"Action: {action} | Before State: {beforeStatus}");
        Debug.Log($"Result: {result} | Current State: {currentStatus}");
    }

    #endregion
}
