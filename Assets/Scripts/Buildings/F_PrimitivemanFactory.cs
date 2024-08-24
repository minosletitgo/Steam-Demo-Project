using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using System;

public class F_PrimitivemanFactory : IBase_Friend_FactoryBuilding
{
    [SerializeField]
    int m_nBornMaxCount = 3;
    
    [SerializeField]
    float m_fBornFrequency = 10f;
    float m_fLastBornUpdate = 0f;
        






    protected override void Awake()
    {
        m_emBuildingType = EM_F_BuildingType.F_PrimitivemanFactory;
        m_emBornCharacterType = EM_F_CharacterType.F_Primitiveman;

        base.Awake();
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
        Debug.LogError("PrimitivemanFactory.OnLogicTriggerEnter Is Not Support To !");
    }

    protected override void OnLogicTriggerExit(Collider other)
    {
        Debug.LogError("PrimitivemanFactory.OnLogicTriggerExit Is Not Support To !");
    }

    public override void OnMoneyCoinFinished()
    {
        Debug.LogError("PrimitivemanFactory.OnMoneyCoinFinished Is Not Support To !");
    }



    #region **********************************Factory****************************************
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
    #endregion **********************************Factory****************************************
}
