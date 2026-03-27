using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject buyingLayout;
    [SerializeField] private GameObject sellingLayout;
    [SerializeField] private GameObject buyingItemView;
    [SerializeField] private GameObject sellingItemView;
    [SerializeField] private List<Item> itemsOnSale;

    
    private void OnEnable()
    {
        if (GameState.Instance == null) return;
        RefreshView();
        GameState.Instance.RefreshShop.AddListener(RefreshView);
        GameState.Instance.AddItemToShop.AddListener(AddItemOnSale);
    }
    
    public void AddItemOnSale(Item item)
    {
        if (itemsOnSale.Contains(item)) return;
        itemsOnSale.Add(item);
        RefreshView();
    }

    public void RemoveItemFromSale(Item item)
    {
        itemsOnSale.Remove(item);
        RefreshView();
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
        foreach(Item item in itemsOnSale)
        {
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
        Dictionary<Item, int> items = GameState.Instance.GetItems();
        foreach (var kvp in items)
        {
            Item item = kvp.Key;
            int amount = kvp.Value;

            if (amount <= 0) continue;

            GameObject obj = Instantiate(sellingItemView, sellingLayout.transform);

            Image icon = obj.transform.Find("ItemImage").GetComponent<Image>();
            TMP_Text nameText = obj.transform.Find("ItemText").GetComponent<TMP_Text>();

            icon.sprite = item.icon;
            nameText.text = $"{item.itemName}: {amount} ({item.valueSelling}$)";
            Button button = obj.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => GameState.Instance.SellItem(item, 1));
        }
    }

}
