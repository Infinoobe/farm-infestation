using TMPro;
using UnityEngine;

public class SelectedPlantUI : MonoBehaviour
{
    public TMP_Text PlantLabel;
    void Update()
    {
        if (! GameState.Instance.IsDay())
        {
            PlantLabel.text = "";
            return;
        }

        var selectedPlantSeed = GameState.Instance.Player.SelectedPlantSeed;
        var items = GameState.Instance.GetItems();
        items.TryGetValue(selectedPlantSeed, out var count);
        PlantLabel.text = $"Selected Plant: {selectedPlantSeed.itemName} ({count})";
        PlantLabel.color = count > 0 ? Color.white : Color.red;
    }
}
