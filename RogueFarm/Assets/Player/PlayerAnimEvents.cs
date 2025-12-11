using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimEvents : MonoBehaviour
{
    public UnityEvent AnimDealDamage = new UnityEvent();

    public void TriggerAnimDealDamage()
    {
        AnimDealDamage.Invoke();
    }
}
