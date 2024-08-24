using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class BD_AIAction_F_Char_Hammerman_InWorking : Conditional
{
    public SharedInt srcOrderType;
    public EM_F_AIActionOrderType compareOrderType;
    public bool isCompareAnyOrderType;

    public override void OnStart()
    {
        base.OnStart();

        if (!isCompareAnyOrderType)
        {
            GameCommon.CHECK(compareOrderType == EM_F_AIActionOrderType.CuttingTree ||
                compareOrderType == EM_F_AIActionOrderType.LevUpBuilding ||
                compareOrderType == EM_F_AIActionOrderType.RepairingBuilding
                );
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (!isCompareAnyOrderType)
        {
            return ((EM_F_AIActionOrderType)(srcOrderType.Value) == compareOrderType) ? TaskStatus.Success : TaskStatus.Failure;
        }
        else
        {
            switch ((EM_F_AIActionOrderType)(srcOrderType.Value))
            {
                case EM_F_AIActionOrderType.CuttingTree:
                case EM_F_AIActionOrderType.LevUpBuilding:
                case EM_F_AIActionOrderType.RepairingBuilding:
                    return TaskStatus.Success;
                default: return TaskStatus.Failure;
            }           
        }
    }
}
