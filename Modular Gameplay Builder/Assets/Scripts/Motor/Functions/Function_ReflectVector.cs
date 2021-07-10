using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReflectVector", menuName = "Base/Functions/Reflect Vector")]
public class Function_ReflectVector : Function_Base
{
    [Header("Source Vector")]
    public string sourceVector;

    [Header("In Direction")]
    public string inDirectionVector;

    [Header("Reflect Vector")]
    public string reflectVector;
    public bool reflectOnWallAverage;


    public override void RunFunction(Motor_Base motor)
    {
        if(sourceVector != "" && inDirectionVector != "" && reflectVector != "")
        {
            Vector3 inDirection = motor.variablesMod.FindVector(inDirectionVector).modVector;
            Vector3 localReflectVector = motor.variablesMod.FindVector(reflectVector).modVector;

            if (reflectOnWallAverage) localReflectVector = (motor.FindModule(typeof(Module_Collision)) as Module_Collision).collisionWallAverage;

            motor.variablesMod.FindVector(sourceVector).modVector = Vector3.Reflect(inDirection, localReflectVector);

            Debug.DrawRay(motor.transform.position ,motor.variablesMod.FindVector(sourceVector).modVector = Vector3.Reflect(inDirection, localReflectVector), Color.red, 7);
        }
    }
}
