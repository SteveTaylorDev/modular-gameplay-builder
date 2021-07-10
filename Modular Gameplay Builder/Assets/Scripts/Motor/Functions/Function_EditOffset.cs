using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EditOffset", menuName = "Base/Functions/(Camera) Edit Offset")]
public class Function_EditOffset : Function_Base
{
    [Header("Set Offset Direction")]
    public bool useCurrentOffset;
    public bool useManualOffset;
    public Vector3 manualOffset;

    [Header("Speed Offset")]
    public string vectorOffset;
    public bool useSpeedOffset;
    public float speedOffsetAmount;
    public float minSpeed;
    public float maxSpeed;
    public bool useVelocity;
    public bool transformSpeedOffset;

    [Header("Options")]
    public bool normalizeOffset;
    public bool transformOffset;
    public bool lerpOffset;
    public float lerpStrength;


    public override void RunFunction(Motor_Base motor)
    {
        if (motor.GetType() != typeof(Motor_Camera))
        {
            Debug.LogWarning("A Camera Edit Offset function is trying to run, yet the motor it is attached to is not a Camera. Replace the motor with a Camera Motor to run this function.");
            return;
        }

        Vector3 localOffset = manualOffset;
        if (useCurrentOffset) localOffset = (motor as Motor_Camera).offset;
        Vector3 speedOffset = Vector3.zero;

        if (vectorOffset != "") speedOffset = motor.variablesMod.FindVector(vectorOffset).modVector * speedOffsetAmount;

        if (transformOffset) localOffset = motor.transform.TransformDirection(localOffset);

        if (useSpeedOffset && (motor as Motor_Camera).cameraTarget.parentMotor != null)
        {
            //if (useVelocity)
            //{
            //    if(((motor as Motor_Camera).cameraTarget.parentMotor as Motor_Physics).rigidbodyVelocity != Vector3.zero) speedOffset = ((motor as Motor_Camera).cameraTarget.parentMotor as Motor_Physics).rigidbodyVelocity * ((motor as Motor_Camera).cameraTarget.parentMotor as Motor_Physics).rigidbodyVelocity.magnitude * speedOffsetAmount;
            //}
            //else
            //{
            //    if (motor.variablesMod.FindFloat(motor.currentSpeed).modFloat != 0) speedOffset = (motor as Motor_Camera).cameraTarget.parentMotor.moveVector * speedOffsetAmount;
            //}
            
            if (speedOffset.magnitude > maxSpeed) speedOffset = speedOffset.normalized * maxSpeed;
            if (speedOffset.magnitude < minSpeed) speedOffset = speedOffset.normalized * minSpeed;
        }

        if (transformSpeedOffset) speedOffset = motor.transform.InverseTransformDirection(speedOffset);
        Vector3 finalOffset = localOffset + speedOffset;

        if (normalizeOffset) finalOffset = finalOffset.normalized;

        if (lerpOffset) (motor as Motor_Camera).offset = Vector3.Lerp((motor as Motor_Camera).offset, finalOffset, lerpStrength * Time.deltaTime);
        else (motor as Motor_Camera).offset = finalOffset;
    }
}
