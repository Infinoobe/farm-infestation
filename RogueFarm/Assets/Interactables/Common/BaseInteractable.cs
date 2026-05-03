using System;
using UnityEngine;

namespace Interactable.Common
{
    public enum ActionType
    {
        NONE,           // Nothing can be done
        ITEM_USE,       // Use equiped item in interaction
        INTERACTION,    // Interaction with no requirements
        DESCRIPTION     // No interaction but provides info
    }

    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        public bool interactionEnabled = true;

        public abstract void Interact(Player p);
        public virtual ActionType GetDescription(out string message)
        {
            message = "Do nothing";
            return ActionType.NONE;
        }

        public void Start()
        {
            GameState.Instance.RegisterInteractable(this);
            OnStart();
        }

        protected virtual void OnStart() { }

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