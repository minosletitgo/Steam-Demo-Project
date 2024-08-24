using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using System;

public class F_NearWarriorATotem : IBase_Friend_TotemBuilding
{
    protected override void Awake()
    {
        m_emBuildingType = EM_F_BuildingType.F_NearWarriorATotem;
        m_emLinkFactoryType = EM_F_BuildingType.F_NearWarriorAFactory;
        m_emLinkCharacterType = EM_F_CharacterType.F_NearWarriorA;

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

        Player stPlayer = other.GetComponent<Player>();
        if (stPlayer != null && stPlayer.IsCanInteractiveWithBuilding(m_emBuildingType))
        {
            int nCostMoneyCoin = 0;
            if (CanLevUpToNext(out nCostMoneyCoin))
            {
                base.ShowMoneyCoinBar(
                    nCostMoneyCoin, 0.0f,
                    delegate ()
                    {
                        stPlayer.DoUnderBuilding(GetBuildingType(), gameObject);
                    }
                    );
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
}
