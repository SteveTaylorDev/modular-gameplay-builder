using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IfBool", menuName = "Base/Conditions/If Bool")]
public class Condition_IfBool : Condition_Base
{
    [Header("Bool")]
    public string targetBool;


    public override bool IsCondition(Motor_Base motor)
    {
        bool isBool = default(bool);

        isBool = motor.variablesMod.FindBool(targetBool).modBool;

        if (inverseCondition) return !isBool;
        else return isBool;
    }
}
