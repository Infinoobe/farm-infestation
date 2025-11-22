using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController Controller;
    public float Speed = 5f;
    public float AttackRange = 2.0f;
    [SerializeField] private float plantingRange = 3f;
    public Plant plantPrefab; // to delete later?
    const int DAMAGE = 100;

    void Start()
    {
        Controller = GetComponent<CharacterController>();
    }
     
    void Update()
    {
        var input =  new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        var velocity = input * Speed;
        Controller.Move(velocity * Time.deltaTime);
        if (input.magnitude > 0.01)
        {
            transform.LookAt(transform.position + input);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GetComponent<Animator>().Play("Attack");
            var enemies = FindObjectsByType<Zombie>(FindObjectsSortMode.None);
            foreach (var enemy in enemies)
            {
                if ((enemy.transform.position - transform.position).magnitude < AttackRange)
                {
                    enemy.DealDamage(DAMAGE);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && GameState.Instance.IsDay()) // plant action
        {
            PlantOnNearestField();
        }
    }

    public void SetPlantToUse(Plant plant)
    {
        plantPrefab = plant;
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
            nearest.PlantSeed(plantPrefab); // TODO choose plant
        }
        else
        {
            Debug.Log($"No available fields. Nearest is {minDistance}!");
        }
    }
}
