using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using System;

/*
    F_PrimitivemanCharacter.cs 
    AI简述（纯粹AI状态机）：
    .【PatrolingToMoneyCoin】
        A：[AIActionMovePatrol3D]            
        D：[AIDecisionDetectTargetRadius3D](5 -> MoneyCoin) 
            True -> 【MoveToMoneyCoin】
            False -> 【PatrolingToMoneyCoin】
    .【MoveToMoneyCoin】
        A: [AIActionMoveTowardsTarget3D]
        D: [AIDecisionTargetIsNull]
            True -> 【PatrolingToPlayer】
*/

public class F_PrimitivemanCharacter : IBase_Friend_Character
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void ApplyLevAttr()
    {
        //throw new NotImplementedException();
    }

    public override void PickMoneyCoin(PickItem2MoneyCoin stMoneyCoin)
    {
        F_PrimitivemanFactory stFactory = GetParentFactory() as F_PrimitivemanFactory;
        GameCommon.CHECK(stFactory != null);

        m_stPickMoneyCoin?.PlayFeedbacks();

        Minos_VillagerFactory.Instance.IncreaseVillager(transform.position, GetCurrentDirection());
        stFactory.DecreaseCharacter(GetOnlyId());
        stFactory.ResetLastBornUpdate();

        Destroy(stMoneyCoin.gameObject);
    }
}
