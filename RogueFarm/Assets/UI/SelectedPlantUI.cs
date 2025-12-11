using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedPlantUI : MonoBehaviour
{
    public TMP_Text PlantLabel;
    public Image PlantImage;
    void Update()
    {
        if (! GameState.Instance.IsDay())
        {
            PlantLabel.text = "";
            PlantImage.color = Color.clear;
            return;
        }

        var selectedPlantSeed = GameState.Instance.Player.SelectedPlantSeed;
        var items = GameState.Instance.GetItems();
        items.TryGetValue(selectedPlantSeed, out var count);
        PlantLabel.text = $"Selected Plant: {selectedPlantSeed.itemName} ({count})";
        PlantLabel.color = count > 0 ? Color.white : Color.red;
        
        PlantImage.sprite = selectedPlantSeed.icon;
        PlantImage.color = count > 0 ? Color.white : Color.red;
    }
}
