using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IfVectorMagnitude", menuName = "Base/Conditions/If Vector Magnitude")]
public class Condition_IfVectorMagnitude : Condition_Base
{
    [Header("Vector to Measure")]
    public string vector;

    [Header("Condition")]
    public bool moreThan;
    public bool lessThan;
    public bool equalTo;
    public float conditionAmount;



    public override bool IsCondition(Motor_Base motor)
    {
        bool conditionBool = default(bool);

        float compareMagntiude = motor.variablesMod.FindVector(vector).modVector.magnitude;

        if ((moreThan && compareMagntiude > conditionAmount) || (lessThan && compareMagntiude < conditionAmount) || (equalTo && compareMagntiude == conditionAmount)) conditionBool = true;

        if (inverseCondition) return !conditionBool;
        else return conditionBool;
    }
}
