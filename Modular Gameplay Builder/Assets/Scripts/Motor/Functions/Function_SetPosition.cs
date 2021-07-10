using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SetPosition", menuName = "Base/Functions/Set Position")]
public class Function_SetPosition : Function_Base
{
    [Header("Source Vector")]
    public string sourceVector;

    [Header("Target Transform")]
    public string targetTransform;

    [Header("Options")]
    public float slerpTime;
    public string floatMultiplier;
    public float floatOffset;


    public override void RunFunction(Motor_Base motor)
    {
        float localSlerpTime = slerpTime;
        if (floatMultiplier != "") localSlerpTime *= motor.variablesMod.FindFloat(floatMultiplier).modFloat + floatOffset;

        if(sourceVector != "")
        {
            if (targetTransform != "")
            {
                if(localSlerpTime == 0) motor.variablesMod.FindTransform(targetTransform).modTransform.position = motor.variablesMod.FindVector(sourceVector).modVector;
                else motor.variablesMod.FindTransform(targetTransform).modTransform.position = Vector3.Slerp(motor.transform.position, motor.variablesMod.FindVector(sourceVector).modVector, localSlerpTime * Time.deltaTime);
            }
        }
    }
}
