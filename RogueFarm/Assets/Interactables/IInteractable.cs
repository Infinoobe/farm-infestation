using Interactable.Common;
using UnityEngine;

namespace Interactable
{
    public interface IInteractable
    {
        public bool IsInteractionEnabled();
        public void EnableInteraction();
        public void DisableInteraction();

        public void Interact(Player p);
        public Vector3 GetPosition();
        public ActionType GetDescription(out string message);
    }
}