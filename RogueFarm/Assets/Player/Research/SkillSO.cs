using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSkill")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public Sprite skillIcon;
    public List<ItemAmount> requiredItems = new List<ItemAmount>();
    private Dictionary<Item, int> itemsDictionary;

    public void InitializeDictionary()
    {
        itemsDictionary = new Dictionary<Item, int>();

        foreach (var entry in requiredItems)
        {
            if (entry == null || entry.item == null)
                continue;

            if (itemsDictionary.ContainsKey(entry.item))
                itemsDictionary[entry.item] += entry.amount;
            else
                itemsDictionary.Add(entry.item, entry.amount);
        }
    }

    public Dictionary<Item, int> GetItemsDictionary()
    {
        if (itemsDictionary == null)
            InitializeDictionary();

        return itemsDictionary;
    }

    public int GetCost(Item item)
    {
        var dict = GetItemsDictionary();

        if (dict.TryGetValue(item, out int amount))
            return amount;

        return 0;
    }

    public bool RequiresItem(Item item)
    {
        return GetItemsDictionary().ContainsKey(item);
    }

    private void OnEnable()
    {
        InitializeDictionary();
    }
}
