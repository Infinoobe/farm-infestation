using System;
using Interactable;
using UI;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    public void Start()
    {
        GameState.Instance.RegisterInteractable(this);
    }

    public void Interact(Player p)
    {
        MainUI.Instance.OpenShop();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public string GetDescription()
    {
        return "Shop";
    }
}
