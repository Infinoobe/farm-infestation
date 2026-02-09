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
    }

    public void TryResearch()
    {
        if (isUnlocked && GameState.Instance.PullItems(skillSO.GetItemsDictionary()))
        {
            Debug.Log("Researched " + skillSO.name);
            UpdateUI();
        }
        else
        {
            Debug.Log("Missing required items");
        }
    }
}
