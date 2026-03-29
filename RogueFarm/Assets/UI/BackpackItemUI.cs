using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BackpackItemUI : MonoBehaviour,  IPointerEnterHandler
{
    [FormerlySerializedAs("item")] public ItemSO itemSo;
    public void SetItem(ItemSO itemSoData, int amount)
    {
        itemSo = itemSoData;
        Image icon = transform.Find("ItemImage").GetComponent<Image>();
        TMP_Text nameText = transform.Find("ItemText").GetComponent<TMP_Text>();

        icon.sprite = itemSo.icon;
        nameText.text = itemSo.itemName;
        if (!itemSo.unique)
        {
            nameText.text += $": {amount}";
        }

        var img = GetComponent<Image>();
        if (itemSo.itemType == ItemType.SEED)
        {
            img.color = new Color(0,0.3f,0);
            if (itemSo == GameState.Instance.Player.selectedItemSo)
            {
                img.color = new Color(0.2f,0.7f,0.2f);
                nameText.color = Color.black;
            }
        }
        if (itemSo.itemType == ItemType.TOOL || itemSo.itemType == ItemType.BUILDING)
        {
            img.color = new Color(0.3f,0.3f,0.3f);
            if (itemSo == GameState.Instance.Player.selectedItemSo)
            {
                img.color = new Color(0.7f,0.7f,0.7f);
                nameText.color = Color.black;
            }
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log($"Select seed: {item.itemName} -- {item.itemType} -- {GameState.Instance.Player.SelectedPlantSeed.itemName}");

        if (itemSo.itemType == ItemType.SEED || itemSo.itemType == ItemType.TOOL || itemSo.itemType == ItemType.BUILDING)
        {
            if (GameState.Instance.Player.selectedItemSo != itemSo)
            {
                GameState.Instance.Player.SelectItem(itemSo);
            }
        }
    }
}
