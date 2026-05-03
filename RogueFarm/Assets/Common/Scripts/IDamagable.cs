using UnityEngine;

public interface IDamagable
{
    public float EnemyAttractionFactor { get; }

    public bool CanBeTargetedByEnemy { get; }
    public void TakeDamage(int damage);
    public void KillYourself();
}
