using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_AIActionOrder2Villager : IBase_Friend_AIActionOrder
{
    public F_AIActionOrder2Villager(EM_F_AIActionOrderHandler emHandler) : base(emHandler)
    {

    }

    protected override IBase_Friend_Character FindIdleAICharacter(ST_F_AIActionOrder stOrder)
    {
        GameCommon.CHECK(stOrder != null);
        Vector3 v3BuildingPos = stOrder.GettTargetBuilding().transform.position;
        return Minos_VillagerFactory.Instance.FindIdleVillager(v3BuildingPos);
    }

    protected override void TryLinkOrder2TargetAIChar(ST_F_AIActionOrder stOrder, IBase_Friend_Character stIdleChar)
    {
        GameCommon.CHECK(stIdleChar != null);

        if (stOrder != null)
        {
            GameCommon.CHECK(stOrder.GettTargetBuilding() != null);

            switch (stOrder.GetOType())
            {
                case EM_F_AIActionOrderType.ProducingBowman:
                case EM_F_AIActionOrderType.ProducingHammerman:
                case EM_F_AIActionOrderType.ProducingFarmer:
                case EM_F_AIActionOrderType.ProducingNearWarriorA:
                case EM_F_AIActionOrderType.ProducingKnight:
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
}