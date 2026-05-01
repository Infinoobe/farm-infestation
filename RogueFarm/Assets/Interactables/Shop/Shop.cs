using System;
using Interactable;
using Interactable.Common;
using UI;
using UnityEngine;

public class Shop : BaseInteractable
{
    public override void Interact(Player p)
    {
        if (!IsInteractionEnabled()) return;
        MainUI.Instance.OpenShop();
    }

    public override ActionType GetDescription(out string message)
    {
        message = "Click to open shop";
        return ActionType.INTERACTION;
    }
}
