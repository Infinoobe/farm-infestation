using System;
using System.Collections.Generic;
using System.Linq;
using Interactable.Common;
using UnityEngine;

public class Turret : Building
{
    [Header("Turret")]
    [SerializeField] private GameObject TurretRotatingTop;
    [SerializeField] private GameObject ShooArea;
    [SerializeField] private TurretBullet BulletPrefab;
    
    public List<Zombie> ZombiesInRange = new List<Zombie>();
    public float cooldown = 0.3f;
    public float cooldownLeft = 0.0f;
    public int damage = 5;

    public override ActionType GetDescription(out string message)
    {
        message = "Im a turret.";
        return ActionType.NONE;
    }

    public void Update()
    {
        cooldownLeft -= Time.deltaTime;
        ZombiesInRange.RemoveAll(z => z== null || z.IsDead);
        if (ZombiesInRange.Count > 0)
        {
            TurretRotatingTop.transform.LookAt(ZombiesInRange.First().transform);
            if (cooldownLeft <= 0.0f)
            {
                cooldownLeft = cooldown;
                var b = Instantiate(BulletPrefab, ShooArea.transform.position, Quaternion.identity);
                b.SpawnAndSetTarget(ZombiesInRange.First(), damage);
            }
        }
        else
        {
            TurretRotatingTop.transform.Rotate(Vector3.up, Time.deltaTime * 90f);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent<Zombie>(out var z))
        {
            return;
        }
        ZombiesInRange.Add(z);
    }
    public void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent<Zombie>(out var z))
        {
            return;
        }
        ZombiesInRange.Remove(z);
    }
}
