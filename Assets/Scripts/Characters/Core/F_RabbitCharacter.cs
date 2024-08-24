using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using System;

public class F_RabbitCharacter : IBase_Friend_Character
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

    protected override void OnHealthDead()
    {
        //base.OnHealthDead();

        int nThrowMoneyCoin = 1;
        for (int iCoin = 0; iCoin < nThrowMoneyCoin; iCoin++)
        {
            PickItem2MoneyCoin itemCoin = PickItem2MoneyCoin.InstancePickItem2MoneyCoin(
                "MoneyCoin_DropToGround", null, Vector3.zero, Quaternion.identity, Vector3.one);
            GameCommon.CHECK(itemCoin != null);
            itemCoin.SetConfigMoneyCoin(1);
            itemCoin.PlayDropEffect(
                transform.position,
                transform.position,
                0.7f,
                null
                );
        }

        F_RabbitFactory stFactory = GetParentFactory() as F_RabbitFactory;
        GameCommon.CHECK(stFactory != null);
        stFactory.DecreaseCharacter(GetOnlyId());
        stFactory.ResetLastBornUpdate();
    }
}
