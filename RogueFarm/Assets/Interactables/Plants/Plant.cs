using UnityEngine;
using System;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Plant : MonoBehaviour, IDamagable
{
    private bool canBeCollected = false;
    [FormerlySerializedAs("collectItem")] [SerializeField] private ItemSO collectItemSo;
    [SerializeField] private int collectItemAmount = 1;
    [SerializeField] private int growthTime = 2;
    [SerializeField] private int collectTimes = 1;
    [SerializeField] private GameObject[] growthStages; // Size = growthTime; index 0 -> stage at day 0
    [SerializeField] private GameObject[] collectStages; // Size = collectTimes; index 0 -> stage at collecting 0 times
    [SerializeField] private int healthMax = 100;

    [SerializeField] private Canvas canvas;
    [SerializeField] private Image hpBar;
    [SerializeField] private TMP_Text hpLabel;

    protected int health = 100;
    protected int currGrowthTime = 0;
    protected int currCollectTimes = 0;
    private GameObject currActiveVisualObject;
    public bool CanBeCollected => canBeCollected;

    public void Start()
    {
        GameState.Instance.OnDayStarted.AddListener(HandleDayStarted);
        canvas.enabled = false;

        InitVisuals();
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
        Grow();
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
