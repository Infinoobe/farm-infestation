using NUnit.Framework.Constraints;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField] private Plant currentPlant;

    [SerializeField] private Plant weedPrefabTest;
    [ContextMenu("Plant Seed")]
    public void test() {
        PlantSeed(weedPrefabTest);
    }


    public void PlantSeed(Plant plantPrefab)
    {
        if (currentPlant == null)
        {
            currentPlant = Instantiate(plantPrefab, transform.position, Quaternion.identity, transform);
        }
    }
}
