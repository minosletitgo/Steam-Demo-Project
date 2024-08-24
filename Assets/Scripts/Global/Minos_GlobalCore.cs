using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

/*
    01.全局逻辑Core，具备【Singleton、MonoBehaviour、MMGameEvent】
    02.如果具备【登录场景】和【游戏场景】，那么【GlobalCore】应该是static，且今早初始化(放置在【登录场景】)
*/

public class Minos_GlobalCore : MonoBehaviour, 
                                MMEventListener<MMGameEvent>,
                                MMEventListener<TopDownEngineEvent>
{
    static Minos_GlobalCore m_inst = null;
    public static Minos_GlobalCore Inst
    {
        get
        {
            if (m_inst == null)
            {
                m_inst = GameHelper.GoFind<Minos_GlobalCore>();
            }
            if (m_inst == null) 
            { 
                GameObject go = new GameObject("Minos_GlobalCore"); 
                m_inst = go.AddComponent<Minos_GlobalCore>(); 
            }
            return m_inst;
        }
    }
    public static bool HasInst { get { return m_inst != null; } }
    static void _Destroy() { m_inst = null; }
    void OnDestroy()
    {
        GameCommon.CHECK(m_inst == null || m_inst == this); _Destroy();
    }

    void Awake()
    {
        Debug.Log(gameObject.name);
        //DontDestroyOnLoad(gameObject);

        GameCommon.CHECK(Minos_CTBLInfo.Inst.LoadEverydayEnemy("Config/Tbl/EverydayEnemy"));
        GameCommon.CHECK(Minos_CTBLInfo.Inst.LoadF_CharacterAttr("Config/Tbl/F_CharacterAttr"));
        GameCommon.CHECK(Minos_CTBLInfo.Inst.LoadE_CharacterAttr("Config/Tbl/E_CharacterAttr"));
        GameCommon.CHECK(Minos_CTBLInfo.Inst.LoadF_Wall("Config/Tbl/F_Wall"));

        //New_Player 穿透 New_MyPeoples
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("New_Player"), LayerMask.NameToLayer("New_MyPeoples"));
        //New_Player 穿透 New_Neutrality
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("New_Player"), LayerMask.NameToLayer("New_Neutrality"));
        //New_Player 穿透 New_Rabbits
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("New_Player"), LayerMask.NameToLayer("New_Rabbits"));
        //New_Player 穿透 New_Enemies
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("New_Player"), LayerMask.NameToLayer("New_Enemies"));
        //New_Player 穿透 New_ObstructEnemy
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("New_Player"), LayerMask.NameToLayer("New_ObstructEnemy"));

        //New_MyPeoples 穿透 New_MyPeoples
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("New_MyPeoples"), LayerMask.NameToLayer("New_MyPeoples"));
        //New_MyPeoples 穿透 New_Neutrality
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("New_MyPeoples"), LayerMask.NameToLayer("New_Neutrality"));
        //New_MyPeoples 穿透 New_Rabbits
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("New_MyPeoples"), LayerMask.NameToLayer("New_Rabbits"));
        //New_MyPeoples 穿透 New_Enemies
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("New_MyPeoples"), LayerMask.NameToLayer("New_Enemies"));
        //New_MyPeoples 穿透 New_ObstructEnemy
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("New_MyPeoples"), LayerMask.NameToLayer("New_ObstructEnemy"));

        //New_Enemies 穿透 New_Enemies
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("New_Enemies"), LayerMask.NameToLayer("New_Enemies"));
        //New_Enemies 穿透 New_Rabbits
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("New_Enemies"), LayerMask.NameToLayer("New_Rabbits"));
        //New_Enemies 穿透 New_Neutrality
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("New_Enemies"), LayerMask.NameToLayer("New_Neutrality"));
    }


    private void Update()
    {
        F_AIActionOrderManager.Instance.Update();
    }

    void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
        this.MMEventStartListening<TopDownEngineEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
        this.MMEventStopListening<TopDownEngineEvent>();
    }

    public void OnMMEvent(MMGameEvent gameEvent)
    {
        Debug.Log("MMGameEvent -> " + gameEvent.EventName);
        switch (gameEvent.EventName)
        {
            case "Load":
                {
                    F_AIActionOrderManager.Instance.Initialization();
                    E_AIActionOrderManager.Instance.Initialization();
                }
                break;
        }
    }

    public void OnMMEvent(TopDownEngineEvent gameEvent)
    {
        Debug.Log("TopDownEngineEvent -> " + gameEvent.EventType.ToString());
        switch (gameEvent.EventType)
        {
            case TopDownEngineEventTypes.PlayerDeath:
                {
                    Debug.LogWarning("PlayerDeath !!!!!!");
                }
                break;
        }
    }
}