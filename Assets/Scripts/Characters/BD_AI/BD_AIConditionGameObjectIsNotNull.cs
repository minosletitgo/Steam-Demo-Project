using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class BD_AIConditionGameObjectIsNotNull : Conditional
{
    public SharedGameObject targetGameObject;

    public override TaskStatus OnUpdate()
    {
        //return (GetDefaultGameObject(targetGameObject.Value) != null) ? TaskStatus.Success : TaskStatus.Failure;
        return (targetGameObject.Value != null) ? TaskStatus.Success : TaskStatus.Failure;
    }

    public override void OnReset()
    {
        targetGameObject = null;
    }
}
