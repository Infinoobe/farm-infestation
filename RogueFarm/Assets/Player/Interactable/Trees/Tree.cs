using UnityEngine;

public class Tree : Building
{
    private void Start()
    {
        transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }
}
