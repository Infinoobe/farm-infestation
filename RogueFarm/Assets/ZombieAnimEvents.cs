using UnityEngine;
using UnityEngine.Events;

public class ZombieAnimEvents : MonoBehaviour
{
    public UnityEvent AnimDealDamage = new UnityEvent();

    public void TriggerAnimDealDamage()
    {
        AnimDealDamage.Invoke();
        Debug.Log("Zombie Attack");
    }
}
