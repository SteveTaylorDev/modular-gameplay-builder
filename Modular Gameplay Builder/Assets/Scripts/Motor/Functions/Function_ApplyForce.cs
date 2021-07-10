using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ApplyForce", menuName = "Base/Functions/(Physics) Apply Force")]
public class Function_ApplyForce : Function_Base
{
    [Header("Force Direction")]
    public bool reverseDirection;
    public string forceDirection;
    public bool useInputDirection;
    public bool useOrientationVector;
    public bool useDirectionVector;
    public bool useMoveDirection;
    public bool usePhysicsVector;
    public bool useLocalGravity;
    public bool useGlobalGravity;
    public bool useGroundAverage;
    public bool useSlopeDirection;
    public bool useVelocityDirection;
    public bool useManualDirection;
    public Vector3 manualDirection;

    [Header("Force Strength")]
    public bool inverseStrength;
    public string forceStrength;
    public bool useInputMagnitude;
    public bool useCurrentSpeed;
    public bool usePhysicsMagnitude;
    public bool useVelocityMagnitude;
    public bool usePhysicFriction;
    public bool useLocalGravStrength;
    public bool useGlobalGravStrength;
    public bool useGroundDistance;
    public bool useSlopeSteepness;
    public float groundDistanceOffset;
    public bool useManualStrength;
    public float manualStrength;

    [Header("Strength Options")]
    public float strengthMultiplier;
    public bool multiplyBySpeed;
    public bool divideBySpeed;
    public float maxSpeedAmount;
    public float minSpeedAmount;
    public float offsetSpeedAmount;
    public bool multiplyBySize;
    public bool multiplyByTime;
    public bool divideByTime;
    public bool ignoreMass;

    [Header("Force Mode")]
    public bool useAdditionalMoveVector;
    public bool magnitudeOnly;


    private void DisableDirectionBools()
    {
        useInputDirection = false;
        useOrientationVector = false;
        useDirectionVector = false;
        useMoveDirection = false;
        usePhysicsVector = false;
        useLocalGravity = false;
        useGlobalGravity = false;
        useGroundAverage = false;
        useSlopeDirection = false;
        useVelocityDirection = false;
        useManualDirection = false;
    }

    public void DisableStrengthBools()
    {
        useInputMagnitude = false;
        useCurrentSpeed = false;
        usePhysicsMagnitude = false;
        useVelocityMagnitude = false;
        usePhysicFriction = false;
        useLocalGravStrength = false;
        useGlobalGravStrength = false;
        useGroundDistance = false;
        useSlopeSteepness = false;
        useManualStrength = false;
    }

