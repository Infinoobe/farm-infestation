using System;
using Interactable;
using UI;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isInteractionEnabled;
    public bool IsInteractionEnabled => isInteractionEnabled;

    public void Start()
    {
        GameState.Instance.RegisterInteractable(this);
    }

    public void Interact(Player p)
    {
        if (!IsInteractionEnabled) return;
        MainUI.Instance.OpenShop();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool GetDescription(out string message)
    {
        message = "shop";
        return true;
    }
}
