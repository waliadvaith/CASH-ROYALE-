using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TroopButton : MonoBehaviour
{
    [Header("UI Element References")]
    [SerializeField] private TextMeshProUGUI nameAndCostText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button buttonComponent;

    private TroopData myData;
    private UISpawnerController uiController;

    // The manager calls this function to initialize the button dynamically
    public void SetupButton(TroopData data, UISpawnerController controller)
    {
        myData = data;
        uiController = controller;

        // 1. Set the text (e.g., "Swordsman\n50g")
        if (nameAndCostText != null)
        {
            nameAndCostText.text = $"{myData.troopName}\n{myData.goldCost}g";
        }

        // 2. Set the picture
        if (iconImage != null && myData.troopIcon != null)
        {
            iconImage.sprite = myData.troopIcon;
        }

        // 3. Automatically listen for a click
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        // Tell the main controller we selected this specific troop
        uiController.SelectTroop(myData);
    }
}