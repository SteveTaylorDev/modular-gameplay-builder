using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ApplySpeed", menuName = "Base/Functions/Apply Speed")]
public class Function_ApplySpeed : Function_Base
{
    [Header("Speed Source")]
    public string sourceFloat;
    public bool applyToSpeed;
    public bool applyToPhysicsVector;

    [Header("Speed Amount")]
    public bool subtractSpeed;
    public string speedFloat;
    public bool useMoveVectorMagnitude;
    public bool useVelocityMagnitude;
    public bool usePhysicsVectorMagnitude;
    public bool usePhysicFriction;
    public bool useSlopeMagnitude;
    public bool useInputMoveDifference;
    public bool useManualSpeed;
    public float manualSpeedStrength;

    [Header("Apply Options")]
    public bool setSpeedDirectly;
    public bool ignoreTime;
    public float speedMultiplier;
    public bool multiplyByInputStrength;
    public bool multiplyByCurrentSpeed;
    public bool divideByCurrentSpeed;
    public float minSpeed;
    public float maxSpeed;
    public float speedOffset;


    public override void RunFunction(Motor_Base motor)
    {
        float deltaFloat = 1;
        if (!ignoreTime) deltaFloat = Time.deltaTime;

        float localSpeed = 0;
        if (useManualSpeed) localSpeed = manualSpeedStrength;

        if (useMoveVectorMagnitude) localSpeed = motor.moveVector.magnitude;

        //if (useVelocityMagnitude)
        //{
        //    if (motor.GetType() == typeof(Motor_Physics))
        //    {
        //        localSpeed = (motor as Motor_Physics).rigidbodyVelocity.magnitude;
        //    }
        //}

        /*if (usePhysicFriction)
        {
            Module_GroundDetection groundMod = motor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;
            Module_Collision collisionMod = motor.FindModule(typeof(Module_Collision)) as Module_Collision;

            if (groundMod != null)
            {
                if (groundMod.groundPhysicMaterial != null) localSpeed = groundMod.groundPhysicMaterial.dynamicFriction;
            }

            else if (collisionMod != null)
            {
                if (collisionMod.collisionPhysicMaterial != null) localSpeed = collisionMod.collisionPhysicMaterial.dynamicFriction;
            }
            else Debug.LogError("An ApplySpeed function is trying to reference a collision or ground Physic Material, but neither a Collision Module or Ground Detection Module are found on this motor. Add one to reference collision or ground Physic Materials.");
        }*/

        if (speedFloat != "")
        {
            localSpeed = motor.variablesMod.FindFloat(speedFloat).modFloat;
        }

        if (usePhysicsVectorMagnitude)
        {
            if(motor.GetType() == typeof(Motor_Physics))
            {
                localSpeed = (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector.magnitude;
            }
        }

        //if (useSlopeMagnitude)
        //{
        //    localSpeed = (motor.FindModule(typeof(Module_Slopes)) as Module_Slopes).slopeVector.magnitude;
        //}

        if (useInputMoveDifference)
        {
            Module_Input inputMod = motor.FindModule(typeof(Module_Input)) as Module_Input;

            localSpeed = Vector3.Angle(motor.variablesMod.FindVector(inputMod.inputVector).modVector, motor.variablesMod.FindVector("Move Direction").modVector);
        }

        if (multiplyByInputStrength)
        {
            Module_Input inputMod = motor.FindModule(typeof(Module_Input)) as Module_Input;
            float localInputStrength = 0;

            if (inputMod != null)
            {
                localInputStrength = inputMod.rawInputVector.magnitude;
                if (localInputStrength > 1) localInputStrength = 1;
                localSpeed *= localInputStrength;
            }
            else Debug.LogError("An ApplySpeed function is trying to reference player input, but an Input Module is not found on this motor. Add an Input Module to use player input as strength multiplier.");
        }

        float localSpeedApply = motor.variablesMod.FindFloat(motor.currentSpeed).modFloat + speedOffset;

        if(minSpeed != 0 && maxSpeed != 0) localSpeedApply = Mathf.Clamp(localSpeedApply, minSpeed, maxSpeed);

        if (multiplyByCurrentSpeed) localSpeed *= localSpeedApply;
        if (divideByCurrentSpeed) localSpeed /= localSpeedApply;

        float localSpeedMultiplier = 1;
        if (speedMultiplier != 0) localSpeedMultiplier = speedMultiplier;

        if (subtractSpeed) localSpeed = -localSpeed;

        if(sourceFloat != "")
        {
            if (!setSpeedDirectly) motor.variablesMod.FindFloat(sourceFloat).modFloat += (localSpeed * localSpeedMultiplier) * deltaFloat;
            else if (localSpeed != 0 || useManualSpeed) motor.variablesMod.FindFloat(sourceFloat).modFloat = localSpeed * localSpeedMultiplier;
        }

        //if (applyToSpeed)
        //{
        //    if (!setSpeedDirectly) motor.variablesMod.FindFloat(motor.currentSpeed).modFloat += (localSpeed * localSpeedMultiplier) * deltaFloat;
        //    else if (localSpeed != 0 || useManualSpeed) motor.variablesMod.FindFloat(motor.currentSpeed).modFloat = localSpeed * localSpeedMultiplier;
        //}

        if (applyToPhysicsVector)
        {
            if (!setSpeedDirectly) (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector += (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector.normalized * ((localSpeed * localSpeedMultiplier) * deltaFloat);
            else if (localSpeed != 0 || useManualSpeed) (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector = (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector.normalized * (localSpeed * localSpeedMultiplier);
        }
    }
}
