using Interactable;
using UnityEngine;

public class Field : Building
{
    [Header("Field")]
    [SerializeField] private Plant currentPlant;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private Material dryDirt;
    [SerializeField] private Material wetDirt;
    [SerializeField] private bool isWatered;

    public void Start()
    {
        GameState.Instance.OnDayStarted.AddListener(HandleDayStarted);
        GameState.Instance.OnNightStarted.AddListener(HandleNightStarted);
        isWatered = false;
        SetMaterial();
    }

    public override void DestroyBuilding()
    {
        GameState.Instance.OnDayStarted.RemoveListener(HandleDayStarted);
        GameState.Instance.OnNightStarted.RemoveListener(HandleNightStarted);
        base.DestroyBuilding();
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
        if (currentPlant) currentPlant.IsWatered = isWatered;
    }

    public override void TakeDamage(int damage)
    {
        if (currentPlant) currentPlant.TakeDamage(damage);
        base.TakeDamage(damage);
    }

    public void PlantSeed(Player p)
    {
        if (!IsEmpty()) return;
        var item = GameState.Instance.Player.SelectedItem;
        if (item == null || item.itemType != ItemType.SEED)
        {
            return;
        }
        if (!GameState.Instance.RemoveItem(item)) return;

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

    override public bool GetDescription(out string message)
    {
        if (IsEmpty())
        {
            var item = GameState.Instance.Player.SelectedItem;
            if (item == null || item.itemType != ItemType.SEED)
            {
                message = "do nothin (needs seed)";
                return false;
            }
            
            if (GameState.Instance.HasItems(item))
            {
                message = $"plant {item.itemName}";
                return true;
            }

            message = $"do nothing (needs {item.itemName})";
            return false;
        }

        if (currentPlant.CanBeCollected) 
        {
            message = $"collect {currentPlant.CollectItemSO.itemName}";
            return true;
        }
        
        if (!isWatered && GameState.Instance.Player.SelectedItem.itemName.Equals("Watercan"))
        {
            message = "water field";
            return true;
        }
        else if(!isWatered)
        {
            message = "water field (needs watercan)";
            return false;
        }

        message = "do nothing (needs time)";
        string m;
        return base.GetDescription(out m);
    }
    
    override public void Interact(Player p)
    {
        base.Interact(p);

        if (IsEmpty())
        {
            PlantSeed(p);
            return;
        }

        if (CanBeCollected())
        {
            CollectPlant();
            return;
        }

        if (!isWatered && p.SelectedItem.itemName.Equals("Watercan"))
        {
            WaterField();
            return;
        }
    }
}
