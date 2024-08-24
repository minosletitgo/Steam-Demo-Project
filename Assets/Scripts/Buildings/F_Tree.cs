using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class F_Tree : IBase_Friend_Building
{
    [SerializeField]
    int m_nCuttingTreeCostMoneyCoin;

    [SerializeField]
    [ReadOnly]
    bool m_bIsTreeDead;


    protected override void Awake()
    {
        m_emBuildingType = EM_F_BuildingType.F_Tree;

        base.Awake();

        GameCommon.CHECK(m_nCuttingTreeCostMoneyCoin > 0, "m_nCuttingTreeCostMoneyCoin > 0");

        m_bIsTreeDead = false;
    }

    protected override void OnLogicTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            /*
                以下会同时产生[OnTriggerEnter]，即使是挂载在同一个GameObject上
                01.CharacterController继承自Collider，默认IsTrigger是不开启
                02.BoxCollider继承自Collider
                03.???Collider继承自Collider
            */
            return;
        }
        
        Player stPlayer = other.GetComponent<Player>();
        if (stPlayer != null && stPlayer.IsCanInteractiveWithBuilding(m_emBuildingType))
        {
            if (!IsSelfSendedAIActionOrder() && !m_bIsTreeDead)
            {
                base.ShowMoneyCoinBar(
                    m_nCuttingTreeCostMoneyCoin, 0.0f,
                    delegate ()
                    {
                        stPlayer.DoUnderBuilding(GetBuildingType(), gameObject);
                    }
                    );
            }
            return;
        }
    }

    protected override void OnLogicTriggerExit(Collider other)
    {
        if (!other.isTrigger)
        {
            /*
                以下会同时产生[OnTriggerExit]，即使是挂载在同一个GameObject上
                01.CharacterController继承自Collider，默认IsTrigger是不开启
                02.BoxCollider继承自Collider
                03.???Collider继承自Collider
            */
            return;
        }
        
        Player stPlayer = other.GetComponent<Player>();
        if (stPlayer != null)
        {
            base.UnShowMoneyCoinBar();
            stPlayer.UndoUnderBuilding();
            return;
        }
    }

    public override void OnMoneyCoinFinished()
    {
        F_AIActionOrderManager.Instance.GetOrderHandler(EM_F_AIActionOrderHandler.Hammerman).CreateOrder(
            EM_F_AIActionOrderType.CuttingTree, this);
    }

    public void SetTreeIsDead(bool bThrowMoneyCoin = true)
    {
        //if (bThrowMoneyCoin)
        //{
        //    PickItem2MoneyCoin itemCoin = PickItem2MoneyCoin.InstancePickItem2MoneyCoin(
        //        "MoneyCoin_DropToGround", null, Vector3.zero, Quaternion.identity, Vector3.one);
        //    GameCommon.CHECK(itemCoin != null);
        //    itemCoin.SetConfigMoneyCoin(m_nCuttingTreeCostMoneyCoin);
        //    itemCoin.PlayDropEffect(
        //        transform.position,
        //        new Vector3(transform.position.x, 0, transform.position.z),
        //        0.7f,
        //        null
        //        );
        //}

        base.m_goBuilding.SetActive(false);
        m_bIsTreeDead = true;
    }

    public void SetTreeIsRevive()
    {
        base.m_goBuilding.SetActive(true);
        m_bIsTreeDead = false;
    }
}