using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using System;



public class E_MonsterFactory : IBase_Enemy_FactoryBuilding
{





    protected override void Awake()
    {
        base.Awake();

        GameCommon.CHECK(m_emBuildingType == EM_E_BuildingType.E_MonsterFactory);
    }

    public override void OnGameDate_IsDayComing()
    {
        base.OnGameDate_IsDayComing();
    }

    public override void OnGameDate_IsNightComing(bool bIsBloodNight, int nBloodNightIndex)
    {
        base.OnGameDate_IsNightComing(bIsBloodNight, nBloodNightIndex);

        int nDayId = Minos_GameDateManager.Instance.GetDayIndex();
        Minos_CTBLInfo.ST_EverydayEnemy stEnemy = Minos_CTBLInfo.Inst.GetEverydayEnemy(nDayId);
        if (stEnemy == null)
        {
            Debug.LogError("今晚没有怪物刷出来: DayId = " + nDayId);
            return;
        }

        if (m_coInstMoneterInNight != null)
        {
            StopCoroutine(m_coInstMoneterInNight);
            m_coInstMoneterInNight = null;
        }
        m_coInstMoneterInNight = CoInstMoneterInNight(stEnemy);
        StartCoroutine(m_coInstMoneterInNight);
    }














    IEnumerator m_coInstMoneterInNight;
    IEnumerator CoInstMoneterInNight(Minos_CTBLInfo.ST_EverydayEnemy stEnemy)
    {
        GameCommon.CHECK(stEnemy != null);

        foreach (Minos_CTBLInfo.ST_EverydayEnemy.ST_EnemyGroup _stGroup in stEnemy.lstEnemyInNight)
        {
            for (int i = 0; i < _stGroup.nCount; i++)
            {
                InstantiateCharacter(_stGroup.emType);
                yield return new WaitForSeconds(0.1f);
            }            
        }

        m_coInstMoneterInNight = null;
    }












    public void DebugInstantiateCharacter(int nCount)
    {
        if (m_coDebugInstCharacter != null)
        {
            StopCoroutine(m_coDebugInstCharacter);
        }
        m_coDebugInstCharacter = CoDebugInstCharacter(nCount);
        StartCoroutine(m_coDebugInstCharacter);
    }

    IEnumerator m_coDebugInstCharacter;
    IEnumerator CoDebugInstCharacter(int nCount)
    {
        for (int i = 0; i < nCount; i++)
        {
            InstantiateCharacter(EM_E_CharacterType.E_MonsterNearA);
            yield return new WaitForSeconds(0.2f);
            InstantiateCharacter(EM_E_CharacterType.E_MonsterNearA);
            yield return new WaitForSeconds(0.2f);
            InstantiateCharacter(EM_E_CharacterType.E_BossNearA);
            yield return new WaitForSeconds(0.2f);
            InstantiateCharacter(EM_E_CharacterType.E_BossNearB);
        }

        m_coDebugInstCharacter = null;
    }

    public void DebugInstantiateCharacter()
    {
        InstantiateCharacter(EM_E_CharacterType.E_MonsterNearA);
    }
}
