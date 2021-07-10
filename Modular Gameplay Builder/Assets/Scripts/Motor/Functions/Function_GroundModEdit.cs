using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GroundModEdit", menuName = "Base/Functions/(Ground) Module Edit")]
public class Function_GroundModEdit : Function_Base
{
    [Header("Ground Variables")]
    public bool setGrounded;
    public bool isGrounded;
    public bool setDisableAirRays;
    public bool disableAirRays;

    [Header("Ground Ray Length")]
    public bool useManualRayLength;
    public string manualRayLength;
    public bool addGroundDistance;
    public float distancePadding;

    [Header("Ray Length Options")]
    [Tooltip("If a float has been entered, Ground Ray Length is multiplied by this. (This is done before adding ground distance to the ray length if addGroundDistance is true)")]
    public string rayMultiplier;
    [Tooltip("If a float is entered, caps the minimum ray length to this amount.")]
    public string minRayLength;

    [Header("Closest Ground Hit")]
    public string closestHitTag;

    [Header("Ground Ray Direction")]
    public bool useManualRayDirection;
    public string manualRayDirection;

    [Header("Angle Measure Direction")]
    public bool setMeasureAgainstGravity;
    public bool measureAgainstGravity;
    public bool useManualMeasureDirection;
    public string manualMeasureDirection;


    public override void RunFunction(Motor_Base motor)
    {
        Module_GroundDetection groundMod = motor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;
        if(groundMod != null)
        {
            // Ground Variables
            if (setGrounded) groundMod.isGrounded = isGrounded;
            if (setDisableAirRays) groundMod.disableAirRays = disableAirRays;

            // Ground Ray Length
            if (useManualRayLength && manualRayLength != "") groundMod.groundRayLength = motor.variablesMod.FindFloat(manualRayLength).modFloat;
            if (rayMultiplier != "") groundMod.groundRayLength *= motor.variablesMod.FindFloat(rayMultiplier).modFloat; 

            if (addGroundDistance)
            {
                if(!useManualRayLength) groundMod.groundRayLength = ((groundMod.closestGroundDistance) + distancePadding);
                else groundMod.groundRayLength += ((groundMod.closestGroundDistance) + distancePadding);
            }

            if (minRayLength != "")
            { 
                if(groundMod.groundRayLength < motor.variablesMod.FindFloat(minRayLength).modFloat) groundMod.groundRayLength = motor.variablesMod.FindFloat(minRayLength).modFloat;
            }

            // Closest Ground Hit
            if (closestHitTag != "") groundMod.closestHitTag = motor.variablesMod.FindString(closestHitTag).modString;

            // Ground Ray Direction
            if (useManualRayDirection && manualRayDirection != "") groundMod.groundRayDirection = motor.variablesMod.FindVector(manualRayDirection).modVector;

            // Angle Measure Direction
            if (setMeasureAgainstGravity) groundMod.measureAgainstGravity = measureAgainstGravity;
            if (useManualMeasureDirection && manualMeasureDirection != "") groundMod.measureVector = motor.variablesMod.FindVector(manualMeasureDirection).modVector;
        }
    }
}
