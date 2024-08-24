using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Feedbacks;

public class PickItem2MoneyCoin : MonoBehaviour
{
    [SerializeField]
    int m_nConfigMoneyCoin = 1;
    
    [SerializeField]
    MMFeedbacks m_stDropEffect;
    [SerializeField]
    MMFeedbacks m_stRaiseEffect;

    //private stuff
    BoxCollider m_stCollider;




    private void Awake()
    {
        m_stCollider = GetComponent<BoxCollider>();
        GameCommon.CHECK(m_stCollider != null);
        GameCommon.CHECK(m_stCollider.isTrigger);
        
        m_stDropEffect?.Initialization();
        m_stRaiseEffect?.Initialization();
    }
    
    public int GetConfigMoneyCoin()
    {
        return m_nConfigMoneyCoin;
    }

    public void SetConfigMoneyCoin(int nValue)
    {
        GameCommon.CHECK(nValue > 0);
        m_nConfigMoneyCoin = nValue;
    }

    public void SetConfigMoneyCoinIsDisabled()
    {
        m_stCollider.enabled = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("PickItem2MoneyCoin OnTriggerEnter: " + other.name);

        Character stChar = other.GetComponent<Character>();
        if (stChar == null)
        {
            return;
        }

        if (!other.isTrigger)
        {
            //注意防止多进入的次数
            return;
        }

        Player stPlayer = stChar.GetComponent<Player>();
        if (stPlayer != null && other.isTrigger)
        {
            stPlayer.PickMoneyCoin(this);
            return;
        }

        IBase_Friend_Character stAIChar = stChar.GetComponent<IBase_Friend_Character>();
        if (stAIChar != null && other.isTrigger)
        {
            stAIChar.PickMoneyCoin(this);
            return;
        }
    }
    
    public void PlayDropEffect(
        Vector3 v3InitialPos, 
        Vector3 v3DestinationPos,
        float fDuration,
        MMFeedback.OnFeedbackFinished dgOnFeedbackFinished
        )
    {
        MMFeedbackPosition stRealEffect = null;
        foreach (MMFeedback _effect in m_stDropEffect.Feedbacks)
        {
            stRealEffect = _effect.GetComponent<MMFeedbackPosition>();
            if (stRealEffect != null)
            {
                break;
            }
        }

        GameCommon.CHECK(stRealEffect != null);
        stRealEffect.InitialPosition = v3InitialPos;
        stRealEffect.DestinationPosition = v3DestinationPos;
        stRealEffect.AnimatePositionDuration = fDuration;
        stRealEffect.m_dgOnFeedbackFinished = dgOnFeedbackFinished;

        m_stDropEffect?.PlayFeedbacks();
    }
    
    public void PlayRaiseEffect(
        Vector3 v3InitialPos,
        Vector3 v3DestinationPos,
        float fDuration,
        MMFeedback.OnFeedbackFinished dgOnFeedbackFinished
        )
    {
        MMFeedbackPosition stRealEffect = null;
        foreach (MMFeedback _effect in m_stRaiseEffect.Feedbacks)
        {
            stRealEffect = _effect.GetComponent<MMFeedbackPosition>();
            if (stRealEffect != null)
            {
                break;
            }
        }

        GameCommon.CHECK(stRealEffect != null);
        stRealEffect.InitialPosition = v3InitialPos;
        stRealEffect.DestinationPosition = v3DestinationPos;
        stRealEffect.AnimatePositionDuration = fDuration;
        stRealEffect.m_dgOnFeedbackFinished = dgOnFeedbackFinished;

        m_stRaiseEffect?.PlayFeedbacks();
    }











    static public PickItem2MoneyCoin InstancePickItem2MoneyCoin(
        string strGoDebugName,
        Transform trParent,
        Vector3 v3LocalPosition,
        Quaternion qtRotation,
        Vector3 v3LocalScale
        )
    {
        string strLoadPath = string.Format("Prefabs/ItemWaitingPick/PickItem2MoneyCoin");
        UnityEngine.Object objSrc = Resources.Load(strLoadPath);
        GameCommon.CHECK(objSrc != null, "Resources.Load Failed: " + strLoadPath);
        GameObject goItem = (GameObject)UnityEngine.Object.Instantiate(objSrc);
        GameCommon.CHECK(goItem != null);
        goItem.name = strGoDebugName;
        goItem.transform.SetParent(trParent);
        goItem.transform.localPosition = v3LocalPosition;
        goItem.transform.rotation = qtRotation;
        goItem.transform.localScale = v3LocalScale;

        return goItem.GetComponent<PickItem2MoneyCoin>();
    }
}