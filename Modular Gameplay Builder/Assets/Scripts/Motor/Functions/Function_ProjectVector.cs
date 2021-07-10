using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectVector", menuName = "Base/Functions/Project Vector")]
public class Function_ProjectVector : Function_Base
{
    [Header("Projection Vector")]
    public string projectVector;
    public bool projectOrientationVector;
    public bool projectDirectionVector;
    public bool projectMoveDirection;
    public bool projectMoveVector;
    public bool projectPhysicsVector;
    public bool projectInputVector;
    public bool projectWallAverage;
    public bool projectMeshDirection;
    public bool projectOrbitVector;
    public bool projectVelocity;
    public bool useCameraTargetForProject;
    public bool useParentCameraForProject;

    [Header("Target Vector")]
    public string targetVector;
    public bool ontoOrientationVector;
    public bool ontoGroundAverage;
    public bool ontoWallAverage;
    public bool ontoLocalGravity;
    public bool ontoGlobalGravity;
    public bool ontoMeshOrientation;
    public bool ontoCamTargetOrientation;
    public bool ontoCamTargetMeshOrientation;
    public bool ontoOrbitVector;
    public bool ontoManualVector;
    public Vector3 manualVector;
    public bool useCameraTargetForTarget;
    public bool useParentCameraForTarget;

    [Header("Lerp Options")]
    public float angleDifference;
    public float lerpSpeed;
    public string floatMultiplier;
    public string floatDivision;
    public float offsetFloatAmount;

    [Header("Other Options")]
    public bool reverseTargetVector;
    public bool normalizeTargetVector;
    public bool normalizeResult;
    public bool ignoreZeroResult;
    public bool preserveMagnitude;


    private void DisableProjectBools()
    {
        projectOrientationVector = false;
        projectDirectionVector = false;
        projectMoveDirection = false;
        projectMoveVector = false;
        projectPhysicsVector = false;
        projectInputVector = false;
        projectWallAverage = false;
        projectMeshDirection = false;
        projectOrbitVector = false;
        projectVelocity = false;
    }

