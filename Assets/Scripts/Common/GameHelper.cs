using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class GameHelper
{
    public abstract class Singleton<T> where T : new()
    {
        /*
            简单的单例模式:
            01.如果后期有【返回开始新游戏】这种【重置逻辑】，则到时候可以再改成【StartReset模式】
            02.这样就会具备完善的单例释放特性【AutoFree】
        */
        static T _instance;
        static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }
                return _instance;
            }
        }
    }

    /// <summary>
    /// Same as Random.Range, but the returned value is between min and max, inclusive.
    /// Unity's Random.Range is less than max instead, unless min == max.
    /// This means Range(0,1) produces 0 instead of 0 or 1. That's unacceptable.
    /// </summary>

    static public int RandomRange(int min, int max)
    {
        if (min == max) return min;
        return UnityEngine.Random.Range(min, max + 1);
    }
    

    /// <summary>
    /// Extension for the game object that checks to see if the component already exists before adding a new one.
    /// If the component is already present it will be returned instead.
    /// </summary>

    static public T AddMissingComponent<T>(this GameObject go) where T : Component
    {
#if UNITY_FLASH
		object comp = go.GetComponent<T>();
#else
        T comp = go.GetComponent<T>();
#endif
        if (comp == null)
        {
//#if UNITY_EDITOR
//            if (!Application.isPlaying)
//                RegisterUndo(go, "Add " + typeof(T));
//#endif
            comp = go.AddComponent<T>();
        }
#if UNITY_FLASH
		return (T)comp;
#else
        return comp;
#endif
    }

    static public T GoFind<T>()
    {
        //UnityEngine.Object 不允许直接 强转为T
        //所以加入中间转换为System.Object
        return (T)((object)(UnityEngine.Object.FindObjectOfType(typeof(T))));
    }
    
    static public void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;

        Transform t = go.transform;

        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            SetLayer(child.gameObject, layer);
        }
    }

    static public Vector3 RotateVector3(Vector3 v3Target, Vector3 v3Axis, float fAngle)
    {
        return Quaternion.AngleAxis(fAngle, v3Axis) * v3Target;
    }

    static public Vector3 RotateVector3InRandom(Vector3 v3Target, Vector3 v3Axis, float fAngleMin, float fAngleMax)
    {
        float fAngleRandom = UnityEngine.Random.Range(Mathf.Min(fAngleMin, fAngleMax), Mathf.Max(fAngleMin, fAngleMax));
        return RotateVector3(v3Target, v3Axis, fAngleRandom);
    }

    static public bool IsRadiusOfVisionHasTarget(Vector3 v3Center, float fVisionRadius, int nTargetMask)
    {
        Collider[] aryTargetCollder = Physics.OverlapSphere(v3Center, fVisionRadius, nTargetMask);
        foreach (Collider _collider in aryTargetCollder)
        {
            return true;
        }
        return false;
    }

    static public bool IsConeOfVisionHasTarget(Vector3 v3Center, Vector3 v3Dir, float fVisionRadius, float fVisionAngle, int nTargetMask)
    {
        Collider[] aryTargetCollder = Physics.OverlapSphere(v3Center, fVisionRadius, nTargetMask);
        foreach (Collider _collider in aryTargetCollder)
        {
            Transform _target = _collider.transform;
            Vector3 _directionToTarget = (_target.position - v3Center).normalized;
            float _angle = Vector3.Angle(v3Dir, _directionToTarget);
            if (_angle < fVisionAngle / 2f)
            {
                return true;
            }
        }
        return false;
    }

    static public IEnumerator CoUntilTrue(System.Func<bool> fn)
    {
        while (!fn())
        {
            /*Debug.Log("fn:" + fn.ToString() + " FALSE");*/
            yield return null;
        }
    }
    static public Coroutine WaitUntilTrue(MonoBehaviour mn, System.Func<bool> fn)
    {
        return mn.StartCoroutine(CoUntilTrue(fn));
    }

    static public bool RandomBingo(float fRandomMin, float fRandomMax, float fBingoTarget)
    {
        float fRandom = UnityEngine.Random.Range(fRandomMin, fRandomMax);
        return fRandom <= fBingoTarget;
    }

    static public bool GetEnum<T>(string strEnumName, out T emRet)
    {
        GameCommon.CHECK(!string.IsNullOrWhiteSpace(strEnumName));

        bool bRet = System.Enum.IsDefined(typeof(T), strEnumName);
        emRet = (T)System.Enum.Parse(typeof(T), strEnumName);
        return bRet;
    }
}