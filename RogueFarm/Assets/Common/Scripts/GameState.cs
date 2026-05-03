using System.Collections.Generic;
using Interactable;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class GameState : MonoBehaviour
{
    private GamePhase currGamePhase = GamePhase.None;
    private int currentDay = 0;

    [Header("Items")]
    public ItemsDatabaseSO itemsDatabase;
    [SerializeField] private List<ItemAmount> startingItems;
    [SerializeField] private Inventory inventory;
    [SerializeField] public List<ItemSO> ItemsInShop = new ();
    public float zombieLootChance = 0.3f;

    [Header("Events")]
    [SerializeField] public UnityEvent OnDayStarted = new ();
    [SerializeField] public UnityEvent OnNightStarted = new ();
    [SerializeField] public UnityEvent RefreshShop = new ();
    [SerializeField] public UnityEvent RefreshBackpack = new ();


    [Header("Interactables")]
    public List<IInteractable> Interactables = new ();

    public static GameState Instance { get; private set; }

    public GamePhase CurrentPhase => currGamePhase;
    public bool IsDay() { return currGamePhase == GamePhase.Day; }
    public bool IsNight() { return currGamePhase == GamePhase.Night; }
    public int CurrentDay => currentDay;
    public bool GodCheat { get; set; }
    public bool DamageCheat { get; set; }

    public Inventory Inventory => inventory;
    public Player Player;
    public int ZombiesToKill;

    public int GetMoney()
    {
        return inventory.GetItems().GetAmount(itemsDatabase.moneyItemSo);
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

    void OnSelectedItemChanged()
    {
        RefreshBackpack.Invoke();
    }

    void Start()
    {
        StartDay();
        inventory.CreateInventory(startingItems);
    }
    
    public void SetPlayer(Player player)
    {
        Player = player;
        Player.OnSelectedItemChanged.AddListener(OnSelectedItemChanged);
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
        Player.Heal(SleepHealValue());
        currGamePhase = GamePhase.Night;
        OnNightStarted.Invoke();
    }

    private int SleepHealValue()
    {
        int healValue = 0;
        Dictionary<ItemSO, int> items = inventory.GetItems().GetItemsOfTypeDict(ItemType.UPGRADE);
        foreach (var kvp in items)
        {
            ItemSO itemSo = kvp.Key;
            int amount = kvp.Value;

            healValue += itemSo.regenerationBonus * amount;
        }
        return healValue;
    }

    public int DamageBonusValue()
    {
        int damageValue = 0;
        Dictionary<ItemSO, int> items = inventory.GetItems().GetItemsOfTypeDict(ItemType.UPGRADE);
        foreach (var kvp in items)
        {
            ItemSO itemSo = kvp.Key;
            int amount = kvp.Value;

            damageValue += itemSo.damageBonus * amount;
        }
        return damageValue;
    }

    public int MaxHealthBonusValue()
    {
        int maxHealthBonus = 0;
        Dictionary<ItemSO, int> items = inventory.GetItems().GetItemsOfTypeDict(ItemType.UPGRADE);
        foreach (var kvp in items)
        {
            ItemSO itemSo = kvp.Key;
            int amount = kvp.Value;

            maxHealthBonus += itemSo.maxHealthBonus * amount;
        }
        return maxHealthBonus;
    }

    public bool HasItems(ItemSO itemSo, int amount = 1)
    {
        return inventory.GetItems().GetAmount(itemSo) >= amount;
    }

    public bool RemoveItem(ItemSO itemSo, int amount = 1)
    {
        if (!HasItems(itemSo, amount)) return false;
        inventory.GetItems().RemoveItem(itemSo, amount);

        RefreshBackpack.Invoke();
        return true;
    }

    public bool RemoveItems(ItemSet itemSet)
    {
        if(!inventory.GetItems().HasItems(itemSet)) return false;
        inventory.GetItems().RemoveItems(itemSet);

        RefreshBackpack.Invoke();
        return true;
    }

    public void AddItem(ItemSO itemSo, int amount = 1)
    {
        inventory.GetItems().AddItem(itemSo, amount);
        RefreshBackpack.Invoke();
    }

    public void SellItem(ItemSO itemSo, int amount = 1)
    {
        if(RemoveItem(itemSo, amount))
        {
            AddItem(itemsDatabase.moneyItemSo, amount * itemSo.valueSelling);
            RefreshShop?.Invoke();
            RefreshBackpack.Invoke();
        }
        else
        {
            Debug.Log("No item in backpack");
        }
    }

    public void BuyItem(ItemSO itemSo, int amount = 1)
    {
        var cost = itemSo.valueBuying * amount;
        if (!RemoveItem(itemsDatabase.moneyItemSo, cost))
        {
            Debug.Log("Not enough money");
            return;
        }
        AddItem(itemSo, amount);
        RefreshShop?.Invoke();
        RefreshBackpack.Invoke();
    }

    public void AddItemToShop(ItemSO itemSo)
    {
        ItemsInShop.Add(itemSo);
        RefreshShop.Invoke();
    }

    public void DelayedStartDay()
    {
        Invoke(nameof(StartDay), 0.0f); // don't change while despawning zombie
    }
    
    public int GetZombiesToKill()
    {
        if (!IsNight()) return 0;
        return ZombiesToKill;
    }

    public void SpawnBossLoot(Vector3 position)
    {
        position.y = 0;
        var p = Instantiate(itemsDatabase.pickupPrefab, position, Quaternion.identity);
        p.items.Clear();
        p.items.Add(new ItemAmount(){itemSo = itemsDatabase.seedItemSo});
    }
    
    public void SpawnZombieLoot(Vector3 position)
    {
        if (Random.value >= zombieLootChance)
        {
            return;
        }
        position.y = 0;
        var p = Instantiate(itemsDatabase.pickupPrefab, position, Quaternion.identity);
        p.items.Clear();
        p.items.Add(new ItemAmount(){itemSo = itemsDatabase.moneyItemSo});
    }
}

public enum GamePhase
{
    Day,
    Night,
    None
}
