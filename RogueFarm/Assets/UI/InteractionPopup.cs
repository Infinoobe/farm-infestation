using Interactable;
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
        if (!p.CanInteract())
        {
            interactionBG.gameObject.SetActive(false);
            return;
        }

        var hasInteraction = p.TryGetInteractable(out var i);
        interactionBG.gameObject.SetActive(hasInteraction);
        if (hasInteraction)
        {
           interactionBG.rectTransform.anchoredPosition = InteractabeToScreenSpace(i);
            
            interactionLabel.text = $"Click to {i.GetDescription()}";
        }
    }

    private Vector2 InteractabeToScreenSpace(IInteractable i)
    {
        var mainCamera = Camera.main;
        Vector2 screenPos = mainCamera.WorldToScreenPoint(i.GetPosition());
        var parentRect = interactionBG.rectTransform.parent as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, screenPos, null, out Vector2 localPoint);
        localPoint.y += interactionBG.rectTransform.rect.height / 2;
        return localPoint;
    }
}
