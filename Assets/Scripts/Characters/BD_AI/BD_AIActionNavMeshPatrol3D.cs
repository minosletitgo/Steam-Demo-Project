using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

/*
       基于两点原因来构建此类：
    01.【TopDownEngine.AIActionMovePatrol3D】只能做到单纯的 “点与点之间的直线移动，碰到障碍物则直连下一个点，出现死胡同就废了”
    02.【BehaviorDesigner.Movement.Patrol】能够很直观的在“点与点之间进行寻路移动”，但最终“直接寻路(NavMeshAgent.SetDestination)”，没有给第三方提供“拆解移动行为”。
        编写纪要：
    01.大体模仿【BehaviorDesigner.Movement.Patrol】，添加“可选式waypoints(即纯点)”，添加“拆解移动行为”。
    02.“拆解移动行为”即：嵌入一些【TopDownEngine】的移动逻辑
*/

[RequireComponent(typeof(CharacterPathfinder3D))]
public class BD_AIActionNavMeshPatrol3D : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Should the agent patrol the waypoints randomly?")]
    public SharedBool randomPatrol = false;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The length of time that the agent should pause when arriving at a waypoint")]
    public SharedFloat waypointPauseDuration = 0;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The agent has arrived when the destination is less than the specified amount. This distance should be greater than or equal to the NavMeshAgent StoppingDistance.")]
    public SharedFloat arriveDistance = 0.2f;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The waypoints to move to")]
    public SharedGameObjectList waypoints;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("The taskStatus to return")]
    public TaskStatus retTaskStatus = TaskStatus.Running;

    // The current index that we are heading towards within the waypoints array
    private int waypointIndex;
    private float waypointReachedTime;
    //...
    private Minos_CharacterPathfinder3D finder;
    private CharacterMovement _characterMovement;


    public override void OnAwake()
    {
        base.OnAwake();
        finder = gameObject.AddMissingComponent<Minos_CharacterPathfinder3D>();
        _characterMovement = this.gameObject.GetComponent<CharacterMovement>();
    }

    public override void OnStart()
    {
        base.OnStart();

        finder.DistanceToWaypointThreshold = arriveDistance.Value;

        // initially move towards the closest waypoint
        float distance = Mathf.Infinity;
        float localDistance;
        for (int i = 0; i < waypoints.Value.Count; ++i)
        {
            if ((localDistance = Vector3.Magnitude(transform.position - waypoints.Value[i].transform.position)) < distance)
            {
                distance = localDistance;
                waypointIndex = i;
            }
        }
        waypointReachedTime = -1;
        SetDestination(TargetTransform());
    }
    
    // Patrol around the different waypoints specified in the waypoint array. Always return a task status of running. 
    public override TaskStatus OnUpdate()
    {
        if (waypoints.Value.Count == 0)
        {
            return TaskStatus.Failure;
        }
        if (HasArrived())
        {
            if (waypointReachedTime == -1)
            {
                waypointReachedTime = Time.time;
            }
            // wait the required duration before switching waypoints.
            if (waypointReachedTime + waypointPauseDuration.Value <= Time.time)
            {
                if (randomPatrol.Value)
                {
                    if (waypoints.Value.Count == 1)
                    {
                        waypointIndex = 0;
                    }
                    else
                    {
                        // prevent the same waypoint from being selected
                        var newWaypointIndex = waypointIndex;
                        while (newWaypointIndex == waypointIndex)
                        {
                            newWaypointIndex = Random.Range(0, waypoints.Value.Count);
                        }
                        waypointIndex = newWaypointIndex;
                    }
                }
                else
                {
                    waypointIndex = (waypointIndex + 1) % waypoints.Value.Count;
                }
                SetDestination(TargetTransform());
                waypointReachedTime = -1;
            }
        }
        else
        {
            if (finder.IsWaypointsFailed())
            {
                //尝试解困
                {
                    Vector3 _directionToTarget = TargetTransform().transform.position - this.transform.position;
                    Vector2 _movementVector = new Vector2(_directionToTarget.x, _directionToTarget.z);
                    _characterMovement.SetMovement(_movementVector);
                }
                return TaskStatus.Failure;
            }
        }

        //return TaskStatus.Running;
        return retTaskStatus;
    }

    // Return the current waypoint index position
    private Vector3 Target()
    {
        if (waypointIndex >= waypoints.Value.Count)
        {
            return transform.position;
        }
        return waypoints.Value[waypointIndex].transform.position;
    }

    // Reset the public variables
    public override void OnReset()
    {
        base.OnReset();

        randomPatrol = false;
        waypointPauseDuration = 0;
        waypoints = null;
    }

    // Draw a gizmo indicating a patrol 
    public override void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (waypoints == null || waypoints.Value == null)
        {
            return;
        }
        var oldColor = UnityEditor.Handles.color;
        UnityEditor.Handles.color = Color.yellow;
        for (int i = 0; i < waypoints.Value.Count; ++i)
        {
            if (waypoints.Value[i] != null)
            {
                UnityEditor.Handles.SphereHandleCap(0, waypoints.Value[i].transform.position, waypoints.Value[i].transform.rotation, 1, EventType.Repaint);
            }
        }
        UnityEditor.Handles.color = oldColor;
#endif
    }

















    public void SetWaypoints(List<GameObject> _points)
    {
        //注意不能扰乱正在进行的索引判断
        for (int i = 0; i < waypoints.Value.Count; i++)
        {
            waypoints.Value[i] = null;
        }

        waypoints.Value.Clear();
        waypoints.Value.AddRange(_points);
    }

    // Return the current waypoint index transform
    private Transform TargetTransform()
    {
        if (waypointIndex >= waypoints.Value.Count)
        {
            return transform;
        }
        return waypoints.Value[waypointIndex].transform;
    }

    /// <summary>
    /// Set a new pathfinding destination.
    /// </summary>
    /// <param name="destination">The destination to set.</param>
    /// <returns>True if the destination is valid.</returns>
    protected /*override*/ bool SetDestination(Transform destination)
    {
        finder.SetNewDestination(destination.position);
        return true;
    }

    /// <summary>
    /// Has the agent arrived at the destination?
    /// </summary>
    /// <returns>True if the agent has arrived at the destination.</returns>
    protected /*override*/ bool HasArrived()
    {
        // The path hasn't been computed yet if the path is pending.
        float remainingDistance = Vector3.Distance(this.transform.position, Target());
        //if (navMeshAgent.pathPending)
        //{
        //    remainingDistance = float.PositiveInfinity;
        //}
        //else
        //{
        //    remainingDistance = navMeshAgent.remainingDistance;
        //}

        return remainingDistance <= arriveDistance.Value;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        finder.SetNewDestinationNull();
    }
}
