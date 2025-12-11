using Interactable;
using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    void Start()
    {
        GameState.Instance.RegisterInteractable(this);
    }
    
    public void Interact(Player p)
    {
        GameState.Instance.GoToSleep();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public string GetDescription()
    {
        return "Sleep";
    }
}
