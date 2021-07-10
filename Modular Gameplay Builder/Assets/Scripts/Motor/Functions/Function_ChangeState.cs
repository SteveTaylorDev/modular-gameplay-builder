using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChangeState", menuName = "Base/Functions/Change State")]
public class Function_ChangeState : Function_Base
{
    public State_Base destinationState;

    public override void RunFunction(Motor_Base motor)
    {
        // The currentState's destruct function is first run.
        motor.currentState.Destruct(motor);

        // The list of states attached to this motor is checked to see if it contains the destinationState attatched to this function.
        // If it is found, the current state is set to the destinationState. Otherwise, a warning is logged.
        if (motor.states.Contains(destinationState)) motor.currentState = destinationState;
        else Debug.LogWarning("The currentState tried to change to a state that isn't attached to this object. State has not been changed.");

        // The Construct function of the new currentState is then run.
        motor.currentState.Construct(motor);

    }
}
