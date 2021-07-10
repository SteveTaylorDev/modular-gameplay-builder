using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckAnimationEvents", menuName = "Base/Conditions/(Animator) Check Animation Events")]
public class Condition_CheckAnimationEvents : Condition_Base
{
    [Header("Target Event")]
    public bool isTransitioning;
    public bool footstepL;
    public bool footstepR;

    [Header("Event Condition")]
    public bool lessThan;
    public bool moreThan;


    public override bool IsCondition(Motor_Base motor)
    {
        bool compareBool = default(bool);

        Module_Animator animMod = motor.FindModule(typeof(Module_Animator)) as Module_Animator;

        if(animMod != null)
        {
            if (isTransitioning) compareBool = animMod.isTransitioning;
            if (footstepL) compareBool = animMod.footstepL;
            if (footstepR) compareBool = animMod.footstepR;
        }

        if (inverseCondition) return !compareBool;
        else return compareBool;
    }
}
