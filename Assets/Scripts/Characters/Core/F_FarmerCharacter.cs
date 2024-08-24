﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class F_FarmerCharacter : IBase_Friend_Character
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
        Minos_CTBLInfo.ST_F_CharacterAttr stAttr = Minos_CTBLInfo.Inst.GetF_CharacterAttr(m_emCharType, m_nLv);
        GameCommon.CHECK(stAttr != null);

        m_stHealth.InitialHealth = (int)stAttr.GetAttr(EM_F_CharacterAttr.Hp);
        m_stHealth.MaximumHealth = (int)stAttr.GetAttr(EM_F_CharacterAttr.Hp);

        m_stTDCharMovement.WalkSpeed = (float)stAttr.GetAttr(EM_F_CharacterAttr.WalkSpeed);
        if (m_stTDCharRun != null)
        { 
            m_stTDCharRun.RunSpeed = (float)stAttr.GetAttr(EM_F_CharacterAttr.RunSpeed); 
        }
    }

    public override void OnGameDate_IsDayComing()
    {
        base.OnGameDate_IsDayComing();
    }

    public override void OnGameDate_IsNightComing(bool bIsBloodNight, int nBloodNightIndex)
    {
        base.OnGameDate_IsNightComing(bIsBloodNight, nBloodNightIndex);
    }
}
