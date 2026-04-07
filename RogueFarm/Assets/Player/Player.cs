using System;
using System.Linq;
using IngameDebugConsole;
using Interactable;
using JetBrains.Annotations;
using UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour, IDamagable
{
    private CharacterController Controller;

    public float Speed = 5f;
    public float AttackRange = 1.0f;
    [SerializeField] private float interactionRange = 1f;
    public Animator animator;
    public GameObject sword;
    public PlayerAnimEvents playerAnimEvents;
    [SerializeField] private float rayLength = 3f;
    [SerializeField] private Transform rayStartingPoint;
    private GridSystemRuntime currGridSystem;

    [FormerlySerializedAs("SelectedItem")] [CanBeNull] public ItemSO selectedItemSo;
    public UnityEvent OnSelectedItemChanged = new UnityEvent();

    public bool IsDead => hitPoints <= 0;

    public float invulnerableTimestamp = 0.0f;

    // Combat
    public int hitPoints = 100;
    public int hitPointsMax = 100;
    private int damage = 10;

    private Vector3 lastInput;
    private Vector3 velocity;


    void Start()
    {
        Controller = GetComponent<CharacterController>();
        GameState.Instance.SetPlayer(this);
        playerAnimEvents.AnimDealDamage.AddListener(DealAttackDamage);
    }

    private void UpdateGridSelection()
    {
        Vector3 startPos = rayStartingPoint.position;
        Vector3 direction = Vector3.down;
        int layerMask = LayerMask.GetMask("GridGround");

        bool itemNeedsGrid = DoesItemNeedGrid();

        if (itemNeedsGrid && Physics.Raycast(startPos, direction, out RaycastHit ground, rayLength, layerMask))
        {
            GridCell target = ground.collider.gameObject.GetComponentInParent<GridCell>();
            GridSystemRuntime grid = target.myGrid;
            grid.PointingAtPosition(ground.point, target, selectedItemSo);
            if (currGridSystem && currGridSystem != grid) currGridSystem.DeleteGizmo();
            currGridSystem = grid;
        }
        else if (currGridSystem)
        {
            currGridSystem.DeleteGizmo();
            currGridSystem = null;
        }

        if (!currGridSystem) return;

        if (Input.GetKeyDown(KeyCode.R) || Input.mouseScrollDelta.y > 0 )
        {
            currGridSystem.RotateGizmo();
        } else if (Input.mouseScrollDelta.y < 0)
        {
            currGridSystem.RotateGizmo(true);

        }
    }

    public bool DoesItemNeedGrid()
    {
        if (selectedItemSo == null) return false;
        if (selectedItemSo.itemName.Equals("Hoe")) return true;
        if (selectedItemSo.itemType == ItemType.BUILDING) return true;
        return false;
    }

    void Update()
    {
        if (IsDead) return;
        
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Planting")) return;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Sword Attack")) return;

        UpdateGridSelection();        

        sword.SetActive(GameState.Instance.IsNight());

        UpdateInput();
        MoveAndRotate();
    }

    private void UpdateInput()
    {
        if (DebugLogManager.Instance.IsLogWindowVisible)
        {
            velocity = Vector3.zero;
            return;
        }

        lastInput =  new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (lastInput.magnitude > 1.0)
            lastInput = lastInput.normalized;
        velocity = lastInput * Speed;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown((0)))
        {
            Interact();
        }
        
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetMouseButtonDown(1))
        {
            MainUI.Instance.ShowBackpack();
        }
        
        if (Input.GetKeyUp(KeyCode.Tab)|| Input.GetMouseButtonUp(1))
        {
            MainUI.Instance.HideBackpack();
        }
        
        // debug:

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CyclePlants();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameState.Instance.GoToSleep();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            MainUI.Instance.OpenShop();
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            MainUI.Instance.OpenResearch();
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
    
    public void Heal(int damageHealed)
    {
        hitPoints += damageHealed;
        hitPoints = math.min(hitPoints, hitPointsMax);
    }

    public void KillYourself()
    {
        animator.Play("Death");
        Debug.Log("Player Died!");
    }

    private void MoveAndRotate()
    {
        if (lastInput.magnitude > 0.01)
        {
            transform.LookAt(transform.position + lastInput);
        }
        
        const float FALL_VELOCITY = 500.0f;
        Controller.Move(velocity * Time.deltaTime);
        Controller.Move(Vector3.down * (FALL_VELOCITY * Time.deltaTime));
        animator.SetFloat("Speed", velocity.magnitude);

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
        var boss = FindObjectsByType<Wendigo>(FindObjectsSortMode.None);
        //Debug.DrawRay(transform.position, attackPosition-transform.position, Color.red, 0.5f);
        foreach (var enemy in boss)
        {
            if ((enemy.transform.position - attackPosition).magnitude < AttackRange)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    public bool CanInteract()
    {
        return currGridSystem == null || !currGridSystem.HasGizmo();
    }
    

    private void Interact()
    {
        if (GameState.Instance.IsNight())
        {
            Attack();
            return;
        }

        if(currGridSystem && currGridSystem.HasGizmo())
        {
            currGridSystem.PlaceBuilding(selectedItemSo);
        }
        else if (TryGetInteractable(out var nearest))
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
            if (!i.IsInteractionEnabled()) continue;
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


    private bool HasNoSeeds()
    {
        if (selectedItemSo == null) return true;
        if (selectedItemSo.itemType != ItemType.SEED) return true;
        return ! GameState.Instance.HasItems(selectedItemSo);
    }

    // Cycles through Plants added in plantPrefabs list
    private void CyclePlants()
    {
        var availableSeeds = GameState.Instance.GetItems()
            .Where(i => i.Key.itemType == ItemType.SEED && i.Value > 0).ToList();
        if (availableSeeds.Count == 0)
        {
            return;
        }

        var selectedIdx = availableSeeds.FindIndex(x => x.Key == selectedItemSo);
        selectedIdx += 1;
        selectedIdx %= availableSeeds.Count;

        selectedItemSo = availableSeeds[selectedIdx].Key;
        OnSelectedItemChanged.Invoke();
    }

    public void SelectItem(ItemSO itemSo)
    {
        selectedItemSo = itemSo;
        OnSelectedItemChanged.Invoke();
    }

}
