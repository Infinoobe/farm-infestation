using UnityEngine;

[ExecuteAlways]
public class GridSystemTool : MonoBehaviour
{
    public float gridCellSize = 1f;
    public int width = 10;
    public int height = 10;
    public GameObject gridCellPrefab;
    public GridCell[,] gridCells;
    private Vector3 lowerLeftCorner;

    void Awake()
    {
        gridCells = new GridCell[width, height];
        lowerLeftCorner = transform.position - new Vector3(width, 0, height) * 0.5f * gridCellSize;
        foreach (Transform child in transform)
        {
            GridCell cell = child.GetComponent<GridCell>();
            (int x, int y) = WorldToGridPosition(cell.gameObject.transform.position);
            gridCells[x, y] = cell;
        }
    }

    public void GenerateGrid()
    {
        if (gridCellSize <= 0 || width <= 0 || height <= 0)
        {
            Debug.Log("Values can't be negative");
            return;
        }

        transform.localScale = new Vector3(width / 10f * gridCellSize, 1f, height / 10f * gridCellSize);

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


    private void PlaceGridCells()
    {
        gridCells = new GridCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridCells[x, y] = Instantiate(gridCellPrefab, GridToWorldPosition(x, y), Quaternion.identity, gameObject.transform).GetComponent<GridCell>();
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