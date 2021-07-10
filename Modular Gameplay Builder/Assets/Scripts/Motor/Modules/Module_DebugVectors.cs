using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_DebugVectors : Module_Base
{
    [Header("Motor Vectors")]
    public bool directionVector;
    public Color directionColor;
    public bool orientationVector;
    public Color orientationColor;
    public bool moveDirection;
    public Color moveDirectionColor;
    public bool moveVector;
    public Color moveVectorColor;
    public bool additionalMoveVector;
    public Color additionalMoveVectorColor;
    public bool localGravUp;
    public Color localGravColor;
    public bool globalGravUp;
    public Color globalGravColor;
    public bool inputVector;
    public Color inputColor;
    public bool physicsVector;
    public Color physicsColor;

    [Header("Collision")]
    public bool wallCollision;
    public float wallCollisionDuration;
    public Color wallCollisionColor;
    public bool groundCollision;
    public float groundCollisionDuration;
    public Color groundCollisionColor;
    public bool collisionWallAverage;
    public Color collisionWallAverageColor;
    public bool collisionGroundAverage;
    public Color collisionGroundAverageColor;

    [Header("Ground")]
    public bool averageGroundNormal;
    public Color averageGroundNormalColor;

    [Header("Slopes")]
    public bool slopeVector;
    public Color slopeVectorColor;

    [Header("Origin")]
    [Tooltip("This vector is added to the origin position the vectors are drawn from. Good for adjusting the drawn vector position if the collider or mesh size is obscuring it.")]
    public Vector3 originOffset;

    private Motor_Base motorRef;


    public override void UpdateModule(Motor_Base motor)
    {
        if (motorRef == null) motorRef = motor;
    }

    void OnDrawGizmos()
    {
        if (motorRef != null)
        {
            Vector3 rayOrigin = transform.position + (transform.up * transform.localScale.y) + originOffset;

            if (directionVector)
            {
                Gizmos.color = directionColor;
                Gizmos.DrawLine(rayOrigin, rayOrigin + motorRef.variablesMod.FindVector("Direction Vector").modVector * transform.localScale.magnitude);
            }

            if (orientationVector)
            {
                Gizmos.color = orientationColor;
                Gizmos.DrawLine(rayOrigin, rayOrigin + motorRef.variablesMod.FindVector("Orientation Vector").modVector * transform.localScale.magnitude);
            }

            if (moveDirection)
            {
                Gizmos.color = moveDirectionColor;
                Gizmos.DrawLine(rayOrigin, rayOrigin + motorRef.variablesMod.FindVector("Move Direction").modVector * transform.localScale.magnitude);
            }

            if (moveVector)
            {
                Gizmos.color = moveVectorColor;
                Gizmos.DrawLine(rayOrigin, rayOrigin + motorRef.moveVector);
            }

            if (additionalMoveVector)
            {
                Gizmos.color = additionalMoveVectorColor;
                Gizmos.DrawLine(rayOrigin, rayOrigin + motorRef.variablesMod.FindVector(motorRef.additionalMoveVector).modVector);
            }

            if (localGravUp)
            {
                Gizmos.color = localGravColor;
                Module_Gravity gravMod = motorRef.FindModule(typeof(Module_Gravity)) as Module_Gravity;
                Gizmos.DrawLine(rayOrigin, rayOrigin + -gravMod.localGravityDirection * transform.localScale.magnitude);
            }

            if (globalGravUp)
            {
                Gizmos.color = globalGravColor;
                Gizmos.DrawLine(rayOrigin, rayOrigin + -GameController.Instance.globalGravityDirection * transform.localScale.magnitude);
            }

            if (inputVector)
            {
                Module_Input inputMod = motorRef.FindModule(typeof(Module_Input)) as Module_Input;
                Gizmos.color = inputColor;
                Gizmos.DrawLine(rayOrigin, rayOrigin + motorRef.variablesMod.FindVector(inputMod.inputVector).modVector);
            }

            if (physicsVector && motorRef.GetType() == typeof(Motor_Physics))
            {
                Gizmos.color = physicsColor;
                Gizmos.DrawLine(rayOrigin, rayOrigin + (motorRef as Motor_Physics).variablesMod.FindVector((motorRef as Motor_Physics).physicsVector).modVector);
            }

            if (wallCollision)
            {
                Module_Collision collisionMod = motorRef.FindModule(typeof(Module_Collision)) as Module_Collision;
                Module_GroundDetection groundDetectMod = motorRef.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;

                if (collisionMod != null && collisionMod.currentCollision != null)
                {
                    for (int i = 0; i < collisionMod.currentCollision.contacts.Length; i++)
                    {
                        if((groundDetectMod != null && Vector3.Angle(collisionMod.measureVector, collisionMod.currentCollision.contacts[i].normal) > groundDetectMod.requiredGroundAngle) || groundDetectMod == null)
                        {
                            Debug.DrawRay(collisionMod.currentCollision.contacts[i].point, collisionMod.currentCollision.contacts[i].normal, wallCollisionColor, wallCollisionDuration);
                        }
                    }
                }
            }

            if (groundCollision)
            {
                Module_Collision collisionMod = motorRef.FindModule(typeof(Module_Collision)) as Module_Collision;
                Module_GroundDetection groundDetectMod = motorRef.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;

                if (collisionMod != null && collisionMod.currentCollision != null && groundDetectMod != null)
                {
                    for (int i = 0; i < collisionMod.currentCollision.contacts.Length; i++)
                    {
                        if (Vector3.Angle(collisionMod.measureVector, collisionMod.currentCollision.contacts[i].normal) <= groundDetectMod.requiredGroundAngle)
                        {
                            Debug.DrawRay(collisionMod.currentCollision.contacts[i].point, collisionMod.currentCollision.contacts[i].normal, groundCollisionColor, groundCollisionDuration);
                        }
                    }
                }
            }

            if (collisionWallAverage)
            {
                Module_Collision collisionMod = motorRef.FindModule(typeof(Module_Collision)) as Module_Collision;
                if (collisionMod != null)
                {
                    Gizmos.color = collisionWallAverageColor;
                    Gizmos.DrawLine(rayOrigin, rayOrigin + collisionMod.collisionWallAverage * transform.localScale.magnitude);
                }
            }

            if (collisionGroundAverage)
            {
                Module_Collision collisionMod = motorRef.FindModule(typeof(Module_Collision)) as Module_Collision;
                if (collisionMod != null)
                {
                    Gizmos.color = collisionGroundAverageColor;
                    Gizmos.DrawLine(rayOrigin, rayOrigin + collisionMod.collisionGroundAverage * transform.localScale.magnitude);
                }
            }

            if (averageGroundNormal)
            {
                Module_GroundDetection groundDetectMod = motorRef.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;
                if(groundDetectMod != null)
                {
                    Gizmos.color = averageGroundNormalColor;
                    Gizmos.DrawLine(rayOrigin, rayOrigin + groundDetectMod.averageGroundNormal * transform.localScale.magnitude);
                }
            }

            if (slopeVector)
            {
                Module_Slopes slopesMod = motorRef.FindModule(typeof(Module_Slopes)) as Module_Slopes;
                if (slopesMod != null)
                {
                    Gizmos.color = slopeVectorColor;
                    Gizmos.DrawLine(rayOrigin, rayOrigin + motorRef.variablesMod.FindVector(slopesMod.slopeVector).modVector);
                }
            }
        }
    }
}
