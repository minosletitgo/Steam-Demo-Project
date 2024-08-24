using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

public abstract class IBase_Friend_Building : MonoBehaviour
{
    [ReadOnly]
    [SerializeField]
    protected EM_F_BuildingType m_emBuildingType = EM_F_BuildingType.Invalid;
    
    [ReadOnly]
    [SerializeField]
    int m_nOnlyId;

    [ReadOnly]
    [SerializeField]
    protected GameObject m_goBuildingRoot;
    [ReadOnly]
    [SerializeField]
    protected GameObject m_goBuilding;

    [SerializeField]
    bool m_bIsLogicTrigger;
    [ReadOnly]
    [SerializeField]
    protected BuildingColliderTrigger m_stLogicTrigger;
    [ReadOnly]
    [SerializeField]
    GameObject m_goHealthTrigger;

    [SerializeField]    
    protected int m_nInitialLevel = 0;//0开始计数
    [SerializeField]
    [ReadOnly]
    protected int m_nCurLevel = 0;
    [SerializeField]
    [ReadOnly]
    protected int m_nConstMinLevel = 0;
    [SerializeField]
    [ReadOnly]
    protected int m_nConstMaxLevel = 0;
    [SerializeField]
    MMFeedbacks m_stLevUpEffect;

    [SerializeField]
    protected List<int> m_lstEveryLevUpCostMoneyCoin = new List<int>();
    [SerializeField]
    protected List<int> m_lstEveryLevHealth = new List<int>();
    [SerializeField]
    protected float m_fBuildingOrCuttingCostSecond;

    [SerializeField]
    protected Vector3 m_v3MoneyCoinBarOffset;

    [SerializeField]
    public List<MMPathMovementElement> m_lstPathAroundSelf;//借用MMPath映射成GameObject
    GameObject m_goPatrolRoot;
    [HideInInspector]
    public List<GameObject> m_lstPatrolSelf;


    [SerializeField]
    [ReadOnly]
    bool m_bIsSelfSendedAIActionOrder = false;

    //private stuff
    Minos_3DGUI_BuildingMoneyCoinBar m_st3DGUI_MoneyCoinBar;
    protected Health m_stHealth;
    MMHealthBar m_stMMHealthBar;




    protected virtual void Awake()
    {
        //Debug.Log(gameObject.name);
        GameCommon.CHECK(m_emBuildingType > EM_F_BuildingType.Invalid && m_emBuildingType < EM_F_BuildingType.Max);

        m_nOnlyId = ++m_nStaticBuildingId;

        m_goBuildingRoot = transform.Find("BuildingRoot").gameObject;
        GameCommon.CHECK(m_goBuildingRoot != null, "m_goBuildingRoot != null : " + gameObject.name);
        GameCommon.CHECK(m_nInitialLevel >= 0, "m_nInitialLevel >= 0 : " + gameObject.name);

        GameCommon.CHECK(m_lstEveryLevUpCostMoneyCoin.Count >= 0);
        GameCommon.CHECK(
            m_lstEveryLevHealth.Count == m_lstEveryLevUpCostMoneyCoin.Count,
            gameObject.name + " -> m_lstEveryLevHealth.Count == m_lstLevUpCostMoneyCoin.Count"
            );
        GameCommon.CHECK(m_fBuildingOrCuttingCostSecond >= 0);

        m_nConstMinLevel = 0;
        m_nConstMaxLevel = (m_lstEveryLevUpCostMoneyCoin.Count > 0) ? (m_lstEveryLevUpCostMoneyCoin.Count - 1) : 0;

        m_stLevUpEffect?.Initialization();

        m_goPatrolRoot = new GameObject("PatrolRoot");
        m_goPatrolRoot.transform.SetParent(transform);
        m_goPatrolRoot.transform.localPosition = Vector3.zero;
        m_goPatrolRoot.transform.localRotation = Quaternion.identity;
        m_goPatrolRoot.transform.localScale = Vector3.one;
        m_lstPatrolSelf = new List<GameObject>();
        for (int i = 0; i < m_lstPathAroundSelf.Count; i++)
        {
            GameObject _goPoint = new GameObject("PatrolPoint_" + i);
            _goPoint.transform.SetParent(m_goPatrolRoot.transform);
            _goPoint.transform.position = transform.TransformPoint(m_lstPathAroundSelf[i].PathElementPosition);
            _goPoint.transform.localRotation = Quaternion.identity;
            _goPoint.transform.localScale = Vector3.one;
            m_lstPatrolSelf.Add(_goPoint);
        }

        Minos_BuildingManager.Instance.Push_F_BuildingAtAwake(this);
    }

