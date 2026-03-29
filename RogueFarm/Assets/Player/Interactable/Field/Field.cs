using Interactable;
using UnityEngine;

public class Field : Building
{
    [SerializeField] private Plant currentPlant;

    public void Start()
    {
        GameState.Instance.RegisterInteractable(this);
    }

    public void PlantSeed(Player p)
    {
        if (!IsEmpty()) return;
        var item = GameState.Instance.Player.selectedItemSo;
        if (item == null || item.itemType != ItemType.SEED)
        {
            return;
        }
        if (!GameState.Instance.RemoveItem(item)) return;

        p.animator.SetTrigger("Plant");
        if (item.plantPrefab == null)
        {
            Debug.LogError($"No plant set for seed {item.itemName} SO {item.name}");
            return;
        }

        var go = Instantiate(item.plantPrefab, transform.position, Quaternion.identity, transform);
        var plant = go.GetComponent<Plant>();
        if (plant == null)
        {
            Debug.LogError($"Invalid plant set for seed {item.itemName} SO {item.name}");
            return;
        }
        currentPlant = plant;
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
        return currentPlant.CanBeCollected;
    }

    override public string GetDescription()
    {
        if (IsEmpty())
        {
            var item = GameState.Instance.Player.selectedItemSo;
            if (item == null || item.itemType != ItemType.SEED)
            {
                return $"Select Seed to plant";
            }
            
            if (GameState.Instance.HasItems(item))
                return $"Plant {item.itemName}";
            return $"Do nothing (no {item.itemName})";
        }
        if (currentPlant.CanBeCollected) return "Collect";
        return "Do nothing (Plant Growing)";
    }
    
    override public void Interact(Player p)
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
}
