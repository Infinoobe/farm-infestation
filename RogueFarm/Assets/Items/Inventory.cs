using System;
using System.Collections.Generic;
using System.Linq;
using IngameDebugConsole;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private ItemSet items;

    public void CreateInventory(List<ItemAmount> itemsList)
    {
        items = new ItemSet(itemsList);
    }

    public ItemSet GetItems()
    {
        return items;
    }

    [ConsoleMethod( "add_item", "Adds item to inventory" )]
    public static void AddItemCheat( string item_name, int amount )
    {
        var allItems = Resources.FindObjectsOfTypeAll<ItemSO>().ToList();
        var item = allItems
            .FirstOrDefault(x => string.Equals(x.name, item_name, StringComparison.InvariantCultureIgnoreCase));
        if (item == null)
        {
            Debug.LogError( $"Item {item_name} not found" );
            return;
        }
        GameState.Instance.AddItem( item, amount );
    }
}