    protected virtual void Start()
    {
        if (m_goBuildingRoot.transform.childCount > 0)
        {
            m_goBuilding = m_goBuildingRoot.transform.GetChild(0).gameObject;
            GameCommon.CHECK(m_goBuilding != null);

            BindBoxTriggers();

            SetCurLevel(m_nInitialLevel);
        }
        else
        {
            BuildingLevChangeTo(m_nInitialLevel, true);
        }
    }

    public static int m_nStaticBuildingId = 10000;
    public int GetOnlyId() { return m_nOnlyId; }

    void BindBoxTriggers()
    {
        //逻辑触发器
        if (m_bIsLogicTrigger)
        {
            GameObject goTrigger = m_goBuilding.transform.Find("BoxLogicTrigger").gameObject;
            GameCommon.CHECK(goTrigger != null, "BoxLogicTrigger Is Failed !");
            m_stLogicTrigger = goTrigger.GetComponent<BuildingColliderTrigger>();
            GameCommon.CHECK(m_stLogicTrigger != null);
            m_stLogicTrigger.gameObject.layer = LayerMask.NameToLayer("New_BuildingsLogicTrigger");
            BoxCollider stCollider = m_stLogicTrigger.GetComponent<BoxCollider>();
            GameCommon.CHECK(stCollider != null);
            GameCommon.CHECK(stCollider.isTrigger);//必须是开启Trigger
            m_stLogicTrigger.m_dgOnTriggerEnter = OnLogicTriggerEnter;
            m_stLogicTrigger.m_dgOnTriggerExit = OnLogicTriggerExit;
        }
        
        //血条触发器
        {
            Transform trTrigger = m_goBuilding.transform.Find("BoxHealthTrigger");
            if (trTrigger != null)
            {
                m_goHealthTrigger = trTrigger.gameObject;
                BoxCollider stCollider = m_goHealthTrigger.GetComponent<BoxCollider>();
                GameCommon.CHECK(stCollider != null);
                GameCommon.CHECK(stCollider.isTrigger);//必须是开启Trigger
                m_stHealth = m_goHealthTrigger.GetComponent<Health>();
                GameCommon.CHECK(m_stHealth != null);
                m_stHealth.DestroyOnDeath = false;
                m_stHealth.OnDeath = OnHealthDead;
                m_stHealth.OnRevive = OnHealthRevive;
                m_stHealth.OnHit = OnHealthHit;
                m_stMMHealthBar = m_goHealthTrigger.GetComponent<MMHealthBar>();
                GameCommon.CHECK(m_stMMHealthBar != null);
                m_stMMHealthBar.AlwaysVisible = false;
                m_stMMHealthBar.HideBarAtZero = false;
            }
            else
            {
                m_goHealthTrigger = null;
                m_stHealth = null;
                m_stMMHealthBar = null;
            }
        }
    }

    protected virtual void SetCurLevel(int nLev)
    {
        GameCommon.CHECK(nLev >= m_nConstMinLevel && nLev <= m_nConstMaxLevel);

        m_nCurLevel = nLev;
        if (m_nConstMaxLevel > 0)
        {
            InitialHealthValue(GetLevHealth(m_nCurLevel));
        }
        else
        {
            InitialHealthValue(0);
        }
    }

    protected virtual void BuildingLevChangeTo(int nTargetLev, bool bIsStart)
    {
        //等级变更，可能升，也可能降
        GameCommon.CHECK(nTargetLev >= 0);

        //if (!bIsStart)
        //{
        //    GameCommon.CHECK(nTargetLev > m_nCurLevel);
        //}

        //容错模型找不到
        string strLoadPath = GameHelper_F_Building.GetBuildingModelName(m_emBuildingType, nTargetLev);
        Object objSrc = Resources.Load(strLoadPath);
        if (objSrc != null)
        {
            if (m_goBuilding != null)
            {
                Object.Destroy(m_goBuilding);
                m_goBuilding = null;
            }

            //GameCommon.CHECK(objSrc != null, "Resources.Load Failed: " + strLoadPath);
            m_goBuilding = (GameObject)Instantiate(objSrc);
            GameCommon.CHECK(m_goBuilding != null);
            m_goBuilding.name = GameHelper_F_Building.GetBuildingModelName(m_emBuildingType, nTargetLev, false);
            m_goBuilding.transform.SetParent(m_goBuildingRoot.transform);
            m_goBuilding.transform.localPosition = Vector3.zero;
            m_goBuilding.transform.rotation = Quaternion.identity;
            m_goBuilding.transform.localScale = Vector3.one;
        }
        else
        {
            Debug.LogWarning("Resources.Load Failed: " + strLoadPath);
        }
        
        BindBoxTriggers();

        SetCurLevel(nTargetLev);

        if (!bIsStart)
        {
            m_stLevUpEffect?.PlayFeedbacks();
        }
    }

