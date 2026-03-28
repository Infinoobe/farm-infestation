using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;
using System.Linq;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private GridCell[,] gridCells; 
    [SerializeField] private GameObject cellPrefab; // must have gridCell
    private float gridCellSize;
    private int width, height;
    private Vector3 lowerLeftCorner;
    [SerializeField] private GameObject buildingToMakePrefab;
    private GameObject currGizmo;
    [SerializeField] private Material ghostMaterialGood;
    [SerializeField] private Material ghostMaterialBad;

    public bool HasGizmo()
    {
        return currGizmo != null;
    }

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

        currGizmo.layer = LayerMask.NameToLayer("BuildingGizmo");
        foreach (Transform child in currGizmo.transform)
            child.gameObject.layer = currGizmo.layer;

        UpdateGizmoMaterial();
    }

    private void UpdateGizmoMaterial ()
    {
        if (!currGizmo) return;
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
        if (!currGizmo) return false;
        List<GridCell> targetCells = GetOverlapingCells();
        if (targetCells == null || targetCells.Count == 0) return false;
        foreach(GridCell cell in targetCells)
        {
            if (!cell.IsEmpty()) return false;
        }
        //TODO enough resources?
        return true;
    }

    public void DeleteGizmo()
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

    public void PlaceBuilding()
    {
        if (!CanBuildingBePlaced()) return;

        // TODO: Grab resources
        GameObject newBuilding = Instantiate(buildingToMakePrefab, currGizmo.transform.position, currGizmo.transform.rotation);
        newBuilding.transform.SetParent(gameObject.transform, true);
        List<GridCell> targetCells = GetOverlapingCells();
        foreach(GridCell cell in targetCells)
        {
            cell.SetBuilding(newBuilding);
        }
        DeleteGizmo();
    }

    private List<GridCell> GetOverlapingCells()
    {
        if (!currGizmo) return new List<GridCell>();
        BuildingShapeUnit[] buildPoints = currGizmo.GetComponent<Building>().BuildingShapeUnits;
        List<GridCell> targetCells = new List<GridCell>();
        foreach(BuildingShapeUnit unit in buildPoints)
        {
            (int x, int y) = WorldToGridPosition(unit.gameObject.transform.position);
            GridCell cell = GetGridCell(x, y);
            if (!cell) return new List<GridCell>();
            targetCells.Add(cell);
        }
        return targetCells;
    }

    public void PointingAtPosition(Vector3 point)
    {
        (int x, int y) = WorldToGridPosition(point);
        GridCell targetCell = GetGridCell(x, y);
        if (!targetCell)
        {
            DeleteGizmo();
            return;
        }

        if (currGizmo) 
        {
            MoveGizmoToCell(targetCell);
            UpdateGizmoMaterial();
        }
        else CreateGizmo(buildingToMakePrefab, targetCell);
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
