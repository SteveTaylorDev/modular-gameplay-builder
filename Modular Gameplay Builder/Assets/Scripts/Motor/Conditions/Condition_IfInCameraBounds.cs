using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IfInCameraBounds", menuName = "Base/Conditions/(Camera) If In Camera Bounds")]
public class Condition_IfInCameraBounds : Condition_Base
{
    [Header("Target to Check")]
    public bool cameraTarget;

    [Header("Position to Check")]
    public bool isXPos;
    public bool isYPos;
    public bool isZPos;

    [Header("Condition")]
    public bool moreThan;
    public bool lessThan;
    public string conditionAmount;

    [Header("Options")]
    public bool useAbsolutePositions;
    public bool debugScreenPosition;

    public override bool IsCondition(Motor_Base motor)
    {
        if (motor.GetType() != typeof(Motor_Camera))
        {
            Debug.LogWarning("Cannot check camera bounds on " + this + " because the motor " + motor + " is not a Camera Motor.");
            return false;
        }

        else
        {
            bool isBool = default(bool);

            Camera motorCam = (motor as Motor_Camera).motorCam;
            Vector3 screenPos = Vector3.zero;

            if (cameraTarget) screenPos = motorCam.WorldToViewportPoint((motor as Motor_Camera).cameraTarget.transform.position);

            if (debugScreenPosition) Debug.Log(screenPos);

            if (isXPos)
            {
                float localX = screenPos.x;
                if (useAbsolutePositions) localX = Mathf.Abs(localX);

                if ((moreThan && localX > motor.variablesMod.FindFloat(conditionAmount).modFloat) || (lessThan && localX < motor.variablesMod.FindFloat(conditionAmount).modFloat)) isBool = true;
            }

            if (isYPos)
            {
                float localY = screenPos.y;
                if (useAbsolutePositions) localY = Mathf.Abs(localY);

                if ((moreThan && localY > motor.variablesMod.FindFloat(conditionAmount).modFloat) || (lessThan && localY < motor.variablesMod.FindFloat(conditionAmount).modFloat)) isBool = true;
            }

            if (isZPos)
            {
                float localZ = screenPos.z;
                if (useAbsolutePositions) localZ = Mathf.Abs(localZ);

                if ((moreThan && localZ > motor.variablesMod.FindFloat(conditionAmount).modFloat) || (lessThan && localZ < motor.variablesMod.FindFloat(conditionAmount).modFloat)) isBool = true;
            }

            if (inverseCondition) return !isBool;
            else return isBool;
        }
    }
}
