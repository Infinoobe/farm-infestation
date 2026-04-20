using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IZombieSpawner
{
    public void OnZombieDied(Zombie z);
}

public class ZombieSpawner : MonoBehaviour, IZombieSpawner
{
    [SerializeField] private Zombie zombiePrefab;
    [SerializeField] private Zombie zombieTankPrefab;
    [SerializeField] private Zombie zombieRangedPrefab;
    [SerializeField] private GameObject wendigoPrefab;
    [SerializeField] private List<GameObject> wendigoPath;

    private int perNightZombiesSpawned;
    private int perNightZombiesAlive;
    private int zombiesToSpawn;
    
    [SerializeField] private List<Zombie> stage1 = new ();
    [SerializeField] private List<Zombie> stage2 = new ();
    [SerializeField] private List<Zombie> stage3 = new ();
    [SerializeField] private List<Zombie> stage4 = new ();

    private int zombieLimit = 10;
    
    [SerializeField] private float zombieTime = 1f;
    private List<Zombie> myZombies = new ();
    private Coroutine spawnCoroutine;
    private Wendigo bossAlive;

    void Start()
    {
        GameState.Instance.OnNightStarted.AddListener(HandleNightStarted);
        GameState.Instance.OnDayStarted.AddListener(HandleDayStarted);
    }

    private void HandleDayStarted()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        var zombiesCpy = new List<Zombie>(myZombies);
        foreach (var zombie in zombiesCpy)
        {
            if (zombie != null)
                zombie.KillYourselfFromDaylight();
        }

        myZombies.Clear();
    }

    private void HandleNightStarted()
    {
        perNightZombiesSpawned = 0;
        perNightZombiesAlive = 0;
        var day = GameState.Instance.CurrentDay;
        if (day == 5)
        {
            SpawnWendigo();
            return;
        }

        zombiesToSpawn = GameState.Instance.CurrentDay * 4;
        GameState.Instance.ZombiesToKill = zombiesToSpawn;
        spawnCoroutine = StartCoroutine(SpawnZombiesCoroutine());
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
            yield return new WaitForSeconds(zombieTime);
        }
    }

    private void SpawnZombie()
    {
        // var prefab = zombiePrefab;
        // if (GameState.Instance.CurrentDay > 3 && Random.value < 0.3f)
        // {
        //     prefab = zombieTankPrefab;
        // }
        var prefab = new Zombie();
        var currentDay = GameState.Instance.CurrentDay;
        if (currentDay == 1)
            prefab = stage1[perNightZombiesSpawned];
        else if (currentDay == 2)
            prefab = stage2[perNightZombiesSpawned];
        else if (currentDay == 3)
            prefab = stage3[perNightZombiesSpawned];
        else if (currentDay == 4)
            prefab = stage3[perNightZombiesSpawned];
        else
        {
            prefab = zombiePrefab;
            if (Random.value < 0.3f)
                prefab = zombieTankPrefab;
            else if (Random.value < 0.4f)
                prefab = zombieRangedPrefab;
        }

        Vector3 pos = transform.position + new Vector3(0, 1, 0);
        var z = Instantiate(prefab, pos, Quaternion.identity, transform);
        z.Spawner = this;
        myZombies.Add(z);
    }

    public void OnZombieDied(Zombie z)
    {
        myZombies.Remove(z);
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
