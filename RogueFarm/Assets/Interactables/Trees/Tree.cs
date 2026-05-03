using UnityEngine;

public class Tree : Building
{
    [SerializeField] private GameObject treeModel;
    protected override void OnStart()
    {
        treeModel.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }
}
