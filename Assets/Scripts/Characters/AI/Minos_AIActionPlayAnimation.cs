using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class Minos_AIActionPlayAnimation : AIAction
{
    [SerializeField]
    string m_strAnimName;
    [SerializeField]
    bool m_bAnimValue;
    [SerializeField]
    bool m_bIsWhenExitToReverseAnimValue = false;

    [Header("PlayFrequencies")]
    [SerializeField]
    float m_fPlayFrequency = 1f;
    float m_fLastPlayUpdate = 0f;


    //private stuff
    protected Character m_stChar;
    protected Animator m_stAnim;


    protected override void Initialization()
    {
        m_stChar = this.gameObject.GetComponent<Character>();
        GameCommon.CHECK(m_stChar != null);
        GameCommon.CHECK(m_stChar._animator != null);
    }

    public override void PerformAction()
    {
        if (string.IsNullOrWhiteSpace(m_strAnimName))
        {
            return;
        }

        if (Time.time - m_fLastPlayUpdate > m_fPlayFrequency)
        {
            m_stChar._animator.SetBool(m_strAnimName, m_bAnimValue);
            m_fLastPlayUpdate = Time.time;
        }       
    }

    public override void OnExitState()
    {
        base.OnExitState();

        if(m_bIsWhenExitToReverseAnimValue)
        {
            m_stChar._animator.SetBool(m_strAnimName, !m_bAnimValue);
        }
    }
}
