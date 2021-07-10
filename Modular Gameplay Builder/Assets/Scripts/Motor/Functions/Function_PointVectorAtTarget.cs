using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PointVectorAtTarget", menuName = "Base/Functions/Point Vector At Target")]
public class Function_PointVectorAtTarget : Function_Base
{
    [Header("Point Vector")]
    public string pointVector;
    public bool pointDirectionVector;
    public bool pointInputVector;

    [Header("Start Position")]
    public bool fromMotorPosition;
    public bool fromCameraTarget;

    [Header("Point Target")]
    public bool atCameraTarget;
    public bool atCameraTargetMesh;
    public bool atInputTarget;

    [Header("Lerp Options")]
    public bool lerpPoint;
    public bool slerpPoint;
    public float lerpSpeed;
    public string lerpFloatMultiplier;
    public float multiplierOffset;

    [Header("Additional Options")]
    public bool normalizeVector;
    public bool applyCameraRectOffset;


    public override void RunFunction(Motor_Base motor)
    {
        Vector3 startPosition = Vector3.zero;
        if (fromMotorPosition)
        {
            if (motor.setToPosition) startPosition = motor.moveVector;
            else startPosition = motor.transform.position;
        }

        if (fromCameraTarget)
        {
            startPosition = (motor as Motor_Camera).cameraTarget.transform.position;
        }

        Vector3 targetPosition = Vector3.zero;

        if (atCameraTarget)
        {
            if ((motor as Motor_Camera).overrideTarget != null) targetPosition = (motor as Motor_Camera).overrideTarget.position;
            else targetPosition = (motor as Motor_Camera).cameraTarget.transform.position;
        }
        if (atCameraTargetMesh)
        {
            if ((motor as Motor_Camera).overrideTarget != null) targetPosition = (motor as Motor_Camera).overrideTarget.position;
            else targetPosition = ((motor as Motor_Camera).cameraTarget.parentMotor.FindModule(typeof(Module_Mesh)) as Module_Mesh).currentMeshInstance.transform.position;
        }
        if (atInputTarget)
        {
            targetPosition = (motor.FindModule(typeof(Module_Input))as Module_Input).inputTarget.transform.position;
        }

        // Apply rectOffset
        if (applyCameraRectOffset) targetPosition += (motor as Motor_Camera).rectOffset;
        //

        Vector3 localPointVector = (targetPosition - startPosition);
        if (normalizeVector) localPointVector = localPointVector.normalized;

        float localLerpSpeed = lerpSpeed;

        if (lerpFloatMultiplier != "") localLerpSpeed *= motor.variablesMod.FindFloat(lerpFloatMultiplier).modFloat + multiplierOffset;

        if(pointVector != "" && motor.variablesMod != null)
        {
            Module_Variables.ModularVector localModVector = motor.variablesMod.FindVector(pointVector);

            if (lerpPoint) localModVector.modVector = Vector3.Lerp(localModVector.modVector, localPointVector, localLerpSpeed * Time.deltaTime);
            else if (slerpPoint) localModVector.modVector = Vector3.Slerp(localModVector.modVector, localPointVector, localLerpSpeed * Time.deltaTime);
            else localModVector.modVector = localPointVector;
        }

        //if (pointDirectionVector)
        //{
        //    if (lerpPoint) motor.directionVector = Vector3.Lerp(motor.directionVector, pointVector, lerpSpeed * Time.deltaTime);
        //    else if (slerpPoint) motor.directionVector = Vector3.Slerp(motor.directionVector, pointVector, lerpSpeed * Time.deltaTime);
        //    else motor.directionVector = pointVector;
        //}

        if (pointInputVector)
        {
            Module_Input inputMod = motor.FindModule(typeof(Module_Input)) as Module_Input;

            if (lerpPoint) motor.variablesMod.FindVector(inputMod.inputVector).modVector = Vector3.Lerp(motor.variablesMod.FindVector(inputMod.inputVector).modVector, localPointVector, localLerpSpeed * Time.deltaTime);
            else if (slerpPoint) motor.variablesMod.FindVector(inputMod.inputVector).modVector = Vector3.Slerp(motor.variablesMod.FindVector(inputMod.inputVector).modVector, localPointVector, localLerpSpeed * Time.deltaTime);
            else motor.variablesMod.FindVector(inputMod.inputVector).modVector = localPointVector;
        }
    }
}
