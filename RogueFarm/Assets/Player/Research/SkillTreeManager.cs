using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillTreeManager : MonoBehaviour
{
    public SkillNode[] skillNodes;
    public Item carrotSeedItem;
    public Item weedSeedItem;
    public GameObject researchPanel;
    public GameObject skillInfoPanel;
    private GameObject currentSkillNodeGameobject;
    public GameObject textPrefab;

    private void Start()
    {
        foreach(SkillNode skillNode in skillNodes)
        {
            skillNode.OnSkillResearched.AddListener(SkillResearched);
            skillNode.OnMouseEnterSkillNode.AddListener(OnMouseEnterSkillNode);
            skillNode.OnMouseExitSkillNode.AddListener(OnMouseExitSkillNode);
        }
        HideInfo();
    }

    private void OnMouseEnterSkillNode(SkillNode skillNode)
    {
        if(currentSkillNodeGameobject != skillNode.gameObject)
        {
            HideInfo();

            if (skillNode.isResearched)
            {
                RectTransform rect = skillInfoPanel.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, 82f);

                GameObject tmp = Instantiate(textPrefab, skillInfoPanel.transform);
                tmp.GetComponent<TextMeshProUGUI>().text = "Researched";
                tmp.GetComponent<TextMeshProUGUI>().color = new Color(0, 255, 0);
            }
            else
            {
                Dictionary<Item, int> requiredItems = skillNode.skillSO.GetItemsDictionary();
                float height = (requiredItems.Count + 1) * 82f;
                RectTransform rect = skillInfoPanel.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
                foreach (var (item, amount) in requiredItems)
                {
                    string itemInfo = $"{amount} x {item.itemName}";
                    GameObject tmp = Instantiate(textPrefab, skillInfoPanel.transform);
                    tmp.GetComponent<TextMeshProUGUI>().text = itemInfo;
                }

                GameObject summaryText = Instantiate(textPrefab, skillInfoPanel.transform);

                if (skillNode.isUnlocked)
                {
                    summaryText.GetComponent<TextMeshProUGUI>().text = "Unlocked";
                }
                else
                {
                    summaryText.GetComponent<TextMeshProUGUI>().text = "Locked";
                    summaryText.GetComponent<TextMeshProUGUI>().color = new Color(255, 0, 0);
                }
                
            }

            currentSkillNodeGameobject = skillNode.gameObject;
            skillInfoPanel.SetActive(true);
           
        }
    }

    private void OnMouseExitSkillNode(SkillNode skillNode)
    {
        if(currentSkillNodeGameobject == skillNode.gameObject)
        {
            HideInfo();
            currentSkillNodeGameobject = null;
        }
    }

    private void Update()
    {
        if (!researchPanel.activeSelf && skillInfoPanel.activeSelf)
        {
            HideInfo();
        }
    }

    private void HideInfo()
    {
        foreach (Transform child in skillInfoPanel.transform)
        {
            Destroy(child.gameObject);
        }
        skillInfoPanel.SetActive(false);
    }

    //private void ShowSkillInfo()
    //{
    //    if (researchPanel.activeSelf && EventSystem.current.IsPointerOverGameObject())
    //    {
    //        PointerEventData pointerData = new PointerEventData(EventSystem.current);
    //        pointerData.position = Input.mousePosition;

    //        List<RaycastResult> results = new List<RaycastResult>();
    //        EventSystem.current.RaycastAll(pointerData, results);

    //        foreach (SkillNode skillNode in skillNodes)
    //        {
    //            if (EventSystem.current.currentSelectedGameObject == skillNode.gameObject)
    //            {
    //                skillInfoPanel.SetActive(true);
                    
    //                return;
    //            }
    //        }
    //    }
    //    ClearInfo();
    //}

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
