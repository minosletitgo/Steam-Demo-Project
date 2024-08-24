using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

public abstract class IBase_Enemy_Building : MonoBehaviour
{
    [SerializeField]
    protected EM_E_BuildingType m_emBuildingType = EM_E_BuildingType.Invalid;

    [SerializeField]
    [ReadOnly]
    int m_nOnlyId;

    [ReadOnly]
    [SerializeField]
    protected GameObject m_goBuildingRoot;
    [ReadOnly]
    [SerializeField]
    protected GameObject m_goBuilding;

    [ReadOnly]
    [SerializeField]
    GameObject m_goHealthTrigger;

    [SerializeField]
    int m_nInitialLevel = 0;//0开始计数
    [SerializeField]
    [ReadOnly]
    int m_nCurLevel = 0;
    [SerializeField]
    [ReadOnly]
    int m_nConstMinLevel = 0;
    [SerializeField]
    [ReadOnly]
    int m_nConstMaxLevel = 0;

    [SerializeField]
    protected List<int> m_lstEveryLevHealth = new List<int>();

    [SerializeField]
    public List<MMPathMovementElement> m_lstPathAroundSelf;//借用MMPath映射成GameObject
    GameObject m_goPatrolRoot;
    [HideInInspector]
    public List<GameObject> m_lstPatrolSelf;



    protected Health m_stHealth;





    protected virtual void Awake()
    {
        GameCommon.CHECK(m_emBuildingType > EM_E_BuildingType.Invalid && m_emBuildingType < EM_E_BuildingType.Max);

        m_nOnlyId = ++m_nStaticBuildingId;

        m_goBuildingRoot = transform.Find("BuildingRoot").gameObject;
        GameCommon.CHECK(m_goBuildingRoot != null, "m_goBuildingRoot != null : " + gameObject.name);
        GameCommon.CHECK(m_nInitialLevel >= 0, "m_nInitialLevel >= 0 : " + gameObject.name);

        GameCommon.CHECK(m_lstEveryLevHealth.Count >= 0);

        m_nConstMinLevel = 0;
        m_nConstMaxLevel = (m_lstEveryLevHealth.Count > 0) ? (m_lstEveryLevHealth.Count - 1) : 0;

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

        Minos_BuildingManager.Instance.Push_E_BuildingAtAwake(this);
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
            }
            else
            {
                m_goHealthTrigger = null;
                m_stHealth = null;
            }
        }
    }

    void SetCurLevel(int nLev)
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

    protected void BuildingLevChangeTo(int nTargetLev, bool bIsStart)
    {
        //等级变更，可能升，也可能降
        GameCommon.CHECK(nTargetLev >= 0);

        //if (!bIsStart)
        //{
        //    GameCommon.CHECK(nTargetLev > m_nCurLevel);
        //}

        //容错模型找不到
        string strLoadPath = GameHelper_E_Building.GetBuildingModelName(m_emBuildingType, nTargetLev);
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
            m_goBuilding.name = GameHelper_E_Building.GetBuildingModelName(m_emBuildingType, nTargetLev, false);
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
    }




    void InitialHealthValue(int nValue)
    {
        GameCommon.CHECK(nValue >= 0);
        //if (m_goHealthTrigger != null)
        //{
        //    m_stHealth.InitialHealth = nValue;
        //    m_stHealth.MaximumHealth = nValue;
        //    m_stHealth.CurrentHealth = nValue;
        //}
    }

    public virtual int GetLevHealth(int nLev)
    {
        GameCommon.CHECK(nLev >= m_nConstMinLevel && nLev <= m_nConstMaxLevel);
        GameCommon.CHECK(nLev >= 0 && nLev < m_lstEveryLevHealth.Count);
        return m_lstEveryLevHealth[nLev];
    }



    public EM_E_BuildingType GetBuildingType() { return m_emBuildingType; }
    public int GetCurLev() { return m_nCurLevel; }

    public virtual void OnGameDate_IsDayComing() { }
    public virtual void OnGameDate_IsNightComing(bool bIsBloodNight, int nBloodNightIndex) { }


    #region *********************************************Health****************************************************
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
    }
    #endregion *********************************************Health****************************************************
}
