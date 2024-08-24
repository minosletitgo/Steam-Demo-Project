using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using System;

public class F_RabbitFactory : IBase_Friend_FactoryBuilding
{
    [SerializeField]
    int m_nBornMaxCount = 3;

    [SerializeField]
    float m_fBornFrequency = 10f;
    float m_fLastBornUpdate = 0f;





    protected override void Awake()
    {
        m_emBuildingType = EM_F_BuildingType.F_RabbitFactory;
        m_emBornCharacterType = EM_F_CharacterType.F_Rabbit;

        base.Awake();
    }

    protected override void OnLogicTriggerEnter(Collider other)
    {
        Debug.LogError("F_RabbitFactory.OnLogicTriggerEnter Is Not Support To !");
    }

    protected override void OnLogicTriggerExit(Collider other)
    {
        Debug.LogError("F_RabbitFactory.OnLogicTriggerExit Is Not Support To !");
    }

    public override void OnMoneyCoinFinished()
    {
        Debug.LogError("F_RabbitFactory.OnMoneyCoinFinished Is Not Support To !");
    }


    private void Update()
    {
        if (m_lstPathAroundSelf == null || m_lstPathAroundSelf.Count < 1)
        {
            // if the path is null we exit
            return;
        }

        if (Time.time > 0 && m_mapCharStorage.Count < m_nBornMaxCount)
        {
            if (m_fLastBornUpdate <= 0 || Time.time - m_fLastBornUpdate > m_fBornFrequency)
            {
                InstantiateCharacter();
                m_fLastBornUpdate = Time.time;
            }
        }
    }

    public void ResetLastBornUpdate()
    {
        m_fLastBornUpdate = Time.time;
    }
}
