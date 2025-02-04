using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonNighttimeController : MonoBehaviour
{
    [Header("Cloth")]
    public GameObject shirt;
    public GameObject shorts;
    public GameObject underwear;
    public GameObject hipClosed, hipOpened;
    public GameObject legClosed, legOpened;

    [Header("Part Collider")]
    public GameObject bodyUpperWearShirtCollider;
    public GameObject bodyUpperNotWearShirtCollider;
    public GameObject bodyUnderWearShortsCollider;
    public GameObject bodyUnderNotWearShortsCollider;
    public GameObject breastRightCollider, breastLeftCollider;
    public GameObject legClosedCollider, legOpenedCollider;
    public GameObject pussyClosedCollider, pussyOpenedCollider;

    [Header("UI Button")]
    public GameObject bodyUpperButton;
    public GameObject bodyUnderButton;
    public GameObject breastLbutton, breastRbutton;
    public GameObject legsButton;
    public GameObject closedLegsPussyLeftButton;
    public GameObject closedLegsPussyRightButton;
    public GameObject openLegsPussyLeftButton;
    public GameObject openLegsPussyRightButton;

    [Header("Text")]
    public TextMeshProUGUI textLeftHand;
    public TextMeshProUGUI textRightHand;

    private int shirtLevel;
    private int shortsLevel;
    private string currentLayerName;
    private string selectedLayerLeft = "Left Hand";
    private string selectedLayerRight = "Right Hand";
    private bool isSpreadLegs;

    private void Update()
    {
        HandleMouseInput();
        CheckClothingLevels();

        textLeftHand.text = selectedLayerLeft;
        textRightHand.text = selectedLayerRight;
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
            if (IsLayerValid(hit.collider.gameObject.layer))
            {
                currentLayerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
                Debug.Log("Current Layer Name: " + currentLayerName);
            }
        }
    }

    private bool IsLayerValid(int layer)
    {
        string[] validLayers = { "Body Upper", "Body Under", "Breast L", "Breast R", "Legs", "Pussy" };
        string layerName = LayerMask.LayerToName(layer);
        return System.Array.IndexOf(validLayers, layerName) != -1;
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
                selectedLayerRight = shortsLevel < 3 ? "กางเกง" : "กางเกงใน";
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
        if (currentLayerName == "Body Upper")
        {
            shirtLevel = Mathf.Clamp(shirtLevel + 1, 0, 3);
        }
        else if (currentLayerName == "Body Under")
        {
            shortsLevel = Mathf.Clamp(shortsLevel + 1, 0, 6);
        }
    }

    public void DressClothes()
    {
        if (currentLayerName == "Body Upper")
        {
            shirtLevel = Mathf.Clamp(shirtLevel - 1, 0, 3);
        }
        else if (currentLayerName == "Body Under")
        {
            shortsLevel = Mathf.Clamp(shortsLevel - 1, 0, 6);
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

    public void OpenLegs() => SetLegsState(true);
    public void ClosedLegs() => SetLegsState(false);

    #endregion

    private void CheckClothingLevels()
    {
        shirt.SetActive(shirtLevel != 3);
        bodyUpperWearShirtCollider.SetActive(shirtLevel != 3);
        bodyUpperNotWearShirtCollider.SetActive(shirtLevel == 3);
        breastLeftCollider.SetActive(shirtLevel == 3);
        breastRightCollider.SetActive(shirtLevel == 3);

        bool isUnderwearRemoved = shortsLevel >= 6;
        bool isShortsRemoved = shortsLevel >= 3;

        bodyUnderWearShortsCollider.SetActive(!isUnderwearRemoved);
        bodyUnderNotWearShortsCollider.SetActive(isUnderwearRemoved);

        legClosedCollider.SetActive(!isShortsRemoved);
        legOpenedCollider.SetActive(isUnderwearRemoved);

        pussyClosedCollider.SetActive(!isSpreadLegs && isUnderwearRemoved);
        pussyOpenedCollider.SetActive(isSpreadLegs && isUnderwearRemoved);

        shorts.SetActive(!isShortsRemoved);
        underwear.SetActive(isShortsRemoved && !isUnderwearRemoved);
    }
}