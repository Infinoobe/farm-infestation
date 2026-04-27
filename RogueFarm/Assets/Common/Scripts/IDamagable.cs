using UnityEngine;

public interface IDamagable
{
    public float AttractionFactor { get; }
    public bool IsVulnerable { get; }

    public bool CanBeTargeted { get; }
    public void TakeDamage(int damage);
    public void KillYourself();
}
