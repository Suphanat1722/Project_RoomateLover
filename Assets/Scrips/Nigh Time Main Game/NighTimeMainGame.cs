using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class NighTimeMainGame : MonoBehaviour
{
    #region Fields and Variables

    [Header("Character Components")]
    public GameObject shirt;
    public GameObject shorts;
    public GameObject shirtHighlight;
    public GameObject shortsHighlight;
    public GameObject shirtButton;
    public GameObject shortsButton;
    public GameObject underwearButton;

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
        shirtButton.SetActive(false);
        shortsButton.SetActive(false);
        underwearButton.SetActive(false);

        // Activate the appropriate button based on the current layer
        switch (currentLayerName)
        {
            case "Shirt":
                shirtButton.SetActive(true);
                break;

            case "Shorts":
                shortsButton.SetActive(true);
                break;

            case "Underwear":
                underwearButton.SetActive(true);               
                break;

            case "cancel":
                shirtButton.SetActive(false);
                shortsButton.SetActive(false);
                underwearButton.SetActive(false);
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

            if (highlightLayerName == "Shirt")
            {
                shirtHighlight.SetActive(true);
            }
            else if (highlightLayerName == "Shorts")
            {
                shortsHighlight.SetActive(true);
            }
        }
        else
        {
            shirtHighlight.SetActive(false);
            shortsHighlight.SetActive(false);
        }
    }

    #endregion

    #region Clothing Methods

    public void TakeOffClothes()
    {
        moodCharacter -= 10;

        if (currentLayerName == "Shirt")
        {
            shirtLevel++;
        }
        else if (currentLayerName == "Shorts")
        {
            shortsLevel++;
        }
    }

    public void DressClothes()
    {
        if (currentLayerName == "Shirt")
        {
            shirtLevel--;
        }
        else if (currentLayerName == "Shorts")
        {
            shortsLevel--;
        }
    }

    private void CheckClothingLevels()
    {
        shirt.SetActive(shirtLevel < 3);
        shorts.SetActive(shortsLevel < 3);
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
