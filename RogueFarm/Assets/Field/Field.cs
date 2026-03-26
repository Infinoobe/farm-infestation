using Interactable;
using UnityEngine;

public class Field : Building, IInteractable
{
    [SerializeField] private Plant currentPlant;

    public void Start()
    {
        GameState.Instance.RegisterInteractable(this);
    }

    public void PlantSeed(Player p)
    {
        if (!IsEmpty()) return;
        if (!GameState.Instance.PullItem(p.SelectedPlantSeed)) return;

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
        if (IsEmpty())
        {
            var selectedPlantSeed = GameState.Instance.Player.SelectedPlantSeed;
            var items = GameState.Instance.GetItems();
            items.TryGetValue(selectedPlantSeed, out var count);
            if (count > 0)
                return $"Plant {selectedPlantSeed.itemName}";
            return $"Do nothing (no {selectedPlantSeed.itemName})";
        }
        if (currentPlant.IsGrown) return "Collect";
        return "Do nothing (Plant Growing)";
    }
    
    public void Interact(Player p)
    {
        // Add checking current player item

        if (IsEmpty())
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
