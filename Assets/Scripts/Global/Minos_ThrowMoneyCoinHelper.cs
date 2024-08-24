using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

public class Minos_ThrowMoneyCoinHelper : GameHelper.Singleton<Minos_ThrowMoneyCoinHelper>
{
    /*
        01.主角往【地面】扔金币
        02.主角往【建筑头顶Bar】扔金币
        03.【建筑头顶Bar】向其下方掉落金币
    */

    //Register stuff
    bool m_bIsRegister = false;
    MonoBehaviour m_stTargetMono;
    string m_strThrowButtonName;
    int m_nObstacleLayer;
    GameObject m_goCharacterModel;
    public delegate int OnGetCurMoneyCoin();
    OnGetCurMoneyCoin m_dgOnGetCurMoneyCoin;
    public delegate void OnThrowMoneyCoinToGround(int nValue);
    OnThrowMoneyCoinToGround m_dgOnThrowMoneyCoinToGround;

    //UnderBuilding stuff    
    bool m_bIsMonitorUnderBuilding = false;    
    EM_F_BuildingType m_emUnderBuildingType = EM_F_BuildingType.Invalid;
    GameObject m_goUnderBuilding;
    Minos_3DGUI_BuildingMoneyCoinBar m_stMoneyCoinBar;
    public delegate void OnThrowMoneyCoinToMoneyCoinBar(int nValue);
    OnThrowMoneyCoinToMoneyCoinBar m_dgOnThrowMoneyCoinToMoneyCoinBar;
    public delegate void OnThrowMoneyCoinToMoneyCoinBarFinished();
    OnThrowMoneyCoinToMoneyCoinBarFinished m_dgOnThrowMoneyCoinToMoneyCoinBarFinished;

    int m_nMoneyCoinBarFilledIndex = -1;
    bool m_bIsHoldPressedMoneyCoinButton = false;
    float m_fHoldPressedMoneyCoinButtonTime = 0;
    
    IEnumerator m_coRaiseMoneyCoinToBuilding;
    bool m_bIsRaiseMoneyCoinToBuilding = false;
    IEnumerator m_coDropMoneyCoinFromBuilding;
    bool m_bIsDropMoneyCoinFromBuilding = false;
    bool m_bIsInThrowMoneyCoinToMoneyCoinBarFinishing = false;



    #region *******************************Register****************************************

    public void Register(
        MonoBehaviour stTargetMono,
        string strThrowButtonName,
        int nObstacleLayer,
        GameObject goCharacterModel,
        OnGetCurMoneyCoin dgOnGetCurMoneyCoin,
        OnThrowMoneyCoinToGround dgOnThrowMoneyCoinToGround
        )
    {
        GameCommon.CHECK(stTargetMono != null);
        GameCommon.CHECK(!string.IsNullOrWhiteSpace(strThrowButtonName));
        GameCommon.CHECK(goCharacterModel != null);
        GameCommon.CHECK(dgOnGetCurMoneyCoin != null);
        GameCommon.CHECK(dgOnThrowMoneyCoinToGround != null);

        m_bIsRegister = true;
        m_stTargetMono = stTargetMono;
        m_strThrowButtonName = strThrowButtonName;
        m_nObstacleLayer = nObstacleLayer;
        m_goCharacterModel = goCharacterModel;
        m_dgOnGetCurMoneyCoin = dgOnGetCurMoneyCoin;
        m_dgOnThrowMoneyCoinToGround = dgOnThrowMoneyCoinToGround;
    }

    public void UnRegister()
    {
        m_bIsRegister = false;
        m_stMoneyCoinBar = null;
        m_strThrowButtonName = null;
        m_nObstacleLayer = 0;
        m_goCharacterModel = null;
        m_dgOnGetCurMoneyCoin = null;
        m_dgOnThrowMoneyCoinToGround = null;
    }

