using UnityEngine;

public class Tree : Building
{
    [SerializeField] private GameObject treeModel;
    private void Start()
    {
        treeModel.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }
}
