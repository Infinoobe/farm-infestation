using System;
using UnityEngine;

public class SeedDispenser : MonoBehaviour
{
    public Player player;
    public Plant plantToUse;
    public Renderer dispenserRenderer;
    public Color defaultHexColor = new Color(0xEA/255.0f,1,0x66/255.0f,0.5f);
    public Color selectedHexColor = new Color(0x00/255.0f,0x00/255.0f,0xFF/255.0f,0.5f);

    private void OnEnable()
    {
        player.OnPlantChanged.AddListener(UpdateColor);
        UpdateColor(player.SelectedPlant);
    }

    private void OnDisable()
    {
        player.OnPlantChanged.RemoveListener(UpdateColor);
    }

    private void UpdateColor(Plant selectedPlant)
    {
        if (selectedPlant == plantToUse)
            dispenserRenderer.material.color = selectedHexColor;
        else
            dispenserRenderer.material.color = defaultHexColor;
    }
}
