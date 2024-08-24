using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

/*
    01.所有在【IBase_Friend_FactoryBuilding】出生的角色，都是由其【BornCharacterLev】来决定【Lv】
    02.而【BornCharacterLev】，由【F_CharacterAttr.txt】决定，如果获取失败，则【Lv】 = 0
    03.例如：【BowmanCharacter】有【1、2、3级】，那么【TotemBuilding】的作用就是提升【BornCharacterLev】
    04.即【TotemBuilding】的【Lv】单向等于【BornCharacterLev】
*/

public abstract class IBase_Friend_TotemBuilding : IBase_Friend_Building
{
    [ReadOnly]
    [SerializeField]
    protected EM_F_BuildingType m_emLinkFactoryType = EM_F_BuildingType.Invalid;

    [ReadOnly]
    [SerializeField]
    protected EM_F_CharacterType m_emLinkCharacterType = EM_F_CharacterType.Invalid;

    [SerializeField]
    protected int m_nEveryLevUpCostMoneyCoin = 6;





    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        GameCommon.CHECK(m_emLinkFactoryType > EM_F_BuildingType.Invalid && m_emLinkFactoryType < EM_F_BuildingType.Max);
        GameCommon.CHECK(m_emLinkCharacterType > EM_F_CharacterType.Invalid && m_emLinkCharacterType < EM_F_CharacterType.Max);

        m_nConstMinLevel = Minos_CTBLInfo.Inst.GetF_CharacterAttr_MinLv(m_emLinkCharacterType);
        m_nConstMaxLevel = Minos_CTBLInfo.Inst.GetF_CharacterAttr_MaxLv(m_emLinkCharacterType);
        GameCommon.CHECK(m_nConstMinLevel > 0);
        GameCommon.CHECK(m_nConstMinLevel <= m_nConstMaxLevel);
        m_nInitialLevel = m_nConstMinLevel;

        SetCurLevel(m_nInitialLevel);
    }

    protected override void SetCurLevel(int nLev)
    {
        //base.SetCurLevel(nLev);
        GameCommon.CHECK(nLev >= m_nConstMinLevel && nLev <= m_nConstMaxLevel);
        m_nCurLevel = nLev;
    }

    public override int GetLevUpCostMoneyCoin(int nLevTo)
    {
        GameCommon.CHECK(nLevTo >= m_nConstMinLevel && nLevTo <= m_nConstMaxLevel);
        return m_nEveryLevUpCostMoneyCoin;
    }

    public override void OnMoneyCoinFinished()
    {
        foreach (IBase_Friend_Building _stBuilding in Minos_BuildingManager.Instance.EnumAll_F_Building())
        {
            if (_stBuilding.GetBuildingType() != m_emLinkFactoryType)
            {
                continue;
            }

            IBase_Friend_FactoryBuilding stFactoryBuilding = _stBuilding as IBase_Friend_FactoryBuilding;
            GameCommon.CHECK(stFactoryBuilding != null);
            stFactoryBuilding.UpgradeBornCharacterLev();
        }

        SetCurLevel(m_nCurLevel + 1);
    }
}
