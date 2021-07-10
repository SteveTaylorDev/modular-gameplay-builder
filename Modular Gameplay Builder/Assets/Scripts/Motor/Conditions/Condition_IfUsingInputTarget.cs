using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IfUsingInputTarget", menuName = "Base/Conditions/If Using Input Target")]
public class Condition_IfUsingInputTarget : Condition_Base
{


    public override bool IsCondition(Motor_Base motor)
    {
        bool conditionBool = default(bool);

        conditionBool = (motor.FindModule(typeof(Module_Input)) as Module_Input).useInputTarget;

        if (inverseCondition) return !conditionBool;
        else return conditionBool;
    }
}
