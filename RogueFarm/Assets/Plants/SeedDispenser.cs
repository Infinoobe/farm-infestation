using System;
using UnityEngine;

public class SeedDispenser : MonoBehaviour
{
    public Plant plantToUse;
    private void OnTriggerEnter(Collider other)
    {
        var p = other.gameObject.GetComponent<Player>();
        if (p == null) { return; }
        p.SetPlantToUse(plantToUse);
    }
}
