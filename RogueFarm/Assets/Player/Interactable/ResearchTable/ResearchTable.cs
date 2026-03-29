using Interactable;
using UI;
using UnityEngine;

public class ResearchTable : Building
{
    void Start()
    {
    }

    override public void Interact(Player p)
    {
        MainUI.Instance.OpenResearch();
    }

    override public string GetDescription()
    {
        return "Research";
    }
}
