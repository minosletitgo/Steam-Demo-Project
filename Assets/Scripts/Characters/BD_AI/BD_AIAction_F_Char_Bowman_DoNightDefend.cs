using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class BD_AIAction_F_Char_Bowman_DoNightDefend : Action
{
    public TaskStatus m_emRet;

    F_BowmanCharacter m_stBaseChar;
    


    public override void OnAwake()
    {
        base.OnAwake();

        m_stBaseChar = gameObject.GetComponent<F_BowmanCharacter>();
        GameCommon.CHECK(m_stBaseChar != null);
    }

    public override void OnStart()
    {
        base.OnStart();

        //朝向指定
        if (m_stBaseChar.IsDefendSideLeft())
        {
            //隶属于左墙壁，则左(East)
            m_stBaseChar.ForceFaceDir(MoreMountains.TopDownEngine.Character.FacingDirections.East);
        }
        else
        {
            //隶属于右墙壁，则左(West)
            m_stBaseChar.ForceFaceDir(MoreMountains.TopDownEngine.Character.FacingDirections.West);
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (Minos_GameDateManager.Instance.IsDayOrNight())
        {
            return TaskStatus.Failure;
        }
        return m_emRet;
    }
}
