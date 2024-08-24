using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using MoreMountains.TopDownEngine;

/*
    TopDownEngine有些不合时宜的逻辑，故此处继承编写之。
    旨在串联【Minos_DamageOnTouch】。
    可以被外界定制【Damage、HitRate】
*/

public class Minos_ProjectileWeapon : ProjectileWeapon
{
    [Header("Damage Caused")]
    public int DamageCaused = 10;

    [Header("Hit Rate")]
    public float HitRate = 100.0f;





    public override GameObject SpawnProjectile(Vector3 spawnPosition, int projectileIndex, int totalProjectiles, bool triggerObjectActivation = true)
    {
        GameObject nextGameObject = base.SpawnProjectile(spawnPosition, projectileIndex, totalProjectiles, triggerObjectActivation);

        if (nextGameObject != null)
        {
            Projectile projectile = nextGameObject.GetComponent<Projectile>();
            if (projectile != null)
            {
                //强行更改Damage
                Minos_DamageOnTouch _damageOnTouch = projectile.GetComponent<Minos_DamageOnTouch>();
                GameCommon.CHECK(_damageOnTouch != null);
                _damageOnTouch.DamageCaused = DamageCaused;
                _damageOnTouch.HitRate = HitRate;
                _damageOnTouch.DgOnDamageMissing = OnDamageMissing;
            }
        }

        return nextGameObject;
    }

    void OnDamageMissing()
    {
        Debug.LogWarning("Missing !!!!");
    }
}
