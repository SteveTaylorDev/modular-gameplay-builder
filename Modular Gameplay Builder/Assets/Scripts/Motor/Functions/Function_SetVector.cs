using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SetVector", menuName = "Base/Functions/Set Vector")]
public class Function_SetVector : Function_Base
{
    [Header("Vector to Set")]
    public string setVector;
    public bool setDirectionVector;
    public bool setOrientationVector;
    public bool setMoveDirection;
    public bool setAdditionalMoveVector;
    public bool setPhysicsVector;
    public bool setInputVector;
    public bool setMeshDirection;
    public bool setMeshOrientation;
    public bool setOrbitVector;
    public bool setTargetPosition;
    public bool useCameraTarget;
    public bool useParentCamera;
    public bool preserveSetVectorMagnitude;

    [Header("Target Vector")]
    public string targetVector;
    public bool toRigidbodyVelocity;
    public bool toMotorPosition;
    public bool toMotorForward;
    public bool reverseVector;
    public bool normalizeVector;
    public bool toInputVector;
    public bool toDirectionVector;
    public bool toOrientationVector;
    public bool toMoveDirection;
    public bool toPhysicsDirection;
    public bool toVelocityDirection;
    public bool toLocalGravity;
    public bool toGlobalGravity;
    public bool toGroundAverage;
    public bool toCollisionGroundAverage;
    public bool toCollisionWallAverage;
    public bool toSlopeVector;
    public bool toMeshOrientation;
    public bool toCamTargetOrientation;
    public bool toCamTargetMoveDirection;
    public bool toCamTargetMeshOrientation;
    public bool toCamTargetGravity;
    public bool toManualVector;
    public Vector3 manualVector;
    public bool useCameraTargetAsTarget;

    [Header("Options")]
    public bool lerpTowards;
    public bool slerpTowards;
    public float lerpSpeed;
    public float maximumLerpSpeed;
    public float minimumLerpSpeed;
    public string floatMultiplier;
    public float offsetFloatAmount;
    public string floatDivision;
    public float offsetDivisionFloatAmount;
    public bool multiplyByCurrentSpeed;
    public bool divideByCurrentSpeed;
    public bool multiplyByVelocityMagnitude;
    public bool divideByVelocityMagnitude;
    public bool multiplyByTargetCurrentSpeed;
    public bool divideByTargetCurrentSpeed;
    public bool multiplyByTargetVelocityMagnitude;
    public bool divideByTargetVelocityMagnitude;
    public float offsetSpeedAmount;
    public bool multiplyBySlopeMagnitude;
    public bool ignoreZeroResult;

