using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ItemSet
{
    private Dictionary<ItemSO, int> itemsDictionary = new();

    public ItemSet(List<ItemAmount> items)
    {
        if (items == null) return;

        foreach (var entry in items)
        {
            if (entry?.itemSo == null) continue;
            AddItem(entry.itemSo, entry.amount);
        }
    }

    public ItemSet(Dictionary<ItemSO, int> items)
    {
        if (items == null) return;

        foreach (var kvp in items)
        {
            if (kvp.Key == null) continue;
            AddItem(kvp.Key, kvp.Value);
        }
    }

    public Dictionary<ItemSO, int> GetItemsOfTypeDict(ItemType type = ItemType.UNASSIGNED)
    {
        var result = new Dictionary<ItemSO, int>();

        foreach (var kvp in itemsDictionary)
        {
            var itemSo = kvp.Key;
            var amount = kvp.Value;

            if (itemSo == null || amount <= 0)
                continue;

            if (type != ItemType.UNASSIGNED && itemSo.itemType != type)
                continue;

            result[itemSo] = amount;
        }

        return result;
    }

    public List<ItemAmount> GetItemsOfTypeList(ItemType type = ItemType.UNASSIGNED)
    {
        var list = new List<ItemAmount>();

        foreach (var kvp in itemsDictionary)
        {
            var itemSo = kvp.Key;
            var amount = kvp.Value;

            if (itemSo == null || amount <= 0)
                continue;

            if (type != ItemType.UNASSIGNED && itemSo.itemType != type)
                continue;

            list.Add(new ItemAmount
            {
                itemSo = itemSo,
                amount = amount
            });
        }

        return list;
    }

    public string GetStringDescription()
    {
        if (itemsDictionary.Count == 0)
            return "Empty";

        StringBuilder sb = new();

        foreach (var kvp in itemsDictionary)
        {
            sb.AppendLine($"{kvp.Key.itemName}: {kvp.Value}");
        }

        return sb.ToString();
    }

    public int GetAmount(ItemSO item)
    {
        if (item == null) return 0;

        return itemsDictionary.TryGetValue(item, out var amount) ? amount : 0;
    }

    public void AddItem(ItemSO item, int amount)
    {
        if (item == null || amount <= 0) return;

        if (itemsDictionary.TryGetValue(item, out var current))
            itemsDictionary[item] = current + amount;
        else
            itemsDictionary[item] = amount;
    }

    public void RemoveItem(ItemSO item, int amount)
    {
        if (item == null || amount <= 0) return;

        if (!itemsDictionary.TryGetValue(item, out var current))
            return;

        int newAmount = current - amount;

        if (newAmount > 0)
            itemsDictionary[item] = newAmount;
        else
            itemsDictionary.Remove(item);
    }

    public void AddItems(ItemSet items)
    {
        if (items == null) return;

        foreach (var kvp in items.itemsDictionary)
        {
            if (kvp.Key == null) continue;
            AddItem(kvp.Key, kvp.Value);
        }
    }

    public void RemoveItems(ItemSet items)
    {
        if (items == null) return;

        foreach (var kvp in items.itemsDictionary)
        {
            if (kvp.Key == null) continue;
            RemoveItem(kvp.Key, kvp.Value);
        }
    }

    public bool HasItems(ItemSet items) 
    {
        if (items == null) return true;

        foreach (var kvp in items.itemsDictionary)
        {
            if (kvp.Key == null || kvp.Value <= 0)
                continue;

            if (GetAmount(kvp.Key) < kvp.Value)
                return false;
        }

        return true;
    }
}


