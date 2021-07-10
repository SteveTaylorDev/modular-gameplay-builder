using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CompareGroundFloat", menuName = "Base/Conditions/(Ground) Compare Float")]
public class Condition_CompareGroundFloat: Condition_Base
{
    [Header("Compare Float")]
    public float compareAngleOffset;
    public bool compareGroundRayLength;
    public bool compareClosestGroundDistance;
    public bool compareLocalGroundAngle;
    public bool compareGlobalGroundAngle;

    [Header("Compare Condition")]
    public bool lessThan;
    public bool moreThan;

    [Header("Target Float")]
    public float targetAngleOffset;
    public bool withLocalGroundAngle;
    public bool withGlobalGroundAngle;
    public bool withRequiredGroundAngle;
    public bool withManualFloat;
    public float manualFloat;

    [Header("Options")]
    public bool useCameraTargetMotor;


    public override bool IsCondition(Motor_Base motor)
    {
        bool compareBool = default(bool);
        float compareFloat = 0;
        float targetFloat = 0;

        Motor_Base targetMotor = motor;
        if (useCameraTargetMotor) targetMotor = (motor as Motor_Camera).cameraTarget.parentMotor;

        if (targetMotor == null) return false;

        Module_GroundDetection groundMod = targetMotor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;

        if (groundMod != null)
        {
            if (compareGroundRayLength) compareFloat = groundMod.groundRayLength;
            if (compareClosestGroundDistance) compareFloat = groundMod.closestGroundDistance;
            if (compareLocalGroundAngle) compareFloat = groundMod.localGroundAngle;
            if (compareGlobalGroundAngle) compareFloat = groundMod.globalGroundAngle;

            if (withLocalGroundAngle) targetFloat = groundMod.localGroundAngle;
            if (withGlobalGroundAngle) targetFloat = groundMod.globalGroundAngle;
            if (withRequiredGroundAngle) targetFloat = groundMod.requiredGroundAngle;
        }

        if (withManualFloat) targetFloat = manualFloat;

        compareFloat += compareAngleOffset;
        targetFloat += targetAngleOffset;

        if ((lessThan && compareFloat < targetFloat) || (moreThan && compareFloat > targetFloat)) compareBool = true;


        if (inverseCondition) return !compareBool;
        else return compareBool;
    }
}
