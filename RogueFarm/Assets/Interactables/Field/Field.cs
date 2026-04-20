using Interactable;
using UnityEngine;

public class Field : Building
{
    [SerializeField] private Plant currentPlant;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private Material dryDirt;
    [SerializeField] private Material wetDirt;
    public bool isWatered;

    public void Start()
    {
        GameState.Instance.OnDayStarted.AddListener(HandleDayStarted);
        GameState.Instance.OnNightStarted.AddListener(HandleNightStarted);
        GameState.Instance.RegisterInteractable(this);
        isWatered = false;
        SetMaterial();
    }

    private void SetMaterial()
    {
        if (isWatered) mesh.material = wetDirt;
        else mesh.material = dryDirt;
    }

    public void WaterField()
    {
        isWatered = true;
        SetMaterial();
    }

    private void HandleDayStarted()
    {
        isWatered = false;
        SetMaterial();
    }

    private void HandleNightStarted()
    {
        if (currentPlant) currentPlant.isWatered = isWatered;
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
        if (!isWatered && GameState.Instance.Player.selectedItemSo.itemName.Equals("Watercan"))
        {
            return "Water field";
        }
        else if(!isWatered)
        {
            return "Water field (Needs watercan)";
        }
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

        if (!isWatered && GameState.Instance.Player.selectedItemSo.itemName.Equals("Watercan"))
        {
            WaterField();
            return;
        }
    }
}
