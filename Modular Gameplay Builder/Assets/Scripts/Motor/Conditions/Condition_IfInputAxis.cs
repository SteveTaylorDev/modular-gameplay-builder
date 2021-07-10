using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IfInputAxis", menuName = "Base/Conditions/If Input Axis")]
public class Condition_IfInputAxis : Condition_Base
{
    [Header("Input Axis")]
    public bool useInputVector;
    public bool useAltInputVector;
    public bool useMouseVector;
    public bool useLeftTrigger;
    public bool useRightTrigger;
    public bool cameraTargetAsSource;

    [Header("Axis Condition")]
    public bool lessThan;
    public bool moreThan;
    public bool orEqualTo;
    public string axisAmount;

    [Header("Timer Condition")]
    public bool isInputVectorTimer;
    public bool isAltVectorTimer;
    public bool ifMouseVectorTimer;
    public bool isLeftTriggerTimer;
    public bool isRightTriggerTimer;
    public bool timerLessThan;
    public bool timerMoreThan;
    public float compareTime;


    public override bool IsCondition(Motor_Base motor)
    {
        bool isAxis = default;

        Motor_Base targetMotor = motor;
        if (cameraTargetAsSource && (motor as Motor_Camera).cameraTarget.parentMotor != null) targetMotor = (motor as Motor_Camera).cameraTarget.parentMotor;

        Module_Input inputMod = targetMotor.FindModule(typeof(Module_Input)) as Module_Input;
        if (inputMod == null)
        {
            Debug.LogWarning("A condition is checking this motor for an Input Module but cannot find one. This condition will only return false until one is added to this motor.");
            return (false);
        }

        float localAxis = 0;
        if (useInputVector) localAxis = targetMotor.variablesMod.FindVector(inputMod.inputVector).modVector.magnitude;
        if (useAltInputVector) localAxis = inputMod.altInputVector.magnitude;
        if (useMouseVector) localAxis = inputMod.mouseVector.magnitude;
        if (useLeftTrigger) localAxis = inputMod.leftTrigger;
        if (useRightTrigger) localAxis = inputMod.rightTrigger;

        if ((lessThan && localAxis < motor.variablesMod.FindFloat(axisAmount).modFloat) || (moreThan && localAxis > motor.variablesMod.FindFloat(axisAmount).modFloat) || (orEqualTo && localAxis == motor.variablesMod.FindFloat(axisAmount).modFloat)) isAxis = true;
        else isAxis = false;

        if (ifMouseVectorTimer)
        {
            if ((moreThan && inputMod.lastMouseVectorTime > compareTime) || (lessThan && inputMod.lastMouseVectorTime < compareTime)) isAxis = true;
            else isAxis = false;
        }

        if (isInputVectorTimer)
        {
            if ((moreThan && inputMod.lastInputVectorTime > compareTime) || (lessThan && inputMod.lastInputVectorTime < compareTime)) isAxis = true;
            else isAxis = false;
        }

        if (isAltVectorTimer)
        {
            if ((moreThan && inputMod.lastAltVectorTime > compareTime) || (lessThan && inputMod.lastAltVectorTime < compareTime)) isAxis = true;
            else isAxis = false;
        }

        if (isLeftTriggerTimer)
        {
            if ((moreThan && inputMod.lastLeftTriggerTime > compareTime) || (lessThan && inputMod.lastLeftTriggerTime < compareTime)) isAxis = true;
            else isAxis = false;
        }

        if (isRightTriggerTimer)
        {
            if ((moreThan && inputMod.lastRightTriggerTime > compareTime) || (lessThan && inputMod.lastRightTriggerTime < compareTime)) isAxis = true;
            else isAxis = false;
        }

        if (inverseCondition) return !isAxis;
        else return isAxis;
    }
}
