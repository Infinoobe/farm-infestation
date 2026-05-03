using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSkill")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public Sprite skillIcon;
    [SerializeField] private ItemSet requiredItems;
    [SerializeField] private List<ItemAmount> requiredItemsList = new ();
    
    public ItemSet GetRequiredItems()
    {
        if (requiredItems == null)
            InitializeDictionary();
        return requiredItems;
    }

    public void InitializeDictionary()
    {
        requiredItems = new ItemSet(requiredItemsList);
    }
}
