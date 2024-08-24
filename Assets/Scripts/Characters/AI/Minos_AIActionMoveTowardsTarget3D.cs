using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Requires a CharacterMovement ability. Makes the character move up to the specified MinimumDistance in the direction of the target. 
    /// </summary>
    [RequireComponent(typeof(CharacterMovement))]
    public class Minos_AIActionMoveTowardsTarget3D : AIAction
    {
        public float MinRandomDistance = 1f;
        public float MaxRandomDistance = 1f;

        protected Vector3 _directionToTarget;
        protected CharacterMovement _characterMovement;
        protected CharacterOrientation3D _characterOrientation3D;
        protected int _numberOfJumps = 0;
        protected Vector2 _movementVector;

        /// <summary>
        /// On init we grab our CharacterMovement ability
        /// </summary>
        protected override void Initialization()
        {
            _characterMovement = this.gameObject.GetComponent<CharacterMovement>();
            _characterOrientation3D = this.gameObject.GetComponent<CharacterOrientation3D>();
        }

        /// <summary>
        /// On PerformAction we move
        /// </summary>
        public override void PerformAction()
        {
            Move();
        }

        /// <summary>
        /// Moves the character towards the target if needed
        /// </summary>
        protected virtual void Move()
        {
            if (_brain.Target == null)
            {
                return;
            }

            _directionToTarget = _brain.Target.position - this.transform.position;
            _movementVector.x = _directionToTarget.x;
            _movementVector.y = _directionToTarget.z;
            _characterMovement.SetMovement(_movementVector);

            float caluMinimumDistance = UnityEngine.Random.Range(MinRandomDistance, MaxRandomDistance);

            //if (Mathf.Abs(this.transform.position.x - _brain.Target.position.x) < caluMinimumDistance)
            //{
            //    _characterMovement.SetHorizontalMovement(0f);
            //}

            //if (Mathf.Abs(this.transform.position.z - _brain.Target.position.z) < caluMinimumDistance)
            //{
            //    _characterMovement.SetVerticalMovement(0f);
            //}

            if (Vector3.Distance(this.transform.position, _brain.Target.position) < caluMinimumDistance)
            {
                _characterMovement.SetHorizontalMovement(0f);
                _characterMovement.SetVerticalMovement(0f);
            }            

            _characterOrientation3D.MovementRotatingModel.transform.rotation = Quaternion.LookRotation(_directionToTarget);
        }

        /// <summary>
        /// On exit state we stop our movement
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();

            _characterMovement.SetHorizontalMovement(0f);
            _characterMovement.SetVerticalMovement(0f);
        }
    }
}
