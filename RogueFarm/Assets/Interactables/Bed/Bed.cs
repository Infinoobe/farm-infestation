using Interactable;
using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isInteractionEnabled;

    public bool IsInteractionEnabled => isInteractionEnabled;
    void Start()
    {
        GameState.Instance.RegisterInteractable(this);
    }

    public void Interact(Player p)
    {
        if (!IsInteractionEnabled) return;
        GameState.Instance.GoToSleep();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
    public bool GetDescription(out string message)
    {
        message = "sleep";
        return true;
    }
}
