using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollisionModEdit", menuName = "Base/Functions/(Collision) Module Edit")]
public class Function_CollisionModEdit : Function_Base
{
    [Header("Collision Name")]
    public string collisionName;

    [Header("Collider Size")]
    public string colliderHeight;
    public string colliderWidth;
    public string colliderCenter;

    public bool useManualSizes;
    public float manualCollisionHeight;
    public float manualCollisionWidth;

    [Header("Collision Ray")]
    public string castCollisionRay;
    public string rayLength;
    public string rayDirection;
    public string rayOffset;
    public string useSphereCast;
    public string sphereRadius;

    [Header("Wall Collision")]
    public string useMinCollisionHeight;
    public string minCollisionHeight;

    [Header("Options")]
    public bool lerpSize;
    public string lerpSpeed;

    public override void RunFunction(Motor_Base motor)
    {
        Module_Collision collisionMod = motor.FindModule(typeof(Module_Collision), collisionName) as Module_Collision;
        if(collisionMod != null)
        {
            // Collision Ray
            if (castCollisionRay != "") collisionMod.castCollisionRay = motor.variablesMod.FindBool(castCollisionRay).modBool;
            if (rayLength != "") collisionMod.rayLength = rayLength;
            if (rayDirection != "") collisionMod.rayDirection = rayDirection;
            if (rayOffset != "") collisionMod.rayOffset = rayOffset;
            if (useSphereCast != "") collisionMod.useSphereCast = motor.variablesMod.FindBool(useSphereCast).modBool;
            if (sphereRadius != "") collisionMod.sphereRadius = sphereRadius;
            //

            float localHeight = 0;
            float localWidth = 0;

            if (colliderHeight != "") localHeight = motor.variablesMod.FindFloat(colliderHeight).modFloat;
            if (colliderWidth != "") localWidth = motor.variablesMod.FindFloat(colliderWidth).modFloat; 

            if (useManualSizes)
            {
                localHeight = manualCollisionHeight;
                localWidth = manualCollisionWidth;
            }

            if (!lerpSize)
            {
                collisionMod.colliderHeight = localHeight;
                collisionMod.colliderWidth = localWidth;
                if (colliderCenter != "") collisionMod.colliderCenter = colliderCenter;
            }

            else
            {
                float localLerpSpeed = 1;
                if (lerpSpeed != "") localLerpSpeed = motor.variablesMod.FindFloat(lerpSpeed).modFloat;
                collisionMod.colliderHeight = Mathf.Lerp(collisionMod.colliderHeight, localHeight, localLerpSpeed * Time.deltaTime);
                collisionMod.colliderWidth = Mathf.Lerp(collisionMod.colliderWidth, localWidth, localLerpSpeed * Time.deltaTime);
                if (colliderCenter != "") collisionMod.colliderCenter = colliderCenter;
            }

            if (useMinCollisionHeight != "") collisionMod.useMinCollisionHeight = motor.variablesMod.FindBool(useMinCollisionHeight).modBool;
            if (minCollisionHeight != "") collisionMod.minimumCollisionHeight = motor.variablesMod.FindFloat(minCollisionHeight).modFloat;
        }
    }
}
