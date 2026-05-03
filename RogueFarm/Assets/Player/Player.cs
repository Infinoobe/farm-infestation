using System;
using System.Linq;
using IngameDebugConsole;
using Interactable;
using Interactable.Common;
using JetBrains.Annotations;
using UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour, IDamagable
{
    [Header("Player combat settings")]
    [SerializeField] private float Speed = 5f;
    [SerializeField] private float AttackRange = 1.0f;
    [SerializeField] private int currHealth = 100;
    [SerializeField] private int damage = 10;
    [SerializeField] protected float enemyAttractionFactor = 1f;

    [Header("Player interaction settings")]
    [SerializeField] private float interactionRange = 1f;
    [SerializeField] private float rayLength = 3f;
    [SerializeField] private float maxInteractionRange = 3.0f;

    [Header("Playtime variables")]
    [SerializeField] private int healthMax = 100;
    [SerializeField] private GridSystemRuntime currGridSystem;

    [SerializeField] private Transform rayStartingPoint;
    [SerializeField] private Vector3 lastInput;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float timeSinceAttackPressed = 999.0f;
    [SerializeField] private float invulnerableTimestamp = 0.0f;
    [FormerlySerializedAs("SelectedItem")][CanBeNull] private ItemSO selectedItemSo;

    [Header("Object settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject sword;
    [SerializeField] private PlayerAnimEvents playerAnimEvents;
    [SerializeField] private CharacterController Controller;

    [Header("Events")]
    public UnityEvent OnSelectedItemChanged = new ();


    public bool IsDead => currHealth <= 0;
    public float EnemyAttractionFactor => enemyAttractionFactor;
    public ItemSO SelectedItemSo => selectedItemSo;
    public bool IsVulnerable => Time.time >= invulnerableTimestamp;
    public bool CanBeTargetedByEnemy => true;

    public int CurrHealth => currHealth;
    public int HealthMax => healthMax;

    void Start()
    {
        Controller = GetComponent<CharacterController>();
        GameState.Instance.SetPlayer(this);
        playerAnimEvents.AnimDealDamage.AddListener(DealAttackDamage);
    }

    private void UpdateGridSelection()
    {
        Vector3 startPos = rayStartingPoint.position;
        if (TryGetMousePosition(out var mousePosition))
        {
            var toMouse = mousePosition - transform.position;
            if (toMouse.magnitude <= maxInteractionRange)
            {
                startPos = mousePosition;
            }
            else
            {
                startPos = transform.position + toMouse.normalized * maxInteractionRange;
            }
        }

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
        animator.SetBool("attackActive", timeSinceAttackPressed < 0.2f );
        timeSinceAttackPressed += Time.deltaTime;
        
        if (IsDead) return;
        
        var currentAnimation = animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimation.IsName("Sword Attack") 
            || currentAnimation.IsName("Sword Attack 2")
            || currentAnimation.IsName("Sword Attack 3"))
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown((0)))
            {
                Attack();
            }

            var v = 0.5f * Speed * transform.forward;
            Controller.Move(v * Time.deltaTime);

            return;
        }

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
        if (!IsVulnerable) return;
        if (GameState.Instance.GodCheat) return;
        
        animator.Play("Damage");
        currHealth -= damageDealt;
        invulnerableTimestamp = Time.time + 0.1f;
        if (currHealth <= 0)
            KillYourself();
    }
    
    public void Heal(int damageHealed)
    {
        currHealth += damageHealed;
        currHealth = math.min(currHealth, healthMax + GameState.Instance.MaxHealthBonusValue());
    }

    public void KillYourself()
    {
        currHealth = 0;
        animator.Play("Death");
        Debug.Log("Player Died!");
    }

    public void OnAttackStarted()
    {
        RotateToMouse();
    }

    private void MoveAndRotate()
    {
        RotateToMouse();
        // if (lastInput.magnitude > 0.01)
        // {
        //     transform.LookAt(transform.position + lastInput);
        // }
        
        const float FALL_VELOCITY = 500.0f;
        Controller.Move(velocity * Time.deltaTime);
        Controller.Move(Vector3.down * (FALL_VELOCITY * Time.deltaTime));
        animator.SetFloat("Speed", velocity.magnitude);

    }

    private bool TryGetMousePosition(out Vector3 mousePosition)
    {
        var cam = Camera.main;
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        var ground = new Plane(Vector3.up, Vector3.zero);
        if (ground.Raycast(ray, out var distance))
        {
            mousePosition = ray.GetPoint(distance);
            mousePosition.y = transform.position.y;
            return true;
        }

        mousePosition = transform.position;
        return false;
    }

    private void RotateToMouse()
    {
        if (!TryGetMousePosition(out Vector3 mousePosition))
        {
            return;
        }
        
        Vector3 direction = mousePosition - transform.position;
        direction.y = 0;  // stay in XZ plane
        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direction);;
        }
    }

    private void Attack()
    {
        timeSinceAttackPressed = 0.0f;
    }

    public void DealAttackDamage()
    {
        var enemies = FindObjectsByType<Zombie>(FindObjectsSortMode.None);
        var attackPosition = transform.position + transform.forward * AttackRange;
 
        var position = attackPosition;
        var range = AttackRange;
        var dmg = damage + GameState.Instance.DamageBonusValue();

        var currentAnimation = animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimation.IsName("Sword Attack 3"))
        {
            position = transform.position;
            range = 2.5f * AttackRange;
            dmg = (int)(2.5 * dmg);
        }
        if (GameState.Instance.DamageCheat)
        {
            dmg = 9999;
        }

        //Debug.DrawRay(transform.position, attackPosition-transform.position, Color.red, 0.5f);
        foreach (var enemy in enemies)
        {
            if ((enemy.transform.position - position).magnitude < range)
            {
                enemy.TakeDamage(dmg);
            }
        }
        var boss = FindObjectsByType<Wendigo>(FindObjectsSortMode.None);
        //Debug.DrawRay(transform.position, attackPosition-transform.position, Color.red, 0.5f);
        foreach (var enemy in boss)
        {
            if ((enemy.transform.position - position).magnitude < range)
            {
                enemy.TakeDamage(dmg);
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
        else if (TryGetInteractable(out var nearest) && nearest.IsInteractionEnabled())
        {
            ActionType actionType = nearest.GetDescription(out var message);

            Debug.Log($"Interaction {actionType.ToString()} ({message}) with {nearest}");
            nearest.Interact(this);
        }
    }

    public bool TryGetInteractable(out IInteractable nearest)
    {
        var interactionPosition = transform.position + transform.forward * AttackRange;
        if (TryGetMousePosition(out var mousePosition))
        {
            var toMouse = mousePosition - transform.position;
            if (toMouse.magnitude <= maxInteractionRange)
            {
                interactionPosition = mousePosition;
            }
            else
            {
                interactionPosition = transform.position + toMouse.normalized * maxInteractionRange;
            }
        }
        interactionPosition.y = 0;
        float minDistance = Mathf.Infinity;
        nearest = null;
        foreach (var i in GameState.Instance.Interactables)
        {
            if (i is not MonoBehaviour mb || mb == null) continue;
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
        var availableSeeds = GameState.Instance.Inventory.GetItems().GetItemsOfTypeList(ItemType.SEED);

        if (availableSeeds.Count == 0)
        {
            return;
        }

        var selectedIdx = availableSeeds.FindIndex(x => x.itemSo == selectedItemSo);
        selectedIdx += 1;
        selectedIdx %= availableSeeds.Count;

        selectedItemSo = availableSeeds[selectedIdx].itemSo;
        OnSelectedItemChanged.Invoke();
    }

    public void SelectItem(ItemSO itemSo)
    {
        selectedItemSo = itemSo;
        OnSelectedItemChanged.Invoke();
    }
}
