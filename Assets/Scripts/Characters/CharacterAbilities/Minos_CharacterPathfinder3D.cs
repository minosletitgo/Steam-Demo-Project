using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine.AI;

/*
    改造【CharacterPathfinder3D】，目标改为“纯粹的点坐标”，不需要“实体目标GameObject”
*/

public class Minos_CharacterPathfinder3D : MonoBehaviour
{
    [Header("PathfindingTarget")]
    /// the target the character should pathfind to
    //public Transform Target;
    [ReadOnly]
    [SerializeField]
    Vector3 TargetPosition;

    /// the distance to waypoint at which the movement is considered complete
    public float DistanceToWaypointThreshold = 1f;

    [Header("Debug")]
    /// whether or not we should draw a debug line to show the current path of the character
    public bool DebugDrawPath;

    [ReadOnly]
    /// the current path
    public NavMeshPath AgentPath;
    [ReadOnly]
    /// a list of waypoints the character will go through
    public Vector3[] Waypoints;
    [ReadOnly]
    /// the index of the next waypoint
    public int NextWaypointIndex;
    [ReadOnly]
    /// the direction of the next waypoint
    public Vector3 NextWaypointDirection;
    [ReadOnly]
    /// the distance to the next waypoint
    public float DistanceToNextWaypoint;

    protected Vector3 _direction;
    protected Vector2 _newMovement;
    protected TopDownController _topDownController;
    protected CharacterMovement _characterMovement;

    [ReadOnly]
    [SerializeField]
    protected bool _isHaveTarget = false;
    [ReadOnly]
    [SerializeField]
    protected bool _isWaypointsFailed = false;




    protected virtual void Awake()
    {
        AgentPath = new NavMeshPath();
        _topDownController = GetComponent<TopDownController>();
        _characterMovement = GetComponent<CharacterMovement>();

        _isHaveTarget = false;
        _isWaypointsFailed = false;
    }

    /// <summary>
    /// Sets a new destination the character will pathfind to
    /// </summary>
    /// <param name="destinationTransform"></param>
    public virtual void SetNewDestination(Vector3 destinationPosition)
    {
        _isHaveTarget = true;
        TargetPosition = destinationPosition;
        DeterminePath(this.transform.position, destinationPosition);
    }

    public virtual void SetNewDestinationNull() 
    { 
        _isHaveTarget = false;
        _characterMovement.SetMovement(Vector2.zero);
        TargetPosition = Vector3.zero;
    }
    
    /// <summary>
    /// On Update, we draw the path if needed, determine the next waypoint, and move to it if needed
    /// </summary>
    protected virtual void Update()
    {
        if (!_isHaveTarget)
        {
            return;
        }

        DrawDebugPath();
        DetermineNextWaypoint();
        DetermineDistanceToNextWaypoint();
        MoveController();
    }
    
    /// <summary>
    /// Moves the controller towards the next point
    /// </summary>
    protected virtual void MoveController()
    {
        if ((!_isHaveTarget) || (NextWaypointIndex < 0) || (Waypoints == null || Waypoints.Length <= 0))
        {
            _characterMovement.SetMovement(Vector2.zero);
            return;
        }
        else
        {
            _direction = (Waypoints[NextWaypointIndex] - this.transform.position).normalized;
            _newMovement.x = _direction.x;
            _newMovement.y = _direction.z;
            _characterMovement.SetMovement(_newMovement);
        }
    }

    /// <summary>
    /// Determines the next path position for the agent. NextPosition will be zero if a path couldn't be found
    /// </summary>
    /// <param name="startingPos"></param>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    protected virtual void DeterminePath(Vector3 startingPos, Vector3 targetPos)
    {
        NextWaypointIndex = 0;

        NavMesh.CalculatePath(startingPos, targetPos, NavMesh.AllAreas, AgentPath);
        Waypoints = AgentPath.corners;
        //Debug.Log(gameObject.name + " AgentPath.corners = " + AgentPath.corners.Length);
        if (AgentPath.corners.Length >= 2)
        {
            NextWaypointIndex = 1;
        }

        if (Waypoints == null || Waypoints.Length <= 0)
        {
            _isWaypointsFailed = true;
        }
        else
        {
            _isWaypointsFailed = false;
        }
    }

    /// <summary>
    /// Determines the next waypoint based on the distance to it
    /// </summary>
    protected virtual void DetermineNextWaypoint()
    {
        if (Waypoints.Length <= 0)
        {
            return;
        }
        if (NextWaypointIndex < 0)
        {
            return;
        }

        if (Vector3.Distance(this.transform.position, Waypoints[NextWaypointIndex]) <= DistanceToWaypointThreshold)
        {
            if (NextWaypointIndex + 1 < Waypoints.Length)
            {
                NextWaypointIndex++;
            }
            else
            {
                NextWaypointIndex = -1;
            }
        }
    }

    /// <summary>
    /// Determines the distance to the next waypoint
    /// </summary>
    protected virtual void DetermineDistanceToNextWaypoint()
    {
        if (NextWaypointIndex <= 0)
        {
            DistanceToNextWaypoint = 0;
        }
        else
        {
            DistanceToNextWaypoint = Vector3.Distance(this.transform.position, Waypoints[NextWaypointIndex]);
        }
    }

    /// <summary>
    /// Draws a debug line to show the current path
    /// </summary>
    protected virtual void DrawDebugPath()
    {
        if (DebugDrawPath)
        {
            for (int i = 0; i < AgentPath.corners.Length - 1; i++)
            {
                Debug.DrawLine(AgentPath.corners[i], AgentPath.corners[i + 1], Color.red);
            }
        }
    }

    public virtual bool IsWaypointsFailed()
    {
        //if (Waypoints == null || Waypoints.Length <= 0)
        //{
        //    return true;
        //}

        //if (NextWaypointIndex < 0)
        //{
        //    return true;
        //}

        //return false;
        return _isWaypointsFailed; 
    }

    public virtual bool IsNextWaypointFailed()
    {
        return NextWaypointIndex == -1;
    }
}
