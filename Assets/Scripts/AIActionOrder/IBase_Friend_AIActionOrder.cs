using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;



//AIActionOrder这里代指【建筑发出指令，指令管理器存储指令，AI角色接取指令，寻路到目标点，触发特定行为】


public delegate void DGOn_F_AIActionCreateIdleSignal(IBase_Friend_Character stChar);

public enum EM_F_AIActionOrderType
{
    //请遵循依次往底下添加，禁止中间插入
    Invalid = -1,

    Idle,

    ProducingBowman,
    ProducingHammerman,
    ProducingFarmer,
    ProducingNearWarriorA,
    ProducingKnight,

    LevUpBuilding,
    RepairingBuilding,
    CuttingTree,

    Farming,

    Max,
};

public enum EM_F_AIActionOrderHandler
{
    Invalid = -1,

    Villager,
    Hammerman,
    Farmer,

    Max,
};

public class ST_F_AIActionOrder
{
    public ST_F_AIActionOrder(
        EM_F_AIActionOrderType emOType, 
        IBase_Friend_Building stTargetBuilding, 
        EM_F_AIActionOrderHandler emOHandler
        )
    {
        this.emOType = emOType;
        this.stTargetBuilding = stTargetBuilding;
        this.emOHandler = emOHandler;
    }

    public ST_F_AIActionOrder(ST_F_AIActionOrder stCopySrc)
    {
        GameCommon.CHECK(stCopySrc != null);
        this.emOType = stCopySrc.emOType;
        this.stTargetBuilding = stCopySrc.stTargetBuilding;
        this.emOHandler = stCopySrc.emOHandler;
    }

    public EM_F_AIActionOrderType GetOType() { return emOType; }
    public IBase_Friend_Building GettTargetBuilding() { return stTargetBuilding; }
    public EM_F_AIActionOrderHandler GetOHandler() { return emOHandler; }

    EM_F_AIActionOrderType emOType;
    IBase_Friend_Building stTargetBuilding;
    EM_F_AIActionOrderHandler emOHandler;
};



public class F_AIActionOrderManager : GameHelper.Singleton<F_AIActionOrderManager>
{   
    protected Dictionary<EM_F_AIActionOrderHandler, IBase_Friend_AIActionOrder> m_mapAIActionOrderHandler = new Dictionary<EM_F_AIActionOrderHandler, IBase_Friend_AIActionOrder>();
    
    public F_AIActionOrderManager()
    {
        for (EM_F_AIActionOrderHandler _emHandler = EM_F_AIActionOrderHandler.Invalid + 1; _emHandler < EM_F_AIActionOrderHandler.Max;
            _emHandler++)
        {
            switch(_emHandler)
            {
                case EM_F_AIActionOrderHandler.Villager:
                    {
                        m_mapAIActionOrderHandler.Add(_emHandler, new F_AIActionOrder2Villager(_emHandler));
                    }
                    break;
                case EM_F_AIActionOrderHandler.Hammerman:
                    {
                        m_mapAIActionOrderHandler.Add(_emHandler, new F_AIActionOrder2Hammerman(_emHandler));
                    }
                    break;
                case EM_F_AIActionOrderHandler.Farmer:
                    {
                        m_mapAIActionOrderHandler.Add(_emHandler, new F_AIActionOrder2Farmer(_emHandler));
                    }
                    break;
                default:
                    {
                        Debug.LogError("m_mapAIActionOrderHandler.Add : " + _emHandler.ToString());
                    }
                    break;
            }
        }
    }

    public IBase_Friend_AIActionOrder GetOrderHandler(EM_F_AIActionOrderHandler _emHandler)
    {
        IBase_Friend_AIActionOrder _stHandler;
        if (m_mapAIActionOrderHandler.TryGetValue(_emHandler, out _stHandler))
        {
            return _stHandler;
        }
        return null;
    }

    public void Initialization()
    {
        Minos_VillagerFactory.Instance.m_dgOnCreateIdleSignal += GetOrderHandler(EM_F_AIActionOrderHandler.Villager).OnCreateIdleOrderSignal;

        foreach (IBase_Friend_Building _stBuilding in Minos_BuildingManager.Instance.EnumAll_F_Building())
        {
            switch (_stBuilding.GetBuildingType())
            {
                case EM_F_BuildingType.F_HammermanFactory:
                    {
                        IBase_Friend_FactoryBuilding _stFactoryBuilding = _stBuilding as IBase_Friend_FactoryBuilding;
                        GameCommon.CHECK(_stFactoryBuilding != null);
                        _stFactoryBuilding.m_dgOnCreateIdleOrderSignal += GetOrderHandler(EM_F_AIActionOrderHandler.Hammerman).OnCreateIdleOrderSignal;
                    }
                    break;
                case EM_F_BuildingType.F_FarmerFactory:
                    {
                        IBase_Friend_FactoryBuilding _stFactoryBuilding = _stBuilding as IBase_Friend_FactoryBuilding;
                        GameCommon.CHECK(_stFactoryBuilding != null);
                        _stFactoryBuilding.m_dgOnCreateIdleOrderSignal += GetOrderHandler(EM_F_AIActionOrderHandler.Farmer).OnCreateIdleOrderSignal;
                    }
                    break;
            }
        }

        Minos_GameDateManager.Instance.m_dgOnIsDayComing += OnGameDateIsDayComing;
        Minos_GameDateManager.Instance.m_dgOnIsNightComing += OnGameDateIsNightComing;
    }
    

