using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SetObjectActive", menuName = "Base/Functions/Set Object Active")]
public class Function_SetObjectActive : Function_Base
{
    [Header("Target Object Transform")]
    public string targetTransform;

    [Header("Target Object Motor")]
    public string targetMotor;

    [Header("Active Status")]
    public bool setActive;
    public bool setInactive;

    public override void RunFunction(Motor_Base motor)
    {
        if(targetTransform != "")
        {
            if (setActive) motor.variablesMod.FindTransform(targetTransform).modTransform.gameObject.SetActive(true);
            if (setInactive) motor.variablesMod.FindTransform(targetTransform).modTransform.gameObject.SetActive(false);
        }

        if (targetMotor != "")
        {
            if (setActive) motor.variablesMod.FindMotor(targetMotor).modMotor.gameObject.SetActive(true);
            if (setInactive) motor.variablesMod.FindMotor(targetMotor).modMotor.gameObject.SetActive(false);
        }
    }
}
