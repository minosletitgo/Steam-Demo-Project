using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/*
    单纯模仿【AIActionShoot3D】 
*/

public class BD_AIActionShoot3D : Action
{
    /// if true, the Character will face the target (left/right) when shooting
    public bool FaceTarget = true;
    /// if true the Character will aim at the target when shooting
    public bool AimAtTarget = false;
    /// an offset to apply to the aim (useful to aim at the head/torso/etc automatically)
    public Vector3 ShootOffset;

    public bool IgnoreNumberOfShoots = false;

    public SharedGameObject ShootTarget;

    public SharedFloat EnoughCloserToShoot;


    protected CharacterOrientation3D _orientation3D;
    protected Character _character;
    protected CharacterHandleWeapon _characterHandleWeapon;
    protected WeaponAim _weaponAim;
    protected ProjectileWeapon _projectileWeapon;
    protected Vector3 _weaponAimDirection;
    protected int _numberOfShoots = 0;
    protected bool _shooting = false;



    public override void OnAwake()
    {
        base.OnAwake();

        Initialization();
    }

    public override void OnStart()
    {
        base.OnStart();

        _numberOfShoots = 0;
        _shooting = true;
        _weaponAim = _characterHandleWeapon.CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>();
        _projectileWeapon = _characterHandleWeapon.CurrentWeapon.gameObject.MMGetComponentNoAlloc<ProjectileWeapon>();
    }

    public override void OnEnd()
    {
        base.OnEnd();

        _characterHandleWeapon.ShootStop();
        _shooting = false;
    }

    public override TaskStatus OnUpdate()
    {
        if (ShootTarget.Value == null)
        {
            //提前终止
            return TaskStatus.Failure;
        }

        float remainingDistance = Vector3.Distance(this.transform.position, ShootTarget.Value.transform.position);
        if (remainingDistance > EnoughCloserToShoot.Value)
        {
            //提前终止
            return TaskStatus.Failure;
        }

        if (_characterHandleWeapon.CurrentWeapon != null)
        {
            if (_weaponAim != null)
            {
                if (_shooting)
                {
                    _weaponAim.SetCurrentAim(_weaponAimDirection);
                }
            }
        }

        MakeChangesToTheWeapon();
        TestAimAtTarget();
        
        if (EnoughCloserToShoot.Value > 0)
        {
            //对于远程攻击
            if (remainingDistance <= EnoughCloserToShoot.Value)
            {
                Shoot();
                return TaskStatus.Running;
            }
            else
            {
                return TaskStatus.Failure;
            }
        }
        else
        {
            //对于近程攻击
            Shoot();
            return TaskStatus.Running;
        }
    }













    /// <summary>
    /// On init we grab our CharacterHandleWeapon ability
    /// </summary>
    protected /*override*/ void Initialization()
    {
        _character = GetComponent<Character>();
        _orientation3D = GetComponent<CharacterOrientation3D>();
        _characterHandleWeapon = this.gameObject.GetComponent<CharacterHandleWeapon>();
    }

    ///// <summary>
    ///// On PerformAction we face and aim if needed, and we shoot
    ///// </summary>
    //public override void PerformAction()
    //{
    //    MakeChangesToTheWeapon();
    //    TestAimAtTarget();
    //    Shoot();
    //}

    /// <summary>
    /// Makes changes to the weapon to ensure it works ok with AI scripts
    /// </summary>
    protected virtual void MakeChangesToTheWeapon()
    {
        if (_characterHandleWeapon.CurrentWeapon != null)
        {
            _characterHandleWeapon.CurrentWeapon.TimeBetweenUsesReleaseInterruption = true;
        }
    }

    ///// <summary>
    ///// Sets the current aim if needed
    ///// </summary>
    //protected virtual void Update()
    //{
    //    if (_characterHandleWeapon.CurrentWeapon != null)
    //    {
    //        if (_weaponAim != null)
    //        {
    //            if (_shooting)
    //            {
    //                _weaponAim.SetCurrentAim(_weaponAimDirection);
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// Aims at the target if required
    /// </summary>
    protected virtual void TestAimAtTarget()
    {
        if (!AimAtTarget || (ShootTarget.Value == null))
        {
            return;
        }

        if (_characterHandleWeapon.CurrentWeapon != null)
        {
            if (_weaponAim == null)
            {
                _weaponAim = _characterHandleWeapon.CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>();
            }

            if (_weaponAim != null)
            {
                if (_projectileWeapon != null)
                {
                    _projectileWeapon.DetermineSpawnPosition();
                    _weaponAimDirection = ShootTarget.Value.transform.position + ShootOffset - (_character.transform.position);
                }
                else
                {
                    _weaponAimDirection = ShootTarget.Value.transform.position + ShootOffset - _character.transform.position;
                }
            }
        }
    }

    /// <summary>
    /// Activates the weapon
    /// </summary>
    protected virtual void Shoot()
    {
        if (IgnoreNumberOfShoots || _numberOfShoots < 1)
        {
            _characterHandleWeapon.ShootStart();
            _numberOfShoots++;
        }
    }

    /// <summary>
    /// When entering the state we reset our shoot counter and grab our weapon
    /// </summary>
    //public override void OnEnterState()
    //{
    //    base.OnEnterState();
    //    _numberOfShoots = 0;
    //    _shooting = true;
    //    _weaponAim = _characterHandleWeapon.CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>();
    //    _projectileWeapon = _characterHandleWeapon.CurrentWeapon.gameObject.MMGetComponentNoAlloc<ProjectileWeapon>();
    //}

    ///// <summary>
    ///// When exiting the state we make sure we're not shooting anymore
    ///// </summary>
    //public override void OnExitState()
    //{
    //    base.OnExitState();
    //    _characterHandleWeapon.ShootStop();
    //    _shooting = false;
    //}
}