    public void Update()
    {
        foreach (KeyValuePair<EM_F_AIActionOrderHandler, IBase_Friend_AIActionOrder> _stPair in m_mapAIActionOrderHandler)
        {
            _stPair.Value.Update();
        }
    }

    void OnGameDateIsDayComing()
    {
        GetOrderHandler(EM_F_AIActionOrderHandler.Hammerman).OpenReTryLinkOrderFlag(true);

        foreach (IBase_Friend_Building _stBuilding in Minos_BuildingManager.Instance.EnumAll_F_Building())
        {
            switch (_stBuilding.GetBuildingType())
            {
                case EM_F_BuildingType.F_HammermanFactory:
                case EM_F_BuildingType.F_BowmanFactory:
                case EM_F_BuildingType.F_FarmerFactory:
                    {
                        IBase_Friend_FactoryBuilding _stFactory = _stBuilding as IBase_Friend_FactoryBuilding;
                        GameCommon.CHECK(_stFactory != null);
                        foreach (IBase_Friend_Character _stChar in _stFactory.EnumCharStorage())
                        {
                            _stChar.OnGameDate_IsDayComing();
                        }
                    }
                    break;
                case EM_F_BuildingType.F_Wall:
                    {
                        F_Wall _stFactory = _stBuilding as F_Wall;
                        GameCommon.CHECK(_stFactory != null);
                        _stFactory.OnGameDate_IsDayComing();
                    }
                    break;
                case EM_F_BuildingType.F_Farmland:
                    {
                        F_Farmland _stFactory = _stBuilding as F_Farmland;
                        GameCommon.CHECK(_stFactory != null);
                        _stFactory.OnGameDate_IsDayComing();
                    }
                    break;
            }
        }
    }

    void OnGameDateIsNightComing(bool bIsBloodNight, int nBloodNightIndex)
    {
        GetOrderHandler(EM_F_AIActionOrderHandler.Hammerman).OpenReTryLinkOrderFlag(false);

        foreach (IBase_Friend_Building _stBuilding in Minos_BuildingManager.Instance.EnumAll_F_Building())
        {
            switch (_stBuilding.GetBuildingType())
            {
                case EM_F_BuildingType.F_HammermanFactory:
                case EM_F_BuildingType.F_BowmanFactory:
                case EM_F_BuildingType.F_FarmerFactory:
                    {
                        IBase_Friend_FactoryBuilding _stFactory = _stBuilding as IBase_Friend_FactoryBuilding;
                        GameCommon.CHECK(_stFactory != null);
                        foreach (IBase_Friend_Character _stChar in _stFactory.EnumCharStorage())
                        {
                            _stChar.OnGameDate_IsNightComing(bIsBloodNight, nBloodNightIndex);
                        }
                    }
                    break;
                case EM_F_BuildingType.F_Wall:
                    {
                        F_Wall _stFactory = _stBuilding as F_Wall;
                        GameCommon.CHECK(_stFactory != null);
                        _stFactory.OnGameDate_IsNightComing(bIsBloodNight, nBloodNightIndex);
                    }
                    break;
                case EM_F_BuildingType.F_Farmland:
                    {
                        F_Farmland _stFactory = _stBuilding as F_Farmland;
                        GameCommon.CHECK(_stFactory != null);
                        _stFactory.OnGameDate_IsNightComing(bIsBloodNight, nBloodNightIndex);
                    }
                    break;
            }
        }
    }
};
















public abstract class IBase_Friend_AIActionOrder
{
    protected EM_F_AIActionOrderHandler m_emOrderHandler;
    protected List<ST_F_AIActionOrder> m_lstOrderCache = new List<ST_F_AIActionOrder>();
    
    protected float m_fReTryLinkOrderTimeStamp;
    protected const float m_fReTryLinkOrderTimeCD = 1.0f;
    protected bool m_bReTryLinkOrderFlag = false;

    //BehaviorTree-TaskName(任务名)
    public const string m_strTaskName_PatrolAroundFactory = "PatrolAroundFactory";

    //BehaviorTree-VariableName(局部变量名)
    public const string m_strVariableName_PatrolStartPoint = "PatrolStartPoint";
    public const string m_strVariableName_AIOrderType = "AIOrderType";
    public const string m_strVariableName_AIOrderMoveTo = "AIOrderMoveTo";
    public const string m_strVariableName_IsNight = "IsNight";


    public IBase_Friend_AIActionOrder(EM_F_AIActionOrderHandler emHandler)
    {
        m_bReTryLinkOrderFlag = false;
        m_emOrderHandler = emHandler;
    }


