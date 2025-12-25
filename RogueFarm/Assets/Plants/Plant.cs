using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Plant : MonoBehaviour, IDamagable
{
    private bool isGrown = false;
    [SerializeField] private Item collectItem;
    [SerializeField] private int collectItemAmount;
    [SerializeField] private GameObject growingPlant;
    [SerializeField] private GameObject grownPlant;
    [SerializeField] private int growthTime = 2;
    [SerializeField] private int health = 100;
    [SerializeField] private int healthMax = 100;

    [SerializeField] private Canvas canvas;
    [SerializeField] private Image hpBar;
    [SerializeField] private TMP_Text hpLabel;

    public bool IsGrown => isGrown;

    public void Start()
    {
        growingPlant.SetActive(!isGrown);
        grownPlant.SetActive(isGrown);
        GameState.Instance.OnDayStarted.AddListener(HandleDayStarted);
        canvas.enabled = false;
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

    public void TakeDamage(int damage)
    {
        health = Math.Max(0, health - damage);
        canvas.enabled = true;
        hpBar.fillAmount = ((float)health) / healthMax;
        hpLabel.text = health + "/" + healthMax;
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
