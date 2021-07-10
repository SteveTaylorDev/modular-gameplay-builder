using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IfString", menuName = "Base/Conditions/If String")]
public class Condition_IfString : Condition_Base
{
    [Header("Target String")]
    public string targetString;

    [Header("Length Condition")]
    public bool lengthMoreThan;
    public bool lengthLessThan;
    public bool lengthEqualTo;
    public string conditionLength;



    public override bool IsCondition(Motor_Base motor)
    {
        bool conditionBool = default(bool);

        if (lengthMoreThan && motor.variablesMod.FindString(targetString).modString.Length > motor.variablesMod.FindFloat(conditionLength).modFloat) conditionBool = true;
        if (lengthLessThan && motor.variablesMod.FindString(targetString).modString.Length < motor.variablesMod.FindFloat(conditionLength).modFloat) conditionBool = true;
        if (lengthEqualTo && motor.variablesMod.FindString(targetString).modString.Length == motor.variablesMod.FindFloat(conditionLength).modFloat) conditionBool = true;

        if (inverseCondition) return !conditionBool;
        else return conditionBool;
    }
}
