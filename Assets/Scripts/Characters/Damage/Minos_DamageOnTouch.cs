using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using System;
using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;

/*
    旨在增加一个属性：【命中率】，且供外界关联。
    类似于Damage供外界关联。

    配合【Minos_Health】一起使用；当【CurrentHealth = 0之时】，还可以接受一次Damageable。
*/

public class Minos_DamageOnTouch : DamageOnTouch
{
    [Header("Hit Rate")]
    public float HitRate = 100.0f;
    public System.Action DgOnDamageMissing;


    protected override void Colliding(GameObject collider)
    {
        //为了【命中率】，重写【Colliding】
        //base.Colliding(collider);

        if (!this.isActiveAndEnabled)
        {
            return;
        }

        // if the object we're colliding with is part of our ignore list, we do nothing and exit
        if (_ignoredGameObjects.Contains(collider))
        {
            return;
        }

        // if what we're colliding with isn't part of the target layers, we do nothing and exit
        if (!MMLayers.LayerInLayerMask(collider.layer, TargetLayerMask))
        {

            return;
        }

        // if we're on our first frame, we don't apply damage
        if (Time.time == 0f)
        {
            return;
        }

        _collisionPoint = this.transform.position;
        _colliderHealth = collider.gameObject.MMGetComponentNoAlloc<Health>();

        // if what we're colliding with is damageable
        if (_colliderHealth != null)
        {
            if (_colliderHealth.CurrentHealth >= 0)// 必须加入 =
            {
                if (GameHelper.RandomBingo(0f, 100f, HitRate))
                {
                    OnCollideWithDamageable(_colliderHealth);
                }
                else
                {
                    if (DgOnDamageMissing != null)
                    {
                        DgOnDamageMissing();
                    }                    
                    OnCollideWithNonDamageable();
                }
            }
            else
            {
                OnCollideWithNonDamageable();
            }
        }
        // if what we're colliding with can't be damaged
        else
        {
            OnCollideWithNonDamageable();
        }
    }
}
