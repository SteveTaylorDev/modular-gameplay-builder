using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SetRotation", menuName = "Base/Functions/Set Rotation")]
public class Function_SetRotation : Function_Base
{
    [Header("Source Quaternion")]
    public string sourceQuaternion;

    [Header("Target Transform")]
    public string targetTransform;
    public bool useLocalRotation;

    [Header("Target Motor")]
    public string targetMotor;
    public bool useLocalMotorRotation;

    [Header("Target Physics Motor")]
    public string targetPhysicsMotor;

    [Header("Options")]
    public float slerpTime;
    public string floatMultiplier;
    public float floatOffset;


    public override void RunFunction(Motor_Base motor)
    {
        if (motor.variablesMod == null) return;

        float localSlerpTime = slerpTime;
        if (floatMultiplier != "") localSlerpTime *= motor.variablesMod.FindFloat(floatMultiplier).modFloat + floatOffset;

        if(sourceQuaternion != "")
        {
            if (targetTransform != "" && motor.variablesMod.FindTransform(targetTransform).modTransform != null)
            {
                if (!useLocalRotation)
                {                  
                    if (localSlerpTime == 0) motor.variablesMod.FindTransform(targetTransform).modTransform.rotation = motor.variablesMod.FindQuaternion(sourceQuaternion).modQuaternion;
                    else motor.variablesMod.FindTransform(targetTransform).modTransform.rotation = Quaternion.Slerp(motor.variablesMod.FindTransform(targetTransform).modTransform.rotation, motor.variablesMod.FindQuaternion(sourceQuaternion).modQuaternion, localSlerpTime * Time.deltaTime);                    
                }
                else
                {
                    if (localSlerpTime == 0) motor.variablesMod.FindTransform(targetTransform).modTransform.localRotation = motor.variablesMod.FindQuaternion(sourceQuaternion).modQuaternion;
                    else motor.variablesMod.FindTransform(targetTransform).modTransform.localRotation = Quaternion.Slerp(motor.variablesMod.FindTransform(targetTransform).modTransform.localRotation, motor.variablesMod.FindQuaternion(sourceQuaternion).modQuaternion, localSlerpTime * Time.deltaTime);
                }
            }

            if (targetMotor != "" && motor.variablesMod.FindMotor(targetMotor).modMotor != null)
            {
                if (!useLocalMotorRotation)
                {
                    if (localSlerpTime == 0) motor.variablesMod.FindMotor(targetMotor).modMotor.transform.rotation = motor.variablesMod.FindQuaternion(sourceQuaternion).modQuaternion;
                    else motor.variablesMod.FindMotor(targetMotor).modMotor.transform.rotation = Quaternion.Slerp(motor.variablesMod.FindMotor(targetMotor).modMotor.transform.rotation, motor.variablesMod.FindQuaternion(sourceQuaternion).modQuaternion, localSlerpTime * Time.deltaTime);
                }
                else
                {
                    if (localSlerpTime == 0) motor.variablesMod.FindMotor(targetMotor).modMotor.transform.localRotation = motor.variablesMod.FindQuaternion(sourceQuaternion).modQuaternion;
                    else motor.variablesMod.FindMotor(targetMotor).modMotor.transform.localRotation = Quaternion.Slerp(motor.variablesMod.FindMotor(targetMotor).modMotor.transform.localRotation, motor.variablesMod.FindQuaternion(sourceQuaternion).modQuaternion, localSlerpTime * Time.deltaTime);
                }
            }

            if (targetPhysicsMotor != "" && motor.variablesMod.FindMotor(targetPhysicsMotor).modMotor != null)
            {
                if (motor.variablesMod.FindMotor(targetPhysicsMotor).modMotor.GetType() != typeof(Motor_Physics))
                {
                    Debug.LogWarning(this + " function has" + targetPhysicsMotor + " set as the targetPhysicsMotor, yet the motor entered is not a Physics Motor.");
                    return;
                }
                if (localSlerpTime == 0) ((motor.variablesMod.FindMotor(targetPhysicsMotor).modMotor) as Motor_Physics).motorRigidbody.MoveRotation(motor.variablesMod.FindQuaternion(sourceQuaternion).modQuaternion);
                else ((motor.variablesMod.FindMotor(targetPhysicsMotor).modMotor) as Motor_Physics).motorRigidbody.MoveRotation (Quaternion.Slerp(((motor.variablesMod.FindMotor(targetPhysicsMotor).modMotor) as Motor_Physics).motorRigidbody.rotation, motor.variablesMod.FindQuaternion(sourceQuaternion).modQuaternion, localSlerpTime * Time.deltaTime));
            }
        }
    }
}