    public void UpdateRegister()
    {
        if (!m_bIsRegister)
        {
            return;
        }

        if (Input.GetButtonDown(m_strThrowButtonName))
        {
            if (m_dgOnGetCurMoneyCoin() <= 0)
            {
                return;
            }

            if (m_emUnderBuildingType > EM_F_BuildingType.Invalid && m_emUnderBuildingType < EM_F_BuildingType.Max)
            {
                //DoNothing
            }
            else
            {
                //检查目标圆锥区域可否扔金币
                Vector3 v3TargetPosOffset;
                float fTargetDir = 1.0f;
                if (!GameHelper.IsConeOfVisionHasTarget(m_goCharacterModel.transform.position, m_goCharacterModel.transform.forward,
                    2.0f, 150f, 1 << m_nObstacleLayer)
                    )
                {
                    v3TargetPosOffset = GameHelper.RotateVector3InRandom(m_goCharacterModel.transform.forward, m_goCharacterModel.transform.up, -20.0f, 20.0f);
                    fTargetDir = 1.0f;
                }
                else
                {
                    v3TargetPosOffset = GameHelper.RotateVector3InRandom(-m_goCharacterModel.transform.forward, m_goCharacterModel.transform.up, -20.0f, 20.0f);
                    fTargetDir = -1.0f;
                }

                v3TargetPosOffset = v3TargetPosOffset.normalized * UnityEngine.Random.Range(1.5f, 2.0f);

                PickItem2MoneyCoin itemCoin = PickItem2MoneyCoin.InstancePickItem2MoneyCoin(
                    "MoneyCoin_DropToGround", null, Vector3.zero, Quaternion.identity, Vector3.one);
                GameCommon.CHECK(itemCoin != null);
                itemCoin.SetConfigMoneyCoin(1);
                itemCoin.PlayDropEffect(
                    m_goCharacterModel.transform.position + fTargetDir * m_goCharacterModel.transform.forward.normalized * 1.0f,
                    m_goCharacterModel.transform.position + v3TargetPosOffset,
                    0.7f,
                    null
                    );

                m_dgOnThrowMoneyCoinToGround(itemCoin.GetConfigMoneyCoin());
            }
        }
    }

    #endregion *******************************Register****************************************



    






    #region *******************************MonitorUnderBuilding****************************************

    public void StartMonitorUnderBuilding(        
        EM_F_BuildingType emUnderBuildingType,
        GameObject goUnderBuilding,
        Minos_3DGUI_BuildingMoneyCoinBar stMoneyCoinBar,        
        OnThrowMoneyCoinToMoneyCoinBar dgOnThrowMoneyCoinToMoneyCoinBar,
        OnThrowMoneyCoinToMoneyCoinBarFinished dgOnThrowMoneyCoinToMoneyCoinBarFinished
        )
    {
        GameCommon.CHECK(emUnderBuildingType > EM_F_BuildingType.Invalid && emUnderBuildingType < EM_F_BuildingType.Max);
        GameCommon.CHECK(goUnderBuilding != null);
        GameCommon.CHECK(stMoneyCoinBar != null);
        GameCommon.CHECK(dgOnThrowMoneyCoinToMoneyCoinBar != null);

        m_bIsMonitorUnderBuilding = true;
        
        m_emUnderBuildingType = emUnderBuildingType;
        m_goUnderBuilding = goUnderBuilding;
        m_stMoneyCoinBar = stMoneyCoinBar;
        m_dgOnThrowMoneyCoinToMoneyCoinBar = dgOnThrowMoneyCoinToMoneyCoinBar;
        m_dgOnThrowMoneyCoinToMoneyCoinBarFinished = dgOnThrowMoneyCoinToMoneyCoinBarFinished;
        m_nMoneyCoinBarFilledIndex = -1;
        m_bIsHoldPressedMoneyCoinButton = false;
        m_fHoldPressedMoneyCoinButtonTime = 0;

        if (m_coRaiseMoneyCoinToBuilding != null)
        {
            m_stTargetMono.StopCoroutine(m_coRaiseMoneyCoinToBuilding);
        }
        m_coRaiseMoneyCoinToBuilding = null;
        if (m_coDropMoneyCoinFromBuilding != null)
        {
            m_stTargetMono.StopCoroutine(m_coDropMoneyCoinFromBuilding);
        }
        m_coDropMoneyCoinFromBuilding = null;
    }

