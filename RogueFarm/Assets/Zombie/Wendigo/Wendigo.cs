using System;
using System.Collections;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class Wendigo : MonoBehaviour, IDamagable
{
    private NavMeshAgent Agent;
    private Player Player;

    public Animator animator;
    public ZombieAnimEvents zombieAnimEvents;

    public ParticleSystem bloodPfx;
    
    // Combat
    private int hitPoints = 300;
    private int damage = 10;
    private float attackRange = 1.0f;
    private bool IsDead = false;
    
    public void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Player = FindAnyObjectByType<Player>();
        zombieAnimEvents.AnimDealDamage.AddListener(DealAttackDamage);
        
        StartCoroutine(RemoveMeAfterTesting());

    }
    
    private IEnumerator RemoveMeAfterTesting()
    {
        // TODO: Remove after testing
        yield return new WaitForSeconds(0.3f);
        Debug.LogWarning("Wendigo force night");
        if (GameState.Instance.IsDay())
        {
            GameState.Instance.GoToSleep();
        }
    }

    public void Update()
    {
        // Agent.destination = Player.transform.position;
    }

    public void TakeDamage(int damageDealt)
    {
        if (bloodPfx != null)
        {
            bloodPfx.Play();
        }
        hitPoints -= damageDealt;
        if (hitPoints <= 0)
            KillYourself();
    }

    public void DealAttackDamage()
    {
        if (IsDead) return;
        var enemies = FindObjectsByType<Player>(FindObjectsSortMode.None);
        var attackPosition = transform.position + transform.forward * attackRange;
        //Debug.DrawRay(transform.position, attackPosition-transform.position, Color.red, 0.5f);
        foreach (var enemy in enemies)
        {
            if ((enemy.transform.position - attackPosition).magnitude < attackRange)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    public void KillYourself()
    {
        if (IsDead) return;
        IsDead = true;
        GetComponent<Collider>().enabled = false;
        GetComponent<BehaviorGraphAgent>().enabled = false;
        Agent.enabled = false;

        animator.Play("Death");
    }

    public int getDamage()
    {
        return damage;
    }
}