    public override void RunFunction(Motor_Base motor)
    {
        Vector3 localTargetVector = Vector3.zero;
        Motor_Base targetMotor = motor;
        if (useCameraTargetAsTarget) targetMotor = (motor as Motor_Camera).cameraTarget.parentMotor;

        if (targetVector != "")
        {
            localTargetVector = motor.variablesMod.FindVector(targetVector).modVector;
        }

        if (toRigidbodyVelocity)
        {
            localTargetVector = motor.GetComponent<Rigidbody>().velocity;
        }

        if (toMotorPosition)
        {
            localTargetVector = targetMotor.transform.position;
        }

        if (toMotorForward)
        {
            localTargetVector = targetMotor.transform.forward;
        }

        if (toInputVector)
        {
            Module_Input inputMod = targetMotor.FindModule(typeof(Module_Input)) as Module_Input;
            if (inputMod != null) localTargetVector = motor.variablesMod.FindVector(inputMod.inputVector).modVector;
            else Debug.LogError("SetVector function is trying to reference a Input module. Add one to this motor, or change the chosen vector, to run function.");
        }

        //if (toDirectionVector)
        //{
        //    targetVector = targetMotor.directionVector;
        //}

        //if (toOrientationVector)
        //{
        //    targetVector = targetMotor.orientationVector;
        //}

        if (toMoveDirection)
        {
            localTargetVector = targetMotor.variablesMod.FindVector("Move Direction").modVector;
        }

        if (toPhysicsDirection && targetMotor.GetType() == typeof(Motor_Physics))
        {
            localTargetVector = (targetMotor as Motor_Physics).variablesMod.FindVector((targetMotor as Motor_Physics).physicsVector).modVector;
        }

        //if (toVelocityDirection && targetMotor.GetType() == typeof(Motor_Physics))
        //{
        //    localTargetVector = (targetMotor as Motor_Physics).rigidbodyVelocity.normalized;
        //}

        if (toGroundAverage)
        {
            Module_GroundDetection groundMod = targetMotor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;
            if (groundMod != null) localTargetVector = groundMod.averageGroundNormal;
            else Debug.LogError("SetVector function is trying to reference a Ground Detection module. Add one to this motor, or change the chosen vector, to run function.");
        }

        if (toLocalGravity)
        {
            Module_Gravity gravMod = targetMotor.FindModule(typeof(Module_Gravity)) as Module_Gravity;
            if (gravMod != null) localTargetVector = gravMod.localGravityDirection;
            else Debug.LogError("SetVector function is trying to reference a Gravity module. Add one to this motor, or change the chosen vector, to run function.");
        }

        if (toGlobalGravity)
        {
            localTargetVector = GameController.Instance.globalGravityDirection;
        }

        if (toCollisionGroundAverage)
        {
            Module_Collision collisionMod = targetMotor.FindModule(typeof(Module_Collision)) as Module_Collision;
            localTargetVector = collisionMod.collisionGroundAverage;
        }

        if (toCollisionWallAverage)
        {
            Module_Collision collisionMod = targetMotor.FindModule(typeof(Module_Collision)) as Module_Collision;
            localTargetVector = collisionMod.collisionWallAverage;
        }

       //if (toSlopeVector)
       //{
       //    Module_Slopes slopesMod = targetMotor.FindModule(typeof(Module_Slopes)) as Module_Slopes;
       //    localTargetVector = slopesMod.slopeVector;
       //}

        //if (toMeshOrientation)
        //{
        //    Module_Mesh meshMod = targetMotor.FindModule(typeof(Module_Mesh)) as Module_Mesh;
        //    if (meshMod != null) targetVector = meshMod.meshOrientation;
        //    else Debug.LogError("SetVector function is trying to reference a Mesh module. Add one to this motor, or change the chosen vector, to run function.");
        //}

        //if (toCamTargetOrientation)
        //{
        //    if (targetMotor.GetType() == typeof(Motor_Camera))
        //    {
        //        if ((targetMotor as Motor_Camera).cameraTarget != null) targetVector = (targetMotor as Motor_Camera).cameraTarget.parentMotor.orientationVector;
        //        else Debug.LogError("A Set Vector function is trying to reference a cameraTarget object, but a reference to one is not found on this camera motor. Add a cameraTarget if you want to reference its variables");
        //    }
        //}

        if (toCamTargetMoveDirection)
        {
            if (targetMotor.GetType() == typeof(Motor_Camera))
            {
                if ((targetMotor as Motor_Camera).cameraTarget != null) localTargetVector = (targetMotor as Motor_Camera).cameraTarget.parentMotor.variablesMod.FindVector("Move Direction").modVector;
                else Debug.LogError("A Set Vector function is trying to reference a cameraTarget object, but a reference to one is not found on this camera motor. Add a cameraTarget if you want to reference its variables");
            }
        }

        //if (toCamTargetMeshOrientation)
        //{
        //    if (targetMotor.GetType() == typeof(Motor_Camera))
        //    {
        //        if ((targetMotor as Motor_Camera).cameraTarget != null) targetVector = ((targetMotor as Motor_Camera).cameraTarget.parentMotor.FindModule(typeof(Module_Mesh)) as Module_Mesh).meshOrientation;
        //        else Debug.LogError("A Set Vector function is trying to reference a cameraTarget object, but a reference to one is not found on this camera motor. Add a cameraTarget if you want to reference its variables");
        //    }
        //}

        if (toCamTargetGravity)
        {
            if (targetMotor.GetType() == typeof(Motor_Camera))
            {
                if ((targetMotor as Motor_Camera).cameraTarget != null) localTargetVector = ((targetMotor as Motor_Camera).cameraTarget.parentMotor.FindModule(typeof(Module_Gravity)) as Module_Gravity).localGravityDirection;
                else Debug.LogError("A Set Vector function is trying to reference a cameraTarget object, but a reference to one is not found on this camera motor. Add a cameraTarget if you want to reference its variables");
            }
        }

        if (toManualVector)
        {
            localTargetVector = manualVector;
        }

        if (reverseVector) localTargetVector = -localTargetVector;
        if (normalizeVector) localTargetVector = localTargetVector.normalized;

        float localLerpSpeed = lerpSpeed;

        if (floatMultiplier != "")
        {
            localLerpSpeed *= motor.variablesMod.FindFloat(floatMultiplier).modFloat + offsetFloatAmount;
        }

        if (floatDivision != "")
        {
            localLerpSpeed /= motor.variablesMod.FindFloat(floatDivision).modFloat + offsetDivisionFloatAmount;
        }

        if (multiplyByCurrentSpeed)
        {
            localLerpSpeed *= motor.variablesMod.FindFloat(motor.currentSpeed).modFloat + offsetSpeedAmount;
        }

        if (divideByCurrentSpeed)
        {
            localLerpSpeed /= motor.variablesMod.FindFloat(motor.currentSpeed).modFloat + offsetSpeedAmount;
        }

        //if (multiplyByVelocityMagnitude)
        //{
        //    localLerpSpeed *= (motor as Motor_Physics).rigidbodyVelocity.magnitude + offsetSpeedAmount;
        //}
        //
        //if (divideByVelocityMagnitude)
        //{
        //    localLerpSpeed /= (motor as Motor_Physics).rigidbodyVelocity.magnitude + offsetSpeedAmount;
        //}

        if (multiplyByTargetCurrentSpeed)
        {
            localLerpSpeed *= motor.variablesMod.FindFloat("camTarget.Current Speed").modFloat + offsetSpeedAmount;
        }

        if (divideByTargetCurrentSpeed)
        {
            localLerpSpeed /= motor.variablesMod.FindFloat(motor.currentSpeed).modFloat + offsetSpeedAmount;
        }

        //if (multiplyByTargetVelocityMagnitude)
        //{
        //    localLerpSpeed *= ((motor as Motor_Camera).cameraTarget.parentMotor as Motor_Physics).rigidbodyVelocity.magnitude + offsetSpeedAmount;
        //}
        //
        //if (divideByTargetVelocityMagnitude)
        //{
        //    localLerpSpeed /= ((motor as Motor_Camera).cameraTarget.parentMotor as Motor_Physics).rigidbodyVelocity.magnitude + offsetSpeedAmount;
        //}

        //if (multiplyBySlopeMagnitude)
        //{
        //    localLerpSpeed *= (motor.FindModule(typeof(Module_Slopes)) as Module_Slopes).slopeVector.magnitude;
        //}

        if (localLerpSpeed >= maximumLerpSpeed && maximumLerpSpeed != 0) localLerpSpeed = maximumLerpSpeed;
        if (localLerpSpeed <= minimumLerpSpeed && minimumLerpSpeed != 0) localLerpSpeed = minimumLerpSpeed;

        Motor_Base setMotor = motor;
        if (useParentCamera) setMotor = (motor.FindModule(typeof(Module_CameraTarget)) as Module_CameraTarget).currentCamera;
        if (useCameraTarget) setMotor = (motor as Motor_Camera).cameraTarget.parentMotor;

        if (ignoreZeroResult && localTargetVector.sqrMagnitude == 0)
        {
            //Debug.Log(this + " set vector result is Vector3.zero and ignoreZeroResult is true. Set vector will not be changed.");
            return;
        }

        if (setVector != "")
        {
            float magnitude = 1;
            if (preserveSetVectorMagnitude) magnitude = motor.variablesMod.FindVector(setVector).modVector.magnitude;

            localTargetVector *= magnitude;
      
            if (lerpTowards) motor.variablesMod.FindVector(setVector).modVector = Vector3.Lerp(motor.variablesMod.FindVector(setVector).modVector, localTargetVector, localLerpSpeed * Time.deltaTime);
            else if (slerpTowards) motor.variablesMod.FindVector(setVector).modVector = Vector3.Slerp(motor.variablesMod.FindVector(setVector).modVector, localTargetVector, localLerpSpeed * Time.deltaTime);
            else motor.variablesMod.FindVector(setVector).modVector = localTargetVector;
        }

        //if (setDirectionVector)
        //{
        //    float magnitude = 1;
        //    if (preserveSetVectorMagnitude) magnitude = setMotor.directionVector.magnitude;
        //
        //    targetVector *= magnitude;
        //
        //    if (lerpTowards) setMotor.directionVector = Vector3.Lerp(setMotor.directionVector, targetVector, localLerpSpeed * Time.deltaTime);
        //    else if (slerpTowards) setMotor.directionVector = Vector3.Slerp(setMotor.directionVector, targetVector, localLerpSpeed * Time.deltaTime);
        //    else setMotor.directionVector = targetVector;
        //}
        
        //if (setOrientationVector)
        //{
        //    Vector3 smoothVector = Vector3.zero;
        //
        //    float magnitude = 1;
        //    if (preserveSetVectorMagnitude) magnitude = setMotor.orientationVector.magnitude;
        //
        //    targetVector *= magnitude;
        //
        //    if (lerpTowards) setMotor.orientationVector = Vector3.Lerp(setMotor.orientationVector, targetVector, localLerpSpeed * Time.deltaTime);
        //    else if(slerpTowards) setMotor.orientationVector = Vector3.Slerp(setMotor.orientationVector, targetVector, localLerpSpeed * Time.deltaTime);
        //
        //    else setMotor.orientationVector = targetVector;
        //}
        
        if (setMoveDirection)
        {
            float magnitude = 1;
            if (preserveSetVectorMagnitude) magnitude = setMotor.variablesMod.FindVector("Move Direction").modVector.magnitude;
        
            localTargetVector *= magnitude;
        
            if (lerpTowards) setMotor.variablesMod.FindVector("Move Direction").modVector = Vector3.Lerp(setMotor.variablesMod.FindVector("Move Direction").modVector, localTargetVector, localLerpSpeed * Time.deltaTime);
            else if(slerpTowards) setMotor.variablesMod.FindVector("Move Direction").modVector = Vector3.Slerp(setMotor.variablesMod.FindVector("Move Direction").modVector, localTargetVector, localLerpSpeed * Time.deltaTime);
            else setMotor.variablesMod.FindVector("Move Direction").modVector = localTargetVector;
        }
        
        if (setAdditionalMoveVector)
        {
            float magnitude = 1;
            if (preserveSetVectorMagnitude) magnitude = setMotor.variablesMod.FindVector(setMotor.additionalMoveVector).modVector.magnitude;
        
            localTargetVector *= magnitude;
        
            if (lerpTowards) setMotor.variablesMod.FindVector(setMotor.additionalMoveVector).modVector = Vector3.Lerp(setMotor.variablesMod.FindVector(setMotor.additionalMoveVector).modVector, localTargetVector, localLerpSpeed * Time.deltaTime);
            else if (slerpTowards) setMotor.variablesMod.FindVector(setMotor.additionalMoveVector).modVector = Vector3.Slerp(setMotor.variablesMod.FindVector(setMotor.additionalMoveVector).modVector, localTargetVector, localLerpSpeed * Time.deltaTime);
            else setMotor.variablesMod.FindVector(setMotor.additionalMoveVector).modVector = localTargetVector;
        }
        
        if (setPhysicsVector)
        {
            if (setMotor.GetType() != typeof(Motor_Physics))
            {
                Debug.LogError("SetVector function cannot use physicsVector as the motor it is attached to is not a Physics Motor. Attach a Motor_Physics component to this object to use physicsVector in any functions.");
                return;
            }
        
            float magnitude = 1;
            if (preserveSetVectorMagnitude) magnitude = (setMotor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector.magnitude;
        
            localTargetVector *= magnitude;
        
            if (lerpTowards) (setMotor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector = Vector3.Lerp((setMotor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector, localTargetVector, localLerpSpeed * Time.deltaTime);
            else if(slerpTowards) (setMotor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector = Vector3.Slerp((setMotor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector, localTargetVector, localLerpSpeed * Time.deltaTime);
            else (setMotor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector = localTargetVector;
        }
        
        if (setInputVector)
        {
            Module_Input inputMod = setMotor.FindModule(typeof(Module_Input)) as Module_Input;
            if (inputMod != null)
            {
                float magnitude = 1;
                if (preserveSetVectorMagnitude) magnitude = motor.variablesMod.FindVector(inputMod.inputVector).modVector.magnitude;
        
                localTargetVector *= magnitude;
        
                if (lerpTowards) motor.variablesMod.FindVector(inputMod.inputVector).modVector = Vector3.Lerp(motor.variablesMod.FindVector(inputMod.inputVector).modVector, localTargetVector, localLerpSpeed * Time.deltaTime);
                else if (slerpTowards) motor.variablesMod.FindVector(inputMod.inputVector).modVector = Vector3.Slerp(motor.variablesMod.FindVector(inputMod.inputVector).modVector, localTargetVector, localLerpSpeed * Time.deltaTime);
                else motor.variablesMod.FindVector(inputMod.inputVector).modVector = localTargetVector;
            }
            else Debug.LogError("SetVector function is trying to reference a Input module. Add one to this motor, or change the chosen vector, to run function.");
        }
        
       //if (setMeshDirection)
       //{
       //    Module_Mesh meshMod = setMotor.FindModule(typeof(Module_Mesh)) as Module_Mesh;
       //    if (meshMod != null)
       //    {
       //        float magnitude = 1;
       //        if (preserveSetVectorMagnitude) magnitude = meshMod.meshDirection.magnitude;
       //
       //        targetVector *= magnitude;
       //
       //        if (lerpTowards) meshMod.meshDirection = Vector3.Lerp(meshMod.meshDirection, targetVector, localLerpSpeed * Time.deltaTime);
       //        else if (slerpTowards) meshMod.meshDirection = Vector3.Slerp(meshMod.meshDirection, targetVector, localLerpSpeed * Time.deltaTime);
       //        else meshMod.meshDirection = targetVector;
       //    }
       //    else Debug.LogError("SetVector function is trying to reference a Mesh module. Add one to this motor, or change the chosen vector, to run function.");
       //}
        
        //if (setMeshOrientation)
        //{
        //    Module_Mesh meshMod = setMotor.FindModule(typeof(Module_Mesh)) as Module_Mesh;
        //    if (meshMod != null)
        //    {
        //        float magnitude = 1;
        //        if (preserveSetVectorMagnitude) magnitude = meshMod.meshOrientation.magnitude;
        //
        //        targetVector *= magnitude;
        //
        //        if (lerpTowards) meshMod.meshOrientation = Vector3.Lerp(meshMod.meshOrientation, targetVector, localLerpSpeed * Time.deltaTime);
        //        else if (slerpTowards) meshMod.meshOrientation = Vector3.Slerp(meshMod.meshOrientation, targetVector, localLerpSpeed * Time.deltaTime);
        //        else meshMod.meshOrientation = targetVector;
        //    }
        //    else Debug.LogError("SetVector function is trying to reference a Mesh module. Add one to this motor, or change the chosen vector, to run function.");
        //}
        
        //if (setOrbitVector)
        //{
        //    if (setMotor.GetType() != typeof(Motor_Camera))
        //    {
        //        Debug.LogError("SetVector function cannot use orbitVector as the motor it is attached to is not a Camera Motor. Attach a Motor_Camera component to this object to use orbitVector in any functions.");
        //        return;
        //    }
        //
        //    float magnitude = 1;
        //    if (preserveSetVectorMagnitude) magnitude = (setMotor as Motor_Camera).orbitVector.magnitude;
        //
        //    targetVector *= magnitude;
        //
        //    if (lerpTowards) (setMotor as Motor_Camera).orbitVector = Vector3.Lerp((setMotor as Motor_Camera).orbitVector, targetVector, localLerpSpeed * Time.deltaTime);
        //    else if (slerpTowards) (setMotor as Motor_Camera).orbitVector = Vector3.Slerp((setMotor as Motor_Camera).orbitVector, targetVector, localLerpSpeed * Time.deltaTime);
        //    else (setMotor as Motor_Camera).orbitVector = targetVector;
        //}
    }
}
