using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_Slopes: Module_Base
{
    [Header("Slope Variables")]
    public string slopeVector;
    public float slopeStrengthFactor;

    [Header("Slope Normal")]
    public string slopeNormal;
    public bool useOrientation;
    public bool useCollision;
    public bool useGroundDetection;

    [Header("Orientation To Measure")]
    public string orientationToMeasure;
    public bool measureFromOrientation;
    public bool measureFromLocalGravity;
    public bool measureFromGlobalGravity;


    public override void UpdateModule(Motor_Base motor)
    {
        Vector3 localSlopeNormal = Vector3.zero;
        Vector3 measureOrientation = Vector3.zero;

     // Slope Normal
        if (slopeNormal != "") localSlopeNormal = motor.variablesMod.FindVector(slopeNormal).modVector;

        //if (useOrientation) localSlopeNormal = motor.variablesMod.FindVector(motor.directionVector).modVector;
        if (useCollision)
        {
            localSlopeNormal = (motor.FindModule(typeof(Module_Collision)) as Module_Collision).collisionGroundAverage;
        }

        if (useGroundDetection)
        {
            localSlopeNormal = (motor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection).averageGroundNormal;
        }

        // Orientation To Measure
        if (orientationToMeasure != "") measureOrientation = motor.variablesMod.FindVector(orientationToMeasure).modVector;

        //if (measureFromOrientation) measureOrientation = motor.variablesMod.FindVector(motor.orientationVector).modVector;
        if (measureFromLocalGravity) measureOrientation = (motor.FindModule(typeof(Module_Gravity)) as Module_Gravity).localGravityDirection;
        if (measureFromGlobalGravity) measureOrientation = GameController.Instance.globalGravityDirection;

        motor.variablesMod.FindVector(slopeVector).modVector = Vector3.Cross(localSlopeNormal, measureOrientation);
        motor.variablesMod.FindVector(slopeVector).modVector = Vector3.Cross(motor.variablesMod.FindVector(slopeVector).modVector, localSlopeNormal) * slopeStrengthFactor;
    }


}