    protected virtual bool CanLevUpToNext(out int nCostMoneyCoin)
    {
        GameCommon.CHECK(m_nCurLevel >= m_nConstMinLevel && m_nCurLevel <= m_nConstMaxLevel);

        if (m_nCurLevel == m_nConstMaxLevel)
        {
            nCostMoneyCoin = 0;
            return false;
        }

        nCostMoneyCoin = GetLevUpCostMoneyCoin(m_nCurLevel + 1);
        return true;
    }

    public virtual void BuildingLevUpToNext()
    {
        BuildingLevChangeTo(GetCurLev() + 1, false);
    }

    public virtual int GetLevUpCostMoneyCoin(int nLevTo)
    {
        GameCommon.CHECK(nLevTo >= m_nConstMinLevel && nLevTo <= m_nConstMaxLevel);
        GameCommon.CHECK(nLevTo >= 0 && nLevTo <= m_lstEveryLevUpCostMoneyCoin.Count);
        
        return m_lstEveryLevUpCostMoneyCoin[nLevTo];
    }

    void InitialHealthValue(int nValue)
    {
        GameCommon.CHECK(nValue >= 0);
        if (m_goHealthTrigger != null)
        {
            m_stHealth.InitialHealth = nValue;
            m_stHealth.MaximumHealth = nValue;
            m_stHealth.CurrentHealth = nValue;
        }
    }

    public virtual int GetLevHealth(int nLev)
    {
        GameCommon.CHECK(nLev >= m_nConstMinLevel && nLev <= m_nConstMaxLevel);
        GameCommon.CHECK(nLev >= 0 && nLev < m_lstEveryLevHealth.Count);
        return m_lstEveryLevHealth[nLev];
    }

    public float GetBuildingOrCuttingCostSecond() { return m_fBuildingOrCuttingCostSecond; }

    public Transform GetLogicTriggerTransform()
    {
        GameCommon.CHECK(m_bIsLogicTrigger);
        GameCommon.CHECK(m_stLogicTrigger != null);

        return m_stLogicTrigger.transform;
    }
    protected abstract void OnLogicTriggerEnter(Collider other);
    protected abstract void OnLogicTriggerExit(Collider other);

    public EM_F_BuildingType GetBuildingType() { return m_emBuildingType; }
    public int GetCurLev() { return m_nCurLevel; }

    public void SetIsSelfSendedAIActionOrder(bool bSended)
    {
        m_bIsSelfSendedAIActionOrder = bSended;
    }

    public bool IsSelfSendedAIActionOrder() { return m_bIsSelfSendedAIActionOrder; }

    protected virtual void ShowMoneyCoinBar(int nCostMoneyCoin, float fDelaySeconds, System.Action dgOnShow)
    {
        GameCommon.CHECK(nCostMoneyCoin > 0);
        
        if (m_coDelayShowMoneyCoinBar != null)
        {
            StopCoroutine(m_coDelayShowMoneyCoinBar);
            m_coDelayShowMoneyCoinBar = null;
        }
        m_coDelayShowMoneyCoinBar = CoDelayShowMoneyCoinBar(nCostMoneyCoin, fDelaySeconds, dgOnShow);
        StartCoroutine(m_coDelayShowMoneyCoinBar);
    }

