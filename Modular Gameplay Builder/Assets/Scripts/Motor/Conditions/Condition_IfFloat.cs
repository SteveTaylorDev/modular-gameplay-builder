using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IfFloat", menuName = "Base/Conditions/If Float")]
public class Condition_IfFloat : Condition_Base
{
    [Header("Float to Measure")]
    public string measureFloat;

    [Header("Condition")]
    public bool moreThan;
    public bool lessThan;
    public bool equalTo;
    public string conditionFloat;



    public override bool IsCondition(Motor_Base motor)
    {
        bool conditionBool = default(bool);

        if (measureFloat == "") return false;

        float compareFloat = motor.variablesMod.FindFloat(measureFloat).modFloat;

        if ((moreThan && compareFloat > motor.variablesMod.FindFloat(conditionFloat).modFloat) || (lessThan && compareFloat < motor.variablesMod.FindFloat(conditionFloat).modFloat) || (equalTo && compareFloat == motor.variablesMod.FindFloat(conditionFloat).modFloat)) conditionBool = true;

        if (inverseCondition) return !conditionBool;
        else return conditionBool;
    }
}
