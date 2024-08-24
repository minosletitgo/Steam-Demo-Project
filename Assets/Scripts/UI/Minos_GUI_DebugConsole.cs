using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class Minos_GUI_DebugConsole : MonoBehaviour
{
    [SerializeField]
    Button m_btnShowInputField;

    [SerializeField]
    GameObject m_goInputField;

    [SerializeField]
    InputField m_inputField;

    [SerializeField]
    Button m_btnSubmit;



    [SerializeField]
    [ReadOnly]
    List<string> m_vStrCache = new List<string>();
    [SerializeField]
    [ReadOnly]
    int m_nCurSelect = -1;




    private void Awake()
    {
        m_btnShowInputField.onClick.AddListener(
            delegate()
            {
                SwitchShowInputField();
            }
            );
        m_btnSubmit.onClick.AddListener(
            delegate ()
            {
                OnInputFieldSubmit(m_inputField.text);
            }
            );
    }

    private void Start()
    {
        SwitchShowInputField();
    }

    void SwitchShowInputField()
    {
        m_goInputField.SetActive(!m_goInputField.activeSelf);
        if (m_goInputField.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(m_inputField.gameObject);
        }
        else if (EventSystem.current.currentSelectedGameObject == m_inputField.gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    void OnInputFieldSubmit(string strInput)
    {
        ProcGMCMD(strInput);

        if (!string.IsNullOrEmpty(strInput))
        {
            if (m_vStrCache.Count >= 10)
            {
                m_vStrCache.RemoveAt(0);
            }
            m_vStrCache.Add(strInput);
        }
        m_inputField.text = null;
        m_nCurSelect = -1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) &&
            m_goInputField.gameObject.activeSelf &&
            m_vStrCache.Count > 0 &&
            EventSystem.current.currentSelectedGameObject == m_inputField.gameObject
            )
        {
            if (m_nCurSelect == -1)
            {
                m_nCurSelect = m_vStrCache.Count - 1;
            }
            else if (m_nCurSelect >= 1)
            {
                m_nCurSelect--;
            }
            m_inputField.text = m_vStrCache[m_nCurSelect];
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) &&
            m_goInputField.gameObject.activeSelf &&
            m_vStrCache.Count > 0 &&
            EventSystem.current.currentSelectedGameObject == m_inputField.gameObject
            )
        {
            if (m_nCurSelect == -1)
            {
                m_nCurSelect = m_vStrCache.Count - 1;
            }
            else if (m_nCurSelect < (m_vStrCache.Count - 1))
            {
                m_nCurSelect++;
            }
            m_inputField.text = m_vStrCache[m_nCurSelect];
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SwitchShowInputField();

            if (!m_goInputField.activeSelf)
            {
                OnInputFieldSubmit(m_inputField.text);
            }
        }
    }

    void LateUpdate()
    {

    }

    bool ProcGMCMD(string strInput)
    {
        if (string.IsNullOrWhiteSpace(strInput) || !strInput.StartsWith("/"))
        {
            return false;
        }

        char[] charSeparators = new char[] { ' ' };
        string[] strSplits = strInput.Split(charSeparators, System.StringSplitOptions.RemoveEmptyEntries);

        if (strSplits.Length <= 0 || !strSplits[0].StartsWith("/"))
        {
            return false;
        }

        string strCMD = strSplits[0].Substring(1);
        switch (strCMD)
        {
            case "AddCurrentHealth":
                {
                    /*
                        /AddCurrentHealth 7
                    */
                    if (strSplits.Length == 2)
                    {
                        int nValue = int.Parse(strSplits[1]);
                        if (nValue > 0)
                        {
                            Player.Inst.AddCurrentHealth(Math.Abs(nValue));
                        }
                        else
                        {
                            Player.Inst.ReduceCurrentHealth(Math.Abs(nValue));
                        }
                    }
                }
                break;
            case "AddHammerman":
                {
                    /*
                        /AddHammerman 1
                    */
                    if (strSplits.Length == 2)
                    {
                        int nCount = int.Parse(strSplits[1]);
                        if (nCount > 0)
                        {
                            GameObject goFactory = GameObject.Find("F_HammermanFactory");
                            F_HammermanFactory stFactory = goFactory.GetComponent<F_HammermanFactory>();
                            for (int i = 0; i < nCount; i++)
                            {
                                stFactory.InstantiateCharacter();
                            }
                        }
                    }
                }
                break;
            case "AddBowman":
                {
                    /*
                        /AddBowman 1
                    */
                    if (strSplits.Length == 2)
                    {
                        int nCount = int.Parse(strSplits[1]);
                        if (nCount > 0)
                        {
                            GameObject goFactory = GameObject.Find("F_BowmanFactory");
                            F_BowmanFactory stFactory = goFactory.GetComponent<F_BowmanFactory>();
                            for (int i = 0; i < nCount; i++)
                            {
                                stFactory.InstantiateCharacter();
                            }
                        }
                    }
                }
                break;
            case "AddFarmer":
                {
                    /*
                        /AddFarmer 1
                    */
                    if (strSplits.Length == 2)
                    {
                        int nCount = int.Parse(strSplits[1]);
                        if (nCount > 0)
                        {
                            GameObject goFactory = GameObject.Find("F_FarmerFactory");
                            F_FarmerFactory stFactory = goFactory.GetComponent<F_FarmerFactory>();
                            for (int i = 0; i < nCount; i++)
                            {
                                stFactory.InstantiateCharacter();
                            }
                        }
                    }
                }
                break;
            case "AddNearWarriorA":
                {
                    /*
                        /AddNearWarriorA
                    */
                    if (strSplits.Length == 1)
                    {
                        GameObject goFactory = GameObject.Find("F_NearWarriorAFactory");
                        F_NearWarriorAFactory stFactory = goFactory.GetComponent<F_NearWarriorAFactory>();
                        stFactory.InstantiateCharacter();
                    }
                }
                break;
            case "AddKnight":
                {
                    /*
                        /AddKnight
                    */
                    if (strSplits.Length == 1)
                    {
                        GameObject goFactory = GameObject.Find("F_KnightFactory");
                        F_KnightFactory stFactory = goFactory.GetComponent<F_KnightFactory>();
                        stFactory.InstantiateCharacter();
                    }
                }
                break;
            case "AddMonster":
                {
                    /*
                        /AddMonster 1
                    */
                    if (strSplits.Length == 2)
                    {
                        int nCount = int.Parse(strSplits[1]);
                        if (nCount > 0)
                        {
                            GameObject goFactory = GameObject.Find("E_MonsterFactory");
                            E_MonsterFactory stFactory = goFactory.GetComponent<E_MonsterFactory>();
                            stFactory.DebugInstantiateCharacter(nCount);
                        }
                    }
                    else if (strSplits.Length == 1)
                    {
                        GameObject goFactory = GameObject.Find("E_MonsterFactory");
                        E_MonsterFactory stFactory = goFactory.GetComponent<E_MonsterFactory>();
                        stFactory.DebugInstantiateCharacter();
                    }
                }
                break;
            case "DamageWall":
                {
                    /*
                        /DamageWall 1
                    */
                    if (strSplits.Length == 2)
                    {
                        int nValue = int.Parse(strSplits[1]);
                        if (nValue > 0)
                        {
                            GameObject goWall = GameObject.Find("F_Wall");
                            F_Wall stWall = goWall.GetComponent<F_Wall>();
                            stWall.DebugDamageHealth(nValue);
                        }
                    }
                }
                break;
            case "DamageTower":
                {
                    /*
                        /DamageTower 1
                    */
                    if (strSplits.Length == 2)
                    {
                        int nValue = int.Parse(strSplits[1]);
                        if (nValue > 0)
                        {
                            GameObject goWall = GameObject.Find("F_BowmanTower");
                            F_BowmanTower stTower = goWall.GetComponent<F_BowmanTower>();
                            stTower.DebugDamageHealth(nValue);
                        }
                    }
                }
                break;
            case "ReviveWall":
                {
                    /*
                        /ReviveWall
                    */
                    if (strSplits.Length == 1)
                    {
                        GameObject goWall = GameObject.Find("F_Wall");
                        F_Wall stWall = goWall.GetComponent<F_Wall>();
                        stWall.ReviveHealth();
                    }
                }
                break;
            case "IBaseAIActionOrder.DebugPrint":
                {
                    /*
                        /IBaseAIActionOrder.DebugPrint
                    */
                    if (strSplits.Length == 1)
                    {
                        F_AIActionOrderManager.Instance.GetOrderHandler(EM_F_AIActionOrderHandler.Villager).
                            DebugPrint();
                        F_AIActionOrderManager.Instance.GetOrderHandler(EM_F_AIActionOrderHandler.Hammerman).
                            DebugPrint();
                    }
                }
                break;
            //case "F_HammermanFactory.InterruptConnectAIActionOrder":
            //    {
            //        /*
            //            /F_HammermanFactory.InterruptConnectAIActionOrder
            //        */
            //        if (strSplits.Length == 1)
            //        {
            //            foreach (IBaseBuilding _stBuilding in Minos_BuildingManager.Instance.EnumAllBuilding())
            //            {
            //                switch (_stBuilding.GetBuildingType())
            //                {
            //                    case EM_BuildingType.F_HammermanFactory:
            //                        {
            //                            F_HammermanFactory _stFactory = _stBuilding as F_HammermanFactory;
            //                            GameCommon.CHECK(_stFactory != null);
            //                            foreach (IBaseAICharacter _stChar in _stFactory.EnumCharStorage())
            //                            {
            //                                _stChar.InterruptConnectAIActionOrder();
            //                            }
            //                        }
            //                        break;
            //                }
            //            }
            //        }
            //    }
            //    break;
            case "DebugSet_OnDayIndexChg":
                {
                    /*
                        /DebugSet_OnDayIndexChg 5
                    */
                    if (strSplits.Length == 2)
                    {
                        int nIndex = int.Parse(strSplits[1]);
                        Minos_GameDateManager.Instance.DebugSet_OnDayIndexChg(nIndex);
                    }
                }
                break;
            case "DebugSet_OnIsDayComing":
                {
                    /*
                        /DebugSet_OnIsDayComing
                    */
                    if (strSplits.Length == 1)
                    {
                        Minos_GameDateManager.Instance.DebugSet_OnIsDayComing();
                    }
                }
                break;
            case "DebugSet_OnIsNightComing":
                {
                    /*
                        /DebugSet_OnIsNightComing 0 0
                    */
                    if (strSplits.Length == 3)
                    {
                        bool bIsBloodNight = int.Parse(strSplits[1]) > 0;
                        int nBloodNightIndex = int.Parse(strSplits[2]);
                        Minos_GameDateManager.Instance.DebugSet_OnIsNightComing(bIsBloodNight, nBloodNightIndex);
                    }
                }
                break;
            case "DebugSet_OnSeasonIndexChg":
                {
                    /*
                        /DebugSet_OnSeasonIndexChg Spring Winter
                    */
                    if (strSplits.Length == 3)
                    {
                        Minos_GameDateManager.EM_Season emBefore = (Minos_GameDateManager.EM_Season)Enum.Parse(typeof(Minos_GameDateManager.EM_Season), strSplits[1]);
                        Minos_GameDateManager.EM_Season emAfter = (Minos_GameDateManager.EM_Season)Enum.Parse(typeof(Minos_GameDateManager.EM_Season), strSplits[2]);
                        Minos_GameDateManager.Instance.DebugSet_OnSeasonIndexChg(emBefore, emAfter);
                    }
                }
                break;
            case "RunStart":
                {
                    /*
                        /RunStart
                    */
                    if (strSplits.Length == 1)
                    {
                        F_VillagerCharacter stChar = GameHelper.GoFind<F_VillagerCharacter>();
                        if (stChar != null)
                        {
                            stChar.DoCharacterAbility_RunStart();
                        }
                    }
                }
                break;
            case "RunStop":
                {
                    /*
                        /RunStop
                    */
                    if (strSplits.Length == 1)
                    {
                        F_VillagerCharacter stChar = GameHelper.GoFind<F_VillagerCharacter>();
                        if (stChar != null)
                        {
                            stChar.DoCharacterAbility_RunStop();
                        }
                    }
                }
                break;
            case "Attack":
                {
                    /*
                        /Attack 1
                    */
                    if (strSplits.Length == 2)
                    {
                        bool bStart = int.Parse(strSplits[1]) > 0;

                        E_MonsterNearCharacter stChar = GameHelper.GoFind<E_MonsterNearCharacter>();
                        if (stChar != null)
                        {
                            stChar.DoCharacterAbility_Shoot(bStart);
                        }
                    }
                }
                break;
            case "DebugDead":
                {
                    /*
                        /DebugDead
                    */
                    if (strSplits.Length == 1)
                    {
                        Vector3 v3Foot = Vector3.zero;
                        F_VillagerCharacter _stChar = GameHelper.GoFind<F_VillagerCharacter>();
                        if (_stChar != null)
                        {
                            Minos_VillagerFactory.Instance.DecreaseVillager(_stChar.GetOnlyId());
                            v3Foot = _stChar.transform.position;                            
                        }
                        else
                        {
                            v3Foot = Player.Inst.transform.position;
                            //v3Foot = Vector3.zero;
                        }

                        GameHelper.GoFind<F_PrimitivemanFactory>().InstantiateCharacter(v3Foot, Vector3.zero);
                    }
                }
                break;
            //case "DebugHarvestGetMoneyCoin":
            //    {
            //        /*
            //            /DebugHarvestGetMoneyCoin
            //        */
            //        if (strSplits.Length == 1)
            //        {
            //            F_Farmland stChar = GameHelper.GoFind<F_Farmland>();
            //            stChar.DebugHarvestGetMoneyCoin();
            //        }
            //    }
            //    break;
            case "Get_F_DefendLeftWall":
                {
                    /*
                        /Get_F_DefendLeftWall
                    */
                    if (strSplits.Length == 1)
                    {
                        F_Wall stWall = Minos_BuildingManager.Instance.Get_F_DefendLeftWall();
                        GameCommon.CHECK(stWall != null);
                        Debug.Log("Get_F_DefendLeftWall = " + stWall.name + " | " + stWall.GetOnlyId());
                    }
                }
                break;
            case "Get_F_DefendRightWall":
                {
                    /*
                        /Get_F_DefendRightWall
                    */
                    if (strSplits.Length == 1)
                    {
                        F_Wall stWall = Minos_BuildingManager.Instance.Get_F_DefendRightWall();
                        GameCommon.CHECK(stWall != null);
                        Debug.Log("Get_F_DefendRightWall = " + stWall.name + " | " + stWall.GetOnlyId());
                    }
                }
                break;
            case "Player_HealthRevive":
                {
                    /*
                        /Player_HealthRevive
                    */
                    if (strSplits.Length == 1)
                    {
                        Player.Inst.HealthRevive();
                    }
                }
                break;
            case "fuck":
                {
                    /*
                        /fuck
                    */
                    if (strSplits.Length == 1)
                    {
                        GameHelper.GoFind<Minos_GameDateManager>().gameObject.SetActive(false);                        
                        GameHelper.GoFind<F_PrimitivemanFactory>().gameObject.SetActive(false);
                        GameHelper.GoFind<E_MonsterFactory>().gameObject.SetActive(false);
                        //GameHelper.GoFind<F_RabbitFactory>().gameObject.SetActive(false);
                        F_RabbitFactory[] aryRabbitFactory = UnityEngine.Object.FindObjectsOfType<F_RabbitFactory>();
                        for (int i = 0; i < aryRabbitFactory.Length; i++)
                        {
                            //aryRabbitFactory[i].gameObject.SetActive(false);
                        }
                    }
                }
                break;
            case "Face":
                {
                    /*
                        /Face North
                    */
                    if (strSplits.Length == 2)
                    {
                        Character.FacingDirections emDir;
                        GameHelper.GetEnum<Character.FacingDirections>(strSplits[1], out emDir);
                        //GameHelper.GoFind<F_BowmanCharacter>().GetTDCharacter().Orientation3D.Face(emDir);
                        //Player.Inst.GetTDChar().Orientation3D.Face(emDir);

                        F_BowmanCharacter stBowman = GameHelper.GoFind<F_BowmanCharacter>();
                        stBowman.ForceFaceDir(emDir);
                    }
                }
                break;
        }

        return true;
    }
}