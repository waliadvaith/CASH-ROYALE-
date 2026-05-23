using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UISpawnerController : MonoBehaviour
{
    [Header("Currency Settings")]
    [SerializeField] private int currentGold = 200;
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("Dynamic Hotbar Setup")]
    [SerializeField] private Transform hotbarParent;     // Your Horizontal Layout Group object
    [SerializeField] private GameObject buttonPrefab;   // A template button prefab

    [Header("All Troops Master List")]
    // Drop ALL your TroopData assets directly into this inspector list!
    [SerializeField] private List<TroopData> availableTroops = new List<TroopData>();

    private TroopData selectedTroopToPlace = null;

    private void Start()
    {
        UpdateGoldUI();
        GenerateSpawnButtons();
    }

    private void GenerateSpawnButtons()
    {
        if (hotbarParent == null || buttonPrefab == null) return;

        // Loop through every single troop data asset in your master list
        foreach (TroopData troop in availableTroops)
        {
            // 1. Duplicate the template button inside the hotbar container
            GameObject newButtonObj = Instantiate(buttonPrefab, hotbarParent);

            // 2. Grab the script on the new button and set it up
            TroopButton troopButtonScript = newButtonObj.GetComponent<TroopButton>();
            if (troopButtonScript != null)
            {
                troopButtonScript.SetupButton(troop, this);
            }
        }
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {currentGold}";
        }
    }

    // Called automatically when ANY generated button is clicked
    public void SelectTroop(TroopData troop)
    {
        if (currentGold >= troop.goldCost)
        {
            selectedTroopToPlace = troop;
            Debug.Log($"Selected {troop.troopName} ({troop.goldCost}g). Click to deploy!");
        }
        else
        {
            Debug.Log($"Not enough gold for {troop.troopName}!");
        }
    }

    public void SpendGold(int amount)
    {
        currentGold -= amount;
        UpdateGoldUI();
    }

    public TroopData GetSelectedTroop() => selectedTroopToPlace;
    public void ClearSelection() => selectedTroopToPlace = null;
}