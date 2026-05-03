using Interactable;
using Interactable.Common;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Building : BaseInteractable, IDamagable
{
    [Header("Building settings")]
    [SerializeField] protected int influenceRange;
    [SerializeField] protected bool canBeDismantled = true;

    [Header("Building Combat")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] protected float enemyAttractionFactor = 1f;
    [SerializeField] protected bool canBeTargetedByEnemy = true;
    [SerializeField] public int zombieSpawnFreeRange = 1;
    [SerializeField] public bool ignoreBuildingWhenSpawning = false;

    [Header("Playtime variables")]
    [SerializeField] protected List<GridCell> occupiedCells;
    [SerializeField] protected bool isPlaced = true;
    [SerializeField] private int currHealth = 100;

    [Header("Object settings")]
    [SerializeField] private BuildingShapeUnit[] buildingShapeUnits;

    
    public BuildingShapeUnit[] BuildingShapeUnits => buildingShapeUnits;
    public bool CanBeTargetedByEnemy => canBeTargetedByEnemy;
    public int CurrHealth => currHealth;
    public int Range => influenceRange;
    public bool CanBeDismantled => canBeDismantled;
    public float EnemyAttractionFactor => enemyAttractionFactor;

    public bool IsPlaced
    {
        get => isPlaced;
        set => isPlaced = value;
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitOnGrid();
    }

    private void InitOnGrid()
    {
        if (occupiedCells != null && occupiedCells.Count != 0 && isPlaced) return;
        else Debug.Log($"Building {gameObject.name} is on grid");
        // Placing on grid
        int layerMask = LayerMask.GetMask("GridGround");
        Vector3 startPos = gameObject.transform.position + new Vector3(0f, 3f, 0f);
        if (Physics.Raycast(startPos, Vector3.down, out RaycastHit ground, 5, layerMask))
        {
            GridCell target = ground.collider.gameObject.GetComponentInParent<GridCell>();
            GridSystemRuntime grid = target.myGrid;

            gameObject.transform.localScale *= grid.grid.gridCellSize;
            gameObject.transform.SetParent(grid.transform, true);
            gameObject.transform.position = target.transform.position;
            // Assume rotation is valid
            List<GridCell> targetCells = grid.GetOverlapingCells(gameObject);
            
            foreach (GridCell cell in targetCells)
            {
                if (cell.currBuilding)
                {
                    Debug.LogError($"Building {gameObject.name} cannot be put on grid!");
                    return;
                }
                cell.SetBuilding(gameObject);
            }

            PlaceBuilding(targetCells);
        }
        else
        {
            Debug.Log("No grid detected");
        }
    }

    override public ActionType GetDescription(out string message)
    {
        if(canBeDismantled)
        {
            message = "Use Axe to dismantle";
            if (GameState.Instance.Player == GameState.Instance.itemsDatabase.axeSo) return ActionType.ITEM_USE;
            else return ActionType.DESCRIPTION;
        }
        return base.GetDescription(out message);
    }

    override public void Interact(Player p)
    {
        if (!canBeDismantled) return;
        if (p.SelectedItemSo == GameState.Instance.itemsDatabase.axeSo) DismantleBuilding();
    }

    virtual public void PlaceBuilding(List<GridCell> onCells)
    {
        occupiedCells = onCells;
        isPlaced = true;
        GameState.Instance.RegisterInteractable(this);
        currHealth = maxHealth;
    }

    public void DismantleBuilding()
    {
        if (!CanBeDismantled) return;
        // TODO: return resources
        DestroyBuilding();
    }

    virtual public void TakeDamage(int damage)
    {
        currHealth = Mathf.Max(0, currHealth-damage);
        if (currHealth == 0) KillYourself();
    }

    public void KillYourself()
    {
        // TODO: particles/animation for dying in combat
        DestroyBuilding();
    }

    virtual public void DestroyBuilding()
    {
        foreach (GridCell cell in occupiedCells)
        {
            cell.UnSetBuilding();
        }
        Destroy(gameObject);
    }
}
