using UnityEngine;

namespace Interactable
{
    public interface IInteractable
    {
        public void Interact(Player p);
        public Vector3 GetPosition();
        public string GetDescription();
    }
}