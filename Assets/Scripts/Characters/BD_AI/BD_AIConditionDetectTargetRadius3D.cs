using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class BD_AIConditionDetectTargetRadius3D : Conditional
{
    /// the radius to search our target in
    public float Radius = 3f;
    /// the offset to apply (from the collider's center)
    public Vector3 DetectionOriginOffset = new Vector3(0, 0, 0);
    /// the layer(s) to search our target on
    public LayerMask TargetLayerMask;
    /// whether or not we should look for obstacles
    public bool ObstacleDetection = true;
    /// the layer(s) to block the sight
    public LayerMask ObstacleMask;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("The object that is within sight")]
    public SharedGameObject returnedObject;

    protected Collider _collider;
    protected Vector3 _raycastOrigin;
    protected Character _character;
    protected Color _gizmoColor = Color.yellow;
    protected bool _init = false;
    protected Vector3 _raycastDirection;
    protected float _lastTargetCheckTimestamp = 0f;
    protected Collider[] _hit;







    public override void OnAwake()
    {
        base.OnAwake();
        Initialization();
    }



    /// <summary>
    /// On init we grab our Character component
    /// </summary>
    public /*override*/ void Initialization()
    {
        _character = this.gameObject.GetComponent<Character>();
        _collider = this.gameObject.GetComponent<Collider>();
        _gizmoColor.a = 0.25f;
        _init = true;
    }

    ///// <summary>
    ///// On Decide we check for our target
    ///// </summary>
    ///// <returns></returns>
    //public override bool Decide()
    //{
    //    return DetectTarget();
    //}

    /// <summary>
    /// Returns true if a target is found within the circle
    /// </summary>
    /// <returns></returns>
    protected virtual bool DetectTarget()
    {
        //if (Time.time - _lastTargetCheckTimestamp < TargetCheckFrequency)
        //{
        //    return false;
        //}

        _lastTargetCheckTimestamp = Time.time;

        _raycastOrigin = _collider.bounds.center + DetectionOriginOffset / 2;
        _hit = Physics.OverlapSphere(_raycastOrigin, Radius, TargetLayerMask);

        if (_hit.Length > 0)
        {
            //// we cast a ray to make sure there's no obstacle
            //_raycastDirection = _hit[0].transform.position - _raycastOrigin;
            //RaycastHit hit = MMDebug.Raycast3D(_raycastOrigin, _raycastDirection, Vector3.Distance(_hit[0].transform.position, _raycastOrigin), ObstacleMask.value, Color.yellow, true);
            //if (hit.collider == null)
            //{
            //    returnedObject.Value = _hit[0].gameObject;                
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}

            float _fMinDis = float.MaxValue;
            Collider _boxNearest = null;
            foreach (Collider _box in _hit)
            {
                float _fMinDisTmp = Vector3.Distance(transform.position, _box.transform.position);
                if (_fMinDisTmp <= _fMinDis)
                {
                    _fMinDis = _fMinDisTmp;
                    _boxNearest = _box;
                }
            }

            if (_boxNearest == null)
            {
                return false;
            }
            // we cast a ray to make sure there's no obstacle
            _raycastDirection = _boxNearest.transform.position - _raycastOrigin;
            RaycastHit hit = MMDebug.Raycast3D(_raycastOrigin, _raycastDirection, Vector3.Distance(_boxNearest.transform.position, _raycastOrigin), ObstacleMask.value, Color.yellow, true);
            if (hit.collider == null)
            {
                returnedObject.Value = _hit[0].gameObject;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Draws gizmos for the detection circle
    /// </summary>
    public override void OnDrawGizmos()
    {
        if (transform == null) return;
        _raycastOrigin = transform.position + DetectionOriginOffset / 2;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_raycastOrigin, Radius);
        if (_init)
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawSphere(_raycastOrigin, Radius);
        }
    }




    public override TaskStatus OnUpdate()
    {
        if (DetectTarget())
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
