using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildQuaternion", menuName = "Base/Functions/Build Quaternion")]
public class Function_BuildQuaternion : Function_Base
{
    [Header("Orientation")]
    public string orientationVector;

    [Header("Direction")]
    public string directionVector;

    [Header("Target Quaternion")]
    public string targetQuaternion;

    [Header("Options")]
    public float slerpTime;
    public string floatMultiplier;
    public float floatOffset;


    public override void RunFunction(Motor_Base motor)
    {
        float localSlerpTime = slerpTime;
        if (floatMultiplier != "") localSlerpTime *= motor.variablesMod.FindFloat(floatMultiplier).modFloat + floatOffset;

        if (targetQuaternion != "")
        {
            if (orientationVector != "" && directionVector != "")
            {
                if(slerpTime == 0) motor.variablesMod.FindQuaternion(targetQuaternion).modQuaternion = Quaternion.LookRotation(motor.variablesMod.FindVector(directionVector).modVector, motor.variablesMod.FindVector(orientationVector).modVector);
                else motor.variablesMod.FindQuaternion(targetQuaternion).modQuaternion = Quaternion.Slerp(motor.variablesMod.FindQuaternion(targetQuaternion).modQuaternion, Quaternion.LookRotation(motor.variablesMod.FindVector(directionVector).modVector, motor.variablesMod.FindVector(orientationVector).modVector), localSlerpTime * Time.deltaTime);
            }
        }
    }   
}
