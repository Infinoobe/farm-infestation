using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController Controller;
    public float Speed = 5f;
    public float AttackRange = 2.0f;
     void Start()
    {
        Controller = GetComponent<CharacterController>();
    }
     
    void Update()
    {
        var input =  new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        var velocity = input * Speed;
        Controller.Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GetComponent<Animator>().Play("Attack");
            var enemies = FindObjectsByType<Zombie>(FindObjectsSortMode.None);
            foreach (var enemy in enemies)
            {
                if ((enemy.transform.position - transform.position).magnitude < AttackRange)
                {
                    enemy.Hit();
                }
            }
        }
    }
}
