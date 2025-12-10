using System;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour, IDamagable
{
    private NavMeshAgent Agent;
    private Player Player;

    public ZombieSpawner Spawner;
    public Animator animator;
    public ZombieAnimEvents zombieAnimEvents;

    // Combat
    private int hitPoints = 20;
    private int damage = 10;
    private float attackRange = 1.0f;
    
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

    public void DealDamage(int damageDealt)
    {
        animator.Play("Damage");
        hitPoints -= damageDealt;
        if (hitPoints <= 0)
            KillYourself();
    }

    public void DealAttackDamage()
    {
        var enemies = FindObjectsByType<Player>(FindObjectsSortMode.None);
        var attackPosition = transform.position + transform.forward * attackRange;
        //Debug.DrawRay(transform.position, attackPosition-transform.position, Color.red, 0.5f);
        foreach (var enemy in enemies)
        {
            if ((enemy.transform.position - attackPosition).magnitude < attackRange)
            {
                enemy.DealDamage(damage);
            }
        }
    }

    public void KillYourself()
    {
        if (Spawner != null) Spawner.OnZombieDied(this);
        Destroy(gameObject);
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
