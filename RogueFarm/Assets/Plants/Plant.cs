using UnityEngine;
using System;

public class Plant : MonoBehaviour, IDamagable
{
    private bool isGrown = false;
    [SerializeField] private Item collectItem;
    [SerializeField] private int collectItemAmount;
    [SerializeField] private GameObject growingPlant;
    [SerializeField] private GameObject grownPlant;
    [SerializeField] private int growthTime = 2;
    [SerializeField] private int health = 100;

    public bool IsGrown => isGrown;

    public void Start()
    {
        growingPlant.SetActive(!isGrown);
        grownPlant.SetActive(isGrown);
        GameState.Instance.OnDayStarted.AddListener(HandleDayStarted);
    }

    private void HandleDayStarted()
    {
        if (isGrown) return;
        growthTime--;
        if (growthTime <= 0)
        {
            Grow();
        }
    }

    public void DealDamage(int damage)
    {
        health = Math.Max(0, health - damage);
        if (health <= 0) KillYourself();
    }

    public void KillYourself()
    {
        GameState.Instance.OnDayStarted.RemoveListener(HandleDayStarted);
        Destroy(gameObject);
    }

    public void CollectItem()
    {
        if (!isGrown) return;
        GameState.Instance.AddItem(collectItem, collectItemAmount);
        KillYourself();
    }

    public void Grow()
    {
        if (isGrown) return;
        isGrown = true;
        growingPlant.SetActive(false);
        grownPlant.SetActive(true);
    }

}
