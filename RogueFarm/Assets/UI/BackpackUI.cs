using System;
using System.Collections.Generic;
using UnityEngine;

public class BackpackUI : MonoBehaviour
{
    public GameObject itemsList;
    [SerializeField] private GameObject itemViewPrefab;

    private void Start()
    {
        GameState.Instance.RefreshBackpack.AddListener(RefreshItems);
    }

    private void OnEnable()
    {
        if (GameState.Instance == null)
            return;
        RefreshItems();
    }

    private void RefreshItems()
    {

        if (itemsList == null)
        {
            Debug.LogError("UI error, items list not initiated properly");
            return;
        }

        ClearBackpackView();
        Dictionary<Item, int> items = GameState.Instance.GetItems();
        foreach (var kvp in items)
        {
            Item item = kvp.Key;
            int amount = kvp.Value;

            if (amount <= 0) continue;

            GameObject obj = Instantiate(itemViewPrefab, itemsList.transform);
            var itemUI = obj.GetComponent<BackpackItemUI>();
            itemUI.SetItem(item, amount);
        }

    }

    private void ClearBackpackView()
    {
        if (itemsList == null) return;
        if (itemsList.transform == null) return;
        foreach (Transform child in itemsList.transform)
            Destroy(child.gameObject);
    }
}
