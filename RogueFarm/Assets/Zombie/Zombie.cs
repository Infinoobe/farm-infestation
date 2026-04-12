using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour, IDamagable
{
    private NavMeshAgent Agent;
    private Player Player;

    public IZombieSpawner Spawner;
    public Animator animator;
    public ZombieAnimEvents zombieAnimEvents;

    public ParticleSystem bloodPfx;
    
    // Combat
    [SerializeField] private int hitPoints = 20;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRange = 1.0f;
    [SerializeField] private bool isRanged = false;

    // Projectile in case if isRanged == true
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float projectileSpeed = 10f;

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

        // Melee
        if (!isRanged)
        {
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
        else // Ranged
        {
            foreach (var enemy in enemies)
            {
                Vector3 direction = (enemy.transform.position - projectileSpawnPoint.position).normalized;
                GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

                Debug.Log(projectileSpawnPoint);
                Debug.DrawRay(projectileSpawnPoint.position, Vector3.up, Color.green, 2f);

                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = direction * projectileSpeed;
                }
                projectile.transform.forward = direction;
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
