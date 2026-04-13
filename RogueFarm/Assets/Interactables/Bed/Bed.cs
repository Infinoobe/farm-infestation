using Interactable;
using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    [SerializeField] private bool interactionEnabled;
    void Start()
    {
        GameState.Instance.RegisterInteractable(this);
    }

    public bool IsInteractionEnabled() { return interactionEnabled; }
    public void EnableInteraction() { interactionEnabled = true; }
    public void DisableInteraction() { interactionEnabled = false; }

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
