using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class F_HammermanCharacter : IBase_Friend_Character
{
    [SerializeField]
    protected MMFeedbacks m_stKnockFeedback;

    protected override void Awake()
    {
        base.Awake();

        m_stKnockFeedback?.Initialization();
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

    public void PlayKnockFeedback()
    {
        m_stKnockFeedback?.PlayFeedbacks();
    }

    public void StopKnockFeedback()
    {
        m_stKnockFeedback?.StopFeedbacks();
    }
}