using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using Interactable;

public class Plant : MonoBehaviour, IDamagable
{
    protected bool isGrown = false;
    [SerializeField] private Item collectItem;
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
    public bool IsGrown => isGrown;

    public void Start()
    {
        SetUpObjectVisual();
        GameState.Instance.OnDayStarted.AddListener(HandleDayStarted);
        canvas.enabled = false;
    }

    private void TurnOffAllVisuals()
    {
        foreach (GameObject growthStage in growthStages)
        {
            if (!growthStage) continue;
            growthStage.SetActive(false);
        }
        foreach (GameObject collectStage in collectStages)
        {
            if (!collectStage) continue;
            collectStage.SetActive(false);
        }
    }

    private GameObject FindValidVisualInArray(GameObject[] stagesArray, int startingIndex)
    {
        if (stagesArray == null || stagesArray.Length == 0)  return null;
        startingIndex = Mathf.Min(startingIndex, stagesArray.Length - 1);

        for (int i = startingIndex; i >= 0; i--)
        {
            if (stagesArray[i]) return stagesArray[i];
        }

        return null;
    }

    private GameObject CurrentVisualObject()
    {
        if (isGrown)
        {
            return FindValidVisualInArray(collectStages, currCollectTimes);
        }
        else
        {
            return FindValidVisualInArray(growthStages, currGrowthTime);
        }
    }

    protected void SetUpObjectVisual()
    {
        TurnOffAllVisuals();
        GameObject currVisualObject = CurrentVisualObject();

        if (currVisualObject)
        {
            currActiveVisualObject = currVisualObject;
            currVisualObject.SetActive(true);
        }
        else
        {
            Debug.Log($"WARNING: No visual model found [{collectItem.itemName}]");
        }
    }

    protected void UpdateObjectVisual()
    {
        GameObject nextVisual;
        if (isGrown)
        {
            nextVisual = collectStages[currCollectTimes];
        }
        else
        {
            nextVisual = growthStages[currGrowthTime];
        }

        if (!nextVisual || nextVisual == currActiveVisualObject) return;
        currActiveVisualObject.SetActive(false);
        nextVisual.SetActive(true);
    }

    private void HandleDayStarted()
    {
        Grow();
        UpdateObjectVisual();
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
        if (!isGrown) return;
        GameState.Instance.AddItem(collectItem, collectItemAmount);
        currCollectTimes++;
        if(currCollectTimes >= collectTimes)
        {
            KillYourself();
        }
        UpdateObjectVisual();
    }

    public virtual void Grow()
    {
        if (isGrown) return;
        currGrowthTime++;
        if (currGrowthTime >= growthTime)
        {
            isGrown = true;
            return;
        }
    }

    //protected void FullyGrown()
    //{
    //    //if (isGrown) return;
    //    isGrown = true;
    //}

}
