using System.Collections.Generic;
using Interactable.Common;
using Unity.VisualScripting;
using UnityEngine;

public class Sprinkler : Building
{
    [SerializeField] private ParticleSystem waterEffect;

    public override void Interact(Player p)
    {
        base.Interact(p);

        waterEffect.Play();
        SprinkleFields();
    }

    public override ActionType GetDescription(out string message)
    {
        message = "Click to sprinkle fields";
        return ActionType.INTERACTION;
    }

    private void SprinkleFields()
    {
        GridCell mainCell = occupiedCells[0]; // assume sprinkler size = 1
        List<GridCell> cells = mainCell.myGrid.grid.GetGridCellsInRange(mainCell, influenceRange);
        foreach (GridCell cell in cells)
        {
            if(cell.currBuilding && cell.currBuilding.TryGetComponent<Field>(out var field))
            {
                field.WaterField();
            }

        }
    }
    
}
