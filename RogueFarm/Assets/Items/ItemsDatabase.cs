using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem")]
public class ItemsDatabaseSO : ScriptableObject
{
    public List<ItemSO> items;

    public Pickup pickupPrefab;

    public ItemSO seedItemSo;
    public ItemSO moneyItemSo;
    public ItemSO handItemSo;
    public ItemSO axeSo;
    public ItemSO waterCanSo;
}
