using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    private InteractionManager interactionManager;
    private string action;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError($"Button component not found on {gameObject.name}");
        }
        else
        {
            Debug.Log($"Button component found on {gameObject.name}, Interactable: {button.interactable}");
        }
    }

    public void Initialize(InteractionManager manager, string actionName)
    {
        Debug.Log($"Initialize called for {gameObject.name} with action: {actionName}");
        if (manager == null || string.IsNullOrEmpty(actionName))
        {
            Debug.LogError($"Invalid initialization parameters for ActionButton on {gameObject.name}");
            return;
        }

        interactionManager = manager;
        action = actionName;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
            Debug.Log($"Added OnButtonClick listener to {gameObject.name}");
        }
        else
        {
            Debug.LogError($"Button is null on {gameObject.name}, cannot add listener!");
        }
    }

    private void OnButtonClick()
    {
        Debug.Log($"OnButtonClick called for action: {action} on {gameObject.name}");
        if (interactionManager != null)
        {
            Debug.Log($"Calling OnActionButtonClick with action: {action}");
            interactionManager.OnActionButtonClick(action);
        }
        else
        {
            Debug.LogError($"InteractionManager is null in ActionButton on {gameObject.name}");
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            //button.onClick.RemoveAllListeners();
            Debug.Log($"Removed listeners from {gameObject.name}");
        }
    }
}