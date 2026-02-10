using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;


public class SkillNode : MonoBehaviour
{
    public SkillSO skillSO;
    public Image skillIcon;
    public Button skillButton;
    public bool isUnlocked;
    public bool isResearched;

    private void Start()
    {
        isResearched = false;
    }

    private void OnValidate()
    {
        if(skillSO != null)
        {  
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        skillIcon.sprite = skillSO.skillIcon;
        skillButton.interactable = isUnlocked;
        if (isResearched)
        {
            skillIcon.color = new Color(0, 255, 0);
        }
        else
        {
            skillIcon.color = new Color(255, 255, 255);
        }
    }

    public void TryResearch()
    {
        if (isUnlocked && !isResearched &&
            GameState.Instance.PullItems(skillSO.GetItemsDictionary()))
        {
            Debug.Log("Researched " + skillSO.name);
            isResearched = true;
            UpdateUI();
        }
        else
        {
            Debug.Log("Missing required items");
        }
    }
}
