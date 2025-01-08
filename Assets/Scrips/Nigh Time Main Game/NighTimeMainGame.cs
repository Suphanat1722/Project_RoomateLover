using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class NighTimeMainGame : MonoBehaviour
{
    public GameObject shirt;
    public GameObject shorts;
    public GameObject shirtHighlight;
    public GameObject shortsHighlight;
    public GameObject shirtButton;
    public GameObject shortsButton;

    int moodCharacter; // -30 ถึง 30

    // standrad คือ 0 และ 3 คือไม่ได้สวมอยู่
    int shirtLevel = 0;
    int shortsLevel = 0;
    int underwearLevel = 0;

    string layerName;
    string layerNameForHighlight;

    private void Update()
    {
        if (moodCharacter >= 10 && moodCharacter < 20)
        {
            Debug.Log("เริ่มมีอารมณ์");
        }
        else if (moodCharacter >= 20 && moodCharacter < 25)
        {
            Debug.Log("มีอารมณ์ร่วม");
        }
        else if (moodCharacter >= 25 && moodCharacter < 30)
        {
            Debug.Log("ใกล้");
        }

        //เช็คว่าสวมเสื้อผ้าอยู่ level ไหน
        if (shirtLevel == 3)
        {
            shirt.SetActive(false);
            //Debug.Log("ถอดเสื้อแล้ว");
        }
        else if (shirtLevel < 3)
        {
            shirt.SetActive(true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            GetHoveredLayerName();
            ShowButtonClilkOnCharacter();
        }

        HighlightInteraction();
    }

    private void GetHoveredLayerName() 
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        //เช็คว่าเมาส์ชี้ที่ layer อะไร
        if (hit.collider != null)
        {
           layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
        }
    }

    public void ShowButtonClilkOnCharacter()
    {
        if (layerName == "Shirt")
        {
            shirtButton.SetActive(true);
        }
        else if (layerName == "Shorts")
        {
            shortsButton.SetActive(true);
        }
    }

    private void HighlightInteraction()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null)
        {
            layerNameForHighlight = LayerMask.LayerToName(hit.collider.gameObject.layer);
        }

        if (hit.collider != null && layerNameForHighlight == "Shirt")
        {
            shirtHighlight.SetActive(true);
        } 
        else if (hit.collider != null && layerNameForHighlight == "Shorts")
        {
            shortsHighlight.SetActive(true);
        }
        else
        {
            shirtHighlight.SetActive(false);
            shortsHighlight.SetActive(false);
        }
    }

    public void TakeOffClothes()
    {
        moodCharacter = moodCharacter - 10;

        if (layerName == "Shirt")
        {
            shirtLevel++;
        }
        else if (layerName == "Shorts")
        {
            shortsLevel++; 
        }
        else if (layerName == "Underwear")
        {

        }
    }

    public void DressingShirts()
    {
        shirtLevel--;
    }

    public void Touch()
    {
        moodCharacter = moodCharacter + 3;
    }

    int beforeStatus; //standard คือ 1
    int currentStatus;

    public void DebugForTest(string action, string result, int beforeStatus, int currentStatus)
    {
        this.beforeStatus = beforeStatus;
        this.currentStatus = currentStatus;

        Debug.Log($"การกระทำ : {action} state : {beforeStatus}");
        Debug.Log($"ผลลัพธ์   : {result} state : {currentStatus}");
    }
}
