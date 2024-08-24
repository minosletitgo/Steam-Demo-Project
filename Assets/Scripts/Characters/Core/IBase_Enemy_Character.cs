using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public abstract class IBase_Enemy_Character : MonoBehaviour
{
    [Header("Id")]
    [SerializeField]
    [ReadOnly]
    int m_nOnlyId;
    [SerializeField]
    [ReadOnly]
    protected EM_E_CharacterType m_emCharType = EM_E_CharacterType.Invalid;

    [Header("Lv")]
    [SerializeField]
    [ReadOnly]
    protected int m_nLv;

    [Header("Attr")]
    [SerializeField]
    [ReadOnly]
    protected float[] m_aryAttr;

    [Header("ParentFactory")]
    [SerializeField]
    [ReadOnly]
    protected IBase_Enemy_FactoryBuilding m_stParentFactory;

    [Header("TopDownEngin")]
    [SerializeField]
    [ReadOnly]
    TopDownController m_stTDController;
    [SerializeField]
    [ReadOnly]
    Character m_stTDCharacter;
    [SerializeField]
    [ReadOnly]
    protected CharacterMovement m_stTDCharMovement;
    [SerializeField]
    [ReadOnly]
    protected CharacterRun m_stTDCharRun;

    [Header("Weapon")]
    [SerializeField]
    [ReadOnly]
    CharacterHandleWeapon m_stTDCharHandleWeapon;


    [Header("AI")]
    [SerializeField]
    [ReadOnly]
    protected BehaviorTree m_stBehaviorTree;

    [SerializeField]
    [ReadOnly]
    protected Health m_stHealth;



    protected virtual void Awake()
    {
        m_stTDController = GetComponent<TopDownController>();
        GameCommon.CHECK(m_stTDController != null);

        m_stTDCharacter = GetComponent<Character>();
        GameCommon.CHECK(m_stTDCharacter != null);

        m_stTDCharMovement = GetComponent<CharacterMovement>();
        GameCommon.CHECK(m_stTDCharMovement != null);

        m_stTDCharHandleWeapon = GetComponent<CharacterHandleWeapon>();
        GameCommon.CHECK(m_stTDCharHandleWeapon != null);

        m_stTDCharRun = GetComponent<CharacterRun>();
        GameCommon.CHECK(m_stTDCharRun != null);
        
        m_stBehaviorTree = GetComponent<BehaviorTree>();
        GameCommon.CHECK(m_stBehaviorTree != null);
        m_stBehaviorTree.SetVariableValue(
            E_AIActionOrderManager.m_strVariableName_IsNight,
            !Minos_GameDateManager.Instance.IsDayOrNight()
            );

        m_stHealth = gameObject.GetComponent<Health>();
        GameCommon.CHECK(m_stHealth != null);
        m_stHealth.DestroyOnDeath = true;
        m_stHealth.OnDestroyObject = OnHealthDestroyObject;
        m_stHealth.OnRevive = OnHealthRevive;
        m_stHealth.OnHit = OnHealthHit;

        m_aryAttr = new float[(int)(EM_E_CharacterAttr.Max)];
        for (int i = 0; i < m_aryAttr.Length; i++)
        {
            m_aryAttr[i] = 0;
        }
    }

    protected virtual void Start()
    {
        ApplyLevAttr();
    }

    protected virtual void OnDestroy()
    {

    }

    protected virtual void Update()
    {

    }

    public void SetOnlyId(int nId, EM_E_CharacterType emCharType)
    {
        GameCommon.CHECK(nId > 0);
        GameCommon.CHECK(emCharType > EM_E_CharacterType.Invalid && emCharType < EM_E_CharacterType.Max);

        m_nOnlyId = nId;
        m_emCharType = emCharType;
    }
    public int GetOnlyId() { return m_nOnlyId; }
    public EM_E_CharacterType GetCharType() { return m_emCharType; }

    public virtual bool SetLv(int nLv)
    {
        int nLvMax = Minos_CTBLInfo.Inst.GetE_CharacterAttr_MaxLv(m_emCharType);
        GameCommon.CHECK(nLvMax >= 0);
        GameCommon.CHECK(nLv >= 0 && nLv <= nLvMax);//没有表格数据，即0级

        bool bRet = false;
        Minos_CTBLInfo.ST_E_CharacterAttr stAttr = Minos_CTBLInfo.Inst.GetE_CharacterAttr(m_emCharType, nLv);
        if (stAttr != null)
        {
            GameCommon.CHECK(m_aryAttr.Length == stAttr.aryAttr.Length);
            bRet = true;
            for (int i = 0; i < m_aryAttr.Length; i++)
            {
                m_aryAttr[i] = stAttr.aryAttr[i];
            }
        }

        m_nLv = nLv;
        return bRet;
    }

    public int GetLv() { return m_nLv; }

    public float GetAttr(EM_E_CharacterAttr emAttr)
    {
        GameCommon.CHECK(emAttr > EM_E_CharacterAttr.Invalid && emAttr < EM_E_CharacterAttr.Max);
        return m_aryAttr[(int)(emAttr)];
    }

    protected abstract void ApplyLevAttr();

    public IBase_Enemy_FactoryBuilding GetParentFactory() { return m_stParentFactory; }
    public void SetParentFactory(IBase_Enemy_FactoryBuilding stParentFactory)
    {
        //GameCommon.CHECK(stParentFactory != null);
        m_stParentFactory = stParentFactory;
    }
    public Character GetTDCharacter() { return m_stTDCharacter; }
    public CharacterMovement GetTDCharMovement() { return m_stTDCharMovement; }

    public Vector3 GetCurrentDirection() { return m_stTDController.CurrentDirection; }
    public void SetCurrentDirection(Vector3 v3Dir)
    {
        m_stTDController.CurrentDirection = v3Dir;
    }

    public virtual void SetPatrolPath(List<GameObject> lstPatrolPath)
    {
        GameCommon.CHECK(lstPatrolPath != null);
        GameCommon.CHECK(lstPatrolPath.Count > 0);

        F_Homeland stHomeland = Minos_BuildingManager.Instance.GetPlayerHomeland();
        GameCommon.CHECK(stHomeland != null);

        List<GameObject> lstPatrolPathCross = new List<GameObject>();
        lstPatrolPathCross.AddRange(lstPatrolPath);
        lstPatrolPathCross.Add(stHomeland.m_lstPatrolSelf[0]);

        m_stBehaviorTree.SetVariableValue(
            E_AIActionOrderManager.m_strVariableName_PatrolStartPoint,
            lstPatrolPathCross[0]
            );

        Task stTask = m_stBehaviorTree.FindTaskWithName(E_AIActionOrderManager.m_strTaskName_PatrolAroundFactory);
        GameCommon.CHECK(stTask != null);
        BD_AIActionNavMeshPatrol3D stPatrol = stTask as BD_AIActionNavMeshPatrol3D;
        GameCommon.CHECK(stPatrol != null);
        stPatrol.SetWaypoints(lstPatrolPathCross);
    }

    public void DoCharacterAbility_Shoot(bool bStart)
    {
        if (bStart)
        {
            m_stTDCharHandleWeapon.ShootStart();
        }
        else
        {
            m_stTDCharHandleWeapon.ShootStop();
        }
    }

    public void DoCharacterAbility_Run(bool bStart)
    {
        if (bStart)
        {
            m_stTDCharRun.RunStart();
        }
        else
        {
            m_stTDCharRun.RunStop();
        }        
    }


    #region *********************************************Health****************************************************
    protected virtual void OnHealthDestroyObject() { }
    protected virtual void OnHealthRevive() { }
    protected virtual void OnHealthHit() { }
    #endregion *********************************************Health****************************************************
}
