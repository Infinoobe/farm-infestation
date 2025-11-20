using UnityEngine;
using System;

public abstract class Plant : MonoBehaviour, IDamagable
{
    private bool isGrown = false;
    [SerializeField] private GameObject growingPlant;
    [SerializeField] private GameObject grownPlant;
    [SerializeField] private float GrowthTime = 5f;
    [SerializeField] private int health = 100;

    public void Start()
    {
        growingPlant.SetActive(!isGrown);
        grownPlant.SetActive(isGrown);
        Invoke(nameof(Grow), GrowthTime);
    }

    public void DealDamage(int damage)
    {
        health = Math.Max(0, health - damage);
        if (health <= 0) KillYourself();
    }

    public void KillYourself()
    {
        Destroy(gameObject);
    }

    public void Grow()
    {
        if (isGrown) return;
        isGrown = true;
        growingPlant.SetActive(false);
        grownPlant.SetActive(true);
    }

}
