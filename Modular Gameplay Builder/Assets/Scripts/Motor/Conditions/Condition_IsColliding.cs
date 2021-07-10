using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IsColliding", menuName = "Base/Conditions/Is Colliding")]
public class Condition_IsColliding : Condition_Base
{
    [Header("Basic Condition")]
    public bool isAnyColliding;
    public bool isGroundColliding;
    public bool isWallColliding;
    public bool isCeilingColliding;

    [Header("Angle Check Condition")]
    public bool checkRawCollisionAverage;
    public bool checkGroundCollision;
    public bool checkWallCollision;
    public bool checkCeilingCollision;
    public bool againstGravityDirection;
    public string againstModVector;

    [Header("Angle Check Amount")]
    public bool angleLessThan;
    public bool angleMoreThan;
    public bool useRequiredGroundAngle;
    public bool useCustomAmount;
    public float customAmount;

    [Header("Collision Height Condition")]
    public bool isHeightGreaterThan;
    public bool isHeightLessThan;
    public string heightAmount;

    [Header("Additional Settings")]
    public bool useCameraTarget;
    public bool debugAngle;
    public bool debugCondition;


    public override bool IsCondition(Motor_Base motor)
    {
        bool isColliding = default(bool);

        Motor_Base targetMotor = motor;
        if (useCameraTarget) targetMotor = (motor as Motor_Camera).cameraTarget.parentMotor;

        Module_Collision collisionMod = targetMotor.FindModule(typeof(Module_Collision)) as Module_Collision;
        if (collisionMod == null)
        {
            Debug.LogWarning("A condition is checking this motor for a Collision Module but cannot find one. This condition will only return false until one is added to this motor.");
            return (false);
        }

        if (collisionMod != null)
        { 
            if (collisionMod.currentCollision != null && isAnyColliding) isColliding = true;
            if (isGroundColliding && collisionMod.collisionGroundAverage != Vector3.zero) isColliding = true;
            if (isWallColliding && collisionMod.collisionWallAverage != Vector3.zero) isColliding = true;
            if (isCeilingColliding && motor.variablesMod.FindVector(collisionMod.collisionCeilingAverage).modVector != Vector3.zero) isColliding = true;

            if (heightAmount != "")
            {
                if (isHeightGreaterThan && collisionMod.collisionHeight < motor.variablesMod.FindFloat(heightAmount).modFloat) return false;
                else if (isHeightLessThan && collisionMod.collisionHeight > motor.variablesMod.FindFloat(heightAmount).modFloat) return false;
            }

            if (checkGroundCollision || checkWallCollision || checkCeilingCollision || checkRawCollisionAverage)
            {
                Module_Gravity gravMod = targetMotor.FindModule(typeof(Module_Gravity)) as Module_Gravity;

                Vector3 collisionVector = Vector3.zero;
                if (checkRawCollisionAverage) collisionVector += collisionMod.rawCollisionAverage;
                if (checkGroundCollision) collisionVector += collisionMod.collisionGroundAverage;
                if (checkWallCollision) collisionVector += collisionMod.collisionWallAverage;
                if (checkCeilingCollision) collisionVector += motor.variablesMod.FindVector(collisionMod.collisionCeilingAverage).modVector;

                Vector3 againstVector = Vector3.zero;
                if (againstGravityDirection)
                {
                    if (gravMod != null) againstVector = -gravMod.localGravityDirection;
                    else againstVector = -GameController.Instance.globalGravityDirection;
                }
                if (againstModVector != "") againstVector = motor.variablesMod.FindVector(againstModVector).modVector;

                float angleAmount = 0;
                if (useRequiredGroundAngle) angleAmount = collisionMod.requiredGroundAngle;
                if (useCustomAmount) angleAmount = customAmount;

                if (collisionVector != Vector3.zero && againstVector != Vector3.zero)
                {
                    if ((angleLessThan && Vector3.Angle(collisionVector, againstVector) < angleAmount) || (angleMoreThan && Vector3.Angle(collisionVector, againstVector) > angleAmount)) isColliding = true;
                    else isColliding = false;
                }
                if (debugAngle) Debug.Log(this + " Angle: " + Vector3.Angle(collisionVector, againstVector));
            }
        }

        if (debugCondition) Debug.Log(this + " Condition: " + isColliding);

        if (inverseCondition) return !isColliding;
        else return isColliding;
    }
}
