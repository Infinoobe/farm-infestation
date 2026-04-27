using Interactable;
using UI;
using UnityEngine;

public class ResearchTable : Building
{

    override public void Interact(Player p)
    {
        MainUI.Instance.OpenResearch();
    }

    override public bool GetDescription(out string message)
    {
        message = "research";
        return true;
    }
}
