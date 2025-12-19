using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour, IDamagable
{
    private NavMeshAgent Agent;
    private Player Player;

    public ZombieSpawner Spawner;
    public Animator animator;
    public ZombieAnimEvents zombieAnimEvents;

    public ParticleSystem bloodPfx;
    
    // Combat
    private int hitPoints = 20;
    private int damage = 10;
    private float attackRange = 1.0f;
    private bool IsDead = false;
    
    public void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Player = FindAnyObjectByType<Player>();
        zombieAnimEvents.AnimDealDamage.AddListener(DealAttackDamage);
    }

    public void Update()
    {
        // Agent.destination = Player.transform.position;
    }

    public void TakeDamage(int damageDealt)
    {
        bloodPfx.Play();
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

        if (Spawner != null) Spawner.OnZombieDied(this);
        animator.Play("Death");
        
    }

    public void KillYourselfFromDaylight()
    {
        KillYourself();
    }

    public int getDamage()
    {
        return damage;
    }
}
