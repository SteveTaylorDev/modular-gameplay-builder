using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DotProduct", menuName = "Base/Functions/Dot Product")]
public class Function_DotProduct : Function_Base
{
    [Header("Left Hand Vector")]
    public bool reverseLhs;
    public string lhsVector;
    public bool lhsTransformRight;
    public bool lhsMoveDirection;
    public bool physicsVector;
    public bool lhsRigidbodyVelocity;

    [Header("Right Hand Vector")]
    public bool reverseRhs;
    public string rhsVector;
    public bool rhsMoveDirection;
    public bool rhsLocalGravity;
    public bool rhsGlobalGravity;
    public bool rhsGroundAverage;

    [Header("Apply Dot to Source")]
    public string sourceFloat;
    public string sourceVector;
    public bool applyToCurrentSpeed;
    public bool applyToMoveDirection;
    public bool applyToDirectionVector;
    public bool applyToPhysicsVector;
    public bool applyToRigidbodyVelocity;

    [Header("Application Method")]
    public bool ignoreTime;
    public bool multiplyBySource;
    public bool setSourceMagnitude;
    public bool addLeftHandToSource;
    public bool addRightHandToSource;
    public bool subtractLeftHandFromSource;
    public bool subtractRightHandFromSource;

    [Header("Options")]
    public bool normalizeVectors;
    public float dotMultiplier;
    public bool useAbsoluteProduct;
    public bool debugDotProduct;


