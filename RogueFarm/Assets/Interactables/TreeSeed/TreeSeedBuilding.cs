using System.Collections.Generic;
using UI;
using UnityEngine;

public class TreeSeedBuilding : Building
{
    public override void PlaceBuilding(IEnumerable<GridCell> onCells)
    {
        base.PlaceBuilding(onCells);

        GridCell mainCell = occupiedCells[0]; // assume treeSeed size 1
        List<GridCell> cells = mainCell.myGrid.grid.GetGridCellsInRange(mainCell, range);
        foreach (GridCell cell in cells)
        {
            cell.AddTreeSeedInRange();
        }
    }

    public override void DestroyBuilding()
    {
        GridCell mainCell = occupiedCells[0]; // assume treeSeed size 1
        List<GridCell> cells = mainCell.myGrid.grid.GetGridCellsInRange(mainCell, range);
        foreach(GridCell cell in cells)
        {
            cell.SubtractTreeSeedInRange();
        }

        base.DestroyBuilding();
    }
}
