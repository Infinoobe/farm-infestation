using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackpackItemUI : MonoBehaviour
{

    public void SetItem(Item item, int amount)
    {
        Image icon = transform.Find("ItemImage").GetComponent<Image>();
        TMP_Text nameText = transform.Find("ItemText").GetComponent<TMP_Text>();

        icon.sprite = item.icon;
        nameText.text = $"{item.itemName}: {amount}";
    }
}
