using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private GridCell[,] gridCells; 
    [SerializeField] private GameObject cellPrefab; // must have gridCell
    private float gridCellSize;
    private int width, height;
    private Vector3 lowerLeftCorner;
    private GridCell currSelectedCell;
    [SerializeField] GameObject testBuild;

    public void Start()
    {
        PlaneResizer pr = gameObject.GetComponent<PlaneResizer>();
        width = pr.width;
        height = pr.height;
        gridCellSize = pr.gridCellSize;

        lowerLeftCorner = transform.position - new Vector3(width, 0, height) * 0.5f * gridCellSize;

        gridCells = new GridCell[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                gridCells[x, y] = Instantiate(cellPrefab, GridToWorldPosition(x, y), Quaternion.identity, gameObject.transform).GetComponent<GridCell>();
            }
        }
    }

    public void Deselect()
    {
        if (!currSelectedCell) return;
        currSelectedCell.HideGizmo();
        currSelectedCell = null;
    }

    public void PlaceBuilding()
    {
        
    }

    public void PointingAtPosition(Vector3 point)
    {
        (int x, int y) = WorldToGridPosition(point);
        if (x < 0 || x >= width || y < 0 || y >= height) 
        {
            if(currSelectedCell) currSelectedCell.HideGizmo();
            currSelectedCell = null;
            return;
        }

        GridCell pointedCell = gridCells[x, y];
        if (currSelectedCell && pointedCell != currSelectedCell)
        {
            currSelectedCell.HideGizmo();
        }

        pointedCell.ShowGizmo(testBuild);
        currSelectedCell = pointedCell;
    }

    private (int x, int y) WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - lowerLeftCorner.x) / gridCellSize);
        int y = Mathf.FloorToInt((worldPosition.z - lowerLeftCorner.z) / gridCellSize);
        return (x, y);
    }

    private Vector3 GridToWorldPosition(int x, int y)
    {
        return lowerLeftCorner + new Vector3((x + 0.5f) * gridCellSize, 0, (y + 0.5f) * gridCellSize);
    }
}
