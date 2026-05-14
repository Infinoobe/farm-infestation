using UnityEngine;

public class TurretBullet : MonoBehaviour
{

    private Zombie target;
    private Vector3 initialPosition;
    private float timeToHit = 0.3f;
    private float timeFlying = 0.0f;
    private int damage = 10;

    public void SpawnAndSetTarget(Zombie zombie, int dmg)
    {
        target = zombie;
        initialPosition = transform.position;
        damage = dmg;
    }

    void Update()
    {
        timeFlying += Time.deltaTime;
        if (timeFlying >= timeToHit)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.position = Vector3.Lerp(initialPosition, target.transform.position, timeFlying / timeToHit);
        transform.LookAt(target.transform);
    }
}