    public void StopMonitorUnderBuilding()
    {
        if (m_nMoneyCoinBarFilledIndex > 0)
        {
            //直接循环掉落完所有硬币
            for (int iLoop = m_nMoneyCoinBarFilledIndex/* - 1*/; iLoop >= 0; iLoop--)
            {
                PickItem2MoneyCoin itemCoin = PickItem2MoneyCoin.InstancePickItem2MoneyCoin(
                    "MoneyCoin_DropFromBuilding", null, Vector3.zero, Quaternion.identity, Vector3.one);
                GameCommon.CHECK(itemCoin != null);
                itemCoin.SetConfigMoneyCoin(1);
                Vector3 v3PosInMoneyCoinBar = m_stMoneyCoinBar.GetMoneyCoinPosition(iLoop);
                itemCoin.PlayDropEffect(
                    v3PosInMoneyCoinBar,
                    new Vector3(v3PosInMoneyCoinBar.x, 0, v3PosInMoneyCoinBar.z - 1.0f),
                    0.3f,
                    null
                    );

                m_stMoneyCoinBar.SetFillMoneyCoin(iLoop, false);
            }
        }

        m_bIsMonitorUnderBuilding = false;        
        m_emUnderBuildingType = EM_F_BuildingType.Invalid;
        m_goUnderBuilding = null;
        m_stMoneyCoinBar = null;                
        //m_dgOnThrowMoneyCoinToMoneyCoinBar = null;//延迟特例处理，此处该委托不做释放

        m_nMoneyCoinBarFilledIndex = -1;
        m_bIsHoldPressedMoneyCoinButton = false;
        m_fHoldPressedMoneyCoinButtonTime = 0;

        if (m_coRaiseMoneyCoinToBuilding != null)
        {
            m_stTargetMono.StopCoroutine(m_coRaiseMoneyCoinToBuilding);
        }
        m_coRaiseMoneyCoinToBuilding = null;
        m_bIsRaiseMoneyCoinToBuilding = false;
        if (m_coDropMoneyCoinFromBuilding != null)
        {
            m_stTargetMono.StopCoroutine(m_coDropMoneyCoinFromBuilding);
        }
        m_coDropMoneyCoinFromBuilding = null;
        m_bIsDropMoneyCoinFromBuilding = false;
    }

    public void UpdateMonitorUnderBuilding()
    {
        if (!m_bIsMonitorUnderBuilding)
        {
            return;
        }

        //Update
        if (Input.GetButtonDown(m_strThrowButtonName))
        {
            if (m_dgOnGetCurMoneyCoin() <= 0)
            {
                return;
            }

            m_bIsHoldPressedMoneyCoinButton = true;
            m_fHoldPressedMoneyCoinButtonTime = Time.time;
        }
        else if (Input.GetButtonUp(m_strThrowButtonName))
        {
            if (m_dgOnGetCurMoneyCoin() <= 0)
            {
                return;
            }

            m_bIsHoldPressedMoneyCoinButton = false;

            if (m_emUnderBuildingType > EM_F_BuildingType.Invalid && m_emUnderBuildingType < EM_F_BuildingType.Max)
            {
                if (!m_bIsDropMoneyCoinFromBuilding && !m_bIsInThrowMoneyCoinToMoneyCoinBarFinishing)
                {
                    if (m_coRaiseMoneyCoinToBuilding != null)
                    {
                        m_stTargetMono.StopCoroutine(m_coRaiseMoneyCoinToBuilding);
                    }
                    m_coRaiseMoneyCoinToBuilding = null;
                    m_bIsRaiseMoneyCoinToBuilding = false;

                    if (m_coDropMoneyCoinFromBuilding != null)
                    {
                        m_stTargetMono.StopCoroutine(m_coDropMoneyCoinFromBuilding);
                    }
                    m_coDropMoneyCoinFromBuilding = CoDropMoneyCoinFromBuilding();
                    m_stTargetMono.StartCoroutine(m_coDropMoneyCoinFromBuilding);
                }
            }
        }




        //LateUpdate
        if (Time.time - m_fHoldPressedMoneyCoinButtonTime >= 0.4f &&
            m_bIsHoldPressedMoneyCoinButton)
        {
            if (m_dgOnGetCurMoneyCoin() <= 0)
            {
                return;
            }

            if (m_bIsRaiseMoneyCoinToBuilding)
            {
                return;
            }

            if (m_emUnderBuildingType > EM_F_BuildingType.Invalid && m_emUnderBuildingType < EM_F_BuildingType.Max)
            {
                if (m_stMoneyCoinBar == null)
                {
                    return;
                }

                if (m_bIsDropMoneyCoinFromBuilding)
                {
                    return;
                }

                if (m_coDropMoneyCoinFromBuilding != null)
                {
                    m_stTargetMono.StopCoroutine(m_coDropMoneyCoinFromBuilding);
                }
                m_coDropMoneyCoinFromBuilding = null;

                if (m_coRaiseMoneyCoinToBuilding != null)
                {
                    m_stTargetMono.StopCoroutine(m_coRaiseMoneyCoinToBuilding);
                }
                m_coRaiseMoneyCoinToBuilding = CoRaiseMoneyCoinToBuilding();
                m_stTargetMono.StartCoroutine(m_coRaiseMoneyCoinToBuilding);
            }
        }
    }