    public override void RunFunction(Motor_Base motor)
    {
        Vector3 forceVector = Vector3.zero;
        float localForceStrength = 0;

        if (motor.GetType() != typeof(Motor_Physics) && !useAdditionalMoveVector)
        {
            Debug.LogError("ApplyForce cannot be applied to this motor. Replace this motor with a Motor_Physics component (or use the AdditionalMoveVector option) to use the ApplyForce function.");
            return;
        }

        // Direction

        if (forceDirection != "") forceVector = motor.variablesMod.FindVector(forceDirection).modVector;

        if (useInputDirection)
        {
            Module_Input inputMod = motor.FindModule(typeof(Module_Input)) as Module_Input;
            if (inputMod == null)
            {
                Debug.LogError("An ApplyForce function is trying to reference player input, but an Input Module is not found on this motor. Cannot apply force. Add an Input Module to use player input as force direction or strength.");
                return;
            }

            DisableDirectionBools();
            useInputDirection = true;

            forceVector = motor.variablesMod.FindVector(inputMod.inputVector).modVector.normalized;
        }

        //if (useOrientationVector)
        //{
        //    DisableDirectionBools();
        //    useOrientationVector = true;
        //
        //    forceVector = motor.orientationVector;
        //}

        //if (useDirectionVector)
        //{
        //    DisableDirectionBools();
        //    useDirectionVector = true;
        //
        //    forceVector = motor.directionVector;
        //}

        if (useMoveDirection)
        {
            DisableDirectionBools();
            useMoveDirection = true;

            forceVector = motor.variablesMod.FindVector("Move Direction").modVector;
        }

        if (usePhysicsVector)
        {
            if(motor.GetType() != typeof(Motor_Physics))
            {
                Debug.LogError("An ApplyForce function is trying to reference a physics vector, but the current motor is not a Physics Motor. Cannot apply force. Change this motor to a Physics Motor to reference its Physics Vector (velocity).");
                return;
            }

            DisableDirectionBools();
            usePhysicsVector = true;

            forceVector = (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector;
        }

        if (useLocalGravity)
        {
            Module_Gravity gravMod = motor.FindModule(typeof(Module_Gravity)) as Module_Gravity;
            if (gravMod == null) Debug.LogError("An ApplyForce function is trying to reference local gravity variables, but a Gravity Module is not found. Cannot apply force. Add a Gravity Module to use local gravity as the force strength or direction.");

            DisableDirectionBools();
            useLocalGravity = true;

            forceVector = gravMod.localGravityDirection;
        }

        if (useGlobalGravity)
        {
            DisableDirectionBools();
            useGlobalGravity = true;

            forceVector = GameController.Instance.globalGravityDirection;
        }

        if (useGroundAverage)
        {
            DisableDirectionBools();
            useGroundAverage = true;

            Module_GroundDetection groundMod = motor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;
            if (groundMod == null) Debug.LogError("An ApplyForce function is trying to reference local Ground Detection variables, but a Ground Detection Module is not found. Cannot apply force. Add a Ground Detection Module to use ground variables as the force strength or direction.");

            forceVector = groundMod.averageGroundNormal;
        }

        //if (useSlopeDirection)
        //{
        //    DisableDirectionBools();
        //    useSlopeDirection = true;
        //
        //    Module_Slopes slopesMod = motor.FindModule(typeof(Module_Slopes)) as Module_Slopes;
        //    if (slopesMod == null) Debug.LogError("An ApplyForce function is trying to reference local slope variables, but a Slopes Module is not found. Cannot apply force. Add a Slopes Module to use its variables as the force strength or direction.");
        //
        //    forceVector = slopesMod.slopeVector.normalized;
        //}

        //if (useVelocityDirection)
        //{
        //    DisableDirectionBools();
        //    useVelocityDirection = true;
        //
        //    forceVector = (motor as Motor_Physics).rigidbodyVelocity;
        //}

        if (useManualDirection)
        {
            DisableDirectionBools();
            useManualDirection = true;

            if (manualDirection != Vector3.zero) forceVector = manualDirection.normalized;
            else Debug.LogWarning("useManualDirection is set as the ApplyForce direction option, yet the manualDirection vector has not been set. Set the manualDirection vector to apply force in this direction.");
        }

        forceVector = forceVector.normalized;
        if (reverseDirection) forceVector = -forceVector;


     // Strength

        Vector3 angleTarget = Vector3.zero;

        if (forceStrength != "") localForceStrength = motor.variablesMod.FindFloat(forceStrength).modFloat;

        if (useInputMagnitude)
        {
            Module_Input inputMod = motor.FindModule(typeof(Module_Input)) as Module_Input;
            if (inputMod == null)
            {
                Debug.LogError("An ApplyForce function is trying to reference player input, but an Input Module is not found on this motor. Cannot apply force. Add an Input Module to use player input as force direction or strength.");
                return;
            }

            DisableStrengthBools();
            useInputMagnitude = true;

            localForceStrength = motor.variablesMod.FindVector(inputMod.inputVector).modVector.magnitude;
        }

        if (useCurrentSpeed)
        {
            DisableStrengthBools();
            useCurrentSpeed = true;

            if (motor.variablesMod.FindFloat(motor.currentSpeed).modFloat == 0) localForceStrength = 1;
            else localForceStrength = motor.variablesMod.FindFloat(motor.currentSpeed).modFloat;
        }

        if (usePhysicsMagnitude)
        {
            if (motor.GetType() != typeof(Motor_Physics))
            {
                Debug.LogError("An ApplyForce function is trying to reference a physics vector, but the current motor is not a Physics Motor. Cannot apply force. Change this motor to a Physics Motor to reference its Physics Vector (velocity).");
                return;
            }

            localForceStrength = (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector.magnitude;

            DisableStrengthBools();
            usePhysicsMagnitude = true;
        }

        //if (useVelocityMagnitude)
        //{
        //    if (motor.GetType() != typeof(Motor_Physics))
        //    {
        //        Debug.LogError("An ApplyForce function is trying to reference a velocity vector, but the current motor is not a Physics Motor. Cannot apply force. Change this motor to a Physics Motor to reference its velocity.");
        //        return;
        //    }
        //
        //    localForceStrength = (motor as Motor_Physics).rigidbodyVelocity.magnitude;
        //
        //    DisableStrengthBools();
        //    useVelocityMagnitude = true;
        //}

        /*if (usePhysicFriction)
        {
            DisableStrengthBools();
            usePhysicFriction = true;

            Module_GroundDetection groundMod = motor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;
            Module_Collision collisionMod = motor.FindModule(typeof(Module_Collision)) as Module_Collision;

            if (groundMod != null)
            {
                if (groundMod.groundPhysicMaterial != null) localForceStrength = groundMod.groundPhysicMaterial.dynamicFriction;
            }

            else if (collisionMod != null)
            {
                if (collisionMod.collisionPhysicMaterial != null) localForceStrength = collisionMod.collisionPhysicMaterial.dynamicFriction;
            }
            else Debug.LogError("An ApplyForce function is trying to reference a collision or ground Physic Material, but neither a Collision Module or Ground Detection Module are found on this motor. Add one to reference collision or ground Physic Materials.");
        }*/

        if (useLocalGravStrength)
        {
            DisableStrengthBools();
            useLocalGravStrength = true;

            Module_Gravity gravMod = motor.FindModule(typeof(Module_Gravity)) as Module_Gravity;
            if (gravMod == null)
            {
                Debug.LogError("An ApplyForce function is trying to reference local gravity variables, but a Gravity Module is not found. Cannot apply force. Add a Gravity Module to use local gravity as the force strength or direction.");
                return;
            }

            localForceStrength = gravMod.localGravityStrength;
        }

        if (useGlobalGravStrength)
        {
            DisableStrengthBools();
            useGlobalGravStrength = true;

            localForceStrength = GameController.Instance.globalGravityStrength;
        }

        if (useGroundDistance)
        {
            Module_GroundDetection groundMod = motor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;
            if (groundMod == null)
            {
                Debug.LogError("An ApplyForce function is trying to reference local Ground Detection variables, but a Ground Detection Module is not found. Cannot apply force. Add a Ground Detection Module to use ground variables as the force strength or direction.");
                return;
            }

            DisableStrengthBools();
            useGroundDistance = true;

            localForceStrength = groundMod.closestGroundDistance + groundDistanceOffset;
        }

       //if (useSlopeSteepness)
       //{
       //    Module_Slopes slopesMod = motor.FindModule(typeof(Module_Slopes)) as Module_Slopes;
       //    if (slopesMod == null)
       //    {
       //        Debug.LogError("An ApplyForce function is trying to reference local slope variables, but a Slopes Module is not found. Cannot apply force. Add a Slopes Module to use its variables as the force strength or direction.");
       //        return;
       //    }
       //
       //    DisableStrengthBools();
       //    useSlopeSteepness = true;
       //
       //    localForceStrength = slopesMod.slopeVector.magnitude;
       //}

        if (useManualStrength)
        {
            DisableStrengthBools();
            useManualStrength = true;

            localForceStrength = manualStrength;
        }

        if (inverseStrength) localForceStrength = -localForceStrength;
        

     // Build Force Vector

        if (multiplyBySize)
        {
            localForceStrength *= motor.transform.localScale.magnitude;
        }

        float localSpeed = motor.variablesMod.FindFloat(motor.currentSpeed).modFloat;

        localSpeed = Mathf.Clamp(localSpeed, minSpeedAmount, maxSpeedAmount);

        if (multiplyBySpeed) localForceStrength *= localSpeed + offsetSpeedAmount;
        if (divideBySpeed) localForceStrength /= localSpeed + offsetSpeedAmount;

        if (divideByTime) localForceStrength /= Time.deltaTime;

        if (strengthMultiplier == 0) strengthMultiplier = 1;
        forceVector *= (localForceStrength * strengthMultiplier);

        if (!useAdditionalMoveVector)
        {
            if (!magnitudeOnly) ApplyForce(motor as Motor_Physics, forceVector, ignoreMass, multiplyByTime);

            else
            {
                float timeFactor = 1;
                if (multiplyByTime) timeFactor = Time.deltaTime;

                float physicsMagnitude = (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector.magnitude;
                float forceMagnitude = forceVector.magnitude * Mathf.Sign(localForceStrength);
                if (Mathf.Sign(physicsMagnitude + forceMagnitude * timeFactor) != Mathf.Sign(physicsMagnitude))
                {
                    physicsMagnitude = 0;
                }
                else
                {
                    physicsMagnitude += forceMagnitude * timeFactor;
                }

                (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector = (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector.normalized * physicsMagnitude;
            }
        }
        else
        {
            float timeFactor = 1;
            if (multiplyByTime) timeFactor = Time.deltaTime;

            if (!magnitudeOnly) motor.variablesMod.FindVector(motor.additionalMoveVector).modVector += forceVector * timeFactor;
            else
            {
                float additionalVectorMagnitude = motor.variablesMod.FindVector(motor.additionalMoveVector).modVector.magnitude;
                float forceMagnitude = forceVector.magnitude * Mathf.Sign(localForceStrength);
                if (Mathf.Sign(additionalVectorMagnitude + forceMagnitude * timeFactor) != Mathf.Sign(additionalVectorMagnitude))
                {
                    additionalVectorMagnitude = 0;
                }
                else
                {
                    additionalVectorMagnitude += forceMagnitude * timeFactor;
                }

                motor.variablesMod.FindVector(motor.additionalMoveVector).modVector = motor.variablesMod.FindVector(motor.additionalMoveVector).modVector.normalized * additionalVectorMagnitude;
            }
        }
    }

    public static void ApplyForce(Motor_Physics motor, Vector3 forceVector, bool ignoreMass = default(bool), bool multiplyByTime = default(bool))
    {
        float deltaFloat = 1;
        if (multiplyByTime) deltaFloat = Time.deltaTime;

        if (ignoreMass) forceVector *= motor.mass;

        if (!motor.ignoreMoveVector) motor.variablesMod.FindVector(motor.physicsVector).modVector += (forceVector) * deltaFloat;
        else if (motor.motorRigidbody != null) motor.motorRigidbody.velocity += (forceVector / motor.mass) * deltaFloat;
        else Debug.LogError("The motor's motorRigidbody reference has not been assigned. Cannot apply force to this motor.");
    }
}
