using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public abstract class IBase_Friend_FactoryBuilding : IBase_Friend_Building
{
    [ReadOnly]
    [SerializeField]
    protected EM_F_CharacterType m_emBornCharacterType = EM_F_CharacterType.Invalid;

    [ReadOnly]
    [SerializeField]
    GameObject m_goBornCharacterRoot;

    [ReadOnly]
    [SerializeField]
    GameObject m_goBornCharacterPosition;

    [ReadOnly]
    [SerializeField]
    protected int m_nBornCharacterLev = 0;
    [ReadOnly]
    [SerializeField]
    protected int m_nBornCharacterMaxLev = 0;

    [ReadOnly]
    [SerializeField]
    protected int m_nCharacterStorageCount;
    protected Dictionary<int, IBase_Friend_Character> m_mapCharStorage = new Dictionary<int, IBase_Friend_Character>();
    
    public DGOn_F_AIActionCreateIdleSignal m_dgOnCreateIdleOrderSignal;


    protected override void Awake()
    {
        base.Awake();

        GameCommon.CHECK(m_emBornCharacterType >  EM_F_CharacterType.Invalid && m_emBornCharacterType < EM_F_CharacterType.Max);

        m_goBornCharacterRoot = transform.Find("BornCharacterRoot").gameObject;
        GameCommon.CHECK(m_goBornCharacterRoot != null, "m_goBornCharacterRoot != null : " + gameObject.name);

        m_nStaticCharacterId = m_nStaticCharacterStartId;
    }

    protected override void Start()
    {
        base.Start();

        m_nBornCharacterLev = Minos_CTBLInfo.Inst.GetF_CharacterAttr_MinLv(m_emBornCharacterType);
        m_nBornCharacterMaxLev = Minos_CTBLInfo.Inst.GetF_CharacterAttr_MaxLv(m_emBornCharacterType);
    }


    public EM_F_CharacterType GetBornCharacterType() { return m_emBornCharacterType; }
    public Transform GetBornCharacterRoot() { return m_goBornCharacterRoot.transform; }
    public int GetBornCharacterLev() { return m_nBornCharacterLev; }

    public void UpgradeBornCharacterLev()
    {
        GameCommon.CHECK(m_nBornCharacterLev < m_nBornCharacterMaxLev);
        m_nBornCharacterLev++;

        foreach (KeyValuePair<int, IBase_Friend_Character> _infoPair in m_mapCharStorage)
        {
            GameCommon.CHECK(_infoPair.Value.UpgradeLv());
        }
    }

    public bool CanUpgradeBornCharacterLev() { return m_nBornCharacterLev < m_nBornCharacterMaxLev; }

    int m_nStaticCharacterStartId = 1000;
    public int GetCharacterStartId() { return m_nStaticCharacterStartId; }

    int m_nStaticCharacterId = 1000;//每个工厂的独有角色id
    protected int AllcocCharacterId()
    {
        return ++m_nStaticCharacterId;
    }

    public virtual IBase_Friend_Character InstantiateCharacter()
    {
        return InstantiateCharacter(
            m_goBornCharacterRoot.transform.position, 
            Player.Inst.transform.position - m_goBornCharacterRoot.transform.position
            );
    }

    public IBase_Friend_Character InstantiateCharacter(Vector3 v3Position, Vector3 v3Dir)
    {
        int nOnlyId = AllcocCharacterId();
        IBase_Friend_Character stChar = GameHelper_F_Character.InstantiateCharacters<IBase_Friend_Character>(
            GetBornCharacterType(),
            nOnlyId,
            GetBornCharacterRoot(),
            Vector3.zero,
            Quaternion.identity,
            Vector3.one
            );
        GameCommon.CHECK(stChar != null, "Missing <????Character> Script !");
        Debug.Log("InstantiateCharacter: " + gameObject.name + " | " + stChar.name);
        stChar.transform.position = v3Position;
        stChar.SetCurrentDirection(v3Dir);
        stChar.SetOnlyId(nOnlyId, GetBornCharacterType(), m_mapCharStorage.Count);
        stChar.SetLv(GetBornCharacterLev());
        stChar.SetParentFactory(this);
        stChar.SetPatrolPath(m_lstPatrolSelf);
        
        IncreaseCharacter(stChar.GetOnlyId(), stChar);

        CreateIdleOrderSignal(stChar);

        return stChar;
    }

    public virtual void IncreaseCharacter<T>(int nOnlyId, T stChar) where T : IBase_Friend_Character
    {
        GameCommon.CHECK(nOnlyId > 0);
        
        IBase_Friend_Character stRealChar = stChar as IBase_Friend_Character;
        GameCommon.CHECK(stRealChar != null);
        
        m_mapCharStorage.Add(nOnlyId, stRealChar);
        m_nCharacterStorageCount = m_mapCharStorage.Count;
    }

    public virtual void DecreaseCharacter(int nOnlyId)
    {
        GameCommon.CHECK(nOnlyId > 0);

        Debug.Log("DecreaseCharacter: " + nOnlyId.ToString() + " | " + gameObject.name);

        IBase_Friend_Character stChar;
        GameCommon.CHECK(m_mapCharStorage.TryGetValue(nOnlyId, out stChar));
        m_mapCharStorage.Remove(nOnlyId);
        m_nCharacterStorageCount = m_mapCharStorage.Count;
        
        Destroy(stChar.gameObject);
    }

    public IEnumerable<IBase_Friend_Character> EnumCharStorage()
    {
        foreach (KeyValuePair<int, IBase_Friend_Character> _stChar in m_mapCharStorage)
        {
            yield return _stChar.Value;
        }
    }

    public int GetCharStorageCount() { return m_mapCharStorage.Count; }

    public void CreateIdleOrderSignal(IBase_Friend_Character stChar)
    {
        GameCommon.CHECK(stChar != null);

        if (m_dgOnCreateIdleOrderSignal != null)
        {
            m_dgOnCreateIdleOrderSignal(stChar);
        }
    }
}