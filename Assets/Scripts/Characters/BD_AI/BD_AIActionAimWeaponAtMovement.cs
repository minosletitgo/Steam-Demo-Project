using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/*
    单纯模仿【AIActionAimWeaponAtMovement】 
*/

public class BD_AIActionAimWeaponAtMovement : Action
{
    public TaskStatus _retTaskStatus = TaskStatus.Running;

    protected TopDownController _controller;
    protected CharacterHandleWeapon _characterHandleWeapon;
    protected WeaponAim _weaponAim;
    protected AIActionShoot2D _aiActionShoot2D;
    protected AIActionShoot3D _aiActionShoot3D;
    protected Vector3 _weaponAimDirection;




    public override void OnAwake()
    {
        base.OnAwake();

        _characterHandleWeapon = this.gameObject.GetComponent<CharacterHandleWeapon>();
        _aiActionShoot2D = this.gameObject.GetComponent<AIActionShoot2D>();
        _aiActionShoot3D = this.gameObject.GetComponent<AIActionShoot3D>();
        _controller = this.gameObject.GetComponent<TopDownController>();
    }

    public override TaskStatus OnUpdate()
    {
        if (!Shooting())
        {
            _weaponAimDirection = _controller.CurrentDirection;
            if (_weaponAim == null)
            {
                GrabWeaponAim();
            }
            if (_weaponAim == null)
            {
                return TaskStatus.Failure;
            }
            _weaponAim.SetCurrentAim(_weaponAimDirection);
        }

        return _retTaskStatus;
    }

    /// <summary>
    /// Returns true if shooting, returns false otherwise
    /// </summary>
    /// <returns></returns>
    protected bool Shooting()
    {
        if (_aiActionShoot2D != null)
        {
            return _aiActionShoot2D.ActionInProgress;
        }
        if (_aiActionShoot3D != null)
        {
            return _aiActionShoot3D.ActionInProgress;
        }
        return false;
    }

    protected virtual void GrabWeaponAim()
    {
        if (_characterHandleWeapon.CurrentWeapon != null)
        {
            _weaponAim = _characterHandleWeapon.CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>();
        }
    }
}
