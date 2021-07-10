using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SetFloat", menuName = "Base/Functions/Set Float")]
public class Function_SetFloat : Function_Base
{
    [Header("Source Float")]
    public string setFloat;

    [Header("Target Float")]
    public string targetFloat;


    public override void RunFunction(Motor_Base motor)
    {
        if (targetFloat != "") motor.variablesMod.FindFloat(setFloat).modFloat = motor.variablesMod.FindFloat(targetFloat).modFloat;
    }
}
