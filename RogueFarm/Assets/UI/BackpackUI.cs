using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class BackpackUI : MonoBehaviour
{
    public GameObject itemsList;
    [SerializeField] private GameObject itemViewPrefab;

    private void OnEnable()
    {
        if (GameState.Instance == null)
            return;
        ClearBackpackView();
        Dictionary<Item, int> items = GameState.Instance.GetItems();
        foreach (var kvp in items)
        {
            Item item = kvp.Key;
            int amount = kvp.Value;

            if (amount <= 0) continue;

            GameObject obj = Instantiate(itemViewPrefab, itemsList.transform);

            Image icon = obj.transform.Find("ItemImage").GetComponent<Image>();
            TMP_Text nameText = obj.transform.Find("ItemText").GetComponent<TMP_Text>();

            icon.sprite = item.icon;
            nameText.text = $"{item.itemName}: {amount}";
        }
    }

    private void ClearBackpackView()
    {
        foreach (Transform child in itemsList.transform)
            Destroy(child.gameObject);
    }
}
