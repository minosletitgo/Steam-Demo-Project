using UnityEngine;
using System.Collections;

public class F_KnightFactory : IBase_Friend_FactoryBuilding
{
    [SerializeField]
    int m_nOneOrderCostMoneyCoin = 0;






    protected override void Awake()
    {
        m_emBuildingType = EM_F_BuildingType.F_KnightFactory;
        m_emBornCharacterType = EM_F_CharacterType.F_Knight;

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
                            if (stOrder.GetOType() == EM_F_AIActionOrderType.ProducingKnight)
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
            CreateOrder(EM_F_AIActionOrderType.ProducingKnight, this);
    }
}
