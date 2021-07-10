using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_CameraTarget : Module_Base
{
    [Header("Motor References")]
    public Motor_Camera currentCamera;
    public Motor_Base parentMotor;

    [Header("Sensitivity")]
    public float xOrbitSensitivity;
    public float yOrbitSensitivity;

    [Header("Options")]
    public bool ignoreCameraCollision;

    public override void UpdateModule(Motor_Base motor)
    {
        parentMotor = motor;

        if (ignoreCameraCollision)
        {
            Module_Collision collisionMod = (motor.FindModule(typeof(Module_Collision)) as Module_Collision);

            if (collisionMod == null)
            {
                Debug.LogWarning("Cannot add camera collider to collision ignore list, as the " + motor + " motor does not contain a Collision Module.");
                return;
            }

            Module_Collision camCollisionMod = (currentCamera.FindModule(typeof(Module_Collision)) as Module_Collision);

            if (camCollisionMod != null)
            {
                if (!collisionMod.colliderIgnoreList.Contains(camCollisionMod.motorCollider))
                {
                    collisionMod.colliderIgnoreList.Add(camCollisionMod.motorCollider);
                }
                else ignoreCameraCollision = false;
            }
        }
    }
}
