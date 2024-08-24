using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public abstract class IBase_Friend_Character : MonoBehaviour
{
    [Header("Id")]
    [SerializeField]
    [ReadOnly]
    int m_nOnlyId;
    [SerializeField]
    [ReadOnly]
    protected EM_F_CharacterType m_emCharType = EM_F_CharacterType.Invalid;
    [SerializeField]
    [ReadOnly]
    int m_nIndexOfList;

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
    protected IBase_Friend_FactoryBuilding m_stParentFactory;

    [Header("TopDownEngin")]
    [SerializeField]
    [ReadOnly]
    protected TopDownController m_stTDController;
    [SerializeField]
    [ReadOnly]
    protected Character m_stTDCharacter;
    [SerializeField]
    [ReadOnly]
    protected CharacterMovement m_stTDCharMovement;
    [SerializeField]
    [ReadOnly]
    protected CharacterRun m_stTDCharRun;

    [Header("MoneyCoin_AutoThrow")]
    [SerializeField]
    bool m_bIsCanAutoThrowMoneyCoin;
    [SerializeField]
    float m_fEnoughNear2PlayerThenThrowMoneyCoin = 3.0f;
    [SerializeField]
    float m_fConstAutoThrowMoneyCoinTimeCD = 0.3f;
    [SerializeField]
    [ReadOnly]
    float m_fAutoThrowMoneyCoinTimeStamp = 0f;

    [Header("MoneyCoin_AutoEat")]
    [SerializeField]
    bool m_bIsCanAutoEatMoneyCoin;

    [SerializeField]
    float m_fConstAutoEatMoneyCoinTimeCD = 0.3f;
    [SerializeField]
    [ReadOnly]
    float m_fAutoEatMoneyCoinTimeStamp = 0f;

    [Header("MoneyCoin_OtherParams")]
    [SerializeField]
    float m_fEnoughFar2PlayerThenEatMoneyCoin = 8.0f;
    [SerializeField]
    float m_fRadiusFindAndEatMoneyCoin = 3.0f;
    [SerializeField]
    [ReadOnly]
    protected int m_nAlreadyEatMoneyCoinCount;
    [SerializeField]
    protected MMFeedbacks m_stPickMoneyCoin;

    [Header("AI")]
    [SerializeField]
    [ReadOnly]
    protected BehaviorTree m_stBehaviorTree;

    [Header("Health")]
    [SerializeField]
    [ReadOnly]
    protected Health m_stHealth;


    //Private Stuff
    protected object m_objAIActionOrder;
    

    protected virtual void Awake()
    {
        m_stTDController = GetComponent<TopDownController>();
        GameCommon.CHECK(m_stTDController != null);

        m_stTDCharacter = GetComponent<Character>();
        GameCommon.CHECK(m_stTDCharacter != null);

        m_stTDCharMovement = GetComponent<CharacterMovement>();
        GameCommon.CHECK(m_stTDCharMovement != null);

        m_stTDCharRun = GetComponent<CharacterRun>();
        //GameCommon.CHECK(m_stTDCharRun != null);

        m_stPickMoneyCoin?.Initialization();
        
        m_stBehaviorTree = GetComponent<BehaviorTree>();
        GameCommon.CHECK(m_stBehaviorTree != null);        
        m_stBehaviorTree.SetVariableValue(
            IBase_Friend_AIActionOrder.m_strVariableName_AIOrderType,
            EM_F_AIActionOrderType.Idle
            );
        m_stBehaviorTree.SetVariableValue(
            IBase_Friend_AIActionOrder.m_strVariableName_IsNight,
            !Minos_GameDateManager.Instance.IsDayOrNight()
            );

        m_stHealth = gameObject.GetComponent<Health>();
        GameCommon.CHECK(m_stHealth != null);
        m_stHealth.DestroyOnDeath = true;
        m_stHealth.OnDeath = OnHealthDead;
        m_stHealth.OnRevive = OnHealthRevive;
        m_stHealth.OnHit = OnHealthHit;

        m_aryAttr = new float[(int)(EM_F_CharacterAttr.Max)];
        for (int i = 0; i < m_aryAttr.Length; i++)
        {
            m_aryAttr[i] = 0;
        }
    }

    protected virtual void Start()
    {
        //此刻所有脚本得初始化才完成
        if (m_stParentFactory != null)
        {
            //SetLv(m_stParentFactory.GetBornCharacterLev());
            ApplyLevAttr();
        }
    }

    protected virtual void OnDestroy()
    {

    }

    protected virtual void Update()
    {
        if (Player.Inst != null && 
            m_bIsCanAutoThrowMoneyCoin &&
            Time.time - m_fAutoThrowMoneyCoinTimeStamp > m_fConstAutoThrowMoneyCoinTimeCD
            )
        {
            if (m_nAlreadyEatMoneyCoinCount > 0 && Vector3.Distance(Player.Inst.transform.position, transform.position) <= m_fEnoughNear2PlayerThenThrowMoneyCoin)
            {
                //与Player距离足够近，则扔金币
                for (int iCoin = 0; iCoin < m_nAlreadyEatMoneyCoinCount; iCoin++)
                {
                    Vector3 v3DirToPlayer = Player.Inst.transform.position - transform.position;
                    Vector3 v3TargetPosOffset = GameHelper.RotateVector3InRandom(v3DirToPlayer, transform.up, -20.0f, 20.0f);
                    v3TargetPosOffset = v3TargetPosOffset.normalized * UnityEngine.Random.Range(1.5f, 2.0f);

                    PickItem2MoneyCoin itemCoin = PickItem2MoneyCoin.InstancePickItem2MoneyCoin(
                        "MoneyCoin_DropToGround", null, Vector3.zero, Quaternion.identity, Vector3.one);
                    GameCommon.CHECK(itemCoin != null);
                    itemCoin.SetConfigMoneyCoin(1);
                    itemCoin.PlayDropEffect(
                        transform.position + 1.0f * v3DirToPlayer.normalized * 1.0f,
                        transform.position + v3TargetPosOffset,
                        0.7f,
                        null
                        );
                }
                m_nAlreadyEatMoneyCoinCount = 0;
            }

            m_fAutoThrowMoneyCoinTimeStamp = Time.time;
        }


        if (Player.Inst != null &&
            m_bIsCanAutoEatMoneyCoin &&
            Time.time - m_fAutoEatMoneyCoinTimeStamp > m_fConstAutoEatMoneyCoinTimeCD
            )
        {
            if (Vector3.Distance(Player.Inst.transform.position, transform.position) > m_fEnoughFar2PlayerThenEatMoneyCoin)
            {
                //与Player距离足够远，则“吞掉”自己圆柱范围内的金币（类似于 <<The Wild Age>> ）
                Collider[] aryTargetCollder = Physics.OverlapSphere(
                    transform.position, 
                    m_fRadiusFindAndEatMoneyCoin, 
                    1 << LayerMask.NameToLayer("New_MoneyCoins")
                    );
                foreach (Collider _collider in aryTargetCollder)
                {
                    //且Player与金币也足够远
                    if (Vector3.Distance(Player.Inst.transform.position, _collider.transform.position) > m_fEnoughFar2PlayerThenEatMoneyCoin)
                    {
                        Destroy(_collider.gameObject);
                        m_nAlreadyEatMoneyCoinCount += 1;
                    }
                }                
            }

            m_fAutoEatMoneyCoinTimeStamp = Time.time;
        }
    }

    public void SetOnlyId(int nId, EM_F_CharacterType emCharType, int nIndexOfList)
    {
        GameCommon.CHECK(nId > 0);
        GameCommon.CHECK(emCharType > EM_F_CharacterType.Invalid && emCharType < EM_F_CharacterType.Max);
        GameCommon.CHECK(nIndexOfList >= 0);

        m_nOnlyId = nId;
        m_emCharType = emCharType;
        m_nIndexOfList = nIndexOfList;
    }
    public int GetOnlyId() { return m_nOnlyId; }
    public EM_F_CharacterType GetCharType() { return m_emCharType; }
    public int GetIndexOfList() { return m_nIndexOfList; }

    public virtual bool SetLv(int nLv)
    {
        int nLvMax = Minos_CTBLInfo.Inst.GetF_CharacterAttr_MaxLv(m_emCharType);
        GameCommon.CHECK(nLvMax >= 0);
        GameCommon.CHECK(nLv >= 0 && nLv <= nLvMax);//没有表格数据，即0级

        bool bRet = false;
        Minos_CTBLInfo.ST_F_CharacterAttr stAttr = Minos_CTBLInfo.Inst.GetF_CharacterAttr(m_emCharType, nLv);
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

    public virtual bool UpgradeLv()
    {
        return SetLv(m_nLv + 1);
    }

    public float GetAttr(EM_F_CharacterAttr emAttr)
    {
        GameCommon.CHECK(emAttr > EM_F_CharacterAttr.Invalid && emAttr < EM_F_CharacterAttr.Max);
        return m_aryAttr[(int)(emAttr)];
    }

    protected abstract void ApplyLevAttr();

    public IBase_Friend_FactoryBuilding GetParentFactory() { return m_stParentFactory; }
    public void SetParentFactory(IBase_Friend_FactoryBuilding stParentFactory)
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

    public virtual void EatMoneyCoin(int nCount)
    {
        GameCommon.CHECK(nCount > 0);
        m_nAlreadyEatMoneyCoinCount += nCount;
    }

    public virtual void PickMoneyCoin(PickItem2MoneyCoin stMoneyCoin) { }
    
    public virtual void SetPatrolPath(List<GameObject> lstPatrolPath) 
    {
        GameCommon.CHECK(lstPatrolPath != null);
        GameCommon.CHECK(lstPatrolPath.Count > 0);

        m_stBehaviorTree.SetVariableValue(
            IBase_Friend_AIActionOrder.m_strVariableName_PatrolStartPoint,
            lstPatrolPath[0]
            );

        Task stTask = m_stBehaviorTree.FindTaskWithName(IBase_Friend_AIActionOrder.m_strTaskName_PatrolAroundFactory);
        GameCommon.CHECK(stTask != null);
        BD_AIActionNavMeshPatrol3D stPatrol = stTask as BD_AIActionNavMeshPatrol3D;
        GameCommon.CHECK(stPatrol != null);
        stPatrol.SetWaypoints(lstPatrolPath);
    }

    public object GetAIActionOrder() { return m_objAIActionOrder; }
    public void ClearAIActionOrder() { m_objAIActionOrder = null; }
    public virtual void ConnectAIActionOrder(object objOrder)
    {
        GameCommon.CHECK(objOrder != null);

        ST_F_AIActionOrder stAAOrder = objOrder as ST_F_AIActionOrder;
        if (stAAOrder != null)
        {
            IBase_Friend_Building stTargetBuilding = stAAOrder.GettTargetBuilding();
            GameCommon.CHECK(stTargetBuilding != null);

#if UNITY_EDITOR
            MethodBase stMBase = System.Reflection.MethodBase.GetCurrentMethod();
            Debug.Log(stMBase.Name + " : " + gameObject.name + " | " + stAAOrder.GetOType().ToString() + " | " + stTargetBuilding.name);
#endif

            m_stBehaviorTree.SetVariableValue(
                IBase_Friend_AIActionOrder.m_strVariableName_AIOrderType,
                stAAOrder.GetOType()
                );
            m_stBehaviorTree.SetVariableValue(
                IBase_Friend_AIActionOrder.m_strVariableName_AIOrderMoveTo,
                stTargetBuilding.GetLogicTriggerTransform().gameObject
                );

            m_objAIActionOrder = objOrder;
        }
    }

    public virtual void OnGameDate_IsDayComing() 
    {
        m_stBehaviorTree.SetVariableValue(
            IBase_Friend_AIActionOrder.m_strVariableName_IsNight,
            !Minos_GameDateManager.Instance.IsDayOrNight()
            );
    }

    public virtual void OnGameDate_IsNightComing(bool bIsBloodNight, int nBloodNightIndex) 
    {
        m_stBehaviorTree.SetVariableValue(
            IBase_Friend_AIActionOrder.m_strVariableName_IsNight,
            !Minos_GameDateManager.Instance.IsDayOrNight()
            );
    }


    #region *********************************************Health****************************************************
    protected virtual void OnHealthDead()
    {
        switch(m_emCharType)
        {
            case EM_F_CharacterType.F_Villager:
            case EM_F_CharacterType.F_Hammerman:
            case EM_F_CharacterType.F_Bowman:
            case EM_F_CharacterType.F_Farmer:
            case EM_F_CharacterType.F_NearWarriorA:
            case EM_F_CharacterType.F_Knight:
                {
                    F_PrimitivemanFactory stTarget = null;
                    foreach (IBase_Friend_Building _stBuilding in Minos_BuildingManager.Instance.EnumAll_F_Building())
                    {
                        if (_stBuilding.GetBuildingType() == EM_F_BuildingType.F_PrimitivemanFactory)
                        {
                            stTarget = _stBuilding as F_PrimitivemanFactory;
                            GameCommon.CHECK(stTarget != null);
                            break;
                        }
                    }

                    GameCommon.CHECK(stTarget != null);
                    stTarget.InstantiateCharacter(transform.position, Vector3.zero);

                    if (m_emCharType == EM_F_CharacterType.F_Villager)
                    {
                        Minos_VillagerFactory.Instance.DecreaseVillager(GetOnlyId());
                    }
                    else
                    {
                        GetParentFactory().DecreaseCharacter(GetOnlyId());
                    }                    
                }
                break;
        }
    }

    protected virtual void OnHealthRevive() { }
    protected virtual void OnHealthHit() { }
    #endregion *********************************************Health****************************************************

    public void DoCharacterAbility_RunStart()
    {
        m_stTDCharRun.RunStart();
    }

    public void DoCharacterAbility_RunStop()
    {
        m_stTDCharRun.RunStop();
    }
}