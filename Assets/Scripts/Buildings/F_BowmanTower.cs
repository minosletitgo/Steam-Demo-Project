using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using System;


public class F_BowmanTower : IBase_Friend_Building
{




    protected override void Awake()
    {
        m_emBuildingType = EM_F_BuildingType.F_BowmanTower;

        base.Awake();        
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

        //Debug.Log("Homeland OnTriggerEnter Success : " + other.name);

        Player stPlayer = other.GetComponent<Player>();
        if (stPlayer != null && stPlayer.IsCanInteractiveWithBuilding(m_emBuildingType))
        {
            if (GetCurLev() > 0)
            {
                GameCommon.CHECK(base.ShowHealthBar(true));
            }

            if (!IsSelfSendedAIActionOrder())
            {
                int nCostMoneyCoin = 0;
                if (CanLevUpToNext(out nCostMoneyCoin))
                {
                    base.ShowMoneyCoinBar(
                        nCostMoneyCoin, 0.0f,
                        delegate()
                        {
                            stPlayer.DoUnderBuilding(GetBuildingType(), gameObject);
                        }
                        );                    
                }
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
            base.ShowHealthBar(false);
            return;
        }
    }

    public override void OnMoneyCoinFinished()
    {
        F_AIActionOrderManager.Instance.GetOrderHandler(EM_F_AIActionOrderHandler.Hammerman).CreateOrder(
            EM_F_AIActionOrderType.LevUpBuilding, this);
    }

    protected override void OnHealthDead()
    {
        base.OnHealthDead();

        BuildingLevChangeTo(0, false);
    }
}
