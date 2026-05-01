using System;
using System.Collections.Generic;
using Interactable.Common;

[Serializable]
public class ItemCount
{
    public ItemSO item;
    public int count = 1;
}

public class Pickup : BaseInteractable
{

    public List<ItemCount> items;
    public override void Interact(Player p)
    {
        foreach (ItemCount i in items)
        {
            GameState.Instance.AddItem(i.item, i.count);
        }
        Destroy(gameObject);
    }


    public override string GetDescription()
    {
        var desc = "";
        foreach (ItemCount i in items)
        {
            desc += $"{i.item.name} ({i.count}) ";
        }
        return "Pickup "+desc;
    }

}
