using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class BD_AIAction_F_Char_Farmer_DoFarming : Action
{
    IBase_Friend_Character m_stBaseChar;
    bool m_bIsActionRunning = false;
    bool m_bIsActionDone = false;
    IEnumerator m_coAction;


    public override void OnAwake()
    {
        base.OnAwake();

        m_stBaseChar = gameObject.GetComponent<IBase_Friend_Character>();
        GameCommon.CHECK(m_stBaseChar != null);
    }

    public override void OnStart()
    {
        base.OnStart();

        m_bIsActionRunning = false;
        m_bIsActionDone = false;
        m_coAction = null;

        m_coAction = CoAction();
        StartCoroutine(m_coAction);
    }

    public override TaskStatus OnUpdate()
    {
        if (m_bIsActionRunning)
        {
            return TaskStatus.Running;
        }
        if (m_bIsActionDone)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }

    IEnumerator CoAction()
    {
        m_bIsActionRunning = true;
        m_bIsActionDone = false;

        GameCommon.CHECK(m_stBaseChar.GetAIActionOrder() != null);
        ST_F_AIActionOrder stAAOrder = m_stBaseChar.GetAIActionOrder() as ST_F_AIActionOrder;
        GameCommon.CHECK(stAAOrder != null);
        GameCommon.CHECK(stAAOrder.GetOType() == EM_F_AIActionOrderType.Farming);

        IBase_Friend_Building stTargetBuilding = stAAOrder.GettTargetBuilding();
        GameCommon.CHECK(stTargetBuilding != null);
        F_Farmland stFarmland = stTargetBuilding as F_Farmland;
        GameCommon.CHECK(stFarmland != null);

        m_stBaseChar.GetTDCharMovement().SetMovement(Vector2.zero);
        m_stBaseChar.GetTDCharMovement().MovementForbidden = true;
        m_stBaseChar.GetTDCharacter()._animator.SetBool("Cutting", true);

        yield return GameHelper.WaitUntilTrue(
            Owner,
            delegate ()
            {
                return !(Minos_GameDateManager.Instance.IsDayOrNight());
            }
            );

        stFarmland.IncreaseFarmingTimes();

        m_stBaseChar.GetTDCharMovement().MovementForbidden = false;
        m_stBaseChar.GetTDCharMovement().SetMovement(Vector2.zero);
        m_stBaseChar.GetTDCharacter()._animator.SetBool("Cutting", false);

        stTargetBuilding.SetIsSelfSendedAIActionOrder(false);
        m_stBaseChar.ClearAIActionOrder();
        Owner.SetVariableValue(
            IBase_Friend_AIActionOrder.m_strVariableName_AIOrderType,
            EM_F_AIActionOrderType.Idle
            );
        Owner.SetVariableValue(
            IBase_Friend_AIActionOrder.m_strVariableName_AIOrderMoveTo,
            null
            );
        m_stBaseChar.GetParentFactory().CreateIdleOrderSignal(m_stBaseChar);

        m_bIsActionRunning = false;
        m_bIsActionDone = true;
        m_coAction = null;
    }

    public override void OnEnd()
    {
        base.OnEnd();

        if (m_coAction != null)
        {
            StopCoroutine(m_coAction);
        }
        m_coAction = null;
    }
}
