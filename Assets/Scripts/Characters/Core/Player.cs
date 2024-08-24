using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using MoreMountains.Feedbacks;


public class Player : MonoBehaviour, MMEventListener<MMDamageTakenEvent>
{
    public delegate void OnCurrentHealthChg(int nBefore, int nAfter);
    public OnCurrentHealthChg m_dgOnCurrentHealthChg;


    [SerializeField]
    MMFeedbacks m_stPickMoneyCoin;

    //private stuff
    Character m_stChar;
    Minos_Health m_stHealth;





    static Player m_inst;
    public static Player Inst
    {
        get
        {
            if (m_inst == null) { m_inst = GameHelper.GoFind<Player>(); }
            return m_inst;
        }
    }

    void Awake()
    {
        m_stChar = GetComponent<Character>();
        GameCommon.CHECK(m_stChar != null);
        m_stHealth = GetComponent<Minos_Health>();
        GameCommon.CHECK(m_stHealth != null);
        GameCommon.CHECK(m_stHealth.CurrentHealth >= 0);
        GameCommon.CHECK(m_stHealth.CurrentHealth <= m_stHealth.MaximumHealth);

        m_stPickMoneyCoin?.Initialization();

        Minos_ThrowMoneyCoinHelper.Instance.Register(
            this,
            "Player1_MoneyCoin",
            LayerMask.NameToLayer("Default"),
            m_stChar.CharacterModel,
            delegate () { return GetCurrentHealth(); },
            delegate (int nValue) { ReduceCurrentHealth(nValue); }
            );
    }

    void Start()
    {
        //Debug.Log("m_stHealth.CurrentHealth = "+ m_stHealth.CurrentHealth);
    }

    private void OnDestroy()
    {
        if (Minos_ThrowMoneyCoinHelper.Instance != null)
        {
            Minos_ThrowMoneyCoinHelper.Instance.StopMonitorUnderBuilding();
            Minos_ThrowMoneyCoinHelper.Instance.UnRegister();
        }

        GameCommon.CHECK(m_inst == null || m_inst == this);
        m_inst = null;
    }

    private void Update()
    {
        bool isSelectedInputField = false;
        if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
        {
            isSelectedInputField = false;
        }
        else if (EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null)
        {
            isSelectedInputField = true;
        }

        if(!isSelectedInputField)
        {
            Minos_ThrowMoneyCoinHelper.Instance.UpdateRegister();
            Minos_ThrowMoneyCoinHelper.Instance.UpdateMonitorUnderBuilding();
        }
    }

    void OnEnable()
    {
        this.MMEventStartListening<MMDamageTakenEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<MMDamageTakenEvent>();
    }

    public void OnMMEvent(MMDamageTakenEvent gameEvent)
    {
        if (gameEvent.AffectedCharacter == m_stChar)
        {
            /*
                 可能是[加血]，也可能是[减血]
                 PreviousHealth + DamageCaused = CurrentHealth
            */
            if (m_dgOnCurrentHealthChg != null)
            {
                m_dgOnCurrentHealthChg(
                    (int)gameEvent.PreviousHealth,
                    (int)gameEvent.CurrentHealth
                    );
            }
        }
    }

    public Character GetTDChar() { return m_stChar; }

    #region ****************************Health************************************
    public int GetInitialHealth()
    {
        return m_stHealth.InitialHealth;
    }

    public int GetCurrentHealth() 
    {
        return m_stHealth.CurrentHealth;
    }
    
    public void AddCurrentHealth(int nValue)
    {
        if (nValue <= 0)
        {
            return;
        }

        m_stHealth.GetHealth(nValue, null);
    }

    public void ReduceCurrentHealth(int nValue)
    {
        GameCommon.CHECK(nValue > 0);

        m_stHealth.Damage(nValue, null, 0, 0);
    }
    #endregion ****************************Health************************************






    #region ****************************UnderBuilding************************************
    public void DoUnderBuilding(EM_F_BuildingType emBuildingType, GameObject goBuilding)
    {
        GameCommon.CHECK(emBuildingType > EM_F_BuildingType.Invalid && emBuildingType < EM_F_BuildingType.Max);
        GameCommon.CHECK(goBuilding != null);

        IBase_Friend_Building stBuilding = goBuilding.GetComponent<IBase_Friend_Building>();
        GameCommon.CHECK(stBuilding != null);
        Minos_ThrowMoneyCoinHelper.Instance.StartMonitorUnderBuilding(
            emBuildingType,
            goBuilding,
            stBuilding.GetMoneyCoinBar(),
            delegate (int nValue) { ReduceCurrentHealth(nValue); },
            delegate () { stBuilding.OnMoneyCoinFinished(); }
            );
    }

    public void UndoUnderBuilding()
    {
        Minos_ThrowMoneyCoinHelper.Instance.StopMonitorUnderBuilding();
    }
    #endregion ****************************UnderBuilding************************************


    public void PickMoneyCoin(PickItem2MoneyCoin stMoneyCoin)
    {
        GameCommon.CHECK(stMoneyCoin != null);

        m_stPickMoneyCoin?.PlayFeedbacks();

        AddCurrentHealth(stMoneyCoin.GetConfigMoneyCoin());

        Destroy(stMoneyCoin.gameObject);
    }

    public bool IsCanInteractiveWithBuilding(EM_F_BuildingType emBuildingType)
    {
        switch(emBuildingType)
        {
            case EM_F_BuildingType.F_Homeland:
                {
                    return true;
                }
            case EM_F_BuildingType.F_PrimitivemanFactory:
                {
                    return false;
                }
            case EM_F_BuildingType.F_HammermanFactory:
            case EM_F_BuildingType.F_HammermanTotem:
            case EM_F_BuildingType.F_BowmanFactory:
            case EM_F_BuildingType.F_BowmanTower:
            case EM_F_BuildingType.F_BowmanTotem:
            case EM_F_BuildingType.F_FarmerTotem:
                {
                    return (Minos_BuildingManager.Instance.GetPlayerHomeland().GetCurLev() > 0);
                }
            case EM_F_BuildingType.F_FarmerFactory:
            case EM_F_BuildingType.F_Farmland:
                {
                    return (Minos_BuildingManager.Instance.GetPlayerHomeland().GetCurLev() >= 2);
                }
            case EM_F_BuildingType.F_NearWarriorAFactory:
            case EM_F_BuildingType.F_NearWarriorATotem:
            case EM_F_BuildingType.F_KnightFactory:
            case EM_F_BuildingType.F_KnightTotem:
                {
                    return (Minos_BuildingManager.Instance.GetPlayerHomeland().GetCurLev() >= 2);
                }
            case EM_F_BuildingType.F_Wall:
            case EM_F_BuildingType.F_Tree:
                {
                    return (Minos_BuildingManager.Instance.GetPlayerHomeland().GetCurLev() > 0);
                }
            case EM_F_BuildingType.F_RabbitFactory:
                {
                    return false;
                }
            default:
                {
                    return false;
                }
        }
    }

    public void HealthRevive()
    {
        m_stHealth.Revive();
    }
}
