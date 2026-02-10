using System;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public SkillNode[] skillNodes;
    public Item carrotSeedItem;
    public Item weedSeedItem;

    private void Start()
    {
        foreach(SkillNode skillNode in skillNodes)
        {
            skillNode.OnSkillResearched.AddListener(SkillResearched);
        }
    }

    public void SkillResearched(SkillNode researchedSkill)
    {
        string skillName = researchedSkill.skillSO.skillName;
        Debug.Log("Researched: " + skillName);

        switch (skillName)
        {
            case "Carrot Plant":
                GameState.Instance.AddItemToShop.Invoke(carrotSeedItem);
                break;
            case "Weed Plant":
                GameState.Instance.AddItemToShop.Invoke(weedSeedItem);
                break;
            default:
                Debug.Log("Researched skill does nothing");
                break;
        }

        foreach (SkillNode skillNode in skillNodes)
        {
            skillNode.TryUnlock();
        }
    }
}
