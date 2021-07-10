using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IfGroundLayer", menuName = "Base/Conditions/If Ground Layer")]
public class Condition_IfGroundLayer : Condition_Base
{
    [Header("Target Layers")]
    public LayerMask targetLayers;

    [Header("Ground Check Options")]
    public bool useClosestHitLayer;
    public bool useAllGroundObjects;

    [Header("Additional Options")]
    public bool useCameraTargetMotor;
    public bool debugCondition;


    public override bool IsCondition(Motor_Base motor)
    {
        bool isBool = default(bool);

        Motor_Base localMotor = motor;
        if (useCameraTargetMotor) localMotor = (motor as Motor_Camera).cameraTarget.parentMotor;

        Module_GroundDetection groundMod = localMotor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;

        if(useAllGroundObjects)
        {
            for(int i = 0; i < groundMod.groundHitObjects.Count; i++)
            {
                if (groundMod.groundHitObjects[i].layer == targetLayers) isBool = true;
            }
        }

        if (useClosestHitLayer)
        {
            if (targetLayers == (targetLayers | (1 << groundMod.closestHitLayer))) isBool = true;
        }

        if (debugCondition) Debug.Log(this + " Condition: " + isBool);

        if (inverseCondition) return !isBool;
        else return isBool;
    }
}
