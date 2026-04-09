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
    [SerializeField] private GameObject wendigoPrefab;
    [SerializeField] private List<GameObject> wendigoPath;

    private int perNightZombiesSpawned;
    private int perNightZombiesAlive;
    private int zombiesToSpawn;
    
    private int zombieLimit = 10;
    
    [SerializeField] private float zombieTime = 1f;
    private List<Zombie> myZombies = new ();
    private Coroutine spawnCoroutine;

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
            Vector3 pos = transform.position + new Vector3(0, 1, 0);
            var z = Instantiate(wendigoPrefab, pos, Quaternion.identity, transform);
            z.GetComponent<Wendigo>().patrolPath = wendigoPath;
            return;
        }

        zombiesToSpawn = 1 + GameState.Instance.CurrentDay * 3;
        GameState.Instance.ZombiesToKill = zombiesToSpawn;
        spawnCoroutine = StartCoroutine(SpawnZombiesCoroutine());
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
        var prefab = zombiePrefab;
        if (GameState.Instance.CurrentDay > 3 && Random.value < 0.3f)
        {
            prefab = zombieTankPrefab;
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
