using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class Minos_AIActionStanding : AIAction
{
    [SerializeField]
    Vector3 m_v3CurrentDirection;


    //private stuff
    protected TopDownController m_stTopDownController;


    protected override void Initialization()
    {
        m_stTopDownController = GetComponent<TopDownController>();
        GameCommon.CHECK(m_stTopDownController != null);
    }

    public override void PerformAction()
    {
        m_stTopDownController.CurrentDirection = m_v3CurrentDirection;
    }
}
