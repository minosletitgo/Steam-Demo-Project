using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_AIActionOrder2Hammerman : IBase_Friend_AIActionOrder
{
    public F_AIActionOrder2Hammerman(EM_F_AIActionOrderHandler emHandler) : base(emHandler)
    {

    }

    protected override IBase_Friend_Character FindIdleAICharacter(ST_F_AIActionOrder stOrder)
    {
        GameCommon.CHECK(stOrder != null);
        Vector3 v3BuildingPos = stOrder.GettTargetBuilding().transform.position;

        IBase_Friend_FactoryBuilding stTarget = null;
        foreach (IBase_Friend_Building _stBuilding in Minos_BuildingManager.Instance.EnumAll_F_Building())
        {
            switch (_stBuilding.GetBuildingType())
            {
                case EM_F_BuildingType.F_HammermanFactory:
                    {
                        IBase_Friend_FactoryBuilding _stFactoryBuilding = _stBuilding as IBase_Friend_FactoryBuilding;
                        GameCommon.CHECK(_stFactoryBuilding != null);
                        stTarget = _stFactoryBuilding;
                    }
                    break;
            }
        }

        GameCommon.CHECK(stTarget != null);
        float fDisBetween = float.MaxValue;
        IBase_Friend_Character stCharNearest = null;
        foreach (IBase_Friend_Character _stChar in stTarget.EnumCharStorage())
        {
            if (_stChar.GetAIActionOrder() == null)
            {
                float _fDis = Vector3.Distance(_stChar.transform.position, v3BuildingPos);
                if (_fDis <= fDisBetween)
                {
                    //找出距离最近的闲置目标
                    fDisBetween = _fDis;
                    stCharNearest = _stChar;
                }                
            }
        }

        return stCharNearest;
    }

    protected override void TryLinkOrder2TargetAIChar(ST_F_AIActionOrder stOrder, IBase_Friend_Character stIdleChar)
    {
        GameCommon.CHECK(stIdleChar != null);

        //如果是晚上，一律不接收指令
        if (!Minos_GameDateManager.Instance.IsDayOrNight())
        {
            Debug.LogWarning("F_AIActionOrder2Hammerman.TryLinkOrder2TargetAIChar Failed !");
            return;
        }

        if (stOrder != null)
        {
            GameCommon.CHECK(stOrder.GettTargetBuilding() != null);

            switch (stOrder.GetOType())
            {
                case EM_F_AIActionOrderType.LevUpBuilding:
                case EM_F_AIActionOrderType.RepairingBuilding:
                case EM_F_AIActionOrderType.CuttingTree:
                    {
                        ST_F_AIActionOrder stOrderClone = new ST_F_AIActionOrder(stOrder);
                        GameCommon.CHECK(stOrderClone != null);
                        stIdleChar.ConnectAIActionOrder(stOrderClone);

                        DeleteOrder(stOrder);
                    }
                    break;
            }
        }
    }

    public override void Update()
    {
        base.Update();

        if (m_bReTryLinkOrderFlag)
        {
            if (Time.time - m_fReTryLinkOrderTimeStamp >= m_fReTryLinkOrderTimeCD)
            {
                if (m_lstOrderCache.Count > 0)
                {
                    TryLinkOrder2TargetAIChar(m_lstOrderCache[0]);
                }

                m_fReTryLinkOrderTimeStamp = Time.time;
            }
        }
    }
}
