using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiplyQuaternion", menuName = "Base/Functions/Multiply Quaternion")]
public class Function_MultiplyQuaternion : Function_Base
{
    [Header("Source Quaternion")]
    public string sourceQuaternion;

    [Header("Target Quaternion")]
    public string targetQuaternion;

    [Header("Options")]
    public bool reverseMultiplyOrder;


    public override void RunFunction(Motor_Base motor)
    {
        if(sourceQuaternion != "")
        {
            if (targetQuaternion != "")
            {
                if(!reverseMultiplyOrder) motor.variablesMod.FindQuaternion(sourceQuaternion).modQuaternion *= motor.variablesMod.FindQuaternion(targetQuaternion).modQuaternion;
                else motor.variablesMod.FindQuaternion(sourceQuaternion).modQuaternion = motor.variablesMod.FindQuaternion(targetQuaternion).modQuaternion * motor.variablesMod.FindQuaternion(sourceQuaternion).modQuaternion;
            }
        }
    }   
}
