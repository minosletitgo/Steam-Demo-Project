using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class F_Wall : IBase_Friend_Building
{
    [SerializeField]
    public List<MMPathMovementElement> m_lstPathPatrolLeft;
    GameObject m_goPatrolLeftRoot;
    [HideInInspector]
    public List<GameObject> m_lstPatrolLeft;

    [SerializeField]
    public List<MMPathMovementElement> m_lstPathPatrolRight;
    GameObject m_goPatrolRightRoot;
    [HideInInspector]
    public List<GameObject> m_lstPatrolRight;

          
    [SerializeField]
    [ReadOnly]
    Transform m_trDoorObstructEnemy;



    [SerializeField]
    public List<MMPathMovementElement> m_lstPathDefendLeft;
    GameObject m_goPathDefendLeftRoot;
    [HideInInspector]
    public List<GameObject> m_lstDefendLeft;

    [SerializeField]
    public List<MMPathMovementElement> m_lstPathDefendRight;
    GameObject m_goPathDefendRightRoot;
    [HideInInspector]
    public List<GameObject> m_lstDefendRight;



    protected override void Awake()
    {
        m_emBuildingType = EM_F_BuildingType.F_Wall;

        base.Awake();

        m_goPatrolLeftRoot = new GameObject("PatrolLeftRoot");
        m_goPatrolLeftRoot.transform.SetParent(transform);
        m_goPatrolLeftRoot.transform.localPosition = Vector3.zero;
        m_goPatrolLeftRoot.transform.localRotation = Quaternion.identity;
        m_goPatrolLeftRoot.transform.localScale = Vector3.one;
        m_lstPatrolLeft = new List<GameObject>();
        for (int i = 0; i < m_lstPathPatrolLeft.Count; i++)
        {
            GameObject _goPoint = new GameObject("PatrolPoint_" + i);
            _goPoint.transform.SetParent(m_goPatrolLeftRoot.transform);
            _goPoint.transform.position = transform.TransformPoint(m_lstPathPatrolLeft[i].PathElementPosition);
            _goPoint.transform.localRotation = Quaternion.identity;
            _goPoint.transform.localScale = Vector3.one;
            m_lstPatrolLeft.Add(_goPoint);
        }

        m_goPatrolRightRoot = new GameObject("PatrolRightRoot");
        m_goPatrolRightRoot.transform.SetParent(transform);
        m_goPatrolRightRoot.transform.localPosition = Vector3.zero;
        m_goPatrolRightRoot.transform.localRotation = Quaternion.identity;
        m_goPatrolRightRoot.transform.localScale = Vector3.one;
        m_lstPatrolRight = new List<GameObject>();
        for (int i = 0; i < m_lstPathPatrolRight.Count; i++)
        {
            GameObject _goPoint = new GameObject("PatrolPoint_" + i);
            _goPoint.transform.SetParent(m_goPatrolRightRoot.transform);
            _goPoint.transform.position = transform.TransformPoint(m_lstPathPatrolRight[i].PathElementPosition);
            _goPoint.transform.localRotation = Quaternion.identity;
            _goPoint.transform.localScale = Vector3.one;
            m_lstPatrolRight.Add(_goPoint);
        }




        m_goPathDefendLeftRoot = new GameObject("DefendLeftRoot");
        m_goPathDefendLeftRoot.transform.SetParent(transform);
        m_goPathDefendLeftRoot.transform.localPosition = Vector3.zero;
        m_goPathDefendLeftRoot.transform.localRotation = Quaternion.identity;
        m_goPathDefendLeftRoot.transform.localScale = Vector3.one;
        m_lstDefendLeft = new List<GameObject>();
        for (int i = 0; i < m_lstPathDefendLeft.Count; i++)
        {
            GameObject _goPoint = new GameObject("DefendLeftPoint_" + i);
            _goPoint.transform.SetParent(m_goPathDefendLeftRoot.transform);
            _goPoint.transform.position = transform.TransformPoint(m_lstPathDefendLeft[i].PathElementPosition);
            _goPoint.transform.localRotation = Quaternion.identity;
            _goPoint.transform.localScale = Vector3.one;
            m_lstDefendLeft.Add(_goPoint);
        }

        m_goPathDefendRightRoot = new GameObject("DefendRightRoot");
        m_goPathDefendRightRoot.transform.SetParent(transform);
        m_goPathDefendRightRoot.transform.localPosition = Vector3.zero;
        m_goPathDefendRightRoot.transform.localRotation = Quaternion.identity;
        m_goPathDefendRightRoot.transform.localScale = Vector3.one;
        m_lstDefendRight = new List<GameObject>();
        for (int i = 0; i < m_lstPathDefendRight.Count; i++)
        {
            GameObject _goPoint = new GameObject("DefendRightPoint_" + i);
            _goPoint.transform.SetParent(m_goPathDefendRightRoot.transform);
            _goPoint.transform.position = transform.TransformPoint(m_lstPathDefendRight[i].PathElementPosition);
            _goPoint.transform.localRotation = Quaternion.identity;
            _goPoint.transform.localScale = Vector3.one;
            m_lstDefendRight.Add(_goPoint);
        }
    }

    protected override void SetCurLevel(int nLev)
    {
        //base.SetCurLevel(nLev);
        int nLvMax = Minos_CTBLInfo.Inst.GetF_Wall_MaxLv();
        GameCommon.CHECK(nLvMax > 0);
        GameCommon.CHECK(nLev >= 0 && nLev <= nLvMax);

        if (nLev > 0)
        {
            Minos_CTBLInfo.ST_F_Wall stWall = Minos_CTBLInfo.Inst.GetF_Wall(nLev);
            GameCommon.CHECK(stWall != null);
            m_stHealth.InitialHealth = stWall.nHp;
            m_stHealth.MaximumHealth = stWall.nHp;
            m_stHealth.CurrentHealth = stWall.nHp;
        }

        m_nCurLevel = nLev;
    }

    public override int GetLevHealth(int nLev)
    {
        //return base.GetLevHealth(nLev);
        Minos_CTBLInfo.ST_F_Wall stWall = Minos_CTBLInfo.Inst.GetF_Wall(nLev);
        GameCommon.CHECK(stWall != null);
        return stWall.nHp;
    }
    
    protected override void BuildingLevChangeTo(int nTargetLev, bool bIsStart)
    {
        base.BuildingLevChangeTo(nTargetLev, bIsStart);
        
        m_trDoorObstructEnemy = base.m_goBuilding.transform.Find("BoxObstructEnemy");
        if (m_trDoorObstructEnemy != null)
        {
            m_trDoorObstructEnemy.gameObject.SetActive(!Minos_GameDateManager.Instance.IsDayOrNight());
        }
    }

    protected override bool CanLevUpToNext(out int nCostMoneyCoin)
    {
        //return base.CanLevUpToNext(out nCostMoneyCoin);
        int nLvMax = Minos_CTBLInfo.Inst.GetF_Wall_MaxLv();
        GameCommon.CHECK(nLvMax > 0);
        GameCommon.CHECK(m_nCurLevel >= 0 && m_nCurLevel <= nLvMax);

        if (m_nCurLevel == nLvMax)
        {
            nCostMoneyCoin = 0;
            return false;
        }

        nCostMoneyCoin = GetLevUpCostMoneyCoin(m_nCurLevel + 1);
        return true;
    }

    public override int GetLevUpCostMoneyCoin(int nLevTo)
    {
        //return base.GetLevUpCostMoneyCoin(nLevTo);
        int nLvMax = Minos_CTBLInfo.Inst.GetF_Wall_MaxLv();
        GameCommon.CHECK(nLvMax > 0);
        GameCommon.CHECK(nLevTo >= 0 && nLevTo <= nLvMax);

        Minos_CTBLInfo.ST_F_Wall stWall = Minos_CTBLInfo.Inst.GetF_Wall(nLevTo);
        GameCommon.CHECK(stWall != null);
        return stWall.nUpGradeNeedMoneyCoins;
    }

    protected override void OnLogicTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            /*
                以下会同时产生[OnTriggerEnter]，即使是挂载在同一个GameObject上
                01.CharacterController继承自Collider，默认IsTrigger是不开启
                02.BoxCollider继承自Collider
                03.???Collider继承自Collider
            */
            return;
        }
        
        Player stPlayer = other.GetComponent<Player>();
        if (stPlayer != null && stPlayer.IsCanInteractiveWithBuilding(m_emBuildingType))
        {
            if (GetCurLev() > 0)
            {
                GameCommon.CHECK(base.ShowHealthBar(true));
            }

            if (!IsSelfSendedAIActionOrder())
            {
                int nCostMoneyCoin = 0;
                if (CanLevUpToNext(out nCostMoneyCoin))
                {
                    base.ShowMoneyCoinBar(
                        nCostMoneyCoin, 1.0f,
                        delegate()
                        {
                            stPlayer.DoUnderBuilding(GetBuildingType(), gameObject);
                        }
                        );                    
                }
            }
            return;
        }
    }

    protected override void OnLogicTriggerExit(Collider other)
    {
        if (!other.isTrigger)
        {
            /*
                以下会同时产生[OnTriggerExit]，即使是挂载在同一个GameObject上
                01.CharacterController继承自Collider，默认IsTrigger是不开启
                02.BoxCollider继承自Collider
                03.???Collider继承自Collider
            */
            return;
        }

        Player stPlayer = other.GetComponent<Player>();
        if (stPlayer != null)
        {
            base.UnShowMoneyCoinBar();
            stPlayer.UndoUnderBuilding();
            base.ShowHealthBar(false);
            return;
        }
    }

    public override void OnMoneyCoinFinished()
    {
        F_AIActionOrderManager.Instance.GetOrderHandler(EM_F_AIActionOrderHandler.Hammerman).CreateOrder(
            EM_F_AIActionOrderType.LevUpBuilding, this);
    }



    protected override void OnHealthDead()
    {
        base.OnHealthDead();

        BuildingLevChangeTo(0, false);
    }
    
    public override void OnGameDate_IsDayComing()
    {
        base.OnGameDate_IsDayComing();

        if (m_trDoorObstructEnemy != null)
        {
            m_trDoorObstructEnemy.gameObject.SetActive(false);
        }
    }

    public override void OnGameDate_IsNightComing(bool bIsBloodNight, int nBloodNightIndex)
    {
        base.OnGameDate_IsNightComing(bIsBloodNight, nBloodNightIndex);

        if (m_trDoorObstructEnemy != null)
        {
            m_trDoorObstructEnemy.gameObject.SetActive(true);
        }
    }
}