using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

[ExecuteAlways]
public class GridSystemTool : MonoBehaviour
{
    public float gridCellSize = 1f;
    public int width = 10;
    public int height = 10;
    public GameObject gridCellPrefab;
    public GridCell[,] gridCells;
    public int includeSpawnRange = 3;
    private Dictionary<GridCell, int> spawnFreeGridCells = new();
    private Dictionary<GridCell, int> potentialSpawnGridCells = new();
    private Vector3 lowerLeftCorner;

    private void AddCells(Dictionary<GridCell, int> dict, List<GridCell> cells)
    {
        foreach (var c in cells)
        {
            if (dict.ContainsKey(c))
                dict[c]++;
            else
                dict[c] = 1;
        }
    }

    private void RemoveCells(Dictionary<GridCell, int> dict, List<GridCell> cells)
    {
        foreach (var c in cells)
        {
            if (!dict.ContainsKey(c)) continue;

            dict[c]--;

            if (dict[c] <= 0)
                dict.Remove(c);
        }
    }

    public void OnBuildingPlaced(GridCell cell)
    {
        Building building = cell.currBuilding.GetComponent<Building>();
        if (building.ignoreBuildingWhenSpawning) return;
        int excludeRange = building.zombieSpawnFreeRange;
        int includeRange = excludeRange + includeSpawnRange;

        var toInclude = GetGridCellsInRange(cell, includeRange);
        var toExclude = GetGridCellsInRange(cell, excludeRange);

        AddCells(potentialSpawnGridCells, toInclude);
        AddCells(spawnFreeGridCells, toExclude);
    }

    public void OnBuildingRemoved(GridCell cell)
    {
        Building building = cell.currBuilding.GetComponent<Building>();
        if (building.ignoreBuildingWhenSpawning) return;
        int excludeRange = building.zombieSpawnFreeRange;
        int includeRange = excludeRange + includeSpawnRange;

        var toInclude = GetGridCellsInRange(cell, includeRange);
        var toExclude = GetGridCellsInRange(cell, excludeRange);

        RemoveCells(potentialSpawnGridCells, toInclude);
        RemoveCells(spawnFreeGridCells, toExclude);
    }

    public List<GridCell> GetZombieSpawnPoints()
    {
        return potentialSpawnGridCells.Keys
        .Where(c => !spawnFreeGridCells.ContainsKey(c))
        .ToList();
    }

    void Awake()
    {
        gridCells = new GridCell[width, height];
        lowerLeftCorner = transform.position - new Vector3(width, 0, height) * 0.5f * gridCellSize;
        foreach (Transform child in transform)
        {
            GridCell cell = child.GetComponent<GridCell>();
            (int x, int y) = WorldToGridPosition(cell.gameObject.transform.position);
            gridCells[x, y] = cell;
            cell.myGrid = gameObject.GetComponent<GridSystemRuntime>();
            cell.OnBuildingPlaced.AddListener(OnBuildingPlaced);
            cell.OnBuildingRemoved.AddListener(OnBuildingRemoved);
        }
    }

    public void GenerateGrid()
    {
        if (gridCellSize <= 0 || width <= 0 || height <= 0)
        {
            Debug.Log("Values can't be negative");
            return;
        }

        lowerLeftCorner = transform.position - new Vector3(width, 0, height) * 0.5f * gridCellSize;

        ClearGridCells();
        PlaceGridCells();
    }

    public void ClearGridCells()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public List<GridCell> GetGridCellsInRange(GridCell cell, int range)
    {
        (int x, int y) = WorldToGridPosition(cell.transform.position);
        return GetGridCellsInRange(x, y, range);
    }

    public List<GridCell> GetGridCellsInRange(int x, int y, int range)
    {
        List<GridCell> cells = new List<GridCell>();
        for (int i = x - range; i < x + range + 1; i++)
        {
            for (int j = y - range; j < y + range + 1; j++)
            {
                GridCell cell = GetGridCell(i, j);
                if (cell) cells.Add(cell);
            }
        }
        return cells;
    }

    public List<GridCell> GetGridCellsBetween(int x, int y, int range1, int range2)
    {
        List<GridCell> cells = new List<GridCell>();
        for (int i = x - range2; i < x + range2 + 1; i++)
        {
            for (int j = y - range2; j < y + range2 + 1; j++)
            {
                if (i >= x - range1 && i <= x + range1 &&
                j >= y - range1 && j <= y + range1)
                    continue;

                GridCell cell = GetGridCell(i, j);
                if (cell) cells.Add(cell);
            }
        }
        return cells;
    }

    private void PlaceGridCells()
    {
        gridCells = new GridCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridCells[x, y] = Instantiate(gridCellPrefab, GridToWorldPosition(x, y), Quaternion.identity, gameObject.transform).GetComponent<GridCell>();
                gridCells[x, y].gameObject.transform.localScale *= gridCellSize;
                gridCells[x, y].myGrid = gameObject.GetComponent<GridSystemRuntime>();
            }
        }
    }

    public Vector3 GridToWorldPosition(int x, int y)
    {
        return lowerLeftCorner + new Vector3((x + 0.5f) * gridCellSize, 0, (y + 0.5f) * gridCellSize);
    }

    public (int x, int y) WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - lowerLeftCorner.x) / gridCellSize);
        int y = Mathf.FloorToInt((worldPosition.z - lowerLeftCorner.z) / gridCellSize);
        return (x, y);
    }

    public GridCell GetGridCell(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return null;
        }
        return gridCells[x, y];
    }
}