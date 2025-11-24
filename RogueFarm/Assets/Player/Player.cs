using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    private CharacterController Controller;
    public float Speed = 5f;
    public float AttackRange = 2.0f;
    [SerializeField] private float plantingRange = 3f;
    public Plant[] plantPrefabs;
    const int DAMAGE = 1;
    public Animator animator;
    public GameObject sword;
    
    private int currentPlantIndex = 0;
    public Plant SelectedPlant => plantPrefabs[currentPlantIndex];

    // Events
    public UnityEvent<Plant> OnPlantChanged = new UnityEvent<Plant>();

    void Start()
    {
        Controller = GetComponent<CharacterController>();
    }
     
    void Update()
    {
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
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameState.Instance.GoToSleep();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CyclePlants();
        }
    }

    private void MoveAndRotate()
    {
        var input =  new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
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
        var enemies = FindObjectsByType<Zombie>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            if ((enemy.transform.position - transform.position).magnitude < AttackRange)
            {
                enemy.DealDamage(DAMAGE);
            }
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
            animator.SetTrigger("Plant");
            nearest.PlantSeed(plantPrefabs[currentPlantIndex]);
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
