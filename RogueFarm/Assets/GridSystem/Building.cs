using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private bool canBeTargeted;
    [SerializeField] private bool canBeDestroyed;
    [SerializeField] private bool canBeWalkedOn;
    [SerializeField] private int health;
    private bool isPlaced;
    private GridCell[] occupiedCells;

    public bool CanBeTargeted => canBeTargeted;
    public bool CanBeWalkedOn => canBeWalkedOn;
    public int CurrHealth => health;

    public void PlaceBuilding(GridCell[] onCells)
    {
        occupiedCells = onCells;
    }

    public void RemoveBuilding()
    {
        if (!canBeDestroyed) return;
        foreach(GridCell cell in occupiedCells){
            cell.UnSetBuilding();
        }
        // TODO: returning some resources
        DestroyBuilding();
    }

    public void DestroyBuilding()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        health = Mathf.Max(0, health-damage);
        if (health == 0) DestroyBuilding();
    }
    
}
