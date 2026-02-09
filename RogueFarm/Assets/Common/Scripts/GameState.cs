using System.Collections.Generic;
using Interactable;
using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    private GamePhase currGamePhase = GamePhase.None;
    private int currentDay = 0;

    // Backpack
    [SerializeField] Item[] startingItems;
    [SerializeField] private Inventory inventory;
    [SerializeField] public int money = 0;
    [SerializeField] private Item moneyItem;

    // Events
    [SerializeField] public UnityEvent OnDayStarted = new UnityEvent();
    [SerializeField] public UnityEvent OnNightStarted = new UnityEvent();
    [SerializeField] public UnityEvent RefreshShop = new UnityEvent();

    // Zombie settings
    [SerializeField] public int zombiesToSpawn = 5;
    [SerializeField] public int zombieLimit = 3;


    public List<IInteractable> Interactables = new ();
    private int perNightZombiesSpawned;
    private int perNightZombiesAlive;

    public static GameState Instance { get; private set; }
    public GamePhase CurrentPhase => currGamePhase;
    public bool IsDay() { return currGamePhase == GamePhase.Day; }
    public bool IsNight() { return currGamePhase == GamePhase.Night; }
    public int CurrentDay => currentDay;
    public Player Player;

    public Dictionary<Item, int> GetItems()
    {
        return inventory.items;
    }

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
        foreach (var item in startingItems)
        {
            AddItem(item, 10);
        }
    }

    public void RegisterInteractable(IInteractable interactable)
    {
        Interactables.Add(interactable);
    }

    public void UnRegisterInteractable(IInteractable interactable)
    {
        Interactables.Remove(interactable);
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

    public bool PullItem(Item item, int amount = 1)
    {
        if (item == moneyItem)
        {
            if (money == 0
                || money < amount) return false;
            money -= amount;
        }
        else
        {
            if (inventory.GetAmount(item) == 0
              || inventory.GetAmount(item) < amount) return false;

            inventory.RemoveItem(item, amount);
        }

        return true;
    }

    public bool PullItems(Dictionary<Item, int> items)
    {
        foreach(var item in items)
        {
            if (item.Key == moneyItem)
            {
                if (money == 0
                    || money < item.Value) return false;
            }
            else
            {
                if (inventory.GetAmount(item.Key) == 0
                    || inventory.GetAmount(item.Key) < item.Value) return false;
            }
        }

        foreach (var item in items)
        {
            if (item.Key == moneyItem)
            {
                money -= item.Value;
            }
            else
            {
                inventory.RemoveItem(item.Key, item.Value);
            } 
        }
        
        return true;
    }

    public void AddItem(Item item, int amount = 1)
    {
        if (item == moneyItem)
            money += amount;
        else
            inventory.AddItem(item, amount);
    }

    public void SellItem(Item item, int amount = 1)
    {
        if(PullItem(item, amount))
        {
            AddItem(moneyItem, amount*item.valueSelling);
            RefreshShop?.Invoke();
        }
        else
        {
            Debug.Log("No item in backpack");
        }
    }

    public void BuyItem(Item item, int amount = 1)
    {
        if (money >= item.valueBuying*amount)
        {
            money -= item.valueBuying * amount;
            AddItem(item, amount);
            RefreshShop?.Invoke();
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }
}

public enum GamePhase
{
    Day,
    Night,
    None
}
