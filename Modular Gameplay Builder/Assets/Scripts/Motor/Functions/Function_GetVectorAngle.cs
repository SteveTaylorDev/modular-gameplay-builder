using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GetVectorAngle", menuName = "Base/Functions/Get Vector Angle")]
public class Function_GetVectorAngle : Function_Base
{
    [Header("First Vector")]
    public string firstVector;

    [Header("Second Vector")]
    public string secondVector;

    [Header("Float to Set")]
    public string setFloat;


    public override void RunFunction(Motor_Base motor)
    {
        if (firstVector != "" && secondVector != "" && setFloat != "") motor.variablesMod.FindFloat(setFloat).modFloat = Vector3.Angle(motor.variablesMod.FindVector(firstVector).modVector, motor.variablesMod.FindVector(secondVector).modVector);
    }
}
