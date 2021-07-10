using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SetString", menuName = "Base/Functions/Set String")]
public class Function_SetString : Function_Base
{
    [Header("Source String")]
    public string setString;

    [Header("Target String")]
    public string targetString;


    public override void RunFunction(Motor_Base motor)
    {
        if(targetString != "") motor.variablesMod.FindString(setString).modString = motor.variablesMod.FindString(targetString).modString;
    }
}
