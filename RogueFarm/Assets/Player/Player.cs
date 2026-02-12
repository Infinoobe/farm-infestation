using System.Numerics;
using Interactable;
using UI;
using UnityEngine;
using UnityEngine.Events;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour, IDamagable
{
    private CharacterController Controller;

    public float Speed = 5f;
    public float AttackRange = 1.0f;
    [SerializeField] private float interactionRange = 1f;
    public Plant[] plantPrefabs;
    public Item[] neededSeeds;
    public Animator animator;
    public GameObject sword;
    public PlayerAnimEvents playerAnimEvents;
    
    private int currentPlantIndex = 0;
    public Plant SelectedPlant => plantPrefabs[currentPlantIndex];
    public Item SelectedPlantSeed => neededSeeds[currentPlantIndex];
    public bool IsDead => hitPoints <= 0;

    public float invulnerableTimestamp = 0.0f;

    // Combat
    public int hitPoints = 100;
    public int hitPointsMax = 100;
    private int damage = 10;

    // Events
    public UnityEvent<Plant> OnPlantChanged = new UnityEvent<Plant>();

    void Start()
    {
        Controller = GetComponent<CharacterController>();
        GameState.Instance.Player = this;
        playerAnimEvents.AnimDealDamage.AddListener(DealAttackDamage);
    }


    void Update()
    {
        if (IsDead) return;
        
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Planting")) return;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Sword Attack")) return;
        
        sword.SetActive(GameState.Instance.IsNight());
        MoveAndRotate();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (GameState.Instance.IsDay())
            {
                Interact();
            }
            else
            {
                Attack();
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameState.Instance.GoToSleep();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CyclePlants();
        }
        
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            MainUI.Instance.SwitchBackpack();
        }

        if (Input.GetKeyUp(KeyCode.Tab) && MainUI.Instance.IsBackpackVisible())
        {
            MainUI.Instance.SwitchBackpack();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            MainUI.Instance.SwitchShop();
        }
    }

    public void TakeDamage(int damageDealt)
    {
        if (IsDead) return;
        if (Time.time < invulnerableTimestamp) return;
        
        animator.Play("Damage");
        hitPoints -= damageDealt;
        invulnerableTimestamp = Time.time + 0.1f;
        if (hitPoints <= 0)
            KillYourself();
    }

    public void KillYourself()
    {
        animator.Play("Death");
        Debug.Log("Player Died!");
    }

    private void MoveAndRotate()
    {
        const float FALL_VELOCITY = 500.0f;
        var input =  new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (input.magnitude > 1.0)
            input = input.normalized;
        var velocity = input * Speed;
        Controller.Move(velocity * Time.deltaTime);
        Controller.Move(Vector3.down * (FALL_VELOCITY * Time.deltaTime));
        animator.SetFloat("Speed", velocity.magnitude);

        if (input.magnitude > 0.01)
        {
            transform.LookAt(transform.position + input);
        }
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
    }

    public void DealAttackDamage()
    {
        var enemies = FindObjectsByType<Zombie>(FindObjectsSortMode.None);
        var attackPosition = transform.position + transform.forward * AttackRange;
        //Debug.DrawRay(transform.position, attackPosition-transform.position, Color.red, 0.5f);
        foreach (var enemy in enemies)
        {
            if ((enemy.transform.position - attackPosition).magnitude < AttackRange)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
    
    private void Interact()
    {
        if (TryGetInteractable(out var nearest))
        {
            Debug.Log($"Interact ({nearest.GetDescription()}) with {nearest}");
            nearest.Interact(this);
        }
    }

    public bool TryGetInteractable(out IInteractable nearest)
    {
        var interactionPosition = transform.position + transform.forward * AttackRange;
        interactionPosition.y = 0;
        float minDistance = Mathf.Infinity;
        nearest = null;
        foreach (var i in GameState.Instance.Interactables)
        {
            float dist = Vector3.Distance(interactionPosition, i.GetPosition());
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = i;
            }
        }

        if (minDistance > interactionRange) nearest = null;
 
        return minDistance <= interactionRange;
    }

    // Cycles through Plants added in plantPrefabs list
    private void CyclePlants()
    {
        currentPlantIndex++;

        if (currentPlantIndex >= plantPrefabs.Length)
            currentPlantIndex = 0;

        Debug.Log("Selected plant: " + SelectedPlant.name);
        OnPlantChanged.Invoke(SelectedPlant);
    }
}
