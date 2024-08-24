using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using System;

public class F_Farmland : IBase_Friend_Building
{
    [SerializeField]
    int m_nOneOrderCostMoneyCoin = 0;

    [SerializeField]
    int m_nHarvestNeedTimes = 2;//耕作2次(天)后，即第三天早上产生金币
    [SerializeField]
    int m_nHarvestGetMoneyCoin = 4;//产生的金币数量
    
    [SerializeField]
    [ReadOnly]
    int m_nCurFarmingTimes;
    [SerializeField]
    [ReadOnly]
    bool m_bIsHarvest = false;



    protected override void Awake()
    {
        m_emBuildingType = EM_F_BuildingType.F_Farmland;

        base.Awake();
        
        GameCommon.CHECK(m_nOneOrderCostMoneyCoin > 0);
        GameCommon.CHECK(m_nHarvestNeedTimes > 0);
        GameCommon.CHECK(m_nHarvestGetMoneyCoin > 0);

        m_nCurFarmingTimes = 0;
        m_bIsHarvest = false;
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
        if (stPlayer != null && 
            stPlayer.IsCanInteractiveWithBuilding(m_emBuildingType) &&
            !IsSelfSendedAIActionOrder()
            )
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
                case EM_F_CharacterType.F_Farmer:
                    {
                        object objOrder = stChar.GetAIActionOrder();
                        if (objOrder != null)
                        {
                            ST_F_AIActionOrder stOrder = objOrder as ST_F_AIActionOrder;
                            GameCommon.CHECK(stOrder != null);
                            GameCommon.CHECK(stOrder.GettTargetBuilding() != null);
                            if (stOrder.GettTargetBuilding() == this)
                            {
                                if (stOrder.GetOType() == EM_F_AIActionOrderType.Farming)
                                {
                                    if (m_bIsHarvest)
                                    {
                                        stChar.EatMoneyCoin((int)stChar.GetAttr(EM_F_CharacterAttr.Income));

                                        m_bIsHarvest = false;
                                    }
                                }
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
        F_AIActionOrderManager.Instance.GetOrderHandler(EM_F_AIActionOrderHandler.Farmer).
            CreateOrder(EM_F_AIActionOrderType.Farming, this);
    }

    public void IncreaseFarmingTimes()
    {
        GameCommon.CHECK(m_nCurFarmingTimes >= 0 && m_nCurFarmingTimes < m_nHarvestNeedTimes);
        m_nCurFarmingTimes++;
    }

    public override void OnGameDate_IsDayComing()
    {
        base.OnGameDate_IsDayComing();

        GameCommon.CHECK(m_nCurFarmingTimes >= 0 && m_nCurFarmingTimes <= m_nHarvestNeedTimes);
        
        if (m_nCurFarmingTimes > 0)
        {
            if (m_nCurFarmingTimes == m_nHarvestNeedTimes)
            {
                m_nCurFarmingTimes = 0;

                //Debug.LogWarning("农田收成金币咯！");
                ////从农田中心向Trigger方向(且随机角度)扔金币
                //for (int iCoin = 0; iCoin < m_nHarvestGetMoneyCoin; iCoin++)
                //{
                //    Vector3 v3DirToTrigger = m_stLogicTrigger.transform.position - transform.position;
                //    Vector3 v3TargetPosOffset = GameHelper.RotateVector3InRandom(v3DirToTrigger, transform.up, -90.0f, 90.0f);
                //    v3TargetPosOffset = v3TargetPosOffset.normalized * UnityEngine.Random.Range(1.5f, 3.5f);

                //    PickItem2MoneyCoin itemCoin = PickItem2MoneyCoin.InstancePickItem2MoneyCoin(
                //        "MoneyCoin_DropToGround", null, Vector3.zero, Quaternion.identity, Vector3.one);
                //    GameCommon.CHECK(itemCoin != null);
                //    itemCoin.SetConfigMoneyCoin(1);
                //    itemCoin.PlayDropEffect(
                //        transform.position + 1.0f * v3DirToTrigger.normalized * 1.0f,
                //        transform.position + v3TargetPosOffset,
                //        0.7f,
                //        null
                //        );
                //}

                //2019.12.25 改为农民来收成
                m_bIsHarvest = true;
            }

            if (Minos_GameDateManager.Instance.GetSeasonIndex() != Minos_GameDateManager.EM_Season.Winter)
            {
                //冬天不能开耕
                F_AIActionOrderManager.Instance.GetOrderHandler(EM_F_AIActionOrderHandler.Farmer).
                    CreateOrder(EM_F_AIActionOrderType.Farming, this);
            }
        }
    }

    public override void OnGameDate_IsNightComing(bool bIsBloodNight, int nBloodNightIndex)
    {
        base.OnGameDate_IsNightComing(bIsBloodNight, nBloodNightIndex);
    }
}
