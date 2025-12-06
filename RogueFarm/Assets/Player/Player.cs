using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour, IDamagable
{
    private CharacterController Controller;

    public float Speed = 5f;
    public float AttackRange = 1.0f;
    [SerializeField] private float plantingRange = 3f;
    public Plant[] plantPrefabs;
    public Item[] neededSeeds;
    public Animator animator;
    public GameObject sword;
    public PlayerAnimEvents playerAnimEvents;
    
    private int currentPlantIndex = 0;
    public Plant SelectedPlant => plantPrefabs[currentPlantIndex];
    public bool IsDead => hitPoints <= 0;

    // Combat
    public int hitPoints = 100;
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
                PlantOnNearestField();
            }
            else
            {
                Attack();
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && GameState.Instance.IsDay())
        {
            CollectNearestPlant();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameState.Instance.GoToSleep();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CyclePlants();
        }
    }

    public void DealDamage(int damageDealt)
    {
        if (IsDead) return;
        
        animator.Play("Damage");
        hitPoints -= damageDealt;
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
        var input =  new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (input.magnitude > 1.0)
            input = input.normalized;
        var velocity = input * Speed;
        Controller.Move(velocity * Time.deltaTime);
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
                enemy.DealDamage(damage);
            }
        }
    }

    private void CollectNearestPlant()
    {
        Field[] allFields = FindObjectsByType<Field>(FindObjectsSortMode.None);
        Field nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 playerPos = transform.position;

        foreach (Field field in allFields)
        {
            if (!field.CanBeCollected()) continue;
            float dist = Vector3.Distance(playerPos, field.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = field;
            }
        }

        if (nearest != null && minDistance <= plantingRange)
        {
            animator.SetTrigger("Plant");
            nearest.CollectPlant();
        }
        else
        {
            Debug.Log("No available fields to collect!");
        }
    }

    private void PlantOnNearestField()
    {
        Field[] allFields = FindObjectsByType<Field>(FindObjectsSortMode.None);
        Field nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 playerPos = transform.position;

        foreach (Field field in allFields)
        {
            if (!field.IsEmpty()) continue;
            float dist = Vector3.Distance(playerPos, field.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = field;
            }
        }

        if (nearest != null && minDistance <= plantingRange)
        {
            if (GameState.Instance.PullItem(neededSeeds[currentPlantIndex]))
            {
                animator.SetTrigger("Plant");
                nearest.PlantSeed(plantPrefabs[currentPlantIndex]);
            }
            else
            {
                Debug.Log($"No item {neededSeeds[currentPlantIndex].name}.");
            }
        }
        else
        {
            Debug.Log($"No available fields. Nearest is {minDistance}!");
        }
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
