using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;

public class BD_AIActionHelloWorld : Action
{
    public TaskStatus m_emUpdateRet;

    public override void OnAwake()
    {
        base.OnAwake();
        Debug.Log(FriendlyName + " OnAwake");
    }

    public override void OnBehaviorComplete()
    {
        base.OnBehaviorComplete();
        Debug.Log(FriendlyName + " OnBehaviorComplete");
    }

    public override void OnBehaviorRestart()
    {
        base.OnBehaviorRestart();
        Debug.Log(FriendlyName + " OnBehaviorRestart");
    }
    
    public override void OnStart()
    {
        base.OnStart();
        Debug.Log(FriendlyName + " OnStart");
    }

    public override void OnEnd()
    {
        base.OnEnd();
        Debug.Log(FriendlyName + " OnEnd");
    }

    public override void OnPause(bool paused)
    {
        base.OnPause(paused);
        Debug.Log(FriendlyName + " OnPause: " + paused.ToString());
    }

    public override void OnReset()
    {
        base.OnReset();
        Debug.Log(FriendlyName + " OnReset");
    }

    public override TaskStatus OnUpdate()
    {
        Debug.Log(FriendlyName + " OnUpdate");
        //return base.OnUpdate();
        return m_emUpdateRet;
    }

    //public override void OnFixedUpdate()
    //{
    //    base.OnFixedUpdate();
    //    Debug.Log(FriendlyName + " OnFixedUpdate");
    //}

    //public override void OnLateUpdate()
    //{
    //    base.OnLateUpdate();
    //    Debug.Log(FriendlyName + " OnLateUpdate");
    //}
}
