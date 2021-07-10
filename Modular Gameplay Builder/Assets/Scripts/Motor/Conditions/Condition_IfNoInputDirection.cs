using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IfNoInputDirection", menuName = "Base/Conditions/If No Input Direction")]
public class Condition_IfNoInputDirection : Condition_Base
{
    [Header("Input Vector")]
    public bool useAltInputVector;
    public bool useMouseVector;
    public bool cameraTargetAsSource;

    [Header("Timer Condition")]
    public bool ifMouseVectorTimer;
    public bool isAltVectorTimer;
    public bool lessThan;
    public bool moreThan;
    public float compareTime;


    public override bool IsCondition(Motor_Base motor)
    {
        bool noInputDirection = default(bool);

        Motor_Base targetMotor = motor;
        if (cameraTargetAsSource && (motor as Motor_Camera).cameraTarget.parentMotor != null) targetMotor = (motor as Motor_Camera).cameraTarget.parentMotor;

        Module_Input inputMod = targetMotor.FindModule(typeof(Module_Input)) as Module_Input;
        if (inputMod == null)
        {
            Debug.LogWarning("A condition is checking this motor for an Input Module but cannot find one. This condition will only return false until one is added to this motor.");
            return (false);
        }

        Vector3 localInputVector = targetMotor.variablesMod.FindVector(inputMod.inputVector).modVector;
        if (useAltInputVector) localInputVector = inputMod.altInputVector;
        if (useMouseVector) localInputVector = inputMod.mouseVector;

        if (localInputVector.magnitude == 0) noInputDirection = true;
        else noInputDirection = false;

        if (ifMouseVectorTimer)
        {
            if ((moreThan && inputMod.lastMouseVectorTime > compareTime) || (lessThan && inputMod.lastMouseVectorTime < compareTime)) noInputDirection = true;
            else noInputDirection = false;
        }

        if (isAltVectorTimer)
        {
            if ((moreThan && inputMod.lastAltVectorTime > compareTime) || (lessThan && inputMod.lastAltVectorTime < compareTime)) noInputDirection = true;
            else noInputDirection = false;
        }

        if (inverseCondition) return !noInputDirection;
        else return noInputDirection;
    }
}
