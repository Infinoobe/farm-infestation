using UnityEngine;

public enum ItemType
{
    UNASSIGNED,
    SEED,
    CURRENCY,
    TOOL,
    RESOURCE,
    BUILDING,
    UPGRADE
}

[CreateAssetMenu(fileName = "NewItem SO")]
public class ItemSO : ScriptableObject
{
    public ItemType itemType;
    public bool canBeSold = true;
    public bool unique;
    public string itemName;
    public Sprite icon;
    public int valueBuying;
    public int valueSelling;
    public GameObject plantPrefab;
    public GameObject buildingPrefab;
    public int damageBonus;
    public int maxHealthBonus;
    public int regenerationBonus;
}
