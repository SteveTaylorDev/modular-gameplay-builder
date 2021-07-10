using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimatorModEdit", menuName = "Base/Functions/(Animator) Module Edit")]
public class Function_AnimatorModEdit : Function_Base
{
    [Header("Animator Name")]
    public string animatorName;

    [Header("Variables To Update")]
    public bool currentSpeed;
    public float minSpeedCap;
    public float maxSpeedCap;
    public bool isGrounded;
    public bool fallingLocalGravity;
    public bool inputToMoveAngle;

    [Header("Animation Events")]
    public bool footstepL;
    public bool footstepR;



    public override void RunFunction(Motor_Base motor)
    {
        Module_Animator animMod = motor.FindModule(typeof(Module_Animator), animatorName) as Module_Animator;
        if(animMod != null)
        {
            animMod.currentSpeed = currentSpeed;
            animMod.minSpeedCap = minSpeedCap;
            animMod.maxSpeedCap = maxSpeedCap;
            animMod.isGrounded = isGrounded;
            animMod.fallingLocalGravity = fallingLocalGravity;
            animMod.inputToMoveAngle = inputToMoveAngle;

            animMod.footstepL = footstepL;
            animMod.footstepR = footstepR;
        }
    }
}
