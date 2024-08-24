using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class F_KnightCharacter : IBase_Friend_Character
{
    [SerializeField]
    [ReadOnly]
    CharacterHandleWeapon m_stHandleWeapon;







    protected override void Awake()
    {
        base.Awake();

        m_stHandleWeapon = gameObject.GetComponent<CharacterHandleWeapon>();
        GameCommon.CHECK(m_stHandleWeapon != null);
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

        MeleeWeapon stWeapon = m_stHandleWeapon.CurrentWeapon as MeleeWeapon;
        GameCommon.CHECK(stWeapon != null);
        stWeapon.DamageCaused = (int)stAttr.GetAttr(EM_F_CharacterAttr.Damage);
        stWeapon.ReloadTime = (float)stAttr.GetAttr(EM_F_CharacterAttr.DamageDelay);

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
