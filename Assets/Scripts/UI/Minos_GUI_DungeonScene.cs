using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class Minos_GUI_DungeonScene : MoreMountains.Tools.Singleton<Minos_GUI_DungeonScene>,
                                      MMEventListener<TopDownEngineEvent>
{
    [SerializeField]
    Text m_txtCurrentHealth;
    
    
    [SerializeField]
    Text m_txtGameDateDayIndex;
    [SerializeField]
    Text m_txtGameDateIsDayOrNight;
    [SerializeField]
    Text m_txtGameDateSeasonIndex;

    [SerializeField]
    Minos_GUI_PauseWindow m_stPauseWindow;


    protected override void Awake()
    {
        base.Awake();

        Minos_GameDateManager.Instance.m_dgOnDayIndexChg += OnGameDateDayIndexChg;
        Minos_GameDateManager.Instance.m_dgOnIsDayComing += OnGameDateIsDayComing;
        Minos_GameDateManager.Instance.m_dgOnIsNightComing += OnGameDateIsNightComing;
        Minos_GameDateManager.Instance.m_dgOnSeasonIndexChg += OnGameDateSeasonIndexChg;

        SetGameDateDayIndex(Minos_GameDateManager.Instance.GetDayIndex());
        SetGameDateIsDayComing();
        SetGameDateSeasonIndex(Minos_GameDateManager.Instance.GetSeasonIndex());

        m_stPauseWindow.gameObject.SetActive(false);
    }

    private void Start()
    {
        Player.Inst.m_dgOnCurrentHealthChg += OnCurrentHealthChg;
        SetCurrentHealth(0, Player.Inst.GetInitialHealth());
    }

    private void OnDestroy()
    {
        ////析构顺序还比较乱，先不写析构
        //if (Player.Instance != null)
        //{
        //    Player.Instance.m_dgOnCurMoneyCoinChg -= OnCurMoneyCoinChg;
        //}

        //if (Minos_GameDateManager.Instance != null)
        //{
        //    Minos_GameDateManager.Instance.m_dgOnDayIndexChg -= OnGameDateDayIndexChg;
        //    Minos_GameDateManager.Instance.m_dgOnIsDayOrNightChg -= OnGameDateIsDayOrNightChg;
        //}
    }
    
    void OnEnable()
    {
        this.MMEventStartListening<TopDownEngineEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<TopDownEngineEvent>();
    }

    public void OnMMEvent(TopDownEngineEvent gameEvent)
    {
        Debug.Log("TopDownEngineEvent -> " + gameEvent.EventType.ToString());
        switch (gameEvent.EventType)
        {
            case TopDownEngineEventTypes.Pause:
                {
                    if (Time.timeScale > 0.0f)
                    {
                        Debug.LogWarning("Show Pause Menu");
                        m_stPauseWindow.gameObject.SetActive(true);
                    }
                    else
                    {
                        Debug.LogWarning("UnShow Pause Menu");
                        m_stPauseWindow.gameObject.SetActive(false);
                    }
                }
                break;
            case TopDownEngineEventTypes.PlayerDeath:
                {
                    TopDownEngineEvent.Trigger(TopDownEngineEventTypes.Pause, null);
                }
                break;
        }
    }



    void OnCurrentHealthChg(int nBefore, int nAfter)
    {
        SetCurrentHealth(nBefore, nAfter);
    }

    void SetCurrentHealth(int nBefore, int nAfter)
    {
        m_txtCurrentHealth.text = "CurrentHealth: " + nAfter.ToString();
    }





    void OnGameDateDayIndexChg(int nBefore, int nAfter)
    {
        SetGameDateDayIndex(nAfter);
    }

    void OnGameDateIsDayComing()
    {
        SetGameDateIsDayComing();
    }

    void OnGameDateIsNightComing(bool bIsBloodNight, int nBloodNightIndex)
    {
        SetGameDateIsNightComing(bIsBloodNight, nBloodNightIndex);
    }

    void OnGameDateSeasonIndexChg(Minos_GameDateManager.EM_Season emBefore, Minos_GameDateManager.EM_Season emAfter)
    {
        SetGameDateSeasonIndex(emAfter);
    }




    void SetGameDateDayIndex(int nDayIndex)
    {
        m_txtGameDateDayIndex.text = string.Format("DayIndex: {0}", nDayIndex);
    }

    void SetGameDateIsDayComing()
    {
        m_txtGameDateIsDayOrNight.text = string.Format("Now Is Day ");
    }

    void SetGameDateIsNightComing(bool bIsBloodNight, int nBloodNightIndex)
    {
        m_txtGameDateIsDayOrNight.text = string.Format("Now Is Night , BloodNight: {0}, {1}", bIsBloodNight, nBloodNightIndex);
    }
    
    void SetGameDateSeasonIndex(Minos_GameDateManager.EM_Season emSeasonIndex)
    {
        m_txtGameDateSeasonIndex.text = string.Format("SeasonIndex: {0}", emSeasonIndex.ToString());
    }
}
