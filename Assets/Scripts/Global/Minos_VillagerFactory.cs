using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minos_VillagerFactory : GameHelper.Singleton<Minos_VillagerFactory>
{
    List<F_VillagerCharacter> m_lstVillagerCharacter = new List<F_VillagerCharacter>();
    static int m_nStaticVillagerId = 1000;
    public DGOn_F_AIActionCreateIdleSignal m_dgOnCreateIdleSignal;




    public void IncreaseVillager(Vector3 v3Position, Vector3 v3Dir)
    {
        int nOnlyId = ++m_nStaticVillagerId;
        F_VillagerCharacter stChar = GameHelper_F_Character.InstantiateCharacters<F_VillagerCharacter>(
            EM_F_CharacterType.F_Villager,
            nOnlyId,
            null,
            v3Position,
            Quaternion.identity,
            Vector3.one
            );
        GameCommon.CHECK(stChar != null, "Missing <????Character> Script !");
        Debug.Log("IncreaseVillager: " + stChar.name);
        stChar.transform.position = v3Position;
        stChar.SetCurrentDirection(v3Dir);
        stChar.SetOnlyId(nOnlyId, EM_F_CharacterType.F_Villager, m_lstVillagerCharacter.Count);
        stChar.SetLv(0);
        stChar.SetParentFactory(null);
        stChar.SetPatrolPath(Minos_BuildingManager.Instance.GetPlayerHomeland().m_lstPatrolSelf);

        m_lstVillagerCharacter.Add(stChar);

        if (m_dgOnCreateIdleSignal != null)
        {
            m_dgOnCreateIdleSignal(stChar);
        }
    }

    public void DecreaseVillager(int nOnlyId)
    {
        Debug.Log("DecreaseVillager: " + nOnlyId);
        GameCommon.CHECK(nOnlyId > 0);        

        F_VillagerCharacter stRealChar = m_lstVillagerCharacter.Find(v => v.GetOnlyId() == nOnlyId);
        GameCommon.CHECK(stRealChar != null);
        m_lstVillagerCharacter.Remove(stRealChar);

        UnityEngine.Object.Destroy(stRealChar.gameObject);
    }

    public F_VillagerCharacter FindIdleVillager(Vector3 v3BuildingPos)
    {
        float fDisBetween = float.MaxValue;
        F_VillagerCharacter stCharNearest = null;
        foreach (F_VillagerCharacter _stChar in m_lstVillagerCharacter)
        {
            if (_stChar.GetAIActionOrder() == null)
            {
                float _fDis = Vector3.Distance(_stChar.transform.position, v3BuildingPos);
                if (_fDis <= fDisBetween)
                {
                    //找出最近的闲置村民
                    fDisBetween = _fDis;
                    stCharNearest = _stChar;
                }
            }
        }
        
        return stCharNearest;
    }
}