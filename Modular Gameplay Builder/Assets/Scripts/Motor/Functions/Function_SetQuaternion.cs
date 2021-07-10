using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SetQuaternion", menuName = "Base/Functions/Set Quaternion")]
public class Function_SetQuaternion : Function_Base
{
    [Header("Source Quaternion")]
    public string sourceQuaternion;

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

        if(sourceQuaternion != "")
        {
            if (targetQuaternion != "")
            {
                if(localSlerpTime == 0) motor.variablesMod.FindQuaternion(sourceQuaternion).modQuaternion = motor.variablesMod.FindQuaternion(targetQuaternion).modQuaternion;
                else motor.variablesMod.FindQuaternion(sourceQuaternion).modQuaternion = Quaternion.Slerp(motor.transform.rotation, motor.variablesMod.FindQuaternion(targetQuaternion).modQuaternion, localSlerpTime * Time.deltaTime);
            }
        }
    }
}
