using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Managers")]
    public ClothingManager clothingManager;
    public UIManager uiManager;
    public InputHandler inputHandler;
    public ActionHandler actionHandler;

    [Header("Auto Setup")]
    public bool autoFindManagers = true;

    void Awake()
    {
        SetupManagers();
    }

    void Start()
    {
        InitializeGame();
    }

    private void SetupManagers()
    {
        if (autoFindManagers)
        {
            // Try to find managers if not assigned
            if (clothingManager == null) clothingManager = FindAnyObjectByType<ClothingManager>();
            if (uiManager == null) uiManager = FindAnyObjectByType<UIManager>();
            if (inputHandler == null) inputHandler = FindAnyObjectByType<InputHandler>();
            if (actionHandler == null) actionHandler = FindAnyObjectByType<ActionHandler>();
        }

        // Validate all managers are present
        ValidateManagers();
    }

    private void ValidateManagers()
    {
        bool hasError = false;

        if (clothingManager == null)
        {
            Debug.LogError("[GameController] ClothingManager not found!");
            hasError = true;
        }

        if (uiManager == null)
        {
            Debug.LogError("[GameController] UIManager not found!");
            hasError = true;
        }

        if (inputHandler == null)
        {
            Debug.LogError("[GameController] InputHandler not found!");
            hasError = true;
        }

        if (actionHandler == null)
        {
            Debug.LogError("[GameController] ActionHandler not found!");
            hasError = true;
        }

        if (hasError)
        {
            Debug.LogError("[GameController] Some managers are missing. Game may not work properly.");
        }
    }

    private void InitializeGame()
    {
        Debug.Log("[GameController] Initializing game...");

        // Initialize all managers in proper order
        if (actionHandler != null && clothingManager != null)
        {
            actionHandler.Initialize(clothingManager);
        }

        if (uiManager != null && actionHandler != null)
        {
            uiManager.Initialize(actionHandler);
        }

        if (inputHandler != null && clothingManager != null && uiManager != null)
        {
            inputHandler.Initialize(clothingManager, uiManager);
        }

        // Update initial display
        if (clothingManager != null)
        {
            clothingManager.UpdateAllClothingDisplay();
        }

        Debug.Log("[GameController] Game initialization complete!");
    }

    // Public methods for external control
    public void ResetGame()
    {
        Debug.Log("[GameController] Resetting game...");

        if (clothingManager != null)
        {
            // Reset all clothing to fully dressed
            ResetAllClothing();
            clothingManager.UpdateAllClothingDisplay();
        }

        if (uiManager != null)
        {
            uiManager.HideActionPanel();
        }
    }

    private void ResetAllClothing()
    {
        clothingManager.shirt.ResetToFullyDressed();
        clothingManager.pants.ResetToFullyDressed();
        clothingManager.underwear.ResetToFullyDressed();
        clothingManager.isLegsOpen = false;
    }

    public void ToggleDebugMode()
    {
        // Add debug functionality here if needed
        Debug.Log("[GameController] Debug mode toggled");
    }

    // Getters for other scripts that might need access
    public ClothingManager ClothingManager => clothingManager;
    public UIManager UIManager => uiManager;
    public InputHandler InputHandler => inputHandler;
    public ActionHandler ActionHandler => actionHandler;
}