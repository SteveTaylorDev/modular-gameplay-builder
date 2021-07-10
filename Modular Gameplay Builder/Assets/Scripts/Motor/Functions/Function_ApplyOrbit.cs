using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ApplyOrbit", menuName = "Base/Functions/(Camera) Apply Orbit")]
public class Function_ApplyOrbit : Function_Base
{
    [Header("Orbit Type")]
    public string rotateVector;
    public bool applyXOrbit;
    public bool applyYOrbit;
    public bool applyXVectorOrbit;

    [Header("Orbit Amount")]
    public bool invertAmount;
    public bool useInputVectorX;
    public bool useInputVectorZ;
    public bool useAltInputVectorX;
    public bool useAltInputVectorZ;
    public bool useMouseX;
    public bool useMouseY;
    public bool setManualOrbit;
    public float manualOrbit;

    [Header("Orbit Options")]
    public string orbitVectorAxis;
    public float lerpSpeed;
    public bool multiplyByTargetSpeed;
    public float speedOffset;
    public bool preserveManualOrbitSign;

    [Header("Orbit Sensitivity")]
    public bool applyXSensitivity;
    public bool applyYSensitivity;
    public bool applyManualSensitivity;
    public float manualSensitivity;
    public float sensitivityMultiplier;

    public override void RunFunction(Motor_Base motor)
    {
        if (motor.GetType() != typeof(Motor_Camera))
        {
            Debug.LogError("ApplyOrbit function cannot run as the motor it is attached to is not a Camera Motor. Attach a Motor_Camera component to this object to use the ApplyOrbit function.");
            return;
        }

        if ((motor as Motor_Camera).cameraTarget.parentMotor == null) return;

        Module_Input inputMod = (motor as Motor_Camera).cameraTarget.parentMotor.FindModule(typeof(Module_Input)) as Module_Input;
        Module_CameraTarget camTarget = (motor as Motor_Camera).cameraTarget.parentMotor.FindModule(typeof(Module_CameraTarget)) as Module_CameraTarget;

        float orbitAmount = 0;

        if (inputMod != null)
        {
            if (useInputVectorX) orbitAmount = motor.variablesMod.FindVector(inputMod.inputVector).modVector.x * Time.deltaTime;
            if (useInputVectorZ) orbitAmount = motor.variablesMod.FindVector(inputMod.inputVector).modVector.z * Time.deltaTime;
            if (useAltInputVectorX) orbitAmount = inputMod.altInputVector.x * Time.deltaTime;
            if (useAltInputVectorZ) orbitAmount = inputMod.altInputVector.z * Time.deltaTime;
            if (useMouseY) orbitAmount = inputMod.mouseVector.y;
            if (useMouseX) orbitAmount = inputMod.mouseVector.x;
        }

        if (invertAmount) orbitAmount = -orbitAmount;

        float localLerpSpeed = lerpSpeed;
        if (multiplyByTargetSpeed) localLerpSpeed *= motor.variablesMod.FindFloat("camTarget.Current Speed").modFloat + speedOffset;

        if (applyXOrbit)
        {
            float sensitivity = 1;
            if (applyManualSensitivity) sensitivity = manualSensitivity;
            if (applyXSensitivity && camTarget != null) sensitivity = camTarget.xOrbitSensitivity;
            if (sensitivityMultiplier != 0) sensitivity *= sensitivityMultiplier;

            float localManualOrbit = manualOrbit - new Vector3 ((motor as Motor_Camera).offset.x, 0, (motor as Motor_Camera).offset.z).magnitude;

            if (!setManualOrbit)
            {
                if(localLerpSpeed != 0) (motor as Motor_Camera).orbitX = Mathf.Lerp((motor as Motor_Camera).orbitX, (motor as Motor_Camera).orbitX + orbitAmount * (sensitivity), localLerpSpeed * Time.deltaTime);
                else (motor as Motor_Camera).orbitX += orbitAmount * (sensitivity);
            }
            else if (preserveManualOrbitSign)
            {
                if (localLerpSpeed != 0) (motor as Motor_Camera).orbitX = Mathf.Lerp((motor as Motor_Camera).orbitX, localManualOrbit * Mathf.Sign((motor as Motor_Camera).orbitX), localLerpSpeed * Time.deltaTime);
                else (motor as Motor_Camera).orbitX = localManualOrbit * Mathf.Sign((motor as Motor_Camera).orbitX);
            }
            else
            {
                if (localLerpSpeed != 0) (motor as Motor_Camera).orbitX = Mathf.Lerp((motor as Motor_Camera).orbitX, localManualOrbit, localLerpSpeed * Time.deltaTime);
                else (motor as Motor_Camera).orbitX = localManualOrbit;
            }
        }

        if (applyYOrbit)
        {
            float sensitivity = 1;
            if (applyManualSensitivity) sensitivity = manualSensitivity;
            if (applyYSensitivity && camTarget != null) sensitivity = camTarget.yOrbitSensitivity;
            if (sensitivityMultiplier != 0) sensitivity *= sensitivityMultiplier;

            float localManualOrbit = manualOrbit - (motor as Motor_Camera).offset.y;

            if (!setManualOrbit)
            {
                if (localLerpSpeed != 0) (motor as Motor_Camera).orbitY = Mathf.Lerp((motor as Motor_Camera).orbitY, (motor as Motor_Camera).orbitY + orbitAmount * (sensitivity), localLerpSpeed * Time.deltaTime);
                else (motor as Motor_Camera).orbitY += orbitAmount * (sensitivity);
            }
            else if (preserveManualOrbitSign)
            {
                if (localLerpSpeed != 0) (motor as Motor_Camera).orbitY = Mathf.Lerp((motor as Motor_Camera).orbitY, localManualOrbit * Mathf.Sign((motor as Motor_Camera).orbitY), localLerpSpeed * Time.deltaTime);
                else (motor as Motor_Camera).orbitY = localManualOrbit * Mathf.Sign((motor as Motor_Camera).orbitY);
            }
            else
            {
                if (localLerpSpeed != 0) (motor as Motor_Camera).orbitY = Mathf.Lerp((motor as Motor_Camera).orbitY, localManualOrbit, localLerpSpeed * Time.deltaTime);
                else (motor as Motor_Camera).orbitY = localManualOrbit;
            }
        }

        if (applyXVectorOrbit)
        {
            float sensitivity = 1;
            if (applyManualSensitivity) sensitivity = manualSensitivity;
            if (applyXSensitivity && camTarget != null) sensitivity = camTarget.xOrbitSensitivity;
            if (sensitivityMultiplier != 0) sensitivity *= sensitivityMultiplier;

            Vector3 localOrbitAxis = motor.transform.up;
            if (orbitVectorAxis != "") localOrbitAxis = motor.variablesMod.FindVector(orbitVectorAxis).modVector;

            if (localLerpSpeed != 0) motor.variablesMod.FindVector(rotateVector).modVector = Vector3.Slerp(motor.variablesMod.FindVector(rotateVector).modVector, Quaternion.AngleAxis(orbitAmount * (sensitivity), localOrbitAxis) * motor.variablesMod.FindVector(rotateVector).modVector, lerpSpeed * Time.deltaTime);
            else motor.variablesMod.FindVector(rotateVector).modVector = Quaternion.AngleAxis(orbitAmount * (sensitivity), localOrbitAxis) * motor.variablesMod.FindVector(rotateVector).modVector;
        }

    }
}
