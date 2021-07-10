using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddVector", menuName = "Base/Functions/Add Vector")]
public class Function_AddVector : Function_Base
{
    [Header("Source Vector")]
    public string sourceVector;

    [Header("Add Vector")]
    public string addVector;
    public bool addManualVector;
    public Vector3 manualVector;

    [Header("Options")]
    public bool ignoreTime;
    public bool normalizeResult;
    public string addMultiplier;


    public override void RunFunction(Motor_Base motor)
    {
        if(sourceVector != "")
        {
            float deltaFloat = Time.deltaTime;
            if (ignoreTime) deltaFloat = 1;

            Vector3 localAddVector = Vector3.zero;
            if (addManualVector) localAddVector = manualVector;

            if (addVector != "")
            {
                float localMultiplier = 1;
                if (addMultiplier != "") localMultiplier = motor.variablesMod.FindFloat(addMultiplier).modFloat;
                localAddVector = motor.variablesMod.FindVector(addVector).modVector * localMultiplier;
            }

            Vector3 resultVector = motor.variablesMod.FindVector(sourceVector).modVector += localAddVector * deltaFloat;
            if (normalizeResult) resultVector.Normalize();

            motor.variablesMod.FindVector(sourceVector).modVector = resultVector;
        }
    }   
}
