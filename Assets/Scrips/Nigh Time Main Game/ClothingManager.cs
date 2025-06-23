using UnityEngine;
using System.Collections.Generic;

public class ClothingManager : MonoBehaviour
{
    [Header("Clothing Pieces")]
    public ClothingData shirt;
    public ClothingData shirtSpread;
    public ClothingData pants;
    public ClothingData pantsSpread;
    public ClothingData underwear;
    public ClothingData underwearSpread;
    public ClothingData legs;

    [Header("Display Settings")]
    public bool isLegsOpen = false;

    // Dictionary for easy access
    private Dictionary<string, ClothingData> clothingByLayer;
    private Dictionary<ClothingType, ClothingPair> clothingPairs;

    public struct ClothingPair
    {
        public ClothingData normal;
        public ClothingData spread;
    }

    void Awake()
    {
        InitializeClothingData();
        SetupDictionaries();
    }

    private void InitializeClothingData()
    {
        // Set clothing types
        shirt.type = ClothingType.Shirt;
        shirtSpread.type = ClothingType.Shirt;
        pants.type = ClothingType.Pants;
        pantsSpread.type = ClothingType.Pants;
        underwear.type = ClothingType.Underwear;
        underwearSpread.type = ClothingType.Underwear;
        legs.type = ClothingType.Legs;

        // Set legs variants
        shirt.hasLegsVariant = true;
        pants.hasLegsVariant = true;
        underwear.hasLegsVariant = true;
        legs.hasLegsVariant = true;

        // Sync levels
        SyncSpreadVersions();

        // Validate all clothing data
        ValidateClothingData();
    }

    private void SetupDictionaries()
    {
        clothingByLayer = new Dictionary<string, ClothingData>
        {
            { shirt.interactionLayer, shirt },
            { pants.interactionLayer, pants },
            { underwear.interactionLayer, underwear },
            { legs.interactionLayer, legs }
        };

        clothingPairs = new Dictionary<ClothingType, ClothingPair>
        {
            { ClothingType.Shirt, new ClothingPair { normal = shirt, spread = shirtSpread } },
            { ClothingType.Pants, new ClothingPair { normal = pants, spread = pantsSpread } },
            { ClothingType.Underwear, new ClothingPair { normal = underwear, spread = underwearSpread } }
        };
    }

    private void ValidateClothingData()
    {
        ClothingData[] allClothing = { shirt, shirtSpread, pants, pantsSpread, underwear, underwearSpread, legs };
        foreach (var clothing in allClothing)
        {
            if (!clothing.IsValid())
            {
                Debug.LogError($"[ClothingManager] Invalid clothing data: {clothing.name}");
            }
        }
    }

    public ClothingData GetClothingByLayer(string layerName)
    {
        clothingByLayer.TryGetValue(layerName, out ClothingData clothing);
        return clothing;
    }

    public bool CanClickClothing(string layerName)
    {
        switch (layerName)
        {
            case "Underwear":
                return pants.IsFullyRemoved;
            case "Legs":
                return underwear.IsFullyRemoved;
            case "Shirt":
            case "Pants":
                return true;
            default:
                return true;
        }
    }

    public void UpdateClothingLevel(ClothingData clothing, bool increase)
    {
        if (increase && clothing.CanBeRemovedFurther)
        {
            clothing.RemoveOneLevel();
        }
        else if (!increase && clothing.CanBeDressed)
        {
            clothing.DressOneLevel();
        }

        SyncSpreadVersion(clothing);
        UpdateAllClothingDisplay();
    }

    public void ToggleLegsState()
    {
        isLegsOpen = !isLegsOpen;
        UpdateAllClothingDisplay();
        Debug.Log($"ขา: {(isLegsOpen ? "เปิด" : "ปิด")}");
    }

    private void SyncSpreadVersions()
    {
        shirtSpread.currentLevel = shirt.currentLevel;
        pantsSpread.currentLevel = pants.currentLevel;
        underwearSpread.currentLevel = underwear.currentLevel;
    }

    private void SyncSpreadVersion(ClothingData clothing)
    {
        if (clothingPairs.TryGetValue(clothing.type, out ClothingPair pair))
        {
            if (clothing == pair.normal)
            {
                pair.spread.currentLevel = clothing.currentLevel;
            }
            else if (clothing == pair.spread)
            {
                pair.normal.currentLevel = clothing.currentLevel;
            }
        }
    }

    public void UpdateAllClothingDisplay()
    {
        foreach (var pair in clothingPairs.Values)
        {
            UpdateClothingPairDisplay(pair);
        }
        UpdateLegsDisplay();
    }

    private void UpdateClothingPairDisplay(ClothingPair pair)
    {
        // Turn off all levels first
        TurnOffAllLevels(pair.normal);
        TurnOffAllLevels(pair.spread);

        // Show appropriate version
        if (isLegsOpen)
        {
            TurnOnLevel(pair.spread, pair.spread.currentLevel);
        }
        else
        {
            TurnOnLevel(pair.normal, pair.normal.currentLevel);
        }
    }

    private void UpdateLegsDisplay()
    {
        if (legs.levels.Length < 2)
        {
            Debug.LogError("[ClothingManager] ขาต้องมีอย่างน้อย 2 levels");
            return;
        }

        legs.levels[0].SetActive(!isLegsOpen); // ขาปิด
        legs.levels[1].SetActive(isLegsOpen);   // ขาเปิด
    }

    private void TurnOffAllLevels(ClothingData clothing)
    {
        foreach (var level in clothing.levels)
        {
            if (level != null)
                level.SetActive(false);
        }
    }

    private void TurnOnLevel(ClothingData clothing, int levelIndex)
    {
        if (levelIndex < clothing.levels.Length && clothing.levels[levelIndex] != null)
        {
            clothing.levels[levelIndex].SetActive(true);
        }
    }

    // State checking methods
    public bool IsFullyDressed()
    {
        return pants.IsFullyDressed && underwear.IsFullyDressed;
    }

    public bool IsFullyRemoved()
    {
        return pants.IsFullyRemoved && underwear.IsFullyRemoved;
    }

    public bool CanSpreadLegs()
    {
        return !isLegsOpen && (IsFullyDressed() || IsFullyRemoved());
    }

    public bool CanCloseLegs()
    {
        return isLegsOpen;
    }
}