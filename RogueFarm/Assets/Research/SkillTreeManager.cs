using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class SkillTreeManager : MonoBehaviour
{
    public SkillNode[] skillNodes;
    [FormerlySerializedAs("carrotSeedItem")] public ItemSO carrotSeedItemSo;
    [FormerlySerializedAs("weedSeedItem")] public ItemSO weedSeedItemSo;
    [FormerlySerializedAs("strawberrySeedItem")] public ItemSO strawberrySeedItemSo;
    public GameObject researchPanel;
    public GameObject skillInfoPanel;
    private GameObject currentSkillNodeGameobject;
    public GameObject textPrefab;
    public GameObject linePrefab;

    private void Start()
    {
        foreach(SkillNode skillNode in skillNodes)
        {
            skillNode.OnSkillResearched.AddListener(SkillResearched);
            skillNode.OnMouseEnterSkillNode.AddListener(OnMouseEnterSkillNode);
            skillNode.OnMouseExitSkillNode.AddListener(OnMouseExitSkillNode);

            if (skillNode.requiredSkillsToUnlock == null || skillNode.requiredSkillsToUnlock.Count == 0) continue;
            RectTransform panel = researchPanel.GetComponent<RectTransform>();
            RectTransform a = skillNode.GetComponent<RectTransform>();
            foreach (SkillNode prevSkill in skillNode.requiredSkillsToUnlock)
            {
                RectTransform b = prevSkill.GetComponent<RectTransform>();   

                RectTransform line = Instantiate(linePrefab, panel).GetComponent<RectTransform>();

                DrawLine(panel, a, b, line);
            }
        }
        HideInfo();
    }

    void DrawLine(RectTransform panel, RectTransform a, RectTransform b, RectTransform line)
    {
        Vector2 localA;
        Vector2 localB;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panel,
            RectTransformUtility.WorldToScreenPoint(null, a.position),
            null,
            out localA
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panel,
            RectTransformUtility.WorldToScreenPoint(null, b.position),
            null,
            out localB
        );

        Vector2 direction = localB - localA;
        float distance = direction.magnitude;

        line.anchoredPosition = localA + direction * 0.5f;
        line.sizeDelta = new Vector2(distance, 2f);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        line.localRotation = Quaternion.Euler(0, 0, angle);
        line.SetAsFirstSibling();
    }

    private void OnMouseEnterSkillNode(SkillNode skillNode)
    {
        if (currentSkillNodeGameobject == skillNode.gameObject) return;
        
        HideInfo();

        currentSkillNodeGameobject = skillNode.gameObject;
        skillInfoPanel.SetActive(true);
        
        if (skillNode.isResearched)
        {
            SetPopupHeight(82f);
            AddPopupLine("Researched", Color.green);
            return;
        }
        
        Dictionary<ItemSO, int> requiredItems = skillNode.skillSO.GetItemsDictionary();
        SetPopupHeight(82f * (requiredItems.Count + 1));
        foreach (var (item, amount) in requiredItems)
        {
            var color = GameState.Instance.HasItems(item, amount) ? Color.green : Color.red;
            AddPopupLine($"{amount} x {item.itemName}", color);
        }
        if (skillNode.isUnlocked)
        {
            AddPopupLine("Available", Color.green);
        }
        else
        {
            AddPopupLine("Requirements not met", Color.red);
        }
    }

    private void SetPopupHeight(float height)
    {
        RectTransform rect = skillInfoPanel.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
    }

    private void AddPopupLine(string text, Color? color = null)
    {
        GameObject tmp = Instantiate(textPrefab, skillInfoPanel.transform);
        tmp.GetComponent<TextMeshProUGUI>().text = text;
        if (color.HasValue)
        {
            tmp.GetComponent<TextMeshProUGUI>().color = color.Value;
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
                GameState.Instance.AddItemToShop(carrotSeedItemSo);
                break;
            case "Weed Plant":
                GameState.Instance.AddItemToShop(weedSeedItemSo);
                break;
            case "Strawberry Plant":
                GameState.Instance.AddItemToShop(strawberrySeedItemSo);
                break;
            case "Damage Boost I":
                //TODO
                break;
            case "Damage Boost II":
                //TODO
                break;
            case "Max Health Boost I":
                //TODO
                break;
            case "Max Health Boost II":
                //TODO
                break;
            case "Regeneration I":
                //TODO
                break;
            case "Regeneration II":
                //TODO
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
