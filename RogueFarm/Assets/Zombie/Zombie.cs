using System;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour, IDamagable
{
    private NavMeshAgent Agent;
    private Player Player;

    public ZombieSpawner Spawner;
    public Animator animator;

    // Combat
    private int hitPoints = 20;
    private int damage = 10;
    
    public void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Player = FindAnyObjectByType<Player>();
    }

    public void Update()
    {
        // Agent.destination = Player.transform.position;
    }

    public void DealDamage(int damageDealt)
    {
        GetComponent<Animator>().Play("Damage");
        hitPoints -= damageDealt;
        if (hitPoints <= 0)
            KillYourself();
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
