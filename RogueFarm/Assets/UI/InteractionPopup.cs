using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPopup : MonoBehaviour
{
    public TMP_Text interactionLabel;
    public Image interactionBG;
    void Update()
    {
        if (!GameState.Instance.IsDay())
        {
            interactionBG.gameObject.SetActive(false);
            return;
        }

        var p = GameState.Instance.Player;
        var hasInteraction = p.TryGetInteractable(out var i);
        interactionBG.gameObject.SetActive(hasInteraction);
        if (hasInteraction)
        {
            interactionLabel.text = $"Press E to {i.GetDescription()}";
        }
    }
}
