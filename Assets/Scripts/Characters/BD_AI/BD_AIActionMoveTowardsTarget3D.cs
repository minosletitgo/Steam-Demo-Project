using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/*
       基于一点原因来构建此类：
    01.大体模仿【TopDownEngine.AIActionMoveTowardsTarget3D】，单纯的改为“BehaviorDesigner的方式”
    02.现在看来没啥必要使用，完全可以被【BD_AIActionFindPathToTarget3D】取代
*/

public class BD_AIActionMoveTowardsTarget3D : Action
{
    /// the minimum distance from the target this Character can reach.
    public SharedFloat arriveDistance = 0.2f;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The object that we are moving towards")]
    public SharedGameObject targetObject;
    public bool movingByRun = false;

    protected Vector3 _directionToTarget;
    protected CharacterMovement _characterMovement;
    protected CharacterOrientation3D _characterOrientation3D;
    protected int _numberOfJumps = 0;
    protected Vector2 _movementVector;
    protected Minos_CharacterRun _characterRun;






    public override void OnAwake()
    {
        base.OnAwake();
        Initialization();
    }


    /// <summary>
    /// On init we grab our CharacterMovement ability
    /// </summary>
    protected /*override*/ void Initialization()
    {
        _characterMovement = this.gameObject.GetComponent<CharacterMovement>();
        _characterOrientation3D = this.gameObject.GetComponent<CharacterOrientation3D>();
        _characterRun = this.gameObject.GetComponent<Minos_CharacterRun>();
    }

    ///// <summary>
    ///// On PerformAction we move
    ///// </summary>
    //public override void PerformAction()
    //{
    //    Move();
    //}

    /// <summary>
    /// Moves the character towards the target if needed
    /// </summary>
    protected virtual bool Move()
    {
        if (targetObject.Value == null)
        {
            return false;
        }

        if (movingByRun && _characterRun != null)
        {
            _characterRun.RunStart();
        }

        _directionToTarget = targetObject.Value.transform.position - this.transform.position;
        _movementVector.x = _directionToTarget.x;
        _movementVector.y = _directionToTarget.z;
        _characterMovement.SetMovement(_movementVector);


        if (Mathf.Abs(this.transform.position.x - targetObject.Value.transform.position.x) < arriveDistance.Value)
        {
            _characterMovement.SetHorizontalMovement(0f);
        }

        if (Mathf.Abs(this.transform.position.z - targetObject.Value.transform.position.z) < arriveDistance.Value)
        {
            _characterMovement.SetVerticalMovement(0f);
        }

        _characterOrientation3D.MovementRotatingModel.transform.rotation = Quaternion.LookRotation(_directionToTarget);

        return true;
    }

    ///// <summary>
    ///// On exit state we stop our movement
    ///// </summary>
    //public override void OnExitState()
    //{
    //    base.OnExitState();

    //    _characterMovement.SetHorizontalMovement(0f);
    //    _characterMovement.SetVerticalMovement(0f);
    //}

    public override TaskStatus OnUpdate()
    {
        if (HasArrived())
        {
            return TaskStatus.Success;
        }

        if (Move())
        {
            return TaskStatus.Running;
        }

        return TaskStatus.Failure;
    }

    /// <summary>
    /// Has the agent arrived at the destination?
    /// </summary>
    /// <returns>True if the agent has arrived at the destination.</returns>
    protected /*override*/ bool HasArrived()
    {
        if (targetObject.Value == null)
        {
            return false;
        }

        float remainingDistance = Vector3.Distance(this.transform.position, targetObject.Value.transform.position);
        return remainingDistance <= arriveDistance.Value;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        if (movingByRun && _characterRun != null)
        {
            _characterRun.RunStop();
        }
    }
}
