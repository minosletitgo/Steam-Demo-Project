using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public abstract class IBase_Enemy_FactoryBuilding : IBase_Enemy_Building
{
    [ReadOnly]
    [SerializeField]
    GameObject m_goBornCharacterRoot;
    
    [ReadOnly]
    [SerializeField]
    protected int m_nCharacterStorageCount;
    protected Dictionary<int, IBase_Enemy_Character> m_mapCharStorage = new Dictionary<int, IBase_Enemy_Character>();





    protected override void Awake()
    {
        base.Awake();
        
        m_goBornCharacterRoot = transform.Find("BornCharacterRoot").gameObject;
        GameCommon.CHECK(m_goBornCharacterRoot != null, "m_goBornCharacterRoot != null : " + gameObject.name);

        m_nStaticCharacterId = m_nStaticCharacterStartId;
    }



    public Transform GetBornCharacterRoot() { return m_goBornCharacterRoot.transform; }

    int m_nStaticCharacterStartId = 1000;
    public int GetCharacterStartId() { return m_nStaticCharacterStartId; }

    int m_nStaticCharacterId = 0;//每个工厂的独有角色id
    protected int AllcocCharacterId()
    {
        return ++m_nStaticCharacterId;
    }

    protected virtual IBase_Enemy_Character InstantiateCharacter(EM_E_CharacterType emCharType)
    {
        GameCommon.CHECK(emCharType > EM_E_CharacterType.Invalid && emCharType < EM_E_CharacterType.Max);

        int nOnlyId = AllcocCharacterId();
        IBase_Enemy_Character stChar = GameHelper_E_Character.InstantiateCharacters<IBase_Enemy_Character>(
            emCharType,
            nOnlyId,
            GetBornCharacterRoot(),
            Vector3.zero,
            Quaternion.identity,
            Vector3.one
            );
        GameCommon.CHECK(stChar != null, "Missing <????Character> Script !");
        Debug.Log("InstantiateCharacter: " + gameObject.name + " | " + stChar.name);
        stChar.SetOnlyId(nOnlyId, emCharType);
        stChar.SetLv(1);
        stChar.SetParentFactory(this);
        stChar.SetPatrolPath(m_lstPatrolSelf);

        IncreaseCharacter(stChar.GetOnlyId(), stChar);

        return stChar;
    }

    public virtual void IncreaseCharacter<T>(int nOnlyId, T stChar) where T : IBase_Enemy_Character
    {
        GameCommon.CHECK(nOnlyId > 0);

        IBase_Enemy_Character stRealChar = stChar as IBase_Enemy_Character;
        GameCommon.CHECK(stRealChar != null);

        m_mapCharStorage.Add(nOnlyId, stRealChar);
        m_nCharacterStorageCount = m_mapCharStorage.Count;
    }

    public virtual void DecreaseCharacter(int nOnlyId)
    {
        GameCommon.CHECK(nOnlyId > 0);

        Debug.Log("DecreaseCharacter: " + nOnlyId.ToString() + " | " + gameObject.name);

        IBase_Enemy_Character stChar;
        GameCommon.CHECK(m_mapCharStorage.TryGetValue(nOnlyId, out stChar));
        m_mapCharStorage.Remove(nOnlyId);
        m_nCharacterStorageCount = m_mapCharStorage.Count;

        Destroy(stChar.gameObject);
    }

    public IEnumerable<IBase_Enemy_Character> EnumCharStorage()
    {
        foreach (KeyValuePair<int, IBase_Enemy_Character> _stChar in m_mapCharStorage)
        {
            yield return _stChar.Value;
        }
    }

    public int GetCharStorageCount() { return m_mapCharStorage.Count; }
}
