using System;
using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<Item, int> items = new Dictionary<Item, int>();
    
    [ConsoleMethod( "add_item", "Adds item to inventory" )]
    public static void AddItemCheat( string item_name, int amount )
    {
        var items = Resources.FindObjectsOfTypeAll<Item>().ToList();
        var item = items
            .FirstOrDefault(x => string.Equals(x.name, item_name, StringComparison.InvariantCultureIgnoreCase));
        if (item == null)
        {
            Debug.LogError( $"Item {item_name} not found" );
            return;
        }
        GameState.Instance.AddItem( item, amount );
    }

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
}