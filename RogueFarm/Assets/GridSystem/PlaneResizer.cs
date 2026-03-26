using UnityEngine;

[ExecuteAlways]
public class PlaneResizer : MonoBehaviour
{
    public float gridCellSize = 1f;
    public int width = 10;
    public int height = 10;

    void OnValidate()
    {
        if(gridCellSize <= 0 || width <= 0 || height <= 0)
        {
            Debug.Log("Values can't be negative");
            return;
        }
        Resize();
    }

    void Resize()
    {
        transform.localScale = new Vector3(width / 10f * gridCellSize, 1f, height / 10f * gridCellSize);
    }
}