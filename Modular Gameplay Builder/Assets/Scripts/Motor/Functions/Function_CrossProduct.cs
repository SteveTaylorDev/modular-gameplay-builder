using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrossProduct", menuName = "Base/Functions/Cross Product")]
public class Function_CrossProduct : Function_Base
{
    [Header("Source Vector")]
    public string sourceVector;
    public bool sourceMoveDirection;
    public bool sourceDirectionVector;
    public bool sourcePhysicsVector;

    [Header("Left Hand Vector")]
    public bool reverseLhs;
    public string lhsVector;
    public bool lhsTransformRight;
    public bool lhsPhysicsVector;

    [Header("Right Hand Vector")]
    public bool reverseRhs;
    public string rhsVector;
    public bool rhsGroundAverage;
    public bool rhsLocalGrav;

    [Header("Options")]
    public bool reverseResult;
    public bool normalizeVector;
    public bool preserveMagnitude;
    public bool debugVector;


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

        if (lhsPhysicsVector)
        {
            localLhsVector = (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector;
        }

        // Right Hand Vector
        if (rhsVector != "") localRhsVector = motor.variablesMod.FindVector(rhsVector).modVector;

        if (rhsGroundAverage)
        {
            Module_GroundDetection groundMod = motor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;
            if (groundMod != null) localRhsVector = groundMod.averageGroundNormal;
        }

        if (rhsLocalGrav)
        {
            Module_Gravity gravMod = motor.FindModule(typeof(Module_Gravity)) as Module_Gravity;
            localRhsVector = gravMod.localGravityDirection;
        }

        // Source Vector
        Vector3 crossVector = Vector3.Cross(localLhsVector, localRhsVector);
        if (normalizeVector) crossVector = crossVector.normalized;
        if (reverseResult) crossVector = -crossVector;

        if (sourceVector != "")
        {
            if(!preserveMagnitude) motor.variablesMod.FindVector(sourceVector).modVector = crossVector;
            else motor.variablesMod.FindVector(sourceVector).modVector = crossVector.normalized * motor.variablesMod.FindVector(sourceVector).modVector.magnitude;
        }

        if (sourceMoveDirection)
        {
            motor.variablesMod.FindVector("Move Direction").modVector = crossVector;
        }

        if (sourcePhysicsVector)
        {
            Module_GroundDetection groundMod = motor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;

            if (!preserveMagnitude) (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector = crossVector;
            else (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector = crossVector.normalized * (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector.magnitude;
        }

        if (debugVector) Debug.DrawRay(motor.transform.position + motor.transform.up, crossVector, Color.white, 1);

        //if (sourceDirectionVector)
        //{
        //    motor.directionVector = crossVector;
        //}
    }
}
