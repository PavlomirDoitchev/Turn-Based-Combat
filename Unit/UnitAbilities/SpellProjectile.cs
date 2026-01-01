using UnityEngine;

public class SpellProjectile : ProjectileBase
{
    private Vector3 moveDirection;
    private float speed = 7f;
    private const float HIT_DISTANCE = 0.2f;
    protected override void OnFire()
    {
        Vector3 targetPoint = GetTargetPoint();

        moveDirection =
            (targetPoint - transform.position).normalized;

        transform.forward = moveDirection;
    }

    private void Update()
    {
        if (target == null)
            return;

        transform.position += moveDirection * speed * Time.deltaTime;

        float distance =
            Vector3.Distance(transform.position, GetTargetPoint());

        if (distance <= HIT_DISTANCE)
        {
            HitTarget();
        }
    }
}
