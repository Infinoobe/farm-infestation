using Interactable;
using Interactable.Common;
using UnityEngine;

public class Field : Building
{
    [Header("Field")]
    [SerializeField] private Plant currentPlant;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private Material dryDirt;
    [SerializeField] private Material wetDirt;
    [SerializeField] private bool isWatered;

    public override bool CanBeTargetedByEnemy => base.CanBeTargetedByEnemy && !IsEmpty();
    public override float EnemyAttractionFactor => !IsEmpty() ? currentPlant.EnemyAttractionFactor : enemyAttractionFactor;
    protected override void OnStart()
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
        var item = GameState.Instance.Player.SelectedItemSo;
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
        return currentPlant.canBeCollected;
    }

    override public ActionType GetDescription(out string message)
    {
        if (IsEmpty())
        {
            var item = GameState.Instance.Player.SelectedItemSo;
            if (item == null || item.itemType != ItemType.SEED)
            {
                message = "Equip seed to plant";
                return ActionType.DESCRIPTION;
            }
            
            if (GameState.Instance.HasItems(item))
            {
                message = $"Click to plant {item.itemName}";
                return ActionType.ITEM_USE;
            }

            message = $"No {item.itemName}";
            return ActionType.DESCRIPTION;
        }

        if (currentPlant.canBeCollected) 
        {
            message = $"Click to collect {currentPlant.CollectItemSO.itemName}";
            return ActionType.INTERACTION;
        }
        
        if (!isWatered)
        {
            message = "Use watercan to water field";
            if (GameState.Instance.Player.SelectedItemSo == GameState.Instance.itemsDatabase.waterCanSo) return ActionType.ITEM_USE;
            else return ActionType.DESCRIPTION;
        }

        message = $"Plant growing ({currentPlant.HowManyDaysToGrow()} days)";
        return ActionType.DESCRIPTION;
        //string m;
        //return base.GetDescription(out m);
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

        if (!isWatered && p.SelectedItemSo == GameState.Instance.itemsDatabase.waterCanSo)
        {
            WaterField();
            return;
        }
    }
}
