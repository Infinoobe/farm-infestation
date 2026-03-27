using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public class SkillNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public List<SkillNode> requiredSkillsToUnlock;
    public SkillSO skillSO;
    public Image skillIcon;
    public Button skillButton;
    public bool isUnlocked;
    public bool isResearched;

    public UnityEvent<SkillNode> OnSkillResearched = new UnityEvent<SkillNode>();
    public UnityEvent<SkillNode> OnMouseEnterSkillNode = new UnityEvent<SkillNode>();
    public UnityEvent<SkillNode> OnMouseExitSkillNode = new UnityEvent<SkillNode>();

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

    public bool TryUnlock()
    {
        if(!isUnlocked && CanUnlock())
        {
            isUnlocked = true;
            UpdateUI();
            return true;
        }
        return false;
    }

    public bool CanUnlock()
    {
        foreach(SkillNode skill in requiredSkillsToUnlock)
        {
            if(skill.isResearched == false)
            {
                return false;
            }
        }
        return true;
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
            GameState.Instance.RemoveItems(skillSO.GetItemsDictionary()))
        {
            isResearched = true;
            UpdateUI();
            OnSkillResearched?.Invoke(this);
            OnMouseExitSkillNode?.Invoke(this);
            OnMouseEnterSkillNode?.Invoke(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseEnterSkillNode?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExitSkillNode?.Invoke(this);
    }
}
