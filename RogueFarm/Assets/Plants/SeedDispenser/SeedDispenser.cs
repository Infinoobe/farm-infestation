using System;
using UnityEngine;

public class SeedDispenser : MonoBehaviour
{
    public Player player;
    public Plant plantToUse;
    public Renderer dispenserRenderer;
    public string defaultHexColor = "EAFF66";
    public string selectedHexColor = "0000FF";
    
    private Color defaultColor;
    private Color selectedColor;

    // Tries to use HexColor, otherwise chooses more default ones
    private void Awake()
    {
        if (!ColorUtility.TryParseHtmlString(selectedHexColor, out selectedColor))
            selectedColor = Color.blue;

        if (!ColorUtility.TryParseHtmlString(defaultHexColor, out defaultColor))
            defaultColor = Color.white;
    }

    private void OnEnable()
    {
        player.OnPlantChanged.AddListener(UpdateColor);
    }

    private void OnDisable()
    {
        player.OnPlantChanged.RemoveListener(UpdateColor);
    }

    private void UpdateColor(Plant selectedPlant)
    {
        if (selectedPlant == plantToUse)
            dispenserRenderer.material.color = selectedColor;
        else
            dispenserRenderer.material.color = defaultColor;
    }
}
