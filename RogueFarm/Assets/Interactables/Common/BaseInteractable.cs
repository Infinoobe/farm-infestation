using System;
using UnityEngine;

namespace Interactable.Common
{
    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        public bool interactionEnabled = true;

        public abstract void Interact(Player p);
        public abstract string GetDescription();

        public void Start()
        {
            GameState.Instance.RegisterInteractable(this);
        }

        public void OnDestroy()
        {
            var gi = GameState.Instance;
            if (gi != null)
            {
                gi.UnRegisterInteractable(this);
            }
        }

        public virtual Vector3 GetPosition()
        {
            return transform.position;
        }
        
        public virtual bool IsInteractionEnabled() 
        {
            if (!interactionEnabled) return false;
            return true;
        }
        
        public virtual void EnableInteraction() { interactionEnabled = true; }
        public virtual void DisableInteraction() { interactionEnabled = false; }
    }
}