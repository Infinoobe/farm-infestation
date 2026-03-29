using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class ItemAmount
{
    [FormerlySerializedAs("item")] public ItemSO itemSo;
    public int amount;
}

