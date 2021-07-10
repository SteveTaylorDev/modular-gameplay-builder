using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EditRectOffset", menuName = "Base/Functions/(Camera) Edit Rect Offset")]
public class Function_EditRectOffset : Function_Base
{
    [Header("Set Offset")]
    public bool useCurrentRectOffset;
    public bool useManualRectOffset;
    public Vector3 manualRectOffset;

    [Header("Speed Offset")]
    public string vectorOffset;
    public bool addSpeedOffset;
    public float speedOffsetAmount;
    public bool useVelocity;
    public float maxMagnitude;

    [Header("Options")]
    public bool transformOffset;
    public bool lerpOffset;
    public float lerpStrength;


    public override void RunFunction(Motor_Base motor)
    {
        if (motor.GetType() != typeof(Motor_Camera))
        {
            Debug.LogWarning("A Camera Edit Rect Offset function is trying to run, yet the motor it is attached to is not a Camera. Replace the motor with a Camera Motor to run this function.");
            return;
        }

        Vector3 localRectOffset = manualRectOffset;
        if (useCurrentRectOffset) localRectOffset = (motor as Motor_Camera).rectOffset;
        Vector3 speedOffset = Vector3.zero;

        if (vectorOffset != "") speedOffset = motor.variablesMod.FindVector(vectorOffset).modVector * speedOffsetAmount;

        if (transformOffset) localRectOffset = motor.transform.TransformDirection(localRectOffset);

        if(addSpeedOffset && (motor as Motor_Camera).cameraTarget.parentMotor != null)
        {
            //if (useVelocity)
            //{
            //    if(((motor as Motor_Camera).cameraTarget.parentMotor as Motor_Physics).rigidbodyVelocity != Vector3.zero) speedOffset = ((motor as Motor_Camera).cameraTarget.parentMotor as Motor_Physics).rigidbodyVelocity * speedOffsetAmount;
            //}
            //else
            //{
            //    if (motor.variablesMod.FindFloat(motor.currentSpeed).modFloat != 0) speedOffset = (motor as Motor_Camera).cameraTarget.parentMotor.moveVector * speedOffsetAmount;
            //}

            if (speedOffset.magnitude > maxMagnitude) speedOffset = speedOffset.normalized * maxMagnitude;
        }

        if (lerpOffset) (motor as Motor_Camera).rectOffset = Vector3.Lerp((motor as Motor_Camera).rectOffset, localRectOffset + speedOffset, lerpStrength * Time.deltaTime);
        else (motor as Motor_Camera).rectOffset = localRectOffset + speedOffset;
    }
}