    public override void RunFunction(Motor_Base motor)
    {
        Vector3 localLhsVector = Vector3.zero;
        Vector3 localRhsVector = Vector3.zero;

        // Left Hand Vector
        if (lhsVector != "") localLhsVector = motor.variablesMod.FindVector(lhsVector).modVector;

        if (lhsTransformRight)
        {
            localLhsVector = motor.transform.right;
        }

        if (lhsMoveDirection)
        {
            localLhsVector = motor.variablesMod.FindVector("Move Direction").modVector;
        }

        if (physicsVector)
        {
            localLhsVector = (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector;
        }

        //if (lhsRigidbodyVelocity)
        //{
        //    lhsVector = (motor as Motor_Physics).rigidbodyVelocity;
        //}

        if (reverseLhs) localLhsVector = -localLhsVector;

        // Right Hand Vector
        if (rhsVector != "") localRhsVector = motor.variablesMod.FindVector(rhsVector).modVector;

        if (rhsMoveDirection)
        {
            localRhsVector = motor.variablesMod.FindVector("Move Direction").modVector;
        }

        if (rhsGroundAverage)
        {
            Module_GroundDetection groundMod = motor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;
            if (groundMod != null) localRhsVector = groundMod.averageGroundNormal;
        }

        if (rhsLocalGravity)
        {
            Module_Gravity gravMod = motor.FindModule(typeof(Module_Gravity)) as Module_Gravity;
            if (gravMod != null) localRhsVector = gravMod.localGravityDirection;
        }

        if (rhsGlobalGravity)
        {
            localRhsVector = GameController.Instance.globalGravityDirection;
        }

        if (reverseRhs) localRhsVector = -localRhsVector;

        if (normalizeVectors)
        {
            localLhsVector = localLhsVector.normalized;
            localRhsVector = localRhsVector.normalized;
        }

        // Source Vector
        float dotProduct = Vector3.Dot(localLhsVector, localRhsVector);
        if (useAbsoluteProduct) dotProduct = Mathf.Abs(dotProduct);
        if (debugDotProduct) Debug.Log(dotProduct);

        if (dotMultiplier != 0)
        {
            if (useAbsoluteProduct) dotProduct *= Mathf.Abs(dotMultiplier);
            else dotProduct *= dotMultiplier;
        }
        if (!ignoreTime) dotProduct *= Time.deltaTime;

        if(sourceFloat != "")
        {
            if (multiplyBySource) motor.variablesMod.FindFloat(sourceFloat).modFloat *= dotProduct;
            if (addLeftHandToSource || addRightHandToSource) motor.variablesMod.FindFloat(sourceFloat).modFloat += dotProduct;
            if (subtractLeftHandFromSource || subtractRightHandFromSource) motor.variablesMod.FindFloat(sourceFloat).modFloat -= dotProduct;
            if (setSourceMagnitude) motor.variablesMod.FindFloat(sourceFloat).modFloat = dotProduct;
        }

        if (sourceVector != "")
        {
            if (multiplyBySource) motor.variablesMod.FindVector(sourceVector).modVector *= dotProduct;
            if (addLeftHandToSource) motor.variablesMod.FindVector(sourceVector).modVector += localLhsVector.normalized * dotProduct;
            if (addRightHandToSource) motor.variablesMod.FindVector(sourceVector).modVector += localRhsVector.normalized * dotProduct;
            if (subtractLeftHandFromSource) motor.variablesMod.FindVector(sourceVector).modVector -= localLhsVector.normalized * dotProduct;
            if (subtractRightHandFromSource) motor.variablesMod.FindVector(sourceVector).modVector -= localRhsVector.normalized * dotProduct;
            if (setSourceMagnitude) motor.variablesMod.FindVector(sourceVector).modVector = motor.variablesMod.FindVector(sourceVector).modVector.normalized * dotProduct;
        }

        if (applyToCurrentSpeed)
        {
            if (multiplyBySource) motor.variablesMod.FindFloat(motor.currentSpeed).modFloat *= dotProduct;
            if (addLeftHandToSource || addRightHandToSource) motor.variablesMod.FindFloat(motor.currentSpeed).modFloat += dotProduct;
            if (subtractLeftHandFromSource || subtractRightHandFromSource) motor.variablesMod.FindFloat(motor.currentSpeed).modFloat -= dotProduct;
            if (setSourceMagnitude) motor.variablesMod.FindFloat(motor.currentSpeed).modFloat = dotProduct;
        }

        if (applyToMoveDirection)
        {
            if (multiplyBySource) motor.variablesMod.FindVector("Move Direction").modVector *= dotProduct;
            if (addLeftHandToSource) motor.moveDirection += localLhsVector.normalized * dotProduct;
            if (addRightHandToSource) motor.moveDirection += localRhsVector.normalized * dotProduct;
            if (subtractLeftHandFromSource) motor.variablesMod.FindVector("Move Direction").modVector -= localLhsVector.normalized * dotProduct;
            if (subtractRightHandFromSource) motor.variablesMod.FindVector("Move Direction").modVector -= localRhsVector.normalized * dotProduct;
            if (setSourceMagnitude) motor.variablesMod.FindVector("Move Direction").modVector = motor.variablesMod.FindVector("Move Direction").modVector.normalized * dotProduct;
        }

        //if (applyToDirectionVector)
        //{
        //    if (multiplyBySource) motor.directionVector *= dotProduct;
        //    if (addLeftHandToSource) motor.directionVector += lhsVector.normalized * dotProduct;
        //    if (addRightHandToSource) motor.directionVector += rhsVector.normalized * dotProduct;
        //    if (subtractLeftHandFromSource) motor.directionVector -= lhsVector.normalized * dotProduct;
        //    if (subtractRightHandFromSource) motor.directionVector -= rhsVector.normalized * dotProduct;
        //    if (setSourceMagnitude) motor.directionVector = motor.directionVector.normalized * dotProduct;
        //}

        if (applyToPhysicsVector)
        {
            if (multiplyBySource) (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector *= dotProduct;
            if (addLeftHandToSource) (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector += localLhsVector.normalized * dotProduct;
            if (addRightHandToSource) (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector += localRhsVector.normalized * dotProduct;
            if (subtractLeftHandFromSource) (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector -= localLhsVector.normalized * dotProduct;
            if (subtractRightHandFromSource) (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector -= localRhsVector.normalized * dotProduct;
            if (setSourceMagnitude) (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector = (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector.normalized * dotProduct;
        }

        //if (applyToRigidbodyVelocity)
        //{
        //    if (multiplyBySource) (motor as Motor_Physics).rigidbodyVelocity *= dotProduct;
        //    if (addLeftHandToSource) (motor as Motor_Physics).rigidbodyVelocity += lhsVector.normalized * dotProduct;
        //    if (addRightHandToSource) (motor as Motor_Physics).rigidbodyVelocity += rhsVector.normalized * dotProduct;
        //    if (subtractLeftHandFromSource) (motor as Motor_Physics).rigidbodyVelocity -= lhsVector.normalized * dotProduct;
        //    if (subtractRightHandFromSource) (motor as Motor_Physics).rigidbodyVelocity -= rhsVector.normalized * dotProduct;
        //    if (setSourceMagnitude) (motor as Motor_Physics).rigidbodyVelocity = (motor as Motor_Physics).rigidbodyVelocity.normalized * dotProduct;
        //}
    }
}
