using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class Minos_CharacterRun : CharacterRun
{

    /// <summary>
    /// Checks if we should exit our running state
    /// </summary>
    protected override void HandleRunningExit()
    {
        //base.HandleRunningExit();

        // if we're running and not grounded, we change our state to Falling
        if (!_controller.Grounded && (_movement.CurrentState == CharacterStates.MovementStates.Running))
        {
            _movement.ChangeState(CharacterStates.MovementStates.Falling);
            StopSfx();
        }
        // if we're not moving fast enough, we go back to idle
        if ((Mathf.Abs(_controller.CurrentMovement.magnitude) < RunSpeed / 10) && (_movement.CurrentState == CharacterStates.MovementStates.Running))
        {
            RunStop();
        }
        if (!_controller.Grounded && _abilityInProgressSfx != null)
        {
            StopSfx();
        }
    }
}
