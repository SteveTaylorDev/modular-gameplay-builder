using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EditCameraFov", menuName = "Base/Functions/(Camera) Edit Fov")]
public class Function_EditCameraFov : Function_Base
{
    [Header("Manual Fov")]
    public bool useCurrentFov;
    public bool useManualFov;
    public float manualFov;

    [Header("Speed Multiplier")]
    public string multiplierFloat;
    public bool addSpeedMultiplier;
    public float speedMultiplierAmount;
    public float minSpeed;
    public float maxSpeed;
    public bool useVelocity;
    public bool useCameraTarget;

    [Header("Options")]
    public float minFov;
    public float maxFov;
    public bool lerpFov;
    public float lerpStrength;


    public override void RunFunction(Motor_Base motor)
    {
        if (motor.GetType() != typeof(Motor_Camera))
        {
            Debug.LogWarning("A Camera Motor Edit function is trying to run, yet the motor it is attached to is not a Camera. Replace the motor with a Camera Motor to run this function.");
            return;
        }

        Camera refCamera = (motor as Motor_Camera).motorCam;
        float fov = 0;

        if (useCurrentFov) fov = (motor as Motor_Camera).motorCam.fieldOfView;
        if (useManualFov) fov = manualFov;

        if (multiplierFloat != "")
        {
            Motor_Base targetMotor = motor;
            if (useCameraTarget) targetMotor = (motor as Motor_Camera).cameraTarget.parentMotor;

            float speed = motor.variablesMod.FindFloat(multiplierFloat).modFloat;
            //if (useVelocity) speed = (targetMotor as Motor_Physics).rigidbodyVelocity.magnitude;

            if (speed < minSpeed) speed = minSpeed;
            if (speed > maxSpeed) speed = maxSpeed;

            fov += speed * speedMultiplierAmount;
        }

        if (fov < minFov) fov = minFov;
        if (fov > maxFov) fov = maxFov;

        if (lerpFov) refCamera.fieldOfView = Mathf.Lerp(refCamera.fieldOfView, fov, lerpStrength * Time.deltaTime);
        else refCamera.fieldOfView = fov;
    }
}
