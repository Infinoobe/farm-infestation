using Interactable;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IInteractable
{
    [SerializeField] private bool canBeTargeted;
    [SerializeField] private bool canBeDestroyed;
    [SerializeField] private bool canBeWalkedOn;
    [SerializeField] private bool interactionEnabled;
    [SerializeField] private int health;
    [SerializeField] private BuildingShapeUnit[] buildingShapeUnits;
    public bool isPlaced = true;
    private GridCell[] occupiedCells;

    public BuildingShapeUnit[] BuildingShapeUnits => buildingShapeUnits;
    public bool CanBeDestroyed => canBeDestroyed;
    public bool CanBeTargeted => canBeTargeted;
    public bool CanBeWalkedOn => canBeWalkedOn;
    public int CurrHealth => health;

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

    public void PlaceBuilding(GridCell[] onCells)
    {
        occupiedCells = onCells;
        isPlaced = true;
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
