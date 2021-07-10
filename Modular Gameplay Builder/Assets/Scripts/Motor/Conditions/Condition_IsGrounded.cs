using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IsGrounded", menuName = "Base/Conditions/Is Grounded")]
public class Condition_IsGrounded : Condition_Base
{
    public bool useCameraTarget;

    public override bool IsCondition(Motor_Base motor)
    {
        bool isGrounded = default(bool);

        Motor_Base targetMotor = motor;
        if (useCameraTarget) targetMotor = (motor as Motor_Camera).cameraTarget.parentMotor;

        if (targetMotor == null) return false;

        Module_GroundDetection groundMod = targetMotor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;
        if (groundMod != null) isGrounded = groundMod.isGrounded;
        else Debug.LogWarning("A condition is checking this motor for a Ground Detection Module but cannot find one. This condition will only return false until one is added to this motor.");

        if (inverseCondition) return !isGrounded;
        else return isGrounded;
    }
}
