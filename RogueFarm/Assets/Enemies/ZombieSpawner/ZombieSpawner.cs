using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IZombieSpawner
{
    public void OnZombieDied(Zombie z);
}

[Serializable]
public class DayInfo
{
    public List<Zombie> zombies = new List<Zombie>();
}

public class ZombieSpawner : MonoBehaviour, IZombieSpawner
{
    public int startNightForDebug = 0;

    [SerializeField] private Zombie zombiePrefab;
    [SerializeField] private Zombie zombieTankPrefab;
    [SerializeField] private Zombie zombieRangedPrefab;

    [SerializeField] private int zombieLimit = 10;
    [SerializeField] private float zombieSpawnTime = 1f;
    
    [SerializeField] private List<DayInfo> daysSpawn = new ();
    
    [SerializeField] private GameObject wendigoPrefab;
    [SerializeField] private List<GameObject> wendigoPath;
    
    [SerializeField] private int zombiesPerDayAfterBoss = 4;

    [SerializeField] private float basicProbabilityAfterBoss = 0.7f*0.6f;
    [SerializeField] private float tankProbabilityAfterBoss = 0.3f;
    [SerializeField] private float shooterProbabilityAfterBoss = 0.7f*0.4f;
    private float probabilityTotal;


    private int perNightZombiesSpawned;
    private int perNightZombiesAlive;
    private int zombiesToSpawn;
    private List<Zombie> spawnToday;
    
    private List<Zombie> zombiesAlive = new ();
    private Wendigo bossAlive;

    private Coroutine spawnCoroutine;

    void Start()
    {
        GameState.Instance.OnNightStarted.AddListener(HandleNightStarted);
        GameState.Instance.OnDayStarted.AddListener(HandleDayStarted);
        probabilityTotal = tankProbabilityAfterBoss + shooterProbabilityAfterBoss + basicProbabilityAfterBoss;
    }

    private void HandleDayStarted()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        var zombiesCpy = new List<Zombie>(zombiesAlive);
        foreach (var zombie in zombiesCpy)
        {
            if (zombie != null)
                zombie.KillYourselfFromDaylight();
        }

        zombiesAlive.Clear();
    }

    private void HandleNightStarted()
    {
        perNightZombiesSpawned = 0;
        perNightZombiesAlive = 0;
        var day = GameState.Instance.CurrentDay;
        day -= 1;
        day += startNightForDebug;
        if (day < daysSpawn.Count)
        {
            spawnToday = daysSpawn[day].zombies;
        }
        else if (day == daysSpawn.Count)
        {
            spawnToday = new();
            SpawnWendigo();
            return;
        }
        else
        {
            zombiesToSpawn = day * zombiesPerDayAfterBoss;
            spawnToday = new List<Zombie>();
            for (var i = 0; i < zombiesToSpawn; i++)
            {
                spawnToday.Add(GetRandomZombiePrefab());
            }
        }
        zombiesToSpawn = spawnToday.Count;
        GameState.Instance.ZombiesToKill = zombiesToSpawn;
        spawnCoroutine = StartCoroutine(SpawnZombiesCoroutine());
    }

    private Zombie GetRandomZombiePrefab()
    {
        var r = probabilityTotal * Random.value;
        if (r < basicProbabilityAfterBoss)
        {
            return zombiePrefab;
        }
        r-= basicProbabilityAfterBoss;
        if (r <= tankProbabilityAfterBoss)
        {
            return zombieTankPrefab;
        }

        return zombieRangedPrefab;
    }

    private void SpawnWendigo()
    {
        Vector3 pos = transform.position + new Vector3(0, 1, 0);
        var z = Instantiate(wendigoPrefab, pos, Quaternion.identity, transform);
        bossAlive = z.GetComponent<Wendigo>();
        bossAlive.patrolPath = wendigoPath;
    }

    private IEnumerator SpawnZombiesCoroutine()
    {
        while (CanSpawnZombieThisNight())
        {
            if (CanSpawnZombieNow())
            {
                SpawnZombie();
                ZombieSpawned();
            }
            yield return new WaitForSeconds(zombieSpawnTime);
        }
    }

    private void SpawnZombie()
    {
        var prefab = spawnToday[perNightZombiesSpawned];

        Vector3 pos = transform.position + new Vector3(0, 1, 0);
        var z = Instantiate(prefab, pos, Quaternion.identity, transform);
        z.Spawner = this;
        zombiesAlive.Add(z);
    }

    public void OnZombieDied(Zombie z)
    {
        zombiesAlive.Remove(z);
        ZombieDied();
    }
    
    public void ZombieSpawned()
    {
        ++perNightZombiesSpawned;
        ++perNightZombiesAlive;
        Debug.Log($"Zombie Spawned! Zombies left: {zombiesToSpawn - perNightZombiesSpawned}" );
    }
    
    public void ZombieDied()
    {
        --perNightZombiesAlive;
        GameState.Instance.ZombiesToKill -= 1;
        Debug.Log($"Zombie Dead! Zombies left: { perNightZombiesAlive}");
        if (perNightZombiesAlive == 0 && !CanSpawnZombieThisNight())
        {
            GameState.Instance.DelayedStartDay();
        }
    }
    

    public bool CanSpawnZombieNow()
    {
        return CanSpawnZombieThisNight() && perNightZombiesAlive < zombieLimit;
    }

    public bool CanSpawnZombieThisNight()
    {
        return perNightZombiesSpawned < zombiesToSpawn;
    }
}
