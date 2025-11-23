using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private Zombie zombieType;
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
        spawnCoroutine = StartCoroutine(SpawnZombiesCoroutine());
    }

    private IEnumerator SpawnZombiesCoroutine()
    {
        while (GameState.Instance.CanSpawnZombieThisNight())
        {
            if (GameState.Instance.CanSpawnZombieNow())
            {
                SpawnZombie();
                GameState.Instance.ZombieSpawned();
            }
            yield return new WaitForSeconds(zombieTime);
        }
    }

    private void SpawnZombie()
    {
        Vector3 pos = transform.position + new Vector3(0, 1, 0);
        var z = Instantiate(zombieType, pos, Quaternion.identity, transform);
        z.Spawner = this;
        myZombies.Add(z);
    }

    public void OnZombieDied(Zombie z)
    {
        myZombies.Remove(z);
        GameState.Instance.ZombieDied();
    }
}
