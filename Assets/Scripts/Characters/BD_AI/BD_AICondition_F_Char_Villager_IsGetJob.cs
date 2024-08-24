using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class BD_AICondition_F_Char_Villager_IsGetJob : Conditional
{
    public SharedInt targetAIOrderType;

    public override TaskStatus OnUpdate()
    {
        switch ((EM_F_AIActionOrderType)(targetAIOrderType.Value))
        {
            case EM_F_AIActionOrderType.ProducingBowman:
            case EM_F_AIActionOrderType.ProducingHammerman:
            case EM_F_AIActionOrderType.ProducingFarmer:
            case EM_F_AIActionOrderType.ProducingNearWarriorA:
            case EM_F_AIActionOrderType.ProducingKnight:
                return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }

}