    IEnumerator CoRaiseMoneyCoinToBuilding()
    {
        m_bIsRaiseMoneyCoinToBuilding = true;
        m_bIsInThrowMoneyCoinToMoneyCoinBarFinishing = false;
        int nLoopTimes = Mathf.Min(m_dgOnGetCurMoneyCoin(), m_stMoneyCoinBar.GetMoneyCoinCount());
        for (int iLoop = 0; iLoop < nLoopTimes; iLoop++)
        {
            PickItem2MoneyCoin itemCoin = PickItem2MoneyCoin.InstancePickItem2MoneyCoin(
                "MoneyCoin_RaiseToBuilding", null, Vector3.zero, Quaternion.identity, Vector3.one * 0.5f);
            GameCommon.CHECK(itemCoin != null);
            itemCoin.SetConfigMoneyCoinIsDisabled();
            Vector3 v3PosInMoneyCoinBar = m_stMoneyCoinBar.GetMoneyCoinPosition(iLoop);
            itemCoin.PlayRaiseEffect(
                m_goCharacterModel.transform.position + m_goCharacterModel.transform.up.normalized * 0.5f,
                v3PosInMoneyCoinBar,
                0.3f,
                delegate (MMFeedback _pThis)
                {
                    m_dgOnThrowMoneyCoinToMoneyCoinBar(1);
                    UnityEngine.Object.Destroy(itemCoin.gameObject);
                }
                );

            m_stMoneyCoinBar.SetFillMoneyCoin(iLoop);
            m_nMoneyCoinBarFilledIndex = iLoop;

            if (iLoop < nLoopTimes - 1)
            {
                yield return new WaitForSeconds(0.2f);
            }            
        }

        if (nLoopTimes >= m_stMoneyCoinBar.GetMoneyCoinCount())
        {
            //Debug.Log("Homeland: MoneyCoin Is Enough To LevUp !");
            m_bIsInThrowMoneyCoinToMoneyCoinBarFinishing = true;
            yield return new WaitForSeconds(0.8f);
            if (m_dgOnThrowMoneyCoinToMoneyCoinBarFinished != null)
            {
                m_dgOnThrowMoneyCoinToMoneyCoinBarFinished();
            }            
            m_bIsRaiseMoneyCoinToBuilding = false;
            m_stMoneyCoinBar.gameObject.SetActive(false);
            m_stMoneyCoinBar = null;
            m_nMoneyCoinBarFilledIndex = -1;
            m_bIsHoldPressedMoneyCoinButton = false;

            m_coRaiseMoneyCoinToBuilding = null;
            m_bIsInThrowMoneyCoinToMoneyCoinBarFinishing = false;

            yield break;
        }
        else
        {
            Debug.Log("Homeland: MoneyCoin Is NotEnough To LevUp !");
            m_bIsRaiseMoneyCoinToBuilding = false;
            m_bIsHoldPressedMoneyCoinButton = false;

            m_coRaiseMoneyCoinToBuilding = null;

            if (m_coDropMoneyCoinFromBuilding != null)
            {
                m_stTargetMono.StopCoroutine(m_coDropMoneyCoinFromBuilding);
            }
            m_coDropMoneyCoinFromBuilding = CoDropMoneyCoinFromBuilding();
            m_stTargetMono.StartCoroutine(m_coDropMoneyCoinFromBuilding);

            yield break;
        }
    }

    IEnumerator CoDropMoneyCoinFromBuilding()
    {
        if (m_nMoneyCoinBarFilledIndex < 0)
        {
            yield break;
        }
        
        m_bIsDropMoneyCoinFromBuilding = true;
        for (int iLoop = m_nMoneyCoinBarFilledIndex; iLoop >= 0; iLoop--)
        {
            PickItem2MoneyCoin itemCoin = PickItem2MoneyCoin.InstancePickItem2MoneyCoin(
                "MoneyCoin_DropFromBuilding", null, Vector3.zero, Quaternion.identity, Vector3.one);
            GameCommon.CHECK(itemCoin != null);
            itemCoin.SetConfigMoneyCoin(1);
            Vector3 v3PosInMoneyCoinBar = m_stMoneyCoinBar.GetMoneyCoinPosition(iLoop);
            itemCoin.PlayDropEffect(
                v3PosInMoneyCoinBar,
                new Vector3(v3PosInMoneyCoinBar.x, 0, v3PosInMoneyCoinBar.z - 1.0f),
                0.6f,
                null
                );
            m_stMoneyCoinBar.SetFillMoneyCoin(iLoop, false);
            m_nMoneyCoinBarFilledIndex = iLoop - 1;

            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(0.3f);

        m_nMoneyCoinBarFilledIndex = -1;
        m_bIsDropMoneyCoinFromBuilding = false;

        m_coDropMoneyCoinFromBuilding = null;
    }
    #endregion *******************************MonitorUnderBuilding****************************************
}