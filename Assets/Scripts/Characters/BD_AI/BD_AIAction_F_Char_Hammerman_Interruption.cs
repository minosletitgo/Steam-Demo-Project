using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class BD_AIAction_F_Char_Hammerman_Interruption : Action
{
    IBase_Friend_Character m_stBaseChar;
    ST_F_AIActionOrder m_stAAOrder;
    TaskStatus m_emUpdateRet;


    public override void OnAwake()
    {
        base.OnAwake();

        m_stBaseChar = gameObject.GetComponent<IBase_Friend_Character>();
        GameCommon.CHECK(m_stBaseChar != null);
    }

    public override void OnStart()
    {
        base.OnStart();
        
        if (m_stBaseChar.GetAIActionOrder() == null)
        {
            m_emUpdateRet = TaskStatus.Failure;
            return;
        }

        m_stAAOrder = m_stBaseChar.GetAIActionOrder() as ST_F_AIActionOrder;
        GameCommon.CHECK(m_stAAOrder != null);

        m_stBaseChar.GetTDCharMovement().MovementForbidden = false;
        m_stBaseChar.GetTDCharMovement().SetMovement(Vector2.zero);
        m_stBaseChar.GetTDCharacter()._animator.SetBool("Cutting", false);
        F_AIActionOrderManager.Instance.GetOrderHandler(m_stAAOrder.GetOHandler()).
            GiveBackOrder(m_stAAOrder.GetOType(), m_stAAOrder.GettTargetBuilding());
        m_stBaseChar.ClearAIActionOrder();
        Owner.SetVariableValue(
            IBase_Friend_AIActionOrder.m_strVariableName_AIOrderType,
            EM_F_AIActionOrderType.Idle
            );
        m_emUpdateRet = TaskStatus.Success;
    }

    public override TaskStatus OnUpdate()
    {
        return m_emUpdateRet;
    }
}
