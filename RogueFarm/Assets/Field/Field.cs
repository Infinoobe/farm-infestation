using Interactable;
using UnityEngine;

public class Field : MonoBehaviour, IInteractable
{
    [SerializeField] private Plant currentPlant;

    public void Start()
    {
        GameState.Instance.RegisterInteractable(this);
    }

    public void PlantSeed(Player p)
    {
        if (currentPlant != null)
        {
            return;
        }

        if (!GameState.Instance.PullItem(p.SelectedPlantSeed))
        {
            return;
        }

        p.animator.SetTrigger("Plant");
        currentPlant = Instantiate(p.SelectedPlant, transform.position, Quaternion.identity, transform);
    }

    public void CollectPlant()
    {
        if (!CanBeCollected()) return;
        currentPlant.CollectItem();
    }

    public bool IsEmpty()
    {
        return currentPlant == null;
    }

    public bool CanBeCollected()
    {
        if (IsEmpty()) return false;
        return currentPlant.IsGrown;
    }

    public string GetDescription()
    {
        if (currentPlant == null) return "Plant";
        if (currentPlant.IsGrown) return "Collect";
        return "XXX (Growing)";
    }
    
    public void Interact(Player p)
    {
        if (currentPlant == null)
        {
            PlantSeed(p);
            return;
        }

        if (CanBeCollected())
        {
            p.animator.SetTrigger("Plant");
            CollectPlant();
            return;
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

}
