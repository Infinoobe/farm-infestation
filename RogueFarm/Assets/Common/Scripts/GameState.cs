using System;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private GamePhase currGamePhase = GamePhase.None;
    private int currentDay = 0;

    // Events
    public event Action OnDayStarted;
    public event Action OnNightStarted;

    public static GameState Instance { get; private set; }
    public GamePhase CurrentPhase => currGamePhase;
    public bool IsDay() { return currGamePhase == GamePhase.Day; }
    public bool IsNight() { return currGamePhase == GamePhase.Night; }
    public int CurrentDay => currentDay;

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
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartNextPhase();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartNextPhase();
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
        OnDayStarted?.Invoke();
    }

    private void StartNight()
    {
        Debug.Log("Starting night");
        currGamePhase = GamePhase.Night;
        OnNightStarted?.Invoke();
    }
}

public enum GamePhase
{
    Day,
    Night,
    None
}
