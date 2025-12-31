using Assets.Assets.Scripts;
using System;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    protected Unit target;
    protected Action onHit;

    public void Fire(Transform firePoint, Unit target, Action onHit)
    {
        this.target = target;
        this.onHit = onHit;

        // RESET POSITION FIRST
        transform.position = firePoint.position;

        OnFire(); // projectile decides direction/rotation

        gameObject.SetActive(true);
    }

    protected abstract void OnFire();

    protected Vector3 GetTargetPoint()
    {
        Collider col = target.GetComponent<Collider>();
        if (col != null)
            return col.bounds.center;

        return target.transform.position;
    }

    protected void HitTarget()
    {
        onHit?.Invoke();
        gameObject.SetActive(false);
    }
}
