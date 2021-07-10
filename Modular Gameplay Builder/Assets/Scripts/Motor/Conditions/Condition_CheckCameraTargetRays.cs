using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckCameraTargetRays", menuName = "Base/Conditions/(Camera) Check Target Rays")]
public class Condition_CheckCameraTargetRays : Condition_Base
{
    [Header("Closest Hit Distance")]
    public bool fromLocalPosition;
    public bool fromCameraTargetPosition;

    [Header("Compare Condition")]
    public bool hasRayHit;
    public bool moreThanAmount;
    public bool lessThanAmount;

    [Header("Compare Amount")]
    public float compareAmount;
    public bool debugDistance;

    [Header("Additional Conditions")]
    public bool ifSnapPosition;

    public override bool IsCondition(Motor_Base motor)
    {
        bool compareBool = default(bool);

        Vector3 distancePosition = Vector3.zero;

        if (fromLocalPosition) distancePosition = motor.transform.position;
        if (fromCameraTargetPosition) distancePosition = (motor as Motor_Camera).cameraTarget.transform.position;

        if(debugDistance) Debug.Log(((motor as Motor_Camera).closestHit.point - distancePosition).magnitude);

        if ((lessThanAmount && ((motor as Motor_Camera).closestHit.point - distancePosition).magnitude < compareAmount) || (moreThanAmount && ((motor as Motor_Camera).closestHit.point - distancePosition).magnitude > compareAmount))
        {
            if(!ifSnapPosition || (ifSnapPosition && (motor as Motor_Camera).hitSnapPosition)) compareBool = true;
        }

        if (hasRayHit && (motor as Motor_Camera).closestHit.collider != null && (!ifSnapPosition || (ifSnapPosition && (motor as Motor_Camera).hitSnapPosition))) compareBool = true;

        if (inverseCondition) return !compareBool;
        else return compareBool;
    }
}
