using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;


public class E_MonsterNearCharacter : IBase_Enemy_Character
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
        Minos_CTBLInfo.ST_E_CharacterAttr stAttr = Minos_CTBLInfo.Inst.GetE_CharacterAttr(m_emCharType, m_nLv);
        GameCommon.CHECK(stAttr != null);

        m_stHealth.InitialHealth = (int)stAttr.GetAttr(EM_E_CharacterAttr.Hp);
        m_stHealth.MaximumHealth = (int)stAttr.GetAttr(EM_E_CharacterAttr.Hp);
        
        m_stTDCharMovement.WalkSpeed = (float)stAttr.GetAttr(EM_E_CharacterAttr.WalkSpeed);
        if (m_stTDCharRun != null)
        {
            m_stTDCharRun.RunSpeed = (float)stAttr.GetAttr(EM_E_CharacterAttr.RunSpeed);
        }

        Minos_MeleeWeapon stWeapon = m_stHandleWeapon.CurrentWeapon as Minos_MeleeWeapon;
        GameCommon.CHECK(stWeapon != null);
        stWeapon.DamageCaused = (int)stAttr.GetAttr(EM_E_CharacterAttr.Damage);
        stWeapon.ReloadTime = (float)stAttr.GetAttr(EM_E_CharacterAttr.DamageDelay);
    }

    protected override void OnHealthDestroyObject()
    {
        base.OnHealthDestroyObject();

        GetParentFactory().DecreaseCharacter(GetOnlyId());
    }
}
