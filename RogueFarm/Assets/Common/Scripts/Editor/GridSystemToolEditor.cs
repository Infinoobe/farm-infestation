using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSystemTool))]
public class GridSystemToolEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridSystemTool grid = (GridSystemTool)target;

        if (GUILayout.Button("Generate Grid"))
        {
            grid.GenerateGrid();
        }

        if (GUILayout.Button("Clear Grid"))
        {
            grid.ClearGridCells();
        }
    }
}