using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] Item[] startingItems;
    public Dictionary<Item, int> items = new Dictionary<Item, int>();

    public void AddItem(Item item, int amount = 1)
    {
        if (items.ContainsKey(item))
        {
            items[item] += amount;
        }
        else
        {
            items[item] = amount;
        }
    }

    public void RemoveItem(Item item, int amount = 1)
    {
        if (items.ContainsKey(item))
        {
            items[item] -= amount;
            if (items[item] <= 0)
                items.Remove(item);
        }
    }

    public int GetAmount(Item item)
    {
        return items.TryGetValue(item, out int amount) ? amount : 0;
    }

    private void Start()
    {
        foreach(var item in startingItems){
            AddItem(item, 5);
        }
    }
}