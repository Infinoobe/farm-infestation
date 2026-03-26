using Unity.VisualScripting;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private GridCell[,] gridCells;
    private float gridCellSize;
    private int width, height;

    public void Start()
    {
        PlaneResizer pr = gameObject.GetComponent<PlaneResizer>();
        width = pr.width;
        height = pr.height;
        gridCellSize = pr.gridCellSize;

        gridCells = new GridCell[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                gridCells[x, y] = new GridCell();
            }
        }
    }
}
