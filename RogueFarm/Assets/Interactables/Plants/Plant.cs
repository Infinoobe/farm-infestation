using UnityEngine;
using System;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.ComponentModel;

public class Plant : MonoBehaviour
{
    [Header("Plant growing settings")]
    [SerializeField] protected int collectItemAmount = 1;
    [SerializeField] protected int growthTime = 2;
    [SerializeField] protected int collectTimes = 1;
    [SerializeField] private GameObject[] growthStages; // Size = growthTime; index 0 -> stage at day 0
    [SerializeField] private GameObject[] collectStages; // Size = collectTimes; index 0 -> stage at collecting 0 times

    [Header("Plant combat settings")]
    [SerializeField] protected int healthMax = 100;
    [SerializeField] protected float attractionFactor = 1f;
    [SerializeField] protected bool isVulnerable = true;

    [Header("Playtime variables")]
    [SerializeField] private GameObject currActiveVisualObject;
    [SerializeField] protected int currHealth = 100;
    [SerializeField] protected bool isWatered;
    [SerializeField] public bool canBeCollected = false;
    [SerializeField] protected int currGrowthTime = 0;
    [SerializeField] protected int currCollectTimes = 0;

    [Header("Object settings")]
    [FormerlySerializedAs("collectItem")][SerializeField] private ItemSO collectItemSo;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image hpBar;
    [SerializeField] private TMP_Text hpLabel;
    
    public bool IsWatered
    {
        get => isWatered;
        set => isWatered = value;
    }
    public float AttractionFactor => attractionFactor;
    public bool IsVulnerable => isVulnerable;
    public ItemSO CollectItemSO => collectItemSo;

    public void Start()
    {
        GameState.Instance.OnDayStarted.AddListener(HandleDayStarted);
        canvas.enabled = false;
        isWatered = false;

        InitVisuals();
    }

    public int HowManyDaysToGrow()
    {
        return growthTime - currGrowthTime;
    }

    protected void InitVisuals()
    {
        foreach (var v in collectStages) v.SetActive(false);
        foreach (var v in growthStages) v.SetActive(false);

        currActiveVisualObject = GetCurrentVisual();
        currActiveVisualObject.SetActive(true);
    }
    
    protected void UpdateObjectVisual()
    {
        var nextVisual = GetCurrentVisual();
        if (!nextVisual || nextVisual == currActiveVisualObject) return;
        currActiveVisualObject.SetActive(false);
        nextVisual.SetActive(true);
        currActiveVisualObject = nextVisual;
    }

    private GameObject GetCurrentVisual()
    {
        //Debug.Log($"GetCurrentVisual [{canBeCollected}] <{(canBeCollected ? currCollectTimes : currGrowthTime)}>", this);
        if (canBeCollected)
        {
            return collectStages[currCollectTimes];
        }
        return growthStages[currGrowthTime];
    }

    private void HandleDayStarted()
    {
        if (isWatered) Grow();
        isWatered = false;
    }

    public void TakeDamage(int damage)
    {
        currHealth = Math.Max(0, currHealth - damage);
        canvas.enabled = true;
        hpBar.fillAmount = ((float)currHealth) / healthMax;
        hpLabel.text = currHealth + "/" + healthMax;
        if (currHealth <= 0) KillYourself();
    }

    public void KillYourself()
    {
        GameState.Instance.OnDayStarted.RemoveListener(HandleDayStarted);
        Destroy(gameObject);
    }

    public virtual void CollectItem()
    {
        if (!canBeCollected) return;
        GameState.Instance.AddItem(collectItemSo, collectItemAmount);
        currCollectTimes++;
        if(currCollectTimes >= collectTimes)
        {
            KillYourself();
        }
        else
        {
            canBeCollected = false;
            UpdateObjectVisual();
        }
    }

    public void Grow()
    {
        if (canBeCollected) return;
        currGrowthTime++;
        if (currGrowthTime > 0 && currGrowthTime % growthTime == 0)
        {
            canBeCollected = true;
        }
        UpdateObjectVisual();
    }

}
