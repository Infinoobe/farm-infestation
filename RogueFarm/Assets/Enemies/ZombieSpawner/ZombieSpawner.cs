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
    
    [SerializeField] private List<DayInfo> daysSpawn = new ();
    
    [SerializeField] private GameObject wendigoPrefab;
    [SerializeField] private List<GameObject> wendigoPath;
    
    [SerializeField] private int zombiesPerDayAfterBoss = 4;

    [SerializeField] private float basicProbabilityAfterBoss = 0.7f*0.6f;
    [SerializeField] private float tankProbabilityAfterBoss = 0.3f;
    [SerializeField] private float shooterProbabilityAfterBoss = 0.7f*0.4f;
    private float probabilityTotal;

    [SerializeField] GridSystemTool grid;
    [SerializeField] List<GridCell> possibleSpawnPoints;
    [SerializeField] private int currWave = 0;
    [SerializeField] private float waveDelay = 10f;
    [SerializeField] private float startNightDelay = 3f;

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
        possibleSpawnPoints = grid.GetZombieSpawnPoints();

        perNightZombiesSpawned = 0;
        perNightZombiesAlive = 0;
        currWave = 0;
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
        spawnCoroutine = StartCoroutine(SpawnZombieWavesCoroutine());
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


    private IEnumerator SpawnZombieWavesCoroutine()
    {
        yield return new WaitForSeconds(startNightDelay);

        while (CanSpawnZombieThisNight())
        {
            SpawnWave();
            yield return new WaitForSeconds(waveDelay);
        }
    }

    private void SpawnZombie(Vector3 pos)
    {
        var prefab = spawnToday[perNightZombiesSpawned];
        
        var z = Instantiate(prefab, pos, Quaternion.identity, transform);
        z.Spawner = this;
        zombiesAlive.Add(z);
    }

    private void SpawnWave()
    {
        int zombieCount;
        if (currWave == 0) zombieCount = zombiesToSpawn % zombieLimit;
        else zombieCount = Math.Min(zombieLimit, zombiesToSpawn - perNightZombiesSpawned);

        List<GridCell> spawnPoints = GetRandomWithoutRepetition(possibleSpawnPoints, zombieCount);
        zombieCount = spawnPoints.Count;
        Debug.Log($"Wave: {currWave}, Zombie Count: {zombieCount}");
        foreach(GridCell spawnPoint in spawnPoints)
        {
            SpawnZombie(spawnPoint.transform.position);
            ZombieSpawned();
        }
        currWave++;
    }

    public void OnZombieDied(Zombie z)
    {
        zombiesAlive.Remove(z);
        ZombieDied();
        GameState.Instance.SpawnZombieLoot(z.transform.position);
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

    public List<GridCell> GetRandomWithoutRepetition(List<GridCell> source, int count)
    {
        if (count > source.Count)
            count = source.Count;

        List<GridCell> temp = new List<GridCell>(source);

        for (int i = temp.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (temp[i], temp[j]) = (temp[j], temp[i]);
        }

        return temp.GetRange(0, count);
    }


    public bool CanStartNextWave()
    {
        return perNightZombiesAlive == 0;
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
