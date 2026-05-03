using Interactable;
using Interactable.Common;
using UI;
using UnityEngine;

public class ResearchTable : Building
{

    override public void Interact(Player p)
    {
        if (!IsInteractionEnabled()) return;
        MainUI.Instance.OpenResearch();
    }

    override public ActionType GetDescription(out string message)
    {
        message = "Click to research";
        return ActionType.INTERACTION;
    }
}
