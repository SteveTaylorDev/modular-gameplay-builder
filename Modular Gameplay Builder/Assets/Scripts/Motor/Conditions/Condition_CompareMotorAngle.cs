using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CompareMotorAngle", menuName = "Base/Conditions/Compare Angle")]
public class Condition_CompareMotorAngle: Condition_Base
{
    [Header("Compare Vector")]
    public string compareVector;
    public bool compareMoveDirection;
    public bool compareInputVector;
    public bool compareRawInputVector;

    [Header("Target Vector")]
    public string targetVector;
    public bool normalizeTargetVector;
    public bool reverseTargetVector;
    public bool withMoveDirection;
    public bool withPhysicsVector;
    public bool withGroundAverage;
    public bool withWallAverage;
    public bool withOrbitVector;
    public bool withSlopeVector;
    public bool withLocalGravity;
    public bool withManualVector;
    public Vector3 manualVector;

    [Header("Compare Condition")]
    public bool lessThan;
    public bool moreThan;
    public bool orEqualTo;
    public float compareAmount;

    [Header("Options")]
    public bool useCamTargetForCompare;
    public bool useCamTargetForTarget;
    public bool debugAngle;
    public bool debugCondition;


    public override bool IsCondition(Motor_Base motor)
    {
        bool compareBool = default(bool);
        Vector3 localCompareVector = Vector3.zero;
        Vector3 localTargetVector = Vector3.zero;

        Motor_Base compareMotor = motor;
        if (useCamTargetForCompare && (motor as Motor_Camera).cameraTarget.parentMotor != null) compareMotor = (motor as Motor_Camera).cameraTarget.parentMotor;

        if (compareVector != "") localCompareVector = motor.variablesMod.FindVector(compareVector).modVector;

        if (compareMoveDirection) localCompareVector = compareMotor.variablesMod.FindVector("Move Direction").modVector;
        if (compareInputVector) localCompareVector = compareMotor.variablesMod.FindVector((compareMotor.FindModule(typeof(Module_Input)) as Module_Input).inputVector).modVector;
        if (compareRawInputVector) localCompareVector = (compareMotor.FindModule(typeof(Module_Input)) as Module_Input).rawInputVector;

        Motor_Base targetMotor = motor;
        if (useCamTargetForTarget) targetMotor = (motor as Motor_Camera).cameraTarget.parentMotor;

        if (withMoveDirection) localTargetVector = targetMotor.variablesMod.FindVector("Move Direction").modVector;
        if (withPhysicsVector) localTargetVector = (targetMotor as Motor_Physics).variablesMod.FindVector((targetMotor as Motor_Physics).physicsVector).modVector;
        if (withGroundAverage)
        {
            localTargetVector = (targetMotor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection).averageGroundNormal;
        }
        if (withWallAverage)
        {
            localTargetVector = (targetMotor.FindModule(typeof(Module_Collision)) as Module_Collision).collisionWallAverage;
        }
        //if (withSlopeVector)
        //{
        //    localTargetVector = (targetMotor.FindModule(typeof(Module_Slopes)) as Module_Slopes).slopeVector;
        //}
        //if (withOrbitVector) targetVector = (targetMotor as Motor_Camera).orbitVector;
        if (withLocalGravity) localTargetVector = (targetMotor.FindModule(typeof(Module_Gravity)) as Module_Gravity).localGravityDirection;


        if (targetVector != "") localTargetVector = motor.variablesMod.FindVector(targetVector).modVector;

        if (withManualVector) localTargetVector = manualVector;

        if (normalizeTargetVector) localTargetVector = localTargetVector.normalized;
        if (reverseTargetVector) localTargetVector = -localTargetVector;

        float vectorAngle = Vector3.Angle(localCompareVector, localTargetVector);

        if (((lessThan && vectorAngle < compareAmount) || (moreThan && vectorAngle > compareAmount)) && localCompareVector != Vector3.zero && localTargetVector != Vector3.zero) compareBool = true;
        if (orEqualTo && vectorAngle == compareAmount) compareBool = true;

        if (debugAngle) Debug.Log(this + " Angle: " + vectorAngle);
        if (debugCondition) Debug.Log(this + " Condition: " + compareBool);

        if (inverseCondition) return !compareBool;
        else return compareBool;
    }
}
