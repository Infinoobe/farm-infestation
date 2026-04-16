using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject buyingLayout;
    [SerializeField] private GameObject sellingLayout;
    [SerializeField] private GameObject buyingItemView;
    [SerializeField] private GameObject sellingItemView;

    
    private void OnEnable()
    {
        if (GameState.Instance == null) return;
        RefreshView();
        GameState.Instance.RefreshShop.AddListener(RefreshView);
    }
    

    private void RefreshView()
    {
        ClearShopView();
        CreateBuyingLayout();
        CreateSellingLayout();
    }

    private void ClearShopView()
    {
        foreach (Transform child in buyingLayout.transform)
            Destroy(child.gameObject);
        foreach (Transform child in sellingLayout.transform)
            Destroy(child.gameObject);
    }

    private void CreateBuyingLayout()
    {
        foreach(ItemSO item in GameState.Instance.ItemsInShop)
        {
            if (item.unique && GameState.Instance.HasItems(item))
            {
                continue;
            }
            GameObject obj = Instantiate(buyingItemView, buyingLayout.transform);
            Image icon = obj.transform.Find("ItemImage").GetComponent<Image>();
            TMP_Text nameText = obj.transform.Find("ItemText").GetComponent<TMP_Text>();
            icon.sprite = item.icon;
            nameText.text = $"{item.itemName} - {item.valueBuying}$";
            Button button = obj.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => GameState.Instance.BuyItem(item, 1));
        }
    }

    private void CreateSellingLayout()
    {
        if (GameState.Instance == null)
            return;
        Dictionary<ItemSO, int> items = GameState.Instance.GetItems();
        foreach (var kvp in items)
        {
            ItemSO itemSo = kvp.Key;
            int amount = kvp.Value;

            if (itemSo.canBeSold == false ) continue;
            if (amount <= 0) continue;
            if (itemSo.itemType == ItemType.UPGRADE) continue;

            GameObject obj = Instantiate(sellingItemView, sellingLayout.transform);

            Image icon = obj.transform.Find("ItemImage").GetComponent<Image>();
            TMP_Text nameText = obj.transform.Find("ItemText").GetComponent<TMP_Text>();

            icon.sprite = itemSo.icon;
            nameText.text = $"{itemSo.itemName}: {amount} ({itemSo.valueSelling}$)";
            Button button = obj.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => GameState.Instance.SellItem(itemSo, 1));
        }
    }

}
