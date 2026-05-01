using System;
using System.Collections.Generic;
using Interactable.Common;

public class Pickup : BaseInteractable
{
    public List<ItemAmount> items;
    public override void Interact(Player p)
    {
        foreach (ItemAmount i in items)
        {
            GameState.Instance.AddItem(i.itemSo, i.amount);
        }
        Destroy(gameObject);
    }


    public override ActionType GetDescription(out string message)
    {
        var desc = "";
        foreach (ItemAmount i in items)
        {
            desc += $"{i.itemSo.name} ({i.amount}) ";
        }
        message = "Pickup " + desc;
        return ActionType.INTERACTION;
    }
}