    public void CreateOrder(EM_F_AIActionOrderType emOType, IBase_Friend_Building stTargetBuilding)
    {
        GameCommon.CHECK(emOType > EM_F_AIActionOrderType.Invalid && emOType < EM_F_AIActionOrderType.Max);
        GameCommon.CHECK(stTargetBuilding != null);

        if (m_lstOrderCache.Exists(v => v.GettTargetBuilding() == stTargetBuilding))
        {
            //一个建筑只能发出一个指令，比如【墙壁被连续进攻】
            Debug.Log("CreateAIActionOrder.Exists: " + stTargetBuilding.ToString() + " -> " + emOType.ToString());
            return;
        }

#if UNITY_EDITOR
        MethodBase stMBase = System.Reflection.MethodBase.GetCurrentMethod();
        Debug.Log(stMBase.Name + " : " + emOType.ToString() + " | " + stTargetBuilding.name);
#endif
        stTargetBuilding.SetIsSelfSendedAIActionOrder(true);

        m_lstOrderCache.Add(new ST_F_AIActionOrder(emOType, stTargetBuilding, m_emOrderHandler));

        TryLinkOrder2TargetAIChar(m_lstOrderCache[0]);
    }

    public void GiveBackOrder(EM_F_AIActionOrderType emOType, IBase_Friend_Building stTargetBuilding)
    {
        GameCommon.CHECK(emOType > EM_F_AIActionOrderType.Invalid && emOType < EM_F_AIActionOrderType.Max);
        GameCommon.CHECK(stTargetBuilding != null);

        if (m_lstOrderCache.Exists(v => v.GettTargetBuilding() == stTargetBuilding))
        {
            //一个建筑只能发出一个指令，比如【墙壁被连续进攻】
            Debug.Log("GiveBackAIActionOrder.Exists: " + stTargetBuilding.ToString() + " -> " + emOType.ToString());
            return;
        }

#if UNITY_EDITOR
        MethodBase stMBase = System.Reflection.MethodBase.GetCurrentMethod();
        Debug.Log(stMBase.Name + " : " + emOType.ToString() + " | " + stTargetBuilding.name);
#endif
        m_lstOrderCache.Add(new ST_F_AIActionOrder(emOType, stTargetBuilding, m_emOrderHandler));
    }

    public void OpenReTryLinkOrderFlag(bool bIsOpen)
    {
        m_bReTryLinkOrderFlag = bIsOpen;
    }

    protected void DeleteOrder(ST_F_AIActionOrder stOrder)
    {
        //该指令的Clone刚转移到目标Char，则此时从列表中销毁该指令
        GameCommon.CHECK(stOrder != null);
        GameCommon.CHECK(m_lstOrderCache.Remove(stOrder));

#if UNITY_EDITOR
        MethodBase stMBase = System.Reflection.MethodBase.GetCurrentMethod();
        Debug.Log(stMBase.Name + " : " + stOrder.GetOType().ToString() + " | " + stOrder.GettTargetBuilding().name);
#endif

        stOrder = null;
    }

    public virtual void Update() { }

    public EM_F_AIActionOrderHandler GetOrderHandler() { return m_emOrderHandler; }

    public void OnCreateIdleOrderSignal(IBase_Friend_Character stChar)
    {
        TryLinkOrder2TargetAIChar(stChar);
    }
    
    protected abstract IBase_Friend_Character FindIdleAICharacter(ST_F_AIActionOrder stOrder);

    protected bool TryLinkOrder2TargetAIChar(ST_F_AIActionOrder stOrder)
    {
        GameCommon.CHECK(stOrder != null);
        GameCommon.CHECK(stOrder.GettTargetBuilding() != null);
        
        IBase_Friend_Character stIdleChar = FindIdleAICharacter(stOrder);
        if (stIdleChar != null)
        {
            TryLinkOrder2TargetAIChar(stOrder, stIdleChar);
            return true;
        }
        return false;
    }

    bool TryLinkOrder2TargetAIChar(IBase_Friend_Character stIdleChar)
    {
        GameCommon.CHECK(stIdleChar != null);

        if (m_lstOrderCache.Count > 0)
        {
            TryLinkOrder2TargetAIChar(m_lstOrderCache[0], stIdleChar);
            return true;
        }

        TryLinkOrder2TargetAIChar(null, stIdleChar);
        return false;
    }

    protected abstract void TryLinkOrder2TargetAIChar(ST_F_AIActionOrder stOrder, IBase_Friend_Character stIdleChar);

    public void DebugPrint()
    {
        string strLog = null;

        strLog += "IBaseAIActionOrder.DebugPrint : \n";
        foreach (ST_F_AIActionOrder _stOrder in m_lstOrderCache)
        {
            strLog += _stOrder.GetOType().ToString();
            strLog += " | ";
            strLog += _stOrder.GettTargetBuilding().gameObject.name;
            strLog += "\n";
        }
        strLog += "\n";
        Debug.Log(strLog);
        
    }
}
