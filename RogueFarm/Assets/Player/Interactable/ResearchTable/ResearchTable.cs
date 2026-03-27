using Interactable;
using UI;
using UnityEngine;

public class ResearchTable : MonoBehaviour, IInteractable
{
    void Start()
    {
        GameState.Instance.RegisterInteractable(this);
    }

    public void Interact(Player p)
    {
        MainUI.Instance.OpenResearch();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public string GetDescription()
    {
        return "Research";
    }
}
