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
    [SerializeField] ItemsDatabaseSO itemsDatabase;

    [SerializeField] private Inventory inventory;
    [FormerlySerializedAs("moneyItem")] [SerializeField] private ItemSO moneyItemSo;
    [FormerlySerializedAs("handItem")] [SerializeField] private ItemSO handItemSo;
    [SerializeField] public List<ItemSO> ItemsInShop = new List<ItemSO>();


    [Header("Events")]
    [SerializeField] public UnityEvent OnDayStarted = new UnityEvent();
    [SerializeField] public UnityEvent OnNightStarted = new UnityEvent();
    [SerializeField] public UnityEvent RefreshShop = new UnityEvent();
    [SerializeField] public UnityEvent RefreshBackpack = new UnityEvent();


    [Header("Interactables")]
    public List<IInteractable> Interactables = new ();

    public static GameState Instance { get; private set; }

    public ItemSO GetHandItemSo => handItemSo;
    public GamePhase CurrentPhase => currGamePhase;
    public bool IsDay() { return currGamePhase == GamePhase.Day; }
    public bool IsNight() { return currGamePhase == GamePhase.Night; }
    public int CurrentDay => currentDay;
    public Player Player;
    public int ZombiesToKill;

    public Dictionary<ItemSO, int> GetItems()
    {
        return inventory.items;
    }

    public int GetMoney()
    {
        return inventory.GetAmount(moneyItemSo);
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
        AddItem(moneyItemSo, 10);
        AddItem(handItemSo, 1);
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
        Dictionary<ItemSO, int> items = GetItems();
        foreach (var kvp in items)
        {
            ItemSO itemSo = kvp.Key;
            int amount = kvp.Value;

            if (amount <= 0 || itemSo.itemType != ItemType.UPGRADE) continue;

            healValue += itemSo.regenerationBonus * amount;
        }
        return healValue;
    }

    public int DamageBonusValue()
    {
        int damageValue = 0;
        Dictionary<ItemSO, int> items = GetItems();
        foreach (var kvp in items)
        {
            ItemSO itemSo = kvp.Key;
            int amount = kvp.Value;

            if (amount <= 0 || itemSo.itemType != ItemType.UPGRADE) continue;

            damageValue += itemSo.damageBonus * amount;
        }
        return damageValue;
    }

    public int MaxHealthBonusValue()
    {
        int maxHealthBonus = 0;
        Dictionary<ItemSO, int> items = GetItems();
        foreach (var kvp in items)
        {
            ItemSO itemSo = kvp.Key;
            int amount = kvp.Value;

            if (amount <= 0 || itemSo.itemType != ItemType.UPGRADE) continue;

            maxHealthBonus += itemSo.maxHealthBonus * amount;
        }
        return maxHealthBonus;
    }

    public bool HasItems(ItemSO itemSo, int amount = 1)
    {
        return inventory.GetAmount(itemSo) >= amount;
    }

    public bool RemoveItem(ItemSO itemSo, int amount = 1)
    {
        if (!HasItems(itemSo, amount)) return false;

        inventory.RemoveItem(itemSo, amount);
        RefreshBackpack.Invoke();
        return true;
    }

    public bool RemoveItems(Dictionary<ItemSO, int> items)
    {
        foreach(var item in items)
        {
            if (!HasItems(item.Key, item.Value)) return false;
        }

        foreach (var item in items)
        {
            inventory.RemoveItem(item.Key, item.Value);
        }
        RefreshBackpack.Invoke();
        return true;
    }

    public void AddItem(ItemSO itemSo, int amount = 1)
    {
        inventory.AddItem(itemSo, amount);
        RefreshBackpack.Invoke();
    }

    public void SellItem(ItemSO itemSo, int amount = 1)
    {
        if(RemoveItem(itemSo, amount))
        {
            AddItem(moneyItemSo, amount * itemSo.valueSelling);
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
        if (!RemoveItem(moneyItemSo, cost))
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
}

public enum GamePhase
{
    Day,
    Night,
    None
}
