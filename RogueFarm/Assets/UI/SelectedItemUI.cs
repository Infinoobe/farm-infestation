using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SelectedPlantUI : MonoBehaviour
{
    [FormerlySerializedAs("PlantLabel")] public TMP_Text ItemLabel;
    [FormerlySerializedAs("PlantImage")] public Image ItemImage;
    void Update()
    {
        if (! GameState.Instance.IsDay())
        {
            ItemLabel.text = "";
            ItemImage.color = Color.clear;
            return;
        }

        var item = GameState.Instance.Player.selectedItemSo;

        if (item == null || item == GameState.Instance.GetHandItemSo)
        {
            ItemLabel.text = $"No item selected";
            ItemImage.color = Color.clear;
            return;
        }
        

        var items = GameState.Instance.GetItems();
        items.TryGetValue(item, out var count);
        ItemLabel.text = item.itemName;
        if (!item.unique)
        {
            ItemLabel.text += $" ({count})";
        }
        ItemLabel.color = count > 0 ? Color.white : Color.red;
        
        ItemImage.sprite = item.icon;
        ItemImage.color = count > 0 ? Color.white : Color.red;
    }
}
