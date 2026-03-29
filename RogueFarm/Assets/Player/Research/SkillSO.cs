using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSkill")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public Sprite skillIcon;
    public List<ItemAmount> requiredItems = new List<ItemAmount>();
    private Dictionary<ItemSO, int> itemsDictionary;

    public void InitializeDictionary()
    {
        itemsDictionary = new Dictionary<ItemSO, int>();

        foreach (var entry in requiredItems)
        {
            if (entry == null || entry.itemSo == null)
                continue;

            if (itemsDictionary.ContainsKey(entry.itemSo))
                itemsDictionary[entry.itemSo] += entry.amount;
            else
                itemsDictionary.Add(entry.itemSo, entry.amount);
        }
    }

    public Dictionary<ItemSO, int> GetItemsDictionary()
    {
        if (itemsDictionary == null)
            InitializeDictionary();

        return itemsDictionary;
    }

    public int GetCost(ItemSO itemSo)
    {
        var dict = GetItemsDictionary();

        if (dict.TryGetValue(itemSo, out int amount))
            return amount;

        return 0;
    }

    public bool RequiresItem(ItemSO itemSo)
    {
        return GetItemsDictionary().ContainsKey(itemSo);
    }

    private void OnEnable()
    {
        InitializeDictionary();
    }
}
