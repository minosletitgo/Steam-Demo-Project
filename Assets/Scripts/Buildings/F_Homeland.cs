using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using System;

/*
    01.对于场景中摆放的建筑，其每一个Awake函数之间的顺序，是不可控制
    02.遵循Awake里只执行【GetComponent】这种无需依赖的逻辑
*/

public class F_Homeland : IBase_Friend_Building
{
    [SerializeField]
    public List<MMPathMovementElement> m_lstPathDefendLeft;
    GameObject m_goDefendLeftRoot;
    [HideInInspector]
    public List<GameObject> m_lstDefendLeft;

    [SerializeField]
    public List<MMPathMovementElement> m_lstPathDefendRight;
    GameObject m_goDefendRightRoot;
    [HideInInspector]
    public List<GameObject> m_lstDefendRight;

    protected override void Awake()
    {
        GameCommon.CHECK(Minos_GlobalCore.Inst != null);

        m_emBuildingType = EM_F_BuildingType.F_Homeland;

        base.Awake();

        m_goDefendLeftRoot = new GameObject("DefendLeftRoot");
        m_goDefendLeftRoot.transform.SetParent(transform);
        m_goDefendLeftRoot.transform.localPosition = Vector3.zero;
        m_goDefendLeftRoot.transform.localRotation = Quaternion.identity;
        m_goDefendLeftRoot.transform.localScale = Vector3.one;
        m_lstDefendLeft = new List<GameObject>();
        for (int i = 0; i < m_lstPathDefendLeft.Count; i++)
        {
            GameObject _goPoint = new GameObject("PatrolPoint_" + i);
            _goPoint.transform.SetParent(m_goDefendLeftRoot.transform);
            _goPoint.transform.position = transform.TransformPoint(m_lstPathDefendLeft[i].PathElementPosition);
            _goPoint.transform.localRotation = Quaternion.identity;
            _goPoint.transform.localScale = Vector3.one;
            m_lstDefendLeft.Add(_goPoint);
        }

        m_goDefendRightRoot = new GameObject("DefendRightRoot");
        m_goDefendRightRoot.transform.SetParent(transform);
        m_goDefendRightRoot.transform.localPosition = Vector3.zero;
        m_goDefendRightRoot.transform.localRotation = Quaternion.identity;
        m_goDefendRightRoot.transform.localScale = Vector3.one;
        m_lstDefendRight = new List<GameObject>();
        for (int i = 0; i < m_lstPathDefendRight.Count; i++)
        {
            GameObject _goPoint = new GameObject("PatrolPoint_" + i);
            _goPoint.transform.SetParent(m_goDefendRightRoot.transform);
            _goPoint.transform.position = transform.TransformPoint(m_lstPathDefendRight[i].PathElementPosition);
            _goPoint.transform.localRotation = Quaternion.identity;
            _goPoint.transform.localScale = Vector3.one;
            m_lstDefendRight.Add(_goPoint);
        }
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

        //Debug.Log("Homeland OnTriggerEnter Success : " + other.name);

        Player stPlayer = other.GetComponent<Player>();
        if (stPlayer != null)
        {
            int nCostMoneyCoin = 0;
            if (CanLevUpToNext(out nCostMoneyCoin))
            {
                base.ShowMoneyCoinBar(
                    nCostMoneyCoin, 0.0f,
                    delegate()
                    {
                        stPlayer.DoUnderBuilding(GetBuildingType(), gameObject);
                    }
                    );                
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

        //Debug.Log("HomelandFactory OnTriggerExit Success : " + other.name);

        Player stPlayer = other.GetComponent<Player>();
        if (stPlayer != null)
        {
            base.UnShowMoneyCoinBar();
            stPlayer.UndoUnderBuilding();            
            return;
        }        
    }

    public override void OnMoneyCoinFinished()
    {
        int nCostMoneyCoin = 0;
        GameCommon.CHECK(CanLevUpToNext(out nCostMoneyCoin));
        BuildingLevUpToNext();
    }
}
