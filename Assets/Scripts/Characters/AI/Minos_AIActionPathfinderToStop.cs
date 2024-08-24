using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class Minos_AIActionPathfinderToStop : AIAction
{
    protected CharacterPathfinder3D _characterPathfinder3D;

    protected override void Initialization()
    {
        _characterPathfinder3D = this.gameObject.GetComponent<CharacterPathfinder3D>();
    }


    public override void PerformAction()
    {
        _characterPathfinder3D.Target = null;
    }

}
