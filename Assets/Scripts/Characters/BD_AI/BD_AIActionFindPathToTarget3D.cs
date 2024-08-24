using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[RequireComponent(typeof(CharacterPathfinder3D))]
public class BD_AIActionFindPathToTarget3D : Action
{
    /// the minimum distance from the target this Character can reach.
    public SharedFloat arriveDistance = 0.2f;

    public SharedBool isUseTargetObject = true;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("The object that we are moving towards")]    
    public SharedGameObject targetObject;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("The position that we are moving towards")]
    public SharedVector3 targetPosition;


    public bool movingByRun = false;


    private Minos_CharacterPathfinder3D finder;
    private Character character;
    private Minos_CharacterRun characterRun;



    public override void OnAwake()
    {
        base.OnAwake();

        finder = gameObject.AddMissingComponent<Minos_CharacterPathfinder3D>();
        GameCommon.CHECK(finder != null);

        character = this.gameObject.GetComponent<Character>();
        GameCommon.CHECK(character != null);

        characterRun = this.gameObject.GetComponent<Minos_CharacterRun>();
        GameCommon.CHECK(characterRun != null);
    }

    public override void OnStart()
    {
        base.OnStart();

        finder.DistanceToWaypointThreshold = arriveDistance.Value;

        if (isUseTargetObject.Value)
        {
            if (targetObject.Value == null)
            {
                return;
            }
            finder.SetNewDestination(targetObject.Value.transform.position);
        }
        else
        {
            finder.SetNewDestination(targetPosition.Value);
        }        
    }

    public override TaskStatus OnUpdate()
    {
        if (isUseTargetObject.Value) 
        {
            if (targetObject.Value == null)
            {
                return TaskStatus.Failure;
            }
        }        

        if (finder.IsWaypointsFailed())
        {
            return TaskStatus.Failure;
        }

        if (movingByRun && character.MovementState.CurrentState != CharacterStates.MovementStates.Running)
        {
            characterRun.RunStart();
        }

        if (HasArrived())
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    /// <summary>
    /// Has the agent arrived at the destination?
    /// </summary>
    /// <returns>True if the agent has arrived at the destination.</returns>
    protected /*override*/ bool HasArrived()
    {
        float remainingDistance = 0;

        if (isUseTargetObject.Value)
        {
            if (targetObject.Value == null)
            {
                return false;
            }

            remainingDistance = Vector3.Distance(this.transform.position, targetObject.Value.transform.position);
        }
        else
        {
            remainingDistance = Vector3.Distance(this.transform.position, targetPosition.Value);
        }
        
        if (remainingDistance <= arriveDistance.Value)
        {
            return true;
        }

        if (finder.IsNextWaypointFailed())
        {
            return true;
        }

        return false;
    }

    public override void OnEnd()
    {
        base.OnEnd();

        finder.SetNewDestinationNull();
        if (movingByRun && character.MovementState.CurrentState == CharacterStates.MovementStates.Running)
        {
            characterRun.RunStop();
        }
    }

    // Draw a gizmo indicating a patrol 
    public override void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (isUseTargetObject.Value)
        {
            if (targetObject.Value == null)
            {
                return;
            }
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.SphereHandleCap(
                0,
                targetObject.Value.transform.position,
                targetObject.Value.transform.rotation,
                1,
                EventType.Repaint
                );
            UnityEditor.Handles.color = oldColor;
        }
        else
        {
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.SphereHandleCap(
                0,
                targetPosition.Value,
                Quaternion.identity,
                1,
                EventType.Repaint
                );
            UnityEditor.Handles.color = oldColor;
        }
#endif
    }
}
