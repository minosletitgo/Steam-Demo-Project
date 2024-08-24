using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System;

public class Minos_AIDecisionNewPlayerOrderCreated : AIDecision
{
    [SerializeField]
    EM_F_AIActionOrderType m_emPlayerOrderType;



    protected override void Start()
    {
        base.Start();
    }

    public override bool Decide()
    {
        throw new NotImplementedException();
    }

    void OnNewPlayerOrder(EM_F_AIActionOrderType emOType, object objOData)
    {
        GameCommon.CHECK(emOType > EM_F_AIActionOrderType.Invalid && emOType < EM_F_AIActionOrderType.Max);
        GameCommon.CHECK(objOData != null);
    }
}