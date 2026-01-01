using Assets.Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    Arrow,
    SpellProjectile,
    Ice
}

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance;

    [SerializeField] private ProjectileBase arrow;
    [SerializeField] private ProjectileBase spellProjectile;
    //[SerializeField] private ProjectileBase ice;

    private Dictionary<ProjectileType, ProjectileBase> map;

    private void Awake()
    {
        Instance = this;
        map = new Dictionary<ProjectileType, ProjectileBase>
        {
            { ProjectileType.Arrow, arrow },
            { ProjectileType.SpellProjectile, spellProjectile },
            //{ ProjectileType.Ice, ice }
        };

        foreach (var p in map.Values)
            p.gameObject.SetActive(false);
    }

    public void Fire(
     ProjectileType type,
     Transform firePoint,
     Unit target,
     Action onHit)
    {
        map[type].Fire(firePoint, target, onHit);
    }


}

