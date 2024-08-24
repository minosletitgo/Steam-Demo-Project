using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using System;

public class F_BowmanFactory : IBase_Friend_FactoryBuilding
{
    [SerializeField]
    int m_nOneOrderCostMoneyCoin = 0;
    







    protected override void Awake()
    {
        m_emBuildingType = EM_F_BuildingType.F_BowmanFactory;
        m_emBornCharacterType = EM_F_CharacterType.F_Bowman;

        base.Awake();
        
        GameCommon.CHECK(m_nOneOrderCostMoneyCoin > 0);
    }

    protected override bool CanLevUpToNext(out int nCostMoneyCoin)
    {
        Debug.LogError(gameObject.name + " CanLevUpToNext Is Not Support To !");
        nCostMoneyCoin = 0;
        return false;
    }

    public override void BuildingLevUpToNext()
    {
        Debug.LogError(gameObject.name + " BuildingLevUpToNext Is Not Support To !");
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
            base.ShowMoneyCoinBar(
                m_nOneOrderCostMoneyCoin, 0.0f,
                delegate ()
                {
                    stPlayer.DoUnderBuilding(GetBuildingType(), gameObject);
                }
                );
            return;
        }

        IBase_Friend_Character stChar = other.GetComponent<IBase_Friend_Character>();
        if (stChar != null)
        {
            switch (stChar.GetCharType())
            {
                case EM_F_CharacterType.F_Villager:
                    {
                        object objOrder = stChar.GetAIActionOrder();
                        if (objOrder != null)
                        {
                            ST_F_AIActionOrder stOrder = objOrder as ST_F_AIActionOrder;
                            GameCommon.CHECK(stOrder != null);
                            GameCommon.CHECK(stOrder.GettTargetBuilding() != null);
                            if (stOrder.GetOType() == EM_F_AIActionOrderType.ProducingBowman)
                            {
                                Minos_VillagerFactory.Instance.DecreaseVillager(stChar.GetOnlyId());

                                InstantiateCharacter();
                            }
                        }
                    }
                    break;
            }
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
        F_AIActionOrderManager.Instance.GetOrderHandler(EM_F_AIActionOrderHandler.Villager).
            CreateOrder(EM_F_AIActionOrderType.ProducingBowman, this);
    }

    public override IBase_Friend_Character InstantiateCharacter()
    {
        //着实有点绕，感觉不合理
        IBase_Friend_Character stChar = base.InstantiateCharacter();
        GameCommon.CHECK(stChar != null);

        F_BowmanCharacter stBowmanChar = stChar as F_BowmanCharacter;
        //自动调整最后一个Char的防守位
        GameCommon.CHECK(stBowmanChar != null);
        GameCommon.CHECK(m_mapCharStorage.Count > 0);

        if (m_mapCharStorage.Count > 1)
        {
            int nCountL = 0;
            int nCountR = 0;
            foreach (KeyValuePair<int, IBase_Friend_Character> infoPair in m_mapCharStorage)
            {
                F_BowmanCharacter _stChar = infoPair.Value as F_BowmanCharacter;
                GameCommon.CHECK(_stChar != null);

                if (stBowmanChar.GetOnlyId() == _stChar.GetOnlyId())
                {
                    continue;
                }

                if (_stChar.IsDefendSideLeft())
                {
                    nCountL++;
                }
                else
                {
                    nCountR++;
                }
            }

            if (nCountL > nCountR)
            {
                stBowmanChar.SetDefendSideLeft(false);
            }
            else
            {
                stBowmanChar.SetDefendSideLeft(true);
            }
        }
        else
        {
            stBowmanChar.SetDefendSideLeft(true);
        }
        
        //放弃使用父建筑的巡逻路点
        if (Minos_GameDateManager.Instance.IsDayOrNight())
        {
            //白天：完全使用【F_Homeland+F_Wall】的防御路点
            stBowmanChar.FillPatrolPath_Homeland();
        }
        else
        {
            //白天：完全使用【F_Wall】的蹲守路点
            stBowmanChar.FillPatrolPath_Wall();
        }

        return stChar;
    }
}