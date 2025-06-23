using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject actionPanel;
    public GameObject buttonPrefab;
    public TextMeshProUGUI actionText;

    [Header("Button Settings")]
    public float buttonSpacing = 100f;

    private List<Button> spawnedButtons = new List<Button>();
    private ActionHandler actionHandler;
    private ClothingData currentClothing;

    public void Initialize(ActionHandler handler)
    {
        actionHandler = handler;
        HideActionPanel();
    }

    public void ShowActionPanel(ClothingData clothing)
    {
        if (clothing == null)
        {
            Debug.LogError("[UIManager] Cannot show action panel - clothing is null");
            return;
        }

        currentClothing = clothing;
        actionPanel.SetActive(true);
        actionText.text = clothing.name;

        ClearExistingButtons();
        CreateActionButtons();
    }

    public void HideActionPanel()
    {
        if (actionPanel != null)
        {
            actionPanel.SetActive(false);
            ClearExistingButtons();
        }
        currentClothing = null;
    }

    public void RefreshActionPanel()
    {
        if (currentClothing != null)
        {
            ShowActionPanel(currentClothing);
        }
    }

    private void CreateActionButtons()
    {
        if (currentClothing == null || actionHandler == null)
        {
            Debug.LogError("[UIManager] Cannot create buttons - missing dependencies");
            return;
        }

        List<ActionType> availableActions = actionHandler.GetAvailableActions(currentClothing);

        for (int i = 0; i < availableActions.Count; i++)
        {
            CreateSingleButton(availableActions[i], i);
        }
    }

    private void CreateSingleButton(ActionType actionType, int index)
    {
        if (buttonPrefab == null)
        {
            Debug.LogError("[UIManager] Button prefab is null");
            return;
        }

        GameObject buttonObj = Instantiate(buttonPrefab, actionPanel.transform);
        Button button = buttonObj.GetComponent<Button>();
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        if (button == null)
        {
            Debug.LogError("[UIManager] Button component not found in prefab");
            Destroy(buttonObj);
            return;
        }

        if (buttonText == null)
        {
            Debug.LogError("[UIManager] TextMeshProUGUI component not found in button prefab");
            Destroy(buttonObj);
            return;
        }

        // Setup button
        button.onClick.AddListener(() => OnActionButtonClick(actionType));
        buttonText.text = actionHandler.GetActionDisplayText(actionType);

        // Position button
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.localPosition = new Vector3(0,-buttonSpacing * index, 0);
        }

        spawnedButtons.Add(button);
    }

    private void OnActionButtonClick(ActionType actionType)
    {
        if (currentClothing == null || actionHandler == null)
        {
            Debug.LogError("[UIManager] Cannot execute action - missing dependencies");
            return;
        }

        actionHandler.ExecuteAction(actionType, currentClothing);
        RefreshActionPanel();
    }

    private void ClearExistingButtons()
    {
        foreach (Button button in spawnedButtons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }
        spawnedButtons.Clear();
    }

    public bool IsActionPanelVisible()
    {
        return actionPanel != null && actionPanel.activeSelf;
    }
}