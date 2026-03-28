using Interactable;
using UI;
using UnityEngine;

public class ResearchTable : Building
{
    void Start()
    {
        GameState.Instance.RegisterInteractable(this);
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
