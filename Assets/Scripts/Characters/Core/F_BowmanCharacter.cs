using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

/*
    重点：
    01.【AIDecisionDetectTargetRadius3D.过程中】，需要打开【ObstacleDetection.&&.ObstacleMask】
    02.否则【MoveTowards】，会直接撞墙卡住，不论有没有MeshBak.

    先使用一种【弓箭手白天黑夜转化AI的方案】：
    01.->白天，移除当下巡逻点，使用【Homeland】防御巡逻点，即可以走出基地打猎的路径，且AI机上限制它只能侦测New_Rabbit
    02.->夜间，移除当下巡逻点，使用【Wall】防御蹲点，让AIBrain结束当前State，跳转到初始State，且AI机上限制它只能侦测New_Enemies
*/

public class F_BowmanCharacter : IBase_Friend_Character
{
    [SerializeField]
    [ReadOnly]
    CharacterHandleWeapon m_stHandleWeapon;


    [SerializeField]
    [ReadOnly]
    bool m_bIsDefendSideLeft;//防守位-左

    List<Vector3> m_lstDefendPosition = new List<Vector3>();

    protected override void Awake()
    {
        base.Awake();

        m_stHandleWeapon = gameObject.GetComponent<CharacterHandleWeapon>();
        GameCommon.CHECK(m_stHandleWeapon != null);
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    protected override void ApplyLevAttr()
    {
        Minos_CTBLInfo.ST_F_CharacterAttr stAttr = Minos_CTBLInfo.Inst.GetF_CharacterAttr(m_emCharType, m_nLv);
        GameCommon.CHECK(stAttr != null);

        m_stHealth.InitialHealth = (int)stAttr.GetAttr(EM_F_CharacterAttr.Hp);
        m_stHealth.MaximumHealth = (int)stAttr.GetAttr(EM_F_CharacterAttr.Hp);
        
        m_stTDCharMovement.WalkSpeed = (float)stAttr.GetAttr(EM_F_CharacterAttr.WalkSpeed);
        if (m_stTDCharRun != null)
        { 
            m_stTDCharRun.RunSpeed = (float)stAttr.GetAttr(EM_F_CharacterAttr.RunSpeed); 
        }

        Minos_ProjectileWeapon stWeapon = m_stHandleWeapon.CurrentWeapon as Minos_ProjectileWeapon;
        GameCommon.CHECK(stWeapon != null);
        stWeapon.DamageCaused = (int)stAttr.GetAttr(EM_F_CharacterAttr.Damage);
        stWeapon.ReloadTime = (float)stAttr.GetAttr(EM_F_CharacterAttr.DamageDelay);
        stWeapon.HitRate = (int)stAttr.GetAttr(EM_F_CharacterAttr.HitRate);
    }

    public CharacterHandleWeapon GetHandleWeapon() { return m_stHandleWeapon; }

    public void SetDefendSideLeft(bool bIsLeft)
    {
        m_bIsDefendSideLeft = bIsLeft;
    }

    public bool IsDefendSideLeft() { return m_bIsDefendSideLeft; }
        
    public void FillPatrolPath_Homeland()
    {
        //使用Homeland巡逻点 + 目标Wall巡逻点
        F_Homeland stHomeland = Minos_BuildingManager.Instance.GetPlayerHomeland();
        GameCommon.CHECK(stHomeland != null);

        List<GameObject> lstPatrolPath = new List<GameObject>();

        F_Wall stWall = null;
        if (m_bIsDefendSideLeft)
        {
            //使用Homeland巡逻点
            lstPatrolPath.AddRange(stHomeland.m_lstDefendLeft);
            //使用墙壁的巡逻点
            stWall = Minos_BuildingManager.Instance.Get_F_DefendLeftWall();
            GameCommon.CHECK(stWall != null);
            lstPatrolPath.AddRange(stWall.m_lstPatrolLeft);
        }
        else
        {
            //使用Homeland巡逻点
            lstPatrolPath.AddRange(stHomeland.m_lstDefendRight);
            //使用墙壁的巡逻点
            stWall = Minos_BuildingManager.Instance.Get_F_DefendRightWall();
            GameCommon.CHECK(stWall != null);
            lstPatrolPath.AddRange(stWall.m_lstPatrolRight);
        }

        //填充进BD
        base.SetPatrolPath(lstPatrolPath);
    }

    public void FillPatrolPath_Wall()
    {
        /*
            角色id从1001开始，逐一增加， 1001为左，1002为右
        */

        List<GameObject> lstPatrolPath = new List<GameObject>();

        Vector3 v3TargetDefendPoint;
        int nIndexInSide;

        F_Wall stWall = null;
        if (m_bIsDefendSideLeft)
        {
            stWall = Minos_BuildingManager.Instance.Get_F_DefendLeftWall();
            GameCommon.CHECK(stWall != null);
            lstPatrolPath.AddRange(stWall.m_lstDefendRight);

            nIndexInSide = ((GetOnlyId() - GetParentFactory().GetCharacterStartId()) + 1) / 2 - 1;
            v3TargetDefendPoint = CalcLeftWallDefendPoint(lstPatrolPath[0].transform.position, nIndexInSide);
        }
        else
        {
            stWall = Minos_BuildingManager.Instance.Get_F_DefendRightWall();
            GameCommon.CHECK(stWall != null);
            lstPatrolPath.AddRange(stWall.m_lstDefendLeft);

            nIndexInSide = (GetOnlyId() - GetParentFactory().GetCharacterStartId()) / 2 - 1;
            v3TargetDefendPoint = CalcRightWallDefendPoint(lstPatrolPath[0].transform.position, nIndexInSide);
        }

        //取出一个点，填充进BD
        m_stBehaviorTree.SetVariableValue(
            "NightDefendPoint",
            v3TargetDefendPoint
            );
    }

    public override void OnGameDate_IsDayComing()
    {
        base.OnGameDate_IsDayComing();
        FillPatrolPath_Homeland();
    }

    public override void OnGameDate_IsNightComing(bool bIsBloodNight, int nBloodNightIndex)
    {
        base.OnGameDate_IsNightComing(bIsBloodNight, nBloodNightIndex);
        FillPatrolPath_Wall();
    }



    float m_fDefendOffsetH = 1f;
    float m_fDefendOffsetV = 1f;

    /*
        右墙壁防守蹲点
        - - - - - - - *
        - - - - - - - *
        - - - - - - * *
        - - - - - - - *
        - - - - - - - *

        - - - 18 13 8 3 *
        - - - 16 11 6 1 *
        - - - 15 10 5 0 *
        - - - 17 12 7 2 *
        - - - 19 14 9 4 *
    */
    Vector3 CalcRightWallDefendPoint(Vector3 v3Point0, int nTargetIndex)
    {
        GameCommon.CHECK(nTargetIndex >= 0);
        if (nTargetIndex == 0) { return v3Point0; }

        float fX = 0;
        float fZ = 0;
        if (nTargetIndex % 5 == 0)
        {
            //0道
            fX = v3Point0.x - m_fDefendOffsetH * (nTargetIndex / 5);
            fZ = v3Point0.z;
        }
        else if (nTargetIndex % 5 == 1)
        {
            //1道
            fX = v3Point0.x - m_fDefendOffsetH * (nTargetIndex / 5);
            fZ = v3Point0.z + m_fDefendOffsetV * 1;
        }
        else if (nTargetIndex % 5 == 2)
        {
            //2道
            fX = v3Point0.x - m_fDefendOffsetH * (nTargetIndex / 5);
            fZ = v3Point0.z - m_fDefendOffsetV * 1;
        }
        else if (nTargetIndex % 5 == 3)
        {
            //3道
            fX = v3Point0.x - m_fDefendOffsetH * (nTargetIndex / 5);
            fZ = v3Point0.z + m_fDefendOffsetV * 2;
        }
        else if (nTargetIndex % 5 == 4)
        {
            //4道
            fX = v3Point0.x - m_fDefendOffsetH * (nTargetIndex / 5);
            fZ = v3Point0.z - m_fDefendOffsetV * 2;
        }

        return new Vector3(fX, 0, fZ);
    }




    /*
        左墙壁防守蹲点
        * - - - - - - -
        * - - - - - - -
        * * - - - - - -
        * - - - - - - -
        * - - - - - - -
        
        * 3 8 13 - - - -
        * 1 6 11 - - - -
        * 0 5 10 - - - -
        * 2 7 12 - - - -
        * 4 9 14 - - - -
    */
    Vector3 CalcLeftWallDefendPoint(Vector3 v3Point0, int nTargetIndex)
    {
        GameCommon.CHECK(nTargetIndex >= 0);
        if (nTargetIndex == 0) { return v3Point0; }

        float fX = 0;
        float fZ = 0;
        if (nTargetIndex % 5 == 0)
        {
            //0道
            fX = v3Point0.x + m_fDefendOffsetH * (nTargetIndex / 5);
            fZ = v3Point0.z;
        }
        else if (nTargetIndex % 5 == 1)
        {
            //1道
            fX = v3Point0.x + m_fDefendOffsetH * (nTargetIndex / 5);
            fZ = v3Point0.z + m_fDefendOffsetV * 1;
        }
        else if (nTargetIndex % 5 == 2)
        {
            //2道
            fX = v3Point0.x + m_fDefendOffsetH * (nTargetIndex / 5);
            fZ = v3Point0.z - m_fDefendOffsetV * 1;
        }
        else if (nTargetIndex % 5 == 3)
        {
            //3道
            fX = v3Point0.x + m_fDefendOffsetH * (nTargetIndex / 5);
            fZ = v3Point0.z + m_fDefendOffsetV * 2;
        }
        else if (nTargetIndex % 5 == 4)
        {
            //4道
            fX = v3Point0.x + m_fDefendOffsetH * (nTargetIndex / 5);
            fZ = v3Point0.z - m_fDefendOffsetV * 2;
        }

        return new Vector3(fX, 0, fZ);
    }


    public void ForceFaceDir(Character.FacingDirections emDir)
    {
        switch (emDir)
        {
            case Character.FacingDirections.East:
                GetHandleWeapon().CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>().SetCurrentAim(Vector3.left);
                break;
            case Character.FacingDirections.North:
                GetHandleWeapon().CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>().SetCurrentAim(Vector3.forward);
                break;
            case Character.FacingDirections.South:
                GetHandleWeapon().CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>().SetCurrentAim(Vector3.back);
                break;
            case Character.FacingDirections.West:
                GetHandleWeapon().CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>().SetCurrentAim(Vector3.right);
                break;
        }
    }
}