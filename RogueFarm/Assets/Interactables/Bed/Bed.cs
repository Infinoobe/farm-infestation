using Interactable;
using Interactable.Common;
using UnityEngine;

public class Bed : BaseInteractable
{
    public override void Interact(Player p)
    {
        if (!IsInteractionEnabled()) return;
        GameState.Instance.GoToSleep();
    }

    public override ActionType GetDescription(out string message)
    {
        message = "Go to sleep";
        return ActionType.INTERACTION;
    }
}
