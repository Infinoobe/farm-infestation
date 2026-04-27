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

        string message;
        if (p.TryGetInteractable(out var i) && i.GetDescription(out message))
        {
            interactionLabel.text = $"Click to {message}";
            interactionBG.rectTransform.anchoredPosition = InteractabeToScreenSpace(i);
            interactionBG.gameObject.SetActive(true);

        }
        else
        {
            interactionBG.gameObject.SetActive(false);
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
