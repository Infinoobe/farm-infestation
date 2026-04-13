using System.Collections.Generic;
using UI;
using UnityEngine;

public class TreeSeedBuilding : Building
{
    public override void PlaceBuilding(IEnumerable<GridCell> onCells)
    {
        base.PlaceBuilding(onCells);

        GridCell mainCell = occupiedCells[0]; // assume treeSeed size 1
        (int x, int y) = mainCell.myGrid.grid.WorldToGridPosition(gameObject.transform.position);
        for(int i = x - range; i < x + range + 1; i++)
        {
            for(int j = y - range; j < y + range + 1; j++)
            {
                GridCell cell = mainCell.myGrid.grid.GetGridCell(i, j);
                if(cell) cell.AddTreeSeedInRange();
            }
        }
    }

    public override void DestroyBuilding()
    {
        GridCell mainCell = occupiedCells[0]; // assume treeSeed size 1
        (int x, int y) = mainCell.myGrid.grid.WorldToGridPosition(gameObject.transform.position);
        for (int i = x - range; i < x + range + 1; i++)
        {
            for (int j = y - range; j < y + range + 1; j++)
            {
                mainCell.myGrid.grid.GetGridCell(i, j).SubtractTreeSeedInRange();
            }
        }

        base.DestroyBuilding();
    }
}
