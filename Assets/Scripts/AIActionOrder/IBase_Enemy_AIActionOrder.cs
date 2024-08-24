using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class E_AIActionOrderManager : GameHelper.Singleton<E_AIActionOrderManager>
{
    //当下只是模仿【F_AIActionOrderManager】，但还没必要拆分出某个工种的Order
    //BehaviorTree-TaskName(任务名)
    public const string m_strTaskName_PatrolAroundFactory = "PatrolAroundFactory";

    //BehaviorTree-VariableName(局部变量名)
    public const string m_strVariableName_PatrolStartPoint = "PatrolStartPoint";
    //public const string m_strVariableName_AIOrderType = "AIOrderType";
    //public const string m_strVariableName_AIOrderMoveTo = "AIOrderMoveTo";
    public const string m_strVariableName_IsNight = "IsNight";





    public void Initialization()
    {
        Minos_GameDateManager.Instance.m_dgOnIsDayComing += OnGameDateIsDayComing;
        Minos_GameDateManager.Instance.m_dgOnIsNightComing += OnGameDateIsNightComing;
    }

    void OnGameDateIsDayComing()
    {
        foreach (IBase_Enemy_Building _stBuilding in Minos_BuildingManager.Instance.EnumAll_E_Building())
        {
            switch (_stBuilding.GetBuildingType())
            {
                case EM_E_BuildingType.E_MonsterFactory:
                    {
                        E_MonsterFactory _stFactory = _stBuilding as E_MonsterFactory;
                        GameCommon.CHECK(_stFactory != null);
                        _stFactory.OnGameDate_IsDayComing();
                    }
                    break;
            }
        }
    }

    void OnGameDateIsNightComing(bool bIsBloodNight, int nBloodNightIndex)
    {
        foreach (IBase_Enemy_Building _stBuilding in Minos_BuildingManager.Instance.EnumAll_E_Building())
        {
            switch (_stBuilding.GetBuildingType())
            {
                case EM_E_BuildingType.E_MonsterFactory:
                    {
                        E_MonsterFactory _stFactory = _stBuilding as E_MonsterFactory;
                        GameCommon.CHECK(_stFactory != null);
                        _stFactory.OnGameDate_IsNightComing(bIsBloodNight, nBloodNightIndex);
                    }
                    break;
            }
        }
    }
}
