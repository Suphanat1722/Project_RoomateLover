using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [Header("Dependencies")]
    private InteractionManager interactionManager;
    private string actionName;
    private Button buttonComponent;

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeButton();
    }

    private void OnDestroy()
    {
        CleanupListeners();
    }
    #endregion

    #region Public Methods
    public void Initialize(InteractionManager manager, string action)
    {
        if (!ValidateInitializationParameters(manager, action))
            return;

        SetupButton(manager, action);
        RegisterClickListener();
    }
    #endregion

    #region Private Methods
    private void InitializeButton()
    {
        buttonComponent = GetComponent<Button>();

        if (buttonComponent == null)
        {
            Debug.LogError($"[ActionButton] Button component missing on {gameObject.name}");
        }
        else
        {
            Debug.Log($"[ActionButton] Button found on {gameObject.name}, Interactable: {buttonComponent.interactable}");
        }
    }

    private bool ValidateInitializationParameters(InteractionManager manager, string action)
    {
        if (manager == null || string.IsNullOrEmpty(action))
        {
            Debug.LogError($"[ActionButton] Invalid parameters for {gameObject.name} - Manager: {manager != null}, Action: {!string.IsNullOrEmpty(action)}");
            return false;
        }
        return true;
    }

    private void SetupButton(InteractionManager manager, string action)
    {
        interactionManager = manager;
        actionName = action;
        Debug.Log($"[ActionButton] Initialized {gameObject.name} with action: {actionName}");
    }

    private void RegisterClickListener()
    {
        if (buttonComponent == null)
        {
            Debug.LogError($"[ActionButton] Cannot register listener - Button is null on {gameObject.name}");
            return;
        }

        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(HandleButtonClick);
        Debug.Log($"[ActionButton] Listener registered for {gameObject.name}");
    }

    private void HandleButtonClick()
    {
        Debug.Log($"[ActionButton] Button clicked: {actionName} on {gameObject.name}");

        if (interactionManager == null)
        {
            Debug.LogError($"[ActionButton] InteractionManager is null on {gameObject.name}");
            return;
        }

        interactionManager.OnActionButtonClick(actionName);
    }

    private void CleanupListeners()
    {
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();
            Debug.Log($"[ActionButton] Cleaned up listeners for {gameObject.name}");
        }
    }
    #endregion
}