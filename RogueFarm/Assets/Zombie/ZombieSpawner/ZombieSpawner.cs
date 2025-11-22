using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.OpenVR;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject zombieType;
    [SerializeField] private int zombieCount = 3;
    [SerializeField] private float zombieTime = 1f;
    private List<GameObject> myZombies = new List<GameObject>();
    private Coroutine spawnCoroutine;

    void Start()
    {
        GameState.Instance.OnNightStarted += HandleNightStarted;
        GameState.Instance.OnDayStarted += HandleDayStarted;
    }

    private void HandleDayStarted()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        foreach (var zombie in myZombies)
        {
            if (zombie != null)
                Destroy(zombie);
        }

        myZombies.Clear();
    }

    private void HandleNightStarted()
    {
        spawnCoroutine = StartCoroutine(SpawnZombiesCoroutine());
    }

    private IEnumerator SpawnZombiesCoroutine()
    {
        for (int i = 0; i < zombieCount; i++)
        {
            SpawnZombie();
            yield return new WaitForSeconds(zombieTime);
        }
    }

    private void SpawnZombie()
    {
        Vector3 pos = transform.position + new Vector3(0, 1, 0);
        myZombies.Add(Instantiate(zombieType, pos, Quaternion.identity, transform));
    }
}
