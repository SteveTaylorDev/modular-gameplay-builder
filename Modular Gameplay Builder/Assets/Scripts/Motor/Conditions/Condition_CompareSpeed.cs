using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CompareSpeed", menuName = "Base/Conditions/Compare Speed")]
public class Condition_CompareSpeed : Condition_Base
{
    [Header("Speed Source")]
    public string sourceFloat;
    public bool useCurrentSpeed;
    public bool usePhysicsMagnitude;
    public bool useVelocity;
    public bool useCameraTarget;

    [Header("Condition")]
    public bool lessThan;
    public bool moreThan;

    [Header("Compare Target")]
    public string compareFloat;
    public bool compareWithCurrentSpeed;
    public bool compareWithPhysicsMagnitude;
    public bool compareWithVelocity;
    public bool compareWithManualAmount;
    public float manualAmount;

    public override bool IsCondition(Motor_Base motor)
    {
        bool compareBool = default(bool);
        Motor_Base targetMotor = motor;
        if (useCameraTarget) targetMotor = (motor as Motor_Camera).cameraTarget.parentMotor;

        if (targetMotor == null) return false;

        float speed = 0;
        if (sourceFloat != "") speed = motor.variablesMod.FindFloat(sourceFloat).modFloat;
        if (useCurrentSpeed) speed = targetMotor.variablesMod.FindFloat(targetMotor.currentSpeed).modFloat;
        if (usePhysicsMagnitude && (targetMotor.GetType() == typeof(Motor_Physics))) speed = (targetMotor as Motor_Physics).variablesMod.FindVector((targetMotor as Motor_Physics).physicsVector).modVector.magnitude;
        //if (useVelocity && (targetMotor.GetType() == typeof(Motor_Physics))) speed = (targetMotor as Motor_Physics).rigidbodyVelocity.magnitude;

        float compareAmount = 0;
        if (compareFloat != "") compareAmount = motor.variablesMod.FindFloat(compareFloat).modFloat;
        if (compareWithCurrentSpeed) compareAmount = targetMotor.variablesMod.FindFloat(targetMotor.currentSpeed).modFloat;
        if (compareWithPhysicsMagnitude) compareAmount = (targetMotor as Motor_Physics).variablesMod.FindVector((targetMotor as Motor_Physics).physicsVector).modVector.magnitude;
        //if (compareWithVelocity) compareAmount = (targetMotor as Motor_Physics).rigidbodyVelocity.magnitude;
        if (compareWithManualAmount) compareAmount = manualAmount;

        if ((lessThan && speed < compareAmount) || (moreThan && speed > compareAmount)) compareBool = true;

        if (inverseCondition) return !compareBool;
        else return compareBool;
    }
}
