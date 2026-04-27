using UnityEngine;

namespace Interactable
{
    public interface IInteractable
    {
        public bool IsInteractionEnabled { get; }

        public void Interact(Player p);
        public Vector3 GetPosition();
        public bool GetDescription(out string message);
    }
}