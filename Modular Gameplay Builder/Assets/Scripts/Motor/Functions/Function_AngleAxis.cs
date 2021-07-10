using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AngleAxis", menuName = "Base/Functions/Angle Axis")]
public class Function_AngleAxis : Function_Base
{
    [Header("Angle Float")]
    public float angleFloat;

    [Header ("Angle Multiplier")]
    public string angleMultiplier;
    public float multiplierOffset;

    [Header("Axis Vector")]
    public string axisVector;

    [Header("Target Quaternion")]
    public string targetQuaternion;

    [Header("Options")]
    public float minAngleAmount;
    public float maxAngleAmount;
    public float slerpSpeed;
    public string slerpMultiplier;


    public override void RunFunction(Motor_Base motor)
    {
        if(axisVector != "" && targetQuaternion != "")
        {
            float localAngle = angleFloat;
            if (angleMultiplier != "")
            {
                localAngle *= motor.variablesMod.FindFloat(angleMultiplier).modFloat + multiplierOffset;
            }

            if (minAngleAmount != 0 || maxAngleAmount != 0) localAngle = Mathf.Clamp(localAngle, minAngleAmount, maxAngleAmount);

            if (slerpSpeed == 0) motor.variablesMod.FindQuaternion(targetQuaternion).modQuaternion = Quaternion.AngleAxis(localAngle, motor.variablesMod.FindVector(axisVector).modVector);
            else
            {
                float localSlerpSpeed = slerpSpeed;
                if (slerpMultiplier != "") localSlerpSpeed *= motor.variablesMod.FindFloat(slerpMultiplier).modFloat;
                motor.variablesMod.FindQuaternion(targetQuaternion).modQuaternion = Quaternion.Slerp(motor.variablesMod.FindQuaternion(targetQuaternion).modQuaternion, Quaternion.AngleAxis(localAngle, motor.variablesMod.FindVector(axisVector).modVector), slerpSpeed * Time.deltaTime);
            }
        }
    }   
}
