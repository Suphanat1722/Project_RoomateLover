using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ClothingData
{
    [Header("Basic Info")]
    public string name;
    public ClothingType type;
    public GameObject[] levels;

    [Header("Current State")]
    public int currentLevel = 0;
    public int maxLevel = 3;

    [Header("Interaction Settings")]
    public string interactionLayer;
    public List<ActionType> availableActions = new List<ActionType>();
    public bool hasLegsVariant = false;

    // Properties for easy state checking
    public bool IsFullyRemoved => currentLevel >= maxLevel;
    public bool CanBeRemovedFurther => currentLevel < maxLevel;
    public bool CanBeDressed => currentLevel > 0;
    public bool IsFullyDressed => currentLevel == 0;

    // Methods for state manipulation
    public void ResetToFullyDressed() => currentLevel = 0;
    public void RemoveOneLevel()
    {
        if (CanBeRemovedFurther)
            currentLevel++;
    }
    public void DressOneLevel()
    {
        if (CanBeDressed)
            currentLevel--;
    }

    // Validation
    public bool IsValid()
    {
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError($"[ClothingData] {name} ไม่มี levels array!");
            return false;
        }
        if (string.IsNullOrEmpty(interactionLayer))
        {
            Debug.LogError($"[ClothingData] {name} ไม่มี interaction layer!");
            return false;
        }
        return true;
    }
}

public enum ClothingType
{
    Shirt,
    Pants,
    Underwear,
    Legs
}

public enum ActionType
{
    TakeOff,
    Dress,
    SpreadLegs,
    CloseLegs,
    FingerInside,
    JerkOff
}