using NUnit.Framework.Constraints;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField] private Plant currentPlant;

    public void PlantSeed(Plant plantPrefab)
    {
        if (currentPlant == null)
        {
            currentPlant = Instantiate(plantPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    public void CollectPlant()
    {
        if (!CanBeCollected()) return;
        currentPlant.CollectItem();
    }

    public bool IsEmpty()
    {
        return currentPlant == null;
    }

    public bool CanBeCollected()
    {
        if (IsEmpty()) return false;
        return currentPlant.IsGrown;
    }
}
