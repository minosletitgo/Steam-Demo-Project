using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

/*
    F_VillagerCharacter.cs 
    AI简述（AI状态机和寻路部件互斥切换，不得已而为之）：
    .【PatrolingAroundHomeland】
        A：[AIActionMovePatrol3D]

    CharacterPathfinder3D
*/

public class F_VillagerCharacter : IBase_Friend_Character
{
    [SerializeField]
    [ReadOnly]
    CharacterPathfinder3D m_stPathfinder3D;



    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void ApplyLevAttr()
    {
        //throw new NotImplementedException();
    }
}