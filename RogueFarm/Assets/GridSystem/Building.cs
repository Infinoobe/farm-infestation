using Interactable;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Building : MonoBehaviour, IInteractable, IDamagable
{
    [Header("Building settings")]
    [SerializeField] protected int influenceRange;
    [SerializeField] protected bool isInteractionEnabled = false;
    [SerializeField] protected bool canBeDismantled = true;

    [Header("Building Combat")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] protected float attractionFactor = 1f;
    [SerializeField] protected bool isVulnerable = true;
    [SerializeField] protected bool canBeTargeted = true;

    [Header("Playtime variables")]
    [SerializeField] protected List<GridCell> occupiedCells;
    [SerializeField] protected bool isPlaced = true;
    [SerializeField] private int currHealth = 100;

    [Header("Object settings")]
    [SerializeField] private BuildingShapeUnit[] buildingShapeUnits;

    //[Header("Events")]
    //public UnityEvent OnBuildingPlaced = new UnityEvent();
    
    public BuildingShapeUnit[] BuildingShapeUnits => buildingShapeUnits;
    public bool CanBeTargeted => canBeTargeted;
    public int CurrHealth => currHealth;
    public int Range => influenceRange;
    public bool IsVulnerable => isVulnerable;
    public bool IsInteractionEnabled => isPlaced && isInteractionEnabled;
    public bool CanBeDismantled => canBeDismantled;
    public float AttractionFactor => attractionFactor;


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

    private void InitOnGrid() // For placing before runtime
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

    virtual public bool GetDescription(out string message)
    {
        if(canBeDismantled)
        {
            message = "dismantle (Axe)";
            return true;
        }
        message = "do nothing";
        return false;
    }

    virtual public void Interact(Player p)
    {
        if (!canBeDismantled) return;
        if (p.SelectedItem.name.Equals("Axe")) DismantleBuilding();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void PlaceBuilding(List<GridCell> onCells)
    {
        occupiedCells = onCells;
        isPlaced = true;
        GameState.Instance.RegisterInteractable(this);
    }

    public void DismantleBuilding()
    {
        if (!CanBeDismantled) return;
        // TODO: return resources
        DestroyBuilding();
    }

    public void TakeDamage(int damage)
    {
        if (!IsVulnerable) return;
        currHealth = Mathf.Max(0, currHealth-damage);
        if (currHealth == 0) KillYourself();
    }

    public void KillYourself()
    {
        // TODO: died in combat
        DestroyBuilding();
    }

    public void DestroyBuilding()
    {
        foreach (GridCell cell in occupiedCells)
        {
            cell.UnSetBuilding();
        }
        GameState.Instance.UnRegisterInteractable(this);
        Destroy(gameObject);
    }
}
