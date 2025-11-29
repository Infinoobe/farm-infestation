using System;
using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    private GamePhase currGamePhase = GamePhase.None;
    private int currentDay = 0;

    // Events
    [SerializeField] public UnityEvent OnDayStarted = new UnityEvent();
    [SerializeField] public UnityEvent OnNightStarted = new UnityEvent();

    // Zombie settings
    [SerializeField] public int zombiesToSpawn = 5;
    [SerializeField] public int zombieLimit = 3;

    private int perNightZombiesSpawned;
    private int perNightZombiesAlive;

    public static GameState Instance { get; private set; }
    public GamePhase CurrentPhase => currGamePhase;
    public bool IsDay() { return currGamePhase == GamePhase.Day; }
    public bool IsNight() { return currGamePhase == GamePhase.Night; }
    public int CurrentDay => currentDay;
    public Player Player;

    private void Awake()
    {
        // Kill doppelganger hehe
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("Sir, we killed doppelganger!");
            return;
        }

        Instance = this;
    }

    void Start()
    {
        StartDay();
    }

    public void GoToSleep()
    {
        if (IsDay())
        {
            StartNight();
        }
    }

    private void StartNextPhase()
    {
        if (currGamePhase == GamePhase.Day)
            StartNight();
        else
            StartDay();
    }

    private void StartDay()
    {
        Debug.Log("Starting day");
        currentDay++;
        currGamePhase = GamePhase.Day;
        OnDayStarted.Invoke();
    }

    private void StartNight()
    {
        Debug.Log("Starting night");
        currGamePhase = GamePhase.Night;
        perNightZombiesSpawned = 0;
        perNightZombiesAlive = 0;
        OnNightStarted.Invoke();
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
        Debug.Log($"Zombie Dead! Zombies left: { perNightZombiesAlive}");
        if (perNightZombiesAlive == 0 && !CanSpawnZombieThisNight())
        {
            Invoke(nameof(StartDay), 0.0f); // don't change while despawning zombie
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

    public int GetZombiesToKill()
    {
        if (!IsNight()) return 0;
        return zombiesToSpawn - perNightZombiesSpawned + perNightZombiesAlive;
    }
}

public enum GamePhase
{
    Day,
    Night,
    None
}
