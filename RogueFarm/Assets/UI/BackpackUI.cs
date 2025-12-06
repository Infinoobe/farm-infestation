using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class BackpackUI : MonoBehaviour
{
    [SerializeField] private GameObject itemViewPrefab;

    private void OnEnable()
    {
        ClearBackpackView();
        Dictionary<Item, int> items = GameState.Instance.GetItems();
        foreach (var kvp in items)
        {
            Item item = kvp.Key;
            int amount = kvp.Value;

            if (amount <= 0) continue;

            GameObject obj = Instantiate(itemViewPrefab, transform);

            Image icon = obj.transform.Find("ItemImage").GetComponent<Image>();
            TMP_Text nameText = obj.transform.Find("ItemText").GetComponent<TMP_Text>();

            icon.sprite = item.icon;
            nameText.text = $"{item.itemName}: {amount}";
        }
    }

    private void ClearBackpackView()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }
}
