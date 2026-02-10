using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public SkillNode[] skillNodes;

    private void Start()
    {
        foreach(SkillNode skillNode in skillNodes)
        {
            skillNode.OnSkillResearched.AddListener(SkillResearched);
        }
    }

    public void SkillResearched(SkillNode researchedSkill)
    {
        Debug.Log("Researched " + researchedSkill.skillSO.name);
        foreach (SkillNode skillNode in skillNodes)
        {
            skillNode.TryUnlock();
        }
    }
}
