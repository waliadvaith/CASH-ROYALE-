using UnityEngine;

public class TroopPlacementManager : MonoBehaviour
{
    [Header("Manager References")]
    [SerializeField] private UISpawnerController uiController;
    [SerializeField] private GameObject previewPrefab;

    [Header("Placement Rules")]
    public float spawnYPosition = -2f; // The flat height line where troops should spawn

    private GameObject currentPreviewObj;
    private TroopData activeTroopData;
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        // 1. Check if the player has selected a troop from the UI
        UpdateSelectedTroop();

        // 2. If a troop is selected, manage the preview and placement click
        if (activeTroopData != null)
        {
            HandlePlacementLoop();
        }
    }

    private void UpdateSelectedTroop()
    {
        TroopData selectedData = uiController.GetSelectedTroop();

        // If the selection changed, handle the preview object status
        if (selectedData != activeTroopData)
        {
            activeTroopData = selectedData;

            if (activeTroopData != null)
            {
                // Spawn the semi-transparent targeting box
                if (currentPreviewObj == null)
                {
                    currentPreviewObj = Instantiate(previewPrefab);
                }
            }
            else
            {
                // Clean up preview if selection cleared
                DestroyPreview();
            }
        }
    }

    private void HandlePlacementLoop()
    {
        // Get mouse position converted to actual world coordinates
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // Lock the preview box to your designated flat ground line (X follows mouse, Y is locked)
        Vector3 placementPos = new Vector3(mouseWorldPos.x, spawnYPosition, 0f);

        if (currentPreviewObj != null)
        {
            currentPreviewObj.transform.position = placementPos;
        }

        // Left Click to place the troop on the field!
        if (Input.GetMouseButtonDown(0))
        {
            // Double check money one last time
            uiController.SpendGold(activeTroopData.goldCost);

            // Spawn the real actual troop prefab from our scriptable object data!
            GameObject newTroop = Instantiate(activeTroopData.troopPrefab, placementPos, Quaternion.identity);

            // Set the spawned unit's team to Player so they fight the right way
            MeleeUnit melee = newTroop.GetComponent<MeleeUnit>();
            if (melee != null) melee.team = UnitTeam.Player;

            RangedUnit ranged = newTroop.GetComponent<RangedUnit>();
            if (ranged != null) ranged.team = UnitTeam.Player;

            // Clear selection so the user has to click the button again for the next unit
            uiController.ClearSelection();
            DestroyPreview();
        }

        // Optional: Right click to cancel deployment
        if (Input.GetMouseButtonDown(1))
        {
            uiController.ClearSelection();
            DestroyPreview();
        }
    }

    private void DestroyPreview()
    {
        if (currentPreviewObj != null)
        {
            Destroy(currentPreviewObj);
            currentPreviewObj = null;
        }
    }
}