using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Wendigo : MonoBehaviour, IDamagable, IZombieSpawner
{
    private NavMeshAgent Agent;
    private Player Player;

    public Animator animator;
    public ZombieAnimEvents zombieAnimEvents;

    public ParticleSystem bloodPfx;
    
    public List<GameObject> patrolPath = new ();
    public int patrolIndex = 0;
    
    public GameObject ZombiePrefab;

    public Image HpBar;
    
    // Combat
    private int maxHitPoints = 300;
    
    private int hitPoints = 300;
    private int damage = 10;
    private float attackRange = 1.0f;
    private bool IsDead = false;
    
    int zombieCount;
    private List<Zombie> zombies = new List<Zombie>();
    
    public void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Player = FindAnyObjectByType<Player>();
        zombieAnimEvents.AnimDealDamage.AddListener(DealAttackDamage);
        
        StartCoroutine(RemoveMeAfterTesting());
        
        GetComponent<BehaviorGraphAgent>().enabled = false;
        animator.SetFloat("WalkAnimMult", 2.0f);
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
        while (!IsDead && Agent != null)
        {
            Agent.SetDestination(patrolPath[patrolIndex].transform.position);
            while (!IsDead && Agent != null && Agent.pathPending)
            {
                yield return new WaitForSeconds(0.3f);
            }
            while (!IsDead && Agent!= null && Agent.enabled && (Agent.pathStatus != NavMeshPathStatus.PathComplete || Agent.remainingDistance > 1))
            {
                Debug.Log($"Point: {patrolPath[patrolIndex].name} Distance: {Agent.remainingDistance} Status {Agent.pathStatus} IsDead {IsDead}");
                yield return new WaitForSeconds(0.3f);
            }

            SpawnZobie();

            patrolIndex += 1;
            patrolIndex %= patrolPath.Count;
            Debug.Log($"Next point: {patrolPath[patrolIndex].name}");
            yield return new WaitForSeconds(0.3f);
        }

        KillZombies();
        StartDay();
    }

    private void KillZombies()
    {
        foreach (var z in zombies)
        {
            if (z != null)
            {
                z.KillYourselfFromDaylight();
            }
        }
    }

    private void StartDay()
    {
        GameState.Instance.DelayedStartDay();
    }

    public void SpawnZobie()
    {
        if (zombieCount > 10)
            return;

        var zGo = Instantiate(ZombiePrefab, transform.position, Quaternion.identity);
        var z = zGo.GetComponent<Zombie>();
        zombies.Add(z);
        z.Spawner = this;
        ++zombieCount;
    }

    public void Update()
    {
        
        var barWidth = 200.0f * hitPoints/ maxHitPoints;
        HpBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barWidth);
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

    public void OnZombieDied(Zombie z)
    {
        --zombieCount;
    }
}
