using UnityEngine;

public enum ItemType
{
    UNASSIGNED,
    SEED,
    CURRENCY,
    TOOL,
    RESOURCE,
}

[CreateAssetMenu(fileName = "NewItem")]
public class Item : ScriptableObject
{
    public ItemType itemType;
    public bool canBeSold = true;
    public string itemName;
    public Sprite icon;
    public int valueBuying;
    public int valueSelling;
    public GameObject plantPrefab;
}
