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
    [SerializeField] private GameObject buildingToMakePrefab;
    private GameObject currGizmo;
    [SerializeField] private Material ghostMaterialGood;
    [SerializeField] private Material ghostMaterialBad;

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

    private void CreateGizmo(GameObject buildingPrefab, GridCell targetCell)
    {
        currGizmo = Instantiate(buildingPrefab, targetCell.transform.position, Quaternion.identity);
        currGizmo.GetComponent<Building>().isPlaced = false;

        if (CanBuildingBePlaced()) SetGizmoMaterial(ghostMaterialGood);
        else SetGizmoMaterial(ghostMaterialBad);
    }

    private void SetGizmoMaterial(Material gizmoMaterial)
    {
        if (!currGizmo) return;
        Renderer[] renderers = currGizmo.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            Material[] mats = r.materials;

            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = gizmoMaterial;
            }

            r.materials = mats;
        }
    }

    private bool CanBuildingBePlaced()
    {
        return true;    //TODO
    }

    private void DeleteGizmo()
    {
        if (!currGizmo) return;
        Destroy(currGizmo);
        currGizmo = null;
    }

    private void MoveGizmoToCell(GridCell targetCell)
    {
        if (!currGizmo) return;
        currGizmo.transform.position = targetCell.transform.position;
    }

    public void RotateGizmo()
    {
        if (!currGizmo) return;
        currGizmo.transform.rotation *= Quaternion.Euler(0f, 90f, 0f);
    }

    public void Deselect()
    {
        if (!currSelectedCell) return;
        currSelectedCell.HideGizmo();
        currSelectedCell = null;
    }

    public void PlaceBuilding()
    {
        if (!CanBuildingBePlaced()) return;
        // TODO
    }

    public void PointingAtPosition(Vector3 point)
    {
        (int x, int y) = WorldToGridPosition(point);
        GridCell targetCell = GetGridCell(x, y);
        if (!targetCell)
        {
            DeleteGizmo();
            currSelectedCell = null;
            return;
        }

        if (currGizmo) MoveGizmoToCell(targetCell);
        else CreateGizmo(buildingToMakePrefab, targetCell);
        currSelectedCell = targetCell;
    }

    private (int x, int y) WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - lowerLeftCorner.x) / gridCellSize);
        int y = Mathf.FloorToInt((worldPosition.z - lowerLeftCorner.z) / gridCellSize);
        return (x, y);
    }

    private GridCell GetGridCell(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return null;
        }
        return gridCells[x, y];
    }

    private Vector3 GridToWorldPosition(int x, int y)
    {
        return lowerLeftCorner + new Vector3((x + 0.5f) * gridCellSize, 0, (y + 0.5f) * gridCellSize);
    }
}
