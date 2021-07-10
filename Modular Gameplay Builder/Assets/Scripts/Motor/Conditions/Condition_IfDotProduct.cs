using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IfDotProduct", menuName = "Base/Conditions/If Dot Product")]
public class Condition_IfDotProduct : Condition_Base
{
    [Header("Left Hand Vector")]
    public string lhsVector;
    public bool reverseLhs;
    public bool lhsTransformRight;
    public bool lhsDirectionVector;
    public bool lhsMoveDirection;
    public bool lhsRigidbodyVelocity;
    public bool lhsInputVector;
    public bool lhsOrbitVector;
    public bool useCameraTargetLhs;

    [Header("Right Hand Vector")]
    public string rhsVector;
    public bool reverseRhs;
    public bool rhsTransformRight;
    public bool rhsOrientationVector;
    public bool rhsMoveDirection;
    public bool rhsLocalGravity;
    public bool rhsGlobalGravity;
    public bool rhsGroundAverage;
    public bool rhsSlopeVector;
    public bool rhsOrbitVector;
    public bool useCameraTargetRhs;

    [Header("Vector Options")]
    public bool normalizeVectors;
    public bool useAbsoluteProduct;
    public bool debugDotProduct;

    [Header("Condition Options")]
    public bool moreThan;
    public bool lessThan;
    public float compareAmount;


    public override bool IsCondition(Motor_Base motor)
    {
        bool isCondition = default(bool);

        Motor_Base lhsMotor = motor;
        if (useCameraTargetLhs) lhsMotor = (motor as Motor_Camera).cameraTarget.parentMotor;

        Motor_Base rhsMotor = motor;
        if (useCameraTargetRhs) rhsMotor = (motor as Motor_Camera).cameraTarget.parentMotor;

        Vector3 localLhsVector = Vector3.zero;
        Vector3 localRhsVector = Vector3.zero;

        // Left Hand Vector

        if(lhsVector != "")
        {
            localLhsVector = motor.variablesMod.FindVector(lhsVector).modVector;
        }

        if (lhsTransformRight)
        {
            localLhsVector = lhsMotor.transform.right;
        }

        //if(lhsDirectionVector)
        //{
        //    lhsVector = lhsMotor.directionVector;
        //}

        //if (lhsMoveDirection)
        //{
        //    localLhsVector = lhsMotor.moveDirection;
        //}

        //if (lhsRigidbodyVelocity)
        //{
        //    localLhsVector = (lhsMotor as Motor_Physics).rigidbodyVelocity;
        //}

        if (lhsInputVector)
        {
            localLhsVector = motor.variablesMod.FindVector((lhsMotor.FindModule(typeof(Module_Input)) as Module_Input).inputVector).modVector;
        }

        //if (lhsOrbitVector)
        //{
        //    lhsVector = (lhsMotor as Motor_Camera).orbitVector;
        //}

        if (reverseLhs) localLhsVector = -localLhsVector;

        // Right Hand Vector
        if(rhsVector != "")
        {
            localRhsVector = motor.variablesMod.FindVector(rhsVector).modVector;
        }

        if (rhsTransformRight)
        {
            localRhsVector = rhsMotor.transform.right;
        }

        //if(rhsOrientationVector)
        //{
        //    rhsVector = rhsMotor.orientationVector;
        //}

        //if (rhsMoveDirection)
        //{
        //    localRhsVector = rhsMotor.moveDirection;
        //}

        if (rhsGroundAverage)
        {
            Module_GroundDetection groundMod = rhsMotor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;
            if (groundMod != null) localRhsVector = groundMod.averageGroundNormal;
        }

        //if (rhsSlopeVector)
        //{
        //    if (rhsMotor == null) return false;
        //    Module_Slopes slopesMod = rhsMotor.FindModule(typeof(Module_Slopes)) as Module_Slopes;
        //    if (slopesMod != null) localRhsVector = slopesMod.slopeVector;
        //}

        if (rhsLocalGravity)
        {
            Module_Gravity gravMod = rhsMotor.FindModule(typeof(Module_Gravity)) as Module_Gravity;
            if (gravMod != null) localRhsVector = gravMod.localGravityDirection;
        }

        if (rhsGlobalGravity)
        {
            localRhsVector = GameController.Instance.globalGravityDirection;
        }

        //if (rhsOrbitVector)
        //{
        //    rhsVector = (rhsMotor as Motor_Camera).orbitVector;
        //}

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

        if (moreThan && dotProduct > compareAmount || lessThan && dotProduct < compareAmount) isCondition = true;

        if (inverseCondition) return !isCondition;
        else return isCondition;
    }
}
