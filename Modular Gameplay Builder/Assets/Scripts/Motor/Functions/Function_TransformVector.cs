using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TransformVector", menuName = "Base/Functions/Transform Vector")]
public class Function_TransformVector : Function_Base
{
    [Header("Vector")]
    public string sourceVector;
    public bool useInputVector;
    public bool useCameraTargetInput;

    [Header("Transform Quaternion")]
    public string transformQuaternion;
    public bool byRotationQuaternion;
    public bool byCameraRotation;
    public bool byOrbitVectorRotation;
    public string manualOrientation;
    public string manualDirection;


    public override void RunFunction(Motor_Base motor)
    {
        Quaternion rotToTransformBy = default(Quaternion);

        if(transformQuaternion != "")
        {
            rotToTransformBy = motor.variablesMod.FindQuaternion(transformQuaternion).modQuaternion;
        }

        //if (byRotationQuaternion)
        //{
        //    rotToTransformBy = motor.rotationQuaternion;
        //}

        if (byCameraRotation)
        {
            Module_CameraTarget cameraMod = motor.FindModule(typeof(Module_CameraTarget)) as Module_CameraTarget;
            if (cameraMod != null && cameraMod.currentCamera != null) rotToTransformBy = cameraMod.currentCamera.transform.rotation;
            else Debug.LogError("TransformVector function is trying to reference a CameraTarget module. Add one to this motor, or change the chosen rotation, to run function.");
        }

        if(manualDirection != "" && manualOrientation != "")
        {
            rotToTransformBy = Quaternion.LookRotation(motor.variablesMod.FindVector(manualDirection).modVector, motor.variablesMod.FindVector(manualOrientation).modVector);
        }

        //if (byOrbitVectorRotation)
        //{
        //    Module_CameraTarget cameraMod = motor.FindModule(typeof(Module_CameraTarget)) as Module_CameraTarget;
        //
        //    if (motor.GetType() == typeof(Motor_Camera)) rotToTransformBy = Quaternion.LookRotation((motor as Motor_Camera).orbitVector, motor.orientationVector);
        //    else if(cameraMod.currentCamera != null) rotToTransformBy = Quaternion.LookRotation(cameraMod.currentCamera.orbitVector, motor.orientationVector);
        //    //else Debug.LogError("TransformVector function is trying to reference a Camera motor, but the chosen motor is not a Camera, or does not have a CameraTarget module with a camera attached. Change the motor, or add a CameraTarget module with a camera, to run function.");
        //}

        if(sourceVector != "")
        {
            motor.variablesMod.FindVector(sourceVector).modVector = rotToTransformBy * motor.variablesMod.FindVector(sourceVector).modVector;
        }

        if (useInputVector)
        {
            Module_Input inputMod = motor.FindModule(typeof(Module_Input)) as Module_Input;
            if (inputMod != null) motor.variablesMod.FindVector(inputMod.inputVector).modVector = rotToTransformBy * motor.variablesMod.FindVector(inputMod.inputVector).modVector;
            else Debug.LogError("TransformVector function is trying to reference a Input module. Add one to this motor, or change the chosen vector, to run function.");
        }

        if (useCameraTargetInput && motor.GetType() == typeof(Motor_Camera))
        {
            Module_Input inputMod = (motor as Motor_Camera).cameraTarget.parentMotor.FindModule(typeof(Module_Input)) as Module_Input;
            if (inputMod != null) motor.variablesMod.FindVector(inputMod.inputVector).modVector = rotToTransformBy * motor.variablesMod.FindVector(inputMod.inputVector).modVector;
            else Debug.LogError("TransformVector function is trying to reference a Input module. Add one to this motor, or change the chosen vector, to run function.");
        }

    }
}
