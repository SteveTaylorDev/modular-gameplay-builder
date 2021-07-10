using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddFloat", menuName = "Base/Functions/Add Float")]
public class Function_AddFloat : Function_Base
{
    [Header("Source Float")]
    public string sourceFloat;

    [Header("Add Float")]
    public string addFloat;

    [Header("Options")]
    public bool ignoreTime;
    public string addMultiplier;
    public string minMultiplierAmount;
    public string maxMultiplierAmount;


    public override void RunFunction(Motor_Base motor)
    {
        if(sourceFloat != "")
        {
            float deltaFloat = Time.deltaTime;
            if (ignoreTime) deltaFloat = 1;

            float localAddFloat = 0;

            if (addFloat != "")
            {
                float localMultiplier = 1;
                if (addMultiplier != "") localMultiplier = motor.variablesMod.FindFloat(addMultiplier).modFloat;

                if (minMultiplierAmount != "") localMultiplier = Mathf.Clamp(localMultiplier, motor.variablesMod.FindFloat(minMultiplierAmount).modFloat, localMultiplier);
                if (maxMultiplierAmount != "") localMultiplier = Mathf.Clamp(localMultiplier, localMultiplier, motor.variablesMod.FindFloat(maxMultiplierAmount).modFloat);

                localAddFloat = motor.variablesMod.FindFloat(addFloat).modFloat * localMultiplier;
            }

            float resultFloat = motor.variablesMod.FindFloat(sourceFloat).modFloat += localAddFloat * deltaFloat;

            motor.variablesMod.FindFloat(sourceFloat).modFloat = resultFloat;
        }
    }   
}