    IEnumerator m_coDelayShowMoneyCoinBar;
    IEnumerator CoDelayShowMoneyCoinBar(int nCostMoneyCoin, float fDelaySeconds, System.Action dgOnShow)
    {
        yield return new WaitForSeconds(fDelaySeconds);

        if (m_st3DGUI_MoneyCoinBar == null)
        {
            string strLoadPath = string.Format("Prefabs/UI/Minos_3DGUI_BuildingMoneyCoinBar");
            Object objSrc = Resources.Load(strLoadPath);
            GameCommon.CHECK(objSrc != null, "Resources.Load Failed: " + strLoadPath);
            GameObject goInstant = (GameObject)Instantiate(objSrc, new Vector3(0, 0, 0), Quaternion.identity);
            GameCommon.CHECK(goInstant != null);
            goInstant.transform.SetParent(GameObject.Find("Canvas 3D").transform);
            goInstant.transform.position = m_stLogicTrigger.transform.position/* + m_stLogicTrigger.transform.up * 5.0f*/;
            goInstant.transform.rotation = Quaternion.identity;
            goInstant.transform.localScale = Vector3.one;
            m_st3DGUI_MoneyCoinBar = goInstant.GetComponent<Minos_3DGUI_BuildingMoneyCoinBar>();
        }

        GameCommon.CHECK(m_st3DGUI_MoneyCoinBar != null);

        MMFollowTarget stFollow = m_st3DGUI_MoneyCoinBar.GetComponent<MMFollowTarget>();
        GameCommon.CHECK(stFollow != null);
        stFollow.Target = m_stLogicTrigger.transform;
        stFollow.Offset = m_v3MoneyCoinBarOffset;
        m_st3DGUI_MoneyCoinBar.Show(nCostMoneyCoin);

        if (dgOnShow != null)
        {
            dgOnShow();
        }

        m_coDelayShowMoneyCoinBar = null;
    }

    protected virtual void UnShowMoneyCoinBar()
    {
        if (m_st3DGUI_MoneyCoinBar != null)
        {
            m_st3DGUI_MoneyCoinBar.UnShow();
        }

        if (m_coDelayShowMoneyCoinBar != null)
        {
            StopCoroutine(m_coDelayShowMoneyCoinBar);
            m_coDelayShowMoneyCoinBar = null;
        }
    }

    public Minos_3DGUI_BuildingMoneyCoinBar GetMoneyCoinBar() { return m_st3DGUI_MoneyCoinBar; }

    public abstract void OnMoneyCoinFinished();

    public virtual void OnGameDate_IsDayComing() { }
    public virtual void OnGameDate_IsNightComing(bool bIsBloodNight, int nBloodNightIndex) { }


    #region *********************************************Health****************************************************
    public bool ShowHealthBar(bool bShow)
    {
        if (m_stMMHealthBar != null)
        {
            m_stMMHealthBar.AlwaysVisible = bShow;
            return true;
        }
        return false;
    }

    protected virtual void OnHealthDead()
    {
        //Debug.Log(gameObject.name + " -> OnHealthDead");
    }

    protected virtual void OnHealthRevive()
    {
        Debug.Log(gameObject.name + " -> OnHealthRevive");
    }

    protected virtual void OnHealthHit()
    {
        Debug.Log(gameObject.name + " -> OnHealthHit");
        GameCommon.CHECK(m_stHealth != null);
        F_AIActionOrderManager.Instance.GetOrderHandler(EM_F_AIActionOrderHandler.Hammerman).CreateOrder(
            EM_F_AIActionOrderType.RepairingBuilding, this);
    }

    public void ReviveHealth()
    {
        if (m_stHealth != null)
        {
            m_stHealth.Revive();
            m_stHealth.gameObject.SetActive(true);
        }
    }

    public void RecoverHealth(int nValueDelta)
    {
        GameCommon.CHECK(nValueDelta > 0);
        if (m_stHealth != null)
        {
            int nValueOld = m_stHealth.CurrentHealth;
            if (m_stHealth.CurrentHealth <= 0)
            {
                m_stHealth.Revive();
                m_stHealth.gameObject.SetActive(true);
            }

            m_stHealth.CurrentHealth = Mathf.Min(nValueOld + nValueDelta, m_stHealth.MaximumHealth);
        }
    }

    public bool IsHealthDamaged()
    {
        if (m_stHealth != null)
        {
            if (m_stHealth.CurrentHealth < m_stHealth.InitialHealth)
            {
                return true;
            }
            return false;
        }
        return false;
    }

    public void DebugDamageHealth(int nValue)
    {
        if (m_stHealth != null)
        {
            m_stHealth.Damage(nValue, null, 0, 0);
        }
    }
    #endregion *********************************************Health****************************************************
}