    public override void RunFunction(Motor_Base motor)
    {
        Motor_Base targetMotor = motor;
        if (useCameraTargetForTarget) targetMotor = (motor as Motor_Camera).cameraTarget.parentMotor;
        if (useParentCameraForTarget) targetMotor = (motor.FindModule(typeof(Module_CameraTarget)) as Module_CameraTarget).currentCamera;

        Vector3 localTargetVector = Vector3.zero;

     // Normal Vector

        //if (ontoOrientationVector)
        //{
        //    targetVector = targetMotor.orientationVector;
        //}sxcsd

        if (targetVector != "")
        {
            localTargetVector = motor.variablesMod.FindVector(targetVector).modVector;
        }

        if (ontoManualVector)
        {
            localTargetVector = manualVector;
        }

        if (ontoGroundAverage)
        {
            Module_GroundDetection groundMod = targetMotor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;
            if (groundMod != null) localTargetVector = groundMod.averageGroundNormal;
            else Debug.LogError("ProjectVector function is trying to reference a Ground Detection module. Add one to this motor, or change the chosen vector, to run function.");
        }

        if (ontoWallAverage)
        {
            Module_Collision collisionMod = targetMotor.FindModule(typeof(Module_Collision)) as Module_Collision;
            if (collisionMod != null) localTargetVector = collisionMod.collisionWallAverage;
            else Debug.LogError("ProjectVector function is trying to reference a Collision module. Add one to this motor, or change the chosen vector, to run function.");
        }

        if (ontoLocalGravity)
        {
            Module_Gravity gravityMod = targetMotor.FindModule(typeof(Module_Gravity)) as Module_Gravity;
            if (gravityMod != null) localTargetVector = gravityMod.localGravityDirection;
            else Debug.LogError("ProjectVector function is trying to reference a Gravity module. Add one to this motor, or change the chosen vector, to run function.");
        }

        if (ontoGlobalGravity)
        {
            localTargetVector = GameController.Instance.globalGravityDirection;
        }

        //if (ontoMeshOrientation)
        //{
        //    Module_Mesh meshMod = targetMotor.FindModule(typeof(Module_Mesh)) as Module_Mesh;
        //    if (meshMod != null) targetVector = meshMod.meshOrientation;
        //    else Debug.LogError("ProjectVector function is trying to reference a Mesh module. Add one to this motor, or change the chosen vector, to run function.");
        //}

        //if (ontoCamTargetOrientation)
        //{
        //    if(targetMotor.GetType() == typeof(Motor_Camera))
        //    {
        //        targetVector = (targetMotor as Motor_Camera).cameraTarget.parentMotor.orientationVector;
        //    }
        //    else Debug.LogError("ProjectVector function is trying to reference a Camera motor, yet the motor this function is attached to is not a Camera. Change the motor, or change the chosen vector, to run function.");
        //}

        //if (ontoCamTargetMeshOrientation)
        //{
        //    if (targetMotor.GetType() == typeof(Motor_Camera))
        //    {
        //        Module_Mesh meshMod = (targetMotor as Motor_Camera).cameraTarget.parentMotor.FindModule(typeof(Module_Mesh)) as Module_Mesh;
        //        if (meshMod != null) targetVector = meshMod.meshOrientation;
        //        else Debug.LogError("ProjectVector function is trying to reference a Mesh module. Add one to this motor, or change the chosen vector, to run function.");            
        //    }
        //    else Debug.LogError("ProjectVector function is trying to reference a Camera motor, yet the motor this function is attached to is not a Camera. Change the motor, or change the chosen vector, to run function.");
        //}

        //if (ontoOrbitVector)
        //{
        //    if (targetMotor.GetType() == typeof(Motor_Camera))
        //    {
        //        targetVector = (motor as Motor_Camera).orbitVector;
        //    }
        //    else Debug.LogError("ProjectVector function is trying to reference a Camera motor, yet the motor this function is attached to is not a Camera. Change the motor, or change the chosen vector, to run function.");
        //}

        if (reverseTargetVector) localTargetVector = -localTargetVector;
        if (normalizeTargetVector) localTargetVector = localTargetVector.normalized;

     // Project Vector

        Motor_Base projectMotor = motor;
        if (useCameraTargetForProject) projectMotor = (motor as Motor_Camera).cameraTarget.parentMotor;
        if (useParentCameraForProject) projectMotor = (motor.FindModule(typeof(Module_CameraTarget)) as Module_CameraTarget).currentCamera;
        if (projectMotor == null) return;

        float localLerpSpeed = lerpSpeed;

        if (floatMultiplier != "")
        {
            localLerpSpeed *= projectMotor.variablesMod.FindFloat(floatMultiplier).modFloat + offsetFloatAmount;
        }

        if (floatDivision != "")
        {
            localLerpSpeed /= projectMotor.variablesMod.FindFloat(floatDivision).modFloat + offsetFloatAmount;
        }

        if (Mathf.Abs(localTargetVector.sqrMagnitude) > 0)
        {
            if(projectVector != "")
            {
                Vector3 projectionResult = ProjectVector(motor.variablesMod.FindVector(projectVector).modVector, localTargetVector);

                if (localLerpSpeed != 0) motor.variablesMod.FindVector(projectVector).modVector = Vector3.Slerp(motor.variablesMod.FindVector(projectVector).modVector, projectionResult, localLerpSpeed * Time.deltaTime);
                else motor.variablesMod.FindVector(projectVector).modVector = projectionResult;
            }

            //if (projectOrientationVector)
            //{
            //    DisableProjectBools();
            //    projectOrientationVector = true;
            //
            //    if (Vector3.Angle(projectMotor.orientationVector, targetVector.normalized) > angleDifference)
            //    {
            //        Vector3 projectionResult = ProjectVector(projectMotor.orientationVector, targetVector);
            //
            //        if (localLerpSpeed != 0) projectMotor.orientationVector = Vector3.Slerp(projectMotor.orientationVector, projectionResult, localLerpSpeed * Time.deltaTime);
            //        else projectMotor.orientationVector = projectionResult;
            //    }
            //}

            //if (projectDirectionVector)
            //{
            //    DisableProjectBools();
            //    projectDirectionVector = true;
            //
            //    if (Vector3.Angle(projectMotor.directionVector, targetVector.normalized) > angleDifference)
            //    {
            //        Vector3 projectionResult = ProjectVector(projectMotor.directionVector, targetVector);
            //
            //        if (localLerpSpeed != 0) projectMotor.directionVector = Vector3.Slerp(projectMotor.directionVector, projectionResult, localLerpSpeed * Time.deltaTime);
            //        else projectMotor.directionVector = projectionResult;
            //    }
            //}

            if (projectMoveDirection)
            {
                DisableProjectBools();
                projectMoveDirection = true;

                if (Vector3.Angle(projectMotor.variablesMod.FindVector("Move Direction").modVector, localTargetVector.normalized) >= angleDifference)
                {
                    Vector3 projectionResult = ProjectVector(projectMotor.variablesMod.FindVector("Move Direction").modVector, localTargetVector);

                    if (localLerpSpeed != 0) projectMotor.variablesMod.FindVector("Move Direction").modVector = Vector3.Slerp(projectMotor.variablesMod.FindVector("Move Direction").modVector, projectionResult, localLerpSpeed * Time.deltaTime);
                    else projectMotor.variablesMod.FindVector("Move Direction").modVector = projectionResult;
                }
            }

            if (projectMoveVector)
            {
                DisableProjectBools();
                projectMoveVector = true;

                if (Vector3.Angle(projectMotor.moveVector, localTargetVector.normalized) >= angleDifference)
                {
                    Vector3 projectionResult = ProjectVector(projectMotor.moveVector, localTargetVector);

                    if (localLerpSpeed != 0) projectMotor.moveVector = Vector3.Slerp(projectMotor.moveVector, projectionResult, localLerpSpeed * Time.deltaTime);
                    else projectMotor.moveVector = projectionResult;
                }
            }

            if (projectPhysicsVector)
            {
                if (projectMotor.GetType() != typeof(Motor_Physics))
                {
                    Debug.LogError("ProjectVector function cannot use physicsVector as the motor it is attached to is not a Physics Motor. Attach a Motor_Physics component to this object to use physicsVector in any functions.");
                    return;
                }

                DisableProjectBools();
                projectPhysicsVector = true;

                if (Vector3.Angle((projectMotor as Motor_Physics).variablesMod.FindVector((projectMotor as Motor_Physics).physicsVector).modVector, localTargetVector.normalized) > angleDifference)
                {
                    Vector3 projectionResult = ProjectVector((projectMotor as Motor_Physics).variablesMod.FindVector((projectMotor as Motor_Physics).physicsVector).modVector, localTargetVector);

                    if (localLerpSpeed != 0) (projectMotor as Motor_Physics).variablesMod.FindVector((projectMotor as Motor_Physics).physicsVector).modVector = Vector3.Slerp((projectMotor as Motor_Physics).variablesMod.FindVector((projectMotor as Motor_Physics).physicsVector).modVector, projectionResult, localLerpSpeed * Time.deltaTime);
                    else (projectMotor as Motor_Physics).variablesMod.FindVector((projectMotor as Motor_Physics).physicsVector).modVector = projectionResult;
                }
            }

            if (projectInputVector)
            {
                DisableProjectBools();
                projectInputVector = true;

                Module_Input inputMod = projectMotor.FindModule(typeof(Module_Input)) as Module_Input;
                if (inputMod != null)
                {
                    if (Vector3.Angle(motor.variablesMod.FindVector(inputMod.inputVector).modVector, localTargetVector.normalized) > angleDifference)
                    {
                        Vector3 projectionResult = ProjectVector(motor.variablesMod.FindVector(inputMod.inputVector).modVector, localTargetVector);

                        if (localLerpSpeed != 0) motor.variablesMod.FindVector(inputMod.inputVector).modVector = Vector3.Slerp(motor.variablesMod.FindVector(inputMod.inputVector).modVector, projectionResult, localLerpSpeed * Time.deltaTime);
                        else motor.variablesMod.FindVector(inputMod.inputVector).modVector = projectionResult;
                    }
                } 
                else Debug.LogError("ProjectVector function is trying to reference a Input module. Add one to this motor, or change the chosen vector, to run function.");
            }

            if (projectWallAverage)
            {
                DisableProjectBools();
                projectWallAverage = true;

                Module_Collision collisionMod = projectMotor.FindModule(typeof(Module_Collision)) as Module_Collision;
                if (collisionMod != null)
                {
                    if (Vector3.Angle(collisionMod.collisionWallAverage, localTargetVector.normalized) > angleDifference)
                    {
                        Vector3 projectionResult = ProjectVector(collisionMod.collisionWallAverage, localTargetVector);

                        if (localLerpSpeed != 0) collisionMod.collisionWallAverage = Vector3.Slerp(collisionMod.collisionWallAverage, projectionResult, localLerpSpeed * Time.deltaTime);
                        else collisionMod.collisionWallAverage = projectionResult;
                    }
                }
                else Debug.LogError("ProjectVector function is trying to reference a Collision module. Add one to this motor, or change the chosen vector, to run function.");
            }

            //if (projectMeshDirection)
            //{
            //    DisableProjectBools();
            //    projectWallAverage = true;
            //
            //    Module_Mesh meshMod = projectMotor.FindModule(typeof(Module_Mesh)) as Module_Mesh;
            //    if (meshMod != null)
            //    {
            //        if (Vector3.Angle(meshMod.meshDirection, targetVector.normalized) > angleDifference)
            //        {
            //            Vector3 projectionResult = ProjectVector(meshMod.meshDirection, targetVector);
            //
            //            if (localLerpSpeed != 0) meshMod.meshDirection = Vector3.Slerp(meshMod.meshDirection, projectionResult, localLerpSpeed * Time.deltaTime);
            //            else meshMod.meshDirection = projectionResult;
            //        }
            //    }
            //    else Debug.LogError("ProjectVector function is trying to reference a Mesh module. Add one to this motor, or change the chosen vector, to run function.");
            //}

            //if (projectOrbitVector)
            //{
            //    DisableProjectBools();
            //    projectOrbitVector = true;
            //
            //    if (projectMotor.GetType() == typeof(Motor_Camera))
            //    {
            //        if (Vector3.Angle((projectMotor as Motor_Camera).orbitVector, targetVector.normalized) > angleDifference)
            //        {
            //            Vector3 projectionResult = ProjectVector((projectMotor as Motor_Camera).orbitVector, targetVector);
            //
            //            if (localLerpSpeed != 0) (projectMotor as Motor_Camera).orbitVector = Vector3.Slerp((projectMotor as Motor_Camera).orbitVector, projectionResult, localLerpSpeed * Time.deltaTime);
            //            else (projectMotor as Motor_Camera).orbitVector = projectionResult;
            //        }
            //    }
            //    else Debug.LogError("ProjectVector function is trying to reference a Camera motor, yet the motor this function is attached to is not a Camera. Change the motor, or change the chosen vector, to run function.");
            //}

            //if (projectVelocity)
            //{
            //    if (projectMotor.GetType() != typeof(Motor_Physics))
            //    {
            //        Debug.LogError("ProjectVector function cannot use rigidbody velocity as the motor it is attached to is not a Physics Motor. Attach a Motor_Physics component to this object to use rigidbody velocity in any functions.");
            //        return;
            //    }
            //
            //    DisableProjectBools();
            //    projectVelocity = true;
            //
            //    if (Vector3.Angle((projectMotor as Motor_Physics).rigidbodyVelocity, localTargetVector) > angleDifference)
            //    {
            //        Vector3 projectionResult = ProjectVector((projectMotor as Motor_Physics).rigidbodyVelocity, localTargetVector);
            //
            //        if (localLerpSpeed != 0) (projectMotor as Motor_Physics).rigidbodyVelocity = Vector3.Slerp((projectMotor as Motor_Physics).rigidbodyVelocity, projectionResult, localLerpSpeed * Time.deltaTime);
            //        else (projectMotor as Motor_Physics).rigidbodyVelocity = projectionResult;
            //    }
            //}
        }

        else
        {
            //Debug.LogWarning("ProjectVector function currently has a projection normal with zero magnitude. ProjectVector will not be run.");
        }
    }

    Vector3 ProjectVector(Vector3 projectionInput, Vector3 targetVector)
    {
        Vector3 projectionVector = projectionInput;
        float magnitude = 1;
        if (preserveMagnitude)
        {
            normalizeResult = true;
            magnitude = projectionVector.magnitude;
        }

        Vector3 projectionResult = Vector3.ProjectOnPlane(projectionVector, targetVector);
        if (normalizeResult) projectionResult = projectionResult.normalized;

        projectionResult *= magnitude;

        if (ignoreZeroResult && projectionResult.sqrMagnitude == 0)
        {
            projectionResult = projectionInput;
            //Debug.Log(this +  " projection result is Vector3.zero and ignoreZeroResult is true. Projection vector will not be changed.");
        }
        return projectionResult;
    }
}
