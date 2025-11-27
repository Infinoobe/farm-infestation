using System;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour, IDamagable
{
    private NavMeshAgent Agent;
    private Player Player;

    public ZombieSpawner Spawner;

    // Combat
    private int hitPoints = 2;
    
    public void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Player = FindAnyObjectByType<Player>();
    }

    public void Update()
    {
        // Agent.destination = Player.transform.position;
    }

    public void DealDamage(int damage)
    {
        GetComponent<Animator>().Play("Damage");
        hitPoints -= damage;
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
}
