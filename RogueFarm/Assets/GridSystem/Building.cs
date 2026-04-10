using Interactable;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Building : MonoBehaviour, IInteractable
{
    [SerializeField] private bool canBeTargeted;
    [SerializeField] private bool canBeDestroyed;
    [SerializeField] private bool canBeWalkedOn;
    [SerializeField] private bool interactionEnabled;
    [SerializeField] private int health;
    [SerializeField] protected int range;
    [SerializeField] private BuildingShapeUnit[] buildingShapeUnits;
    public bool isPlaced = true;
    protected List<GridCell> occupiedCells;

    public BuildingShapeUnit[] BuildingShapeUnits => buildingShapeUnits;
    public bool CanBeDestroyed => canBeDestroyed;
    public bool CanBeTargeted => canBeTargeted;
    public bool CanBeWalkedOn => canBeWalkedOn;
    public int CurrHealth => health;
    public int Range => range;

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
                    Debug.Log($"Building {gameObject.name} cannot be put on grid");
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

    public bool IsInteractionEnabled() 
    {
        if (!isPlaced || !interactionEnabled) return false;
        return true;
    }
    public void EnableInteraction() { interactionEnabled = true; }
    public void DisableInteraction() { interactionEnabled = false; }

    virtual public string GetDescription()
    {
        return "Do nothing (building)";
    }

    virtual public void Interact(Player p)
    {
        return;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    virtual public void PlaceBuilding(IEnumerable<GridCell> onCells)
    {
        occupiedCells = onCells.ToList();
        isPlaced = true;
        EnableInteraction();
        GameState.Instance.RegisterInteractable(this);
    }

    public void RemoveBuilding()
    {
        if (!canBeDestroyed) return;
        foreach(GridCell cell in occupiedCells){
            cell.UnSetBuilding();
        }
        GameState.Instance.UnRegisterInteractable(this);

        // TODO: returning some resources
        DestroyBuilding();
    }

    virtual public void DestroyBuilding()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        health = Mathf.Max(0, health-damage);
        if (health == 0) DestroyBuilding();
    }  
}
