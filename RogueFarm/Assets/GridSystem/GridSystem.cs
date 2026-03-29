using UnityEngine;
using System.Collections.Generic;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private GridCell[,] gridCells; 
    [SerializeField] private GameObject cellPrefab; // must have gridCell
    private float gridCellSize;
    private int width, height;
    private Vector3 lowerLeftCorner;
    private GameObject currGizmo;
    [SerializeField] private Material ghostMaterialGood;
    [SerializeField] private Material ghostMaterialBad;

    private ItemSO lastSelectedItemSo;
    
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

    private void CreateGizmo(ItemSO itemSo, GridCell targetCell)
    {
        var buildingPrefab = itemSo.buildingPrefab;
        if (buildingPrefab == null)
        {
            Debug.LogError($"Item {itemSo.name} : Missing building prefab on.");
        }

        currGizmo = Instantiate(buildingPrefab, targetCell.transform.position, Quaternion.identity);
        var b = currGizmo.GetComponent<Building>();
        if (b == null)
        {
            Debug.LogError($"Item {itemSo.name} : buildingPrefab {buildingPrefab.name} is not a Building.");
        }
        b.isPlaced = false;

        currGizmo.layer = LayerMask.NameToLayer("BuildingGizmo");
        SetLayerRecursively(currGizmo, currGizmo.layer);

        UpdateGizmoMaterial(itemSo);
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private void UpdateGizmoMaterial (ItemSO itemSo)
    {
        if (!currGizmo) return;
        if (CanBuildingBePlaced(itemSo)) SetGizmoMaterial(ghostMaterialGood);
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

    private bool CanBuildingBePlaced(ItemSO itemSo)
    {
        if (!currGizmo) return false;
        List<GridCell> targetCells = GetOverlapingCells();
        if (targetCells == null || targetCells.Count == 0) return false;
        foreach(GridCell cell in targetCells)
        {
            if (!cell.IsEmpty()) return false;
        }
        return GameState.Instance.HasItems(itemSo);
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

    public void RotateGizmo(bool rotateCounterclockwise = false)
    {
        if (!currGizmo) return;
        var angle = 90;
        if (rotateCounterclockwise) angle *= -1;
        currGizmo.transform.rotation *= Quaternion.Euler(0f, angle, 0f);
    }

    public void PlaceBuilding(ItemSO itemSoUsed)
    {
        if (!CanBuildingBePlaced(itemSoUsed)) return;

        if (itemSoUsed.itemType == ItemType.BUILDING)
        {
            GameState.Instance.RemoveItem(itemSoUsed);
        }

        GameObject newBuilding = Instantiate(itemSoUsed.buildingPrefab, currGizmo.transform.position, currGizmo.transform.rotation);
        newBuilding.transform.SetParent(gameObject.transform, true);
        List<GridCell> targetCells = GetOverlapingCells();
        foreach(GridCell cell in targetCells)
        {
            cell.SetBuilding(newBuilding);
        }
        DeleteGizmo();
        var b = newBuilding.GetComponent<Building>();
        b.PlaceBuilding(targetCells);
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

    public void PointingAtPosition(Vector3 point, ItemSO selectedItemSo)
    {
        (int x, int y) = WorldToGridPosition(point);
        GridCell targetCell = GetGridCell(x, y);
        if (!targetCell)
        {
            DeleteGizmo();
            return;
        }
        if (currGizmo == null) 
        {
            CreateGizmo(selectedItemSo, targetCell);
            lastSelectedItemSo = selectedItemSo;
            return;
        }

        if (lastSelectedItemSo != selectedItemSo)
        {
            DeleteGizmo();
            CreateGizmo(selectedItemSo, targetCell);
            lastSelectedItemSo = selectedItemSo;
            return;
        }

        MoveGizmoToCell(targetCell);
        UpdateGizmoMaterial(selectedItemSo);

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
