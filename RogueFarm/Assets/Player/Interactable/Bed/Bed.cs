using Interactable;
using UnityEngine;

public class Bed : Building
{
    void Start()
    {
        GameState.Instance.RegisterInteractable(this);
    }
    
    override public void Interact(Player p)
    {
        GameState.Instance.GoToSleep();
    }

    override public string GetDescription()
    {
        return "Sleep";
    }
}
