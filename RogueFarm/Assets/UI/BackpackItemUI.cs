using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackpackItemUI : MonoBehaviour,  IPointerEnterHandler
{
    public Item item;
    public void SetItem(Item itemData, int amount)
    {
        item = itemData;
        Image icon = transform.Find("ItemImage").GetComponent<Image>();
        TMP_Text nameText = transform.Find("ItemText").GetComponent<TMP_Text>();

        icon.sprite = item.icon;
        nameText.text = $"{item.itemName}: {amount}";

        if (item.itemType == ItemType.SEED)
        {
            GetComponent<Image>().color = new Color(0,0.3f,0);
            if (item == GameState.Instance.Player.SelectedPlantSeed)
            {
                GetComponent<Image>().color = new Color(0.2f,0.7f,0.2f);
                nameText.color = Color.black;
            }
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log($"Select seed: {item.itemName} -- {item.itemType} -- {GameState.Instance.Player.SelectedPlantSeed.itemName}");

        if (item.itemType == ItemType.SEED)
        {
            if (GameState.Instance.Player.SelectedPlantSeed != item)
            {
                GameState.Instance.Player.SelectSeed(item);
            }
        }
    }
}
