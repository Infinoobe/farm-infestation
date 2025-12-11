using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
            var mainCamera = Camera.main;
            Vector2 screenPos = mainCamera.WorldToScreenPoint(i.GetPosition());
            var parentRect = interactionBG.rectTransform.parent as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
               parentRect, screenPos, null, out Vector2 localPoint);
            localPoint.y += interactionBG.rectTransform.rect.height / 2;
            interactionBG.rectTransform.anchoredPosition = localPoint;
            
            interactionLabel.text = $"Press E to {i.GetDescription()}";
        }
    }
}
