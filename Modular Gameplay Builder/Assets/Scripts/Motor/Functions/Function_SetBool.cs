using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SetBool", menuName = "Base/Functions/Set Bool")]
public class Function_SetBool : Function_Base
{
    [Header("Source Bool")]
    public string setBool;

    [Header("Target Bool")]
    public string targetBool;
    public bool inverseSourceBool;


    public override void RunFunction(Motor_Base motor)
    {
        if(targetBool != "") motor.variablesMod.FindBool(setBool).modBool = motor.variablesMod.FindBool(targetBool).modBool;
        if (inverseSourceBool) motor.variablesMod.FindBool(setBool).modBool = !motor.variablesMod.FindBool(setBool).modBool;
    }
}
