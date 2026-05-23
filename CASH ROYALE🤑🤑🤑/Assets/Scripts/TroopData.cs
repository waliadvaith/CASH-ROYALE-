using UnityEngine;

[CreateAssetMenu(fileName = "New Troop Data", menuName = "Tactical/Troop Data")]
public class TroopData : ScriptableObject
{
    public string troopName;
    public int goldCost;
    public Sprite troopIcon;      // The picture for the UI button!
    public GameObject troopPrefab; // The actual unit prefab to spawn
}