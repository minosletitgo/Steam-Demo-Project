using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;

public class Minos_GameDateManager : MoreMountains.Tools.Singleton<Minos_GameDateManager>
{
    [ReadOnly]
    [SerializeField]
    float m_fTimeSinceLevelLoad;
    [ReadOnly]
    [SerializeField]
    float m_fTimeScale;

    [SerializeField]
    int m_nSecondsDefineIsDay = 200;
    [SerializeField]
    int m_nSecondsDefineIsNight = 160;
    [SerializeField]
    int m_nDaysDefineOneSeason = 14;
    [SerializeField]
    int m_nDaysDefineBloodNight = 7;

    public enum EM_Season
    {
        Invalid  = -1,
        Spring, Summer, Autumn, Winter,
        Max,
    }


    [ReadOnly]
    [SerializeField]
    int m_nDayIndex = 0;//Day真实索引
    public delegate void OnDayIndexChg(int nBeforeIndex, int nAfterIndex);
    public OnDayIndexChg m_dgOnDayIndexChg;

    [ReadOnly]
    [SerializeField]
    bool m_bIsDayOrNight;//是否是白天
    [ReadOnly]
    [SerializeField]
    int m_nBloodNightIndex = 0;//BloodNight索引
    public delegate void OnIsDayComing();
    public OnIsDayComing m_dgOnIsDayComing;
    public delegate void OnIsNightComing(bool bIsBloodNight, int nBloodNightIndex);
    public OnIsNightComing m_dgOnIsNightComing;
    
    [ReadOnly]
    [SerializeField]
    int m_nSeansonIndex = 0;//Season真实索引
    [ReadOnly]
    [SerializeField]
    EM_Season m_emSeansonIndex = EM_Season.Invalid;//Season显示索引
    public delegate void OnSeasonIndexChg(EM_Season emBefore, EM_Season emAfter);
    public OnSeasonIndexChg m_dgOnSeasonIndexChg;







    public Minos_GameDateManager()
    {
        m_nDayIndex = 0;
        m_bIsDayOrNight = true;
        m_nBloodNightIndex = 0;
        m_nSeansonIndex = 0;
        m_emSeansonIndex = EM_Season.Spring;        
    }
    
    private void Update()
    {
        m_fTimeSinceLevelLoad = Time.timeSinceLevelLoad;
        m_fTimeScale = Time.timeScale;

        //DayIndex
        int nTmpDayIndex = m_nDayIndex;
        {
            m_nDayIndex = (int)Time.timeSinceLevelLoad / GetOneDaySeconds();
            if (nTmpDayIndex != m_nDayIndex)
            {
                Invoke_OnDayIndexChg(nTmpDayIndex, m_nDayIndex);
            }
        }

        //IsDayOrNight
        bool bTmpIsDayOrNight = m_bIsDayOrNight;
        {
            float fSecondRemains = (float)Time.timeSinceLevelLoad % GetOneDaySeconds();
            if (fSecondRemains <= m_nSecondsDefineIsDay)
            {
                m_bIsDayOrNight = true;
            }
            else
            {
                m_bIsDayOrNight = false;
            }

            if (m_bIsDayOrNight)
            {
                //进入白天
                if (bTmpIsDayOrNight != m_bIsDayOrNight)
                {
                    Invoke_OnIsDayComing();
                }
            }
            else
            {
                //进入黑夜
                if (bTmpIsDayOrNight != m_bIsDayOrNight)
                {
                    int nTmpBloodNightIndex = m_nBloodNightIndex;
                    m_nBloodNightIndex = (int)(m_nDayIndex / m_nDaysDefineBloodNight);
                    if (nTmpBloodNightIndex != m_nBloodNightIndex)
                    {
                        Invoke_OnIsNightComing(true, m_nBloodNightIndex);
                    }
                    else
                    {
                        Invoke_OnIsNightComing(false, m_nBloodNightIndex);
                    }                    
                }
            }
        }

        //Season
        int nTmpSeasonIndex = m_nSeansonIndex;
        EM_Season emTmpSeasonIndex = m_emSeansonIndex;
        {
            m_nSeansonIndex = (int)(m_nDayIndex / m_nDaysDefineOneSeason);
            m_emSeansonIndex = (EM_Season)Mathf.Max((int)EM_Season.Spring, (int)m_nSeansonIndex);
            m_emSeansonIndex = (EM_Season)Mathf.Min((int)EM_Season.Winter, (int)m_nSeansonIndex);
            if (nTmpSeasonIndex != m_nSeansonIndex)
            {
                Invoke_OnSeasonIndexChg(emTmpSeasonIndex, m_emSeansonIndex);
            }
        }
    }



    int GetOneDaySeconds() { return m_nSecondsDefineIsDay + m_nSecondsDefineIsNight; }
    int GetOneYearDays() { return (int)(Minos_GameDateManager.EM_Season.Max) * m_nDaysDefineOneSeason; }
    public int GetDayIndex() { return m_nDayIndex; }
    public bool IsDayOrNight() { return m_bIsDayOrNight; }
    public int GetBloodNightIndex() { return m_nBloodNightIndex; }
    public EM_Season GetSeasonIndex() { return m_emSeansonIndex; }
    



    void Invoke_OnDayIndexChg(int nBeforeIndex, int nAfterIndex)
    {
        Debug.LogWarning(string.Format("OnDayIndexChg: 进入第{0}天", nAfterIndex));
        if (m_dgOnDayIndexChg != null)
        {
            m_dgOnDayIndexChg(nBeforeIndex, nAfterIndex);
        }
    }

    void Invoke_OnIsDayComing()
    {
        Debug.LogWarning("OnIsDayComing ");
        if (m_dgOnIsDayComing != null)
        {
            m_dgOnIsDayComing();
        }
    }

    void Invoke_OnIsNightComing(bool bIsBloodNight, int nBloodNightIndex)
    {
        Debug.LogWarning(string.Format("OnIsNightComing BloodNight:{0} , BloodNightIndex:{1}", bIsBloodNight, nBloodNightIndex));
        if (m_dgOnIsNightComing != null)
        {
            m_dgOnIsNightComing(bIsBloodNight, nBloodNightIndex);
        }
    }

    void Invoke_OnSeasonIndexChg(EM_Season emBefore, EM_Season emAfter)
    {
        Debug.LogWarning(string.Format("OnSeasonIndexChg: 进入{0}", emAfter.ToString()));
        if (m_dgOnSeasonIndexChg != null)
        {
            m_dgOnSeasonIndexChg(emBefore, emAfter);
        }
    }










    #region *******************************Debug********************************************
    public void DebugSet_OnDayIndexChg(int nDayIndex)
    {
        GameCommon.CHECK(nDayIndex >= 0);

        m_nDayIndex = nDayIndex;
        Invoke_OnDayIndexChg(0, nDayIndex);
    }

    public void DebugSet_OnIsDayComing()
    {
        m_bIsDayOrNight = true;
        Invoke_OnIsDayComing();
    }

    public void DebugSet_OnIsNightComing(bool bIsBloodNight, int nBloodNightIndex)
    {
        m_bIsDayOrNight = false;
        m_nBloodNightIndex = nBloodNightIndex;
        Invoke_OnIsNightComing(bIsBloodNight, nBloodNightIndex);
    }

    public void DebugSet_OnSeasonIndexChg(EM_Season emBefore, EM_Season emAfter)
    {
        GameCommon.CHECK(emBefore > EM_Season.Invalid && emBefore < EM_Season.Max);
        GameCommon.CHECK(emAfter > EM_Season.Invalid && emAfter < EM_Season.Max);

        m_emSeansonIndex = emAfter;
        Invoke_OnSeasonIndexChg(emBefore, emAfter);
    }
    #endregion *******************************Debug********************************************
}
