using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour
{
    [Header("Raycast Settings")]
    public LayerMask interactionLayers = -1;

    private ClothingManager clothingManager;
    private UIManager uiManager;
    private Camera mainCamera;

    public void Initialize(ClothingManager clothing, UIManager ui)
    {
        clothingManager = clothing;
        uiManager = ui;
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("[InputHandler] Main camera not found!");
        }
    }

    void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        // Check if clicking on UI first
        if (IsClickingOnUI())
        {
            return; // Don't process world clicks when clicking UI
        }

        Vector2 mouseWorldPos = GetMouseWorldPosition();
        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero, Mathf.Infinity, interactionLayers);

        if (hits.Length > 0)
        {
            ProcessClothingHit(hits);
        }
        else
        {
            // Clicked on empty space
            if (uiManager.IsActionPanelVisible())
            {
                uiManager.HideActionPanel();
            }
        }
    }

    private Vector2 GetMouseWorldPosition()
    {
        if (mainCamera == null) return Vector2.zero;
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void ProcessClothingHit(RaycastHit2D[] hits)
    {
        RaycastHit2D bestHit = FindBestHit(hits);

        if (bestHit.collider == null) return;

        string layerName = LayerMask.LayerToName(bestHit.collider.gameObject.layer);

        if (!clothingManager.CanClickClothing(layerName))
        {
            Debug.Log($"[InputHandler] Cannot click {layerName} - prerequisites not met");
            return;
        }

        ClothingData clickedClothing = clothingManager.GetClothingByLayer(layerName);
        if (clickedClothing != null)
        {
            uiManager.ShowActionPanel(clickedClothing);
        }
    }

    private RaycastHit2D FindBestHit(RaycastHit2D[] hits)
    {
        // Priority order for clothing layers
        string[] priorityLayers = { "Shirt", "Pants", "Underwear", "Legs" };

        foreach (string layerName in priorityLayers)
        {
            RaycastHit2D hit = FindHitByLayer(hits, layerName);
            if (hit.collider != null && clothingManager.CanClickClothing(layerName))
            {
                return hit;
            }
        }

        // Return first hit if no priority match
        return hits.Length > 0 ? hits[0] : new RaycastHit2D();
    }

    private RaycastHit2D FindHitByLayer(RaycastHit2D[] hits, string targetLayerName)
    {
        foreach (RaycastHit2D hit in hits)
        {
            string hitLayerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
            if (hitLayerName == targetLayerName)
            {
                return hit;
            }
        }
        return new RaycastHit2D();
    }

    private bool IsClickingOnUI()
    {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
}