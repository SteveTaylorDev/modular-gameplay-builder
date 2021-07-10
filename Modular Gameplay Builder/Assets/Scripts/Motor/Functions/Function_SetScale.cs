using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SetScale", menuName = "Base/Functions/Set Scale")]
public class Function_SetScale : Function_Base
{
    [Header("Source Transform")]
    public string sourceTransform;

    [Header("Target Scale")]
    public string xScale;
    public string yScale;
    public string zScale;

    [Header("Options")]
    public float slerpTime;
    public string floatMultiplier;
    public float floatOffset;


    public override void RunFunction(Motor_Base motor)
    {
        float localSlerpTime = slerpTime;
        if (localSlerpTime == 0) localSlerpTime = 1;
        if (floatMultiplier != "") localSlerpTime *= motor.variablesMod.FindFloat(floatMultiplier).modFloat + floatOffset;

        if(sourceTransform != "")
        {
            Vector3 localScale = new Vector3(1, 1, 1);

            if (xScale != "") localScale.x = motor.variablesMod.FindFloat(xScale).modFloat;
            if (yScale != "") localScale.y = motor.variablesMod.FindFloat(yScale).modFloat;
            if (zScale != "") localScale.z = motor.variablesMod.FindFloat(zScale).modFloat;

            if (slerpTime == 0) motor.variablesMod.FindTransform(sourceTransform).modTransform.localScale = localScale;
            else motor.variablesMod.FindTransform(sourceTransform).modTransform.localScale = Vector3.Lerp(motor.variablesMod.FindTransform(sourceTransform).modTransform.localScale, localScale, slerpTime * Time.deltaTime);
        }
    }
}
