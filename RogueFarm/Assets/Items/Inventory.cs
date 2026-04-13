using System;
using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<ItemSO, int> items = new Dictionary<ItemSO, int>();
    
    [ConsoleMethod( "add_item", "Adds item to inventory" )]
    public static void AddItemCheat( string item_name, int amount )
    {
        var items = Resources.FindObjectsOfTypeAll<ItemSO>().ToList();
        var item = items
            .FirstOrDefault(x => string.Equals(x.name, item_name, StringComparison.InvariantCultureIgnoreCase));
        if (item == null)
        {
            Debug.LogError( $"Item {item_name} not found" );
            return;
        }
        GameState.Instance.AddItem( item, amount );
    }

    public void AddItem(ItemSO itemSo, int amount = 1)
    {
        if (items.ContainsKey(itemSo))
        {
            items[itemSo] += amount;
        }
        else
        {
            items[itemSo] = amount;
        }
    }

    public void RemoveItem(ItemSO itemSo, int amount = 1)
    {
        if (items.ContainsKey(itemSo))
        {
            items[itemSo] -= amount;
            if (items[itemSo] <= 0)
                items.Remove(itemSo);
        }
    }

    public int GetAmount(ItemSO itemSo)
    {
        return items.TryGetValue(itemSo, out int amount) ? amount : 0;
    }
}