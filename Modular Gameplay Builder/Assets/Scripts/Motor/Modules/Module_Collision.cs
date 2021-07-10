using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_Collision : Module_Base
{
    [Header("Components")]
    public Collider motorCollider;

    [Header("Collision")]
    [Tooltip("The average normal found when combining all current collision normals.")]
    public Vector3 rawCollisionAverage;
    [Tooltip("The current collision this motor is involved in.")]
    public Collision currentCollision;
    [Tooltip("The Physic Material that the last hit collider is using. Used for calculating bounce and friction.")]
    public PhysicMaterial collisionPhysicMaterial;
    [Tooltip("The height of the current collider.")]
    public float colliderHeight;
    [Tooltip("The width of the current collider.")]
    public float colliderWidth;
    [Tooltip("The center vector of the current collider.")]
    public string colliderCenter;

    [Header("Collision Objects")]
    public List<GameObject> collisionObjects;

    [Header("Ignore Options")]
    [Tooltip("The list of colliders that will be ignored if this collider comes into contact with them.")]
    public List<Collider> colliderIgnoreList;
    [Tooltip("The layer mask that determines which layers are ignored during collisions.")]
    public LayerMask ignoreLayers;
    [Tooltip("If true, adds any objects on layers that are in the ignoreLayers list to the colliderIgnoreList and ignores all collision with those objects.")]
    public bool addIgnoreLayerToList;


    [Header("Angle Options")]
    [Tooltip("If set to true, measures the ground and ceiling angles using either local or global gravity direction, depending on if a Gravity Module is found on this motor. If false, angle is measured against transform.up.")]
    public bool measureAgainstGravity;
    [Tooltip("This vector will be used and measured against when any angles are calculcated using collision normals. This is determined by the orientation that is being measured against.")]
    public Vector3 measureVector;

    [Header("Wall")]
    [Tooltip("The average normal found when combining all current wall collision normals.")]
    public Vector3 collisionWallAverage;
    [Tooltip("The list of collision normals currently being detected by the collider. (If requiredGroundAngle is set, only normals outside this angle will be listed)")]
    public List<Vector3> collisionWallNormals;
    [Tooltip("The height of the current collision relative to measureVector (Vector3.up if measureVector is not set)")]
    public float collisionHeight;
    [Tooltip("If true, will ignore wall collisions that are below minimumCollisionHeight.")]
    public bool useMinCollisionHeight;
    [Tooltip("If the collision hit is below this height when measured against the motor position, it is ignored.")]
    public float minimumCollisionHeight;

    [Header("Minimum Height Raycast")]
    [Tooltip("If true, casts a raycast at minimumCollisionHeight in rayDirection to detect any collisions outside of the collider's size.")]
    public bool castCollisionRay;
    [Tooltip("The length of the collision ray.")]
    public string rayLength;
    [Tooltip("The direction of the collision ray.")]
    public string rayDirection;
    [Tooltip("The amount the ray origin will be offset by. (Usually to account for rays being cast on the other side of a wall")]
    public string rayOffset;
    [Tooltip("If true, casts a spherecast as the collision ray instead of a regular raycast.")]
    public bool useSphereCast;
    [Tooltip("The radius of the spherecast.")]
    public string sphereRadius;

    [Header("Ground")]
    [Range(0, 360)]
    [Tooltip("The maximum angle difference that a normal must be within to be considered ground. Depending on whether measureAgainstGravity is true, or a Gravity Module is present, the angle is measured against either localGravityDirection, globalGravityDirection, or transform.up.")]
    public float requiredGroundAngle;
    [Tooltip("The average normal found when combining all current ground collision normals.")]
    public Vector3 collisionGroundAverage;
    [Tooltip("The list of collision normals within the required ground angle currently being detected by the collider.")]
    public List<Vector3> collisionGroundNormals;

    [Header("Ceiling")]
    [Range(0, 360)]
    [Tooltip("The maximum angle difference that a normal must be within to be considered ceiling. Depending on whether measureAgainstGravity is true, or a Gravity Module is present, the angle is measured against either localGravityDirection, globalGravityDirection, or transform.up.")]
    public float requiredCeilingAngle;
    [Tooltip("The average normal found when combining all current ceiling collision normals.")]
    public string collisionCeilingAverage;
    [Tooltip("The list of collision normals within the required ceiling angle currently being detected by the collider.")]
    public List<Vector3> collisionCeilingNormals;

    [Header("Debug")]
    public bool debugCastCollisionRay;
    public Color castCollisionRayColor;
    public bool debugWallCollisionRay;
    public Color wallCollisionColor;
    public bool debugGroundCollisionRay;
    public Color groundCollisionColor;
    public bool debugCeilingCollisionRay;
    public Color ceilingCollisionColor;
    public float rayTime;

    // This local motor reference is to allow OnCollision functions to reference the motor that was fed in by UpdateModule.
    private Motor_Base motor;


    private void OnCollisionEnter(Collision collision)
    {
        if (motor == null) motor = GetComponent<Motor_Base>();
        else
        {
            currentCollision = collision;
            CheckCollision();
            motor.currentState.CollisionEnterState(motor);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (motor == null) motor = GetComponent<Motor_Base>();
        else
        {
            currentCollision = collision;
            CheckCollision();            
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        currentCollision = null;
        CheckCollision();
    }


    public override void UpdateModule(Motor_Base motor)
    {
        if (this.motor == null) this.motor = motor;

        // If motorCollider is not set...
        if (motorCollider == null)
        {
            //...and there is a collider found on this object, set the first found collider component on this object to motorCollider for referencing.
            if (GetComponent<Collider>() != null)
            {
                motorCollider = GetComponent<Collider>();
            }

            // If no collider component is found, add one and set the motorCollider reference to the newly added collider component.
            else
            {
                Debug.LogWarning("This motor GameObject does not contain a collider component. Now adding a capsule collider component to this GameObject.");
                motorCollider = gameObject.AddComponent<CapsuleCollider>();
                motorCollider.isTrigger = true;
            }
        }

        // Capsule Collider dimensions
        if (motorCollider.GetType() == typeof(CapsuleCollider))
        {
            (motorCollider as CapsuleCollider).radius = colliderWidth;
            (motorCollider as CapsuleCollider).height = colliderHeight;
            if (colliderCenter != "") (motorCollider as CapsuleCollider).center = motor.variablesMod.FindVector(colliderCenter).modVector;
        }
        //
    }

    private void FixedUpdate()
    {
        // Set the collision average normals to Vector3.zero
        if (motor != null)
        {
            collisionGroundAverage = Vector3.zero;
            collisionWallAverage = Vector3.zero;
            if (collisionCeilingAverage != "") motor.variablesMod.FindVector(collisionCeilingAverage).modVector = Vector3.zero;
        }

        // Clear the collision lists.
        collisionObjects.Clear();
        collisionWallNormals.Clear();
        collisionGroundNormals.Clear();
        collisionCeilingNormals.Clear();
    }

    private void CheckCollision()
    {
        if (currentCollision != null)
        {
            if (measureAgainstGravity)
            {
                Module_Gravity gravityMod = motor.FindModule(typeof(Module_Gravity)) as Module_Gravity;

                if (gravityMod != null) measureVector = -gravityMod.localGravityDirection;
                else measureVector = -GameController.Instance.globalGravityDirection;
            }
            else measureVector = motor.transform.up;

            // Min Collison Height Ray
            if (castCollisionRay)
            {
                if (rayDirection != "" && rayLength != "")
                {
                    Vector3 rayOrigin = motor.transform.position;
                    if (rayOffset != "") rayOrigin += motor.variablesMod.FindVector(rayOffset).modVector;

                    if (debugCastCollisionRay) Debug.DrawRay((rayOrigin + (motor.variablesMod.FindVector(rayDirection).modVector * colliderWidth)) - (measureVector * minimumCollisionHeight), motor.variablesMod.FindVector(rayDirection).modVector * motor.variablesMod.FindFloat(rayLength).modFloat, castCollisionRayColor);

                    if (!useSphereCast)
                    {
                        if (Physics.Raycast((rayOrigin + (motor.variablesMod.FindVector(rayDirection).modVector * colliderWidth)) - (measureVector * minimumCollisionHeight), motor.variablesMod.FindVector(rayDirection).modVector, out RaycastHit rayHit, motor.variablesMod.FindFloat(rayLength).modFloat, ~ignoreLayers))
                        {
                            if (!collisionObjects.Contains(rayHit.collider.gameObject)) collisionObjects.Add(rayHit.collider.gameObject);

                            if (Vector3.Angle(rayHit.normal, measureVector) < requiredGroundAngle)
                            {
                                if (!collisionGroundNormals.Contains(rayHit.normal)) collisionGroundNormals.Add(rayHit.normal);
                            }
                            else if (Vector3.Angle(rayHit.normal, measureVector) > requiredCeilingAngle)
                            {
                                if (!collisionCeilingNormals.Contains(rayHit.normal)) collisionCeilingNormals.Add(rayHit.normal);
                            }
                            else
                            {
                                if (!collisionWallNormals.Contains(rayHit.normal)) collisionWallNormals.Add(rayHit.normal);
                            }
                        }
                    }
                    else
                    {
                        if (Physics.SphereCast((rayOrigin + (motor.variablesMod.FindVector(rayDirection).modVector * colliderWidth)) - (measureVector * minimumCollisionHeight), motor.variablesMod.FindFloat(sphereRadius).modFloat, motor.variablesMod.FindVector(rayDirection).modVector, out RaycastHit rayHit, motor.variablesMod.FindFloat(rayLength).modFloat, ~ignoreLayers))
                        {
                            if (!collisionObjects.Contains(rayHit.collider.gameObject)) collisionObjects.Add(rayHit.collider.gameObject);

                            if (Vector3.Angle(rayHit.normal, measureVector) < requiredGroundAngle)
                            {
                                if (!collisionGroundNormals.Contains(rayHit.normal)) collisionGroundNormals.Add(rayHit.normal);
                            }
                            else if (Vector3.Angle(rayHit.normal, measureVector) > requiredCeilingAngle)
                            {
                                if (!collisionCeilingNormals.Contains(rayHit.normal)) collisionCeilingNormals.Add(rayHit.normal);
                            }
                            else
                            {
                                if (!collisionWallNormals.Contains(rayHit.normal)) collisionWallNormals.Add(rayHit.normal);
                            }
                        }
                    }
                }
            }

            //

            for (int i = 0; i < currentCollision.contacts.Length; i++)
            {
                for(int c = 0; c < colliderIgnoreList.Count; c++)
                {
                    if (currentCollision.contacts[i].otherCollider == colliderIgnoreList[c])
                    {
                        Physics.IgnoreCollision(motorCollider, colliderIgnoreList[c]);
                        return;
                    }
                }

                if (ignoreLayers == (ignoreLayers | (1 << currentCollision.contacts[i].otherCollider.gameObject.layer)))
                {
                    if (addIgnoreLayerToList) Physics.IgnoreCollision(motorCollider, currentCollision.contacts[i].otherCollider);
                    return;
                }

                if (!collisionObjects.Contains(currentCollision.contacts[i].otherCollider.gameObject)) collisionObjects.Add(currentCollision.contacts[i].otherCollider.gameObject);

                Vector3 motorToCollision = -(currentCollision.contacts[i].point - motor.transform.position);

                collisionHeight = Vector3.Dot(motorToCollision, measureVector);

                if (Vector3.Angle(currentCollision.contacts[i].normal, measureVector) < requiredGroundAngle)
                {
                    if (debugGroundCollisionRay) Debug.DrawRay(currentCollision.contacts[i].point, currentCollision.contacts[i].normal, groundCollisionColor, rayTime);
                    if (!collisionGroundNormals.Contains(currentCollision.contacts[i].normal)) collisionGroundNormals.Add(currentCollision.contacts[i].normal);
                }
                else if (Vector3.Angle(currentCollision.contacts[i].normal, measureVector) > requiredCeilingAngle)
                {
                    if (debugCeilingCollisionRay) Debug.DrawRay(currentCollision.contacts[i].point, currentCollision.contacts[i].normal, ceilingCollisionColor, rayTime);
                    if (!collisionCeilingNormals.Contains(currentCollision.contacts[i].normal)) collisionCeilingNormals.Add(currentCollision.contacts[i].normal);
                }
                else
                {
                    if (!useMinCollisionHeight || collisionHeight < minimumCollisionHeight)
                    {
                        if (debugWallCollisionRay) Debug.DrawRay(currentCollision.contacts[i].point, currentCollision.contacts[i].normal, wallCollisionColor, rayTime);
                        if (!collisionWallNormals.Contains(currentCollision.contacts[i].normal)) collisionWallNormals.Add(currentCollision.contacts[i].normal);
                    }
                }
            }

            collisionPhysicMaterial = currentCollision.collider.material;
        }

        rawCollisionAverage = Vector3.zero;

        // If collisionGroundNormals is longer than zero, add all current ground collision normals together, and divide by the amount to find the average.
        if (collisionGroundNormals.Count > 0)
        {
            for (int i = 0; i < collisionGroundNormals.Count; i++) collisionGroundAverage += collisionGroundNormals[i];
            collisionGroundAverage /= collisionGroundNormals.Count;
            collisionGroundAverage = collisionGroundAverage.normalized;

            rawCollisionAverage += collisionGroundAverage;
        }

        // If collisionWallNormals is longer than zero, add all current wall collision normals together, and divide by the amount to find the average.
        if (collisionWallNormals.Count > 0)
        {
            for (int i = 0; i < collisionWallNormals.Count; i++) collisionWallAverage += collisionWallNormals[i];
            collisionWallAverage /= collisionWallNormals.Count;
            collisionWallAverage = collisionWallAverage.normalized;

            rawCollisionAverage += collisionWallAverage;
        }

        // If collisionCeilingNormals is longer than zero, add all current ceiling collision normals together, and divide by the amount to find the average.
        if (collisionCeilingNormals.Count > 0 && (collisionCeilingAverage != ""))
        {
            for (int i = 0; i < collisionCeilingNormals.Count; i++) motor.variablesMod.FindVector(collisionCeilingAverage).modVector += collisionCeilingNormals[i];
            motor.variablesMod.FindVector(collisionCeilingAverage).modVector /= collisionCeilingNormals.Count;
            motor.variablesMod.FindVector(collisionCeilingAverage).modVector = motor.variablesMod.FindVector(collisionCeilingAverage).modVector.normalized;

            rawCollisionAverage += motor.variablesMod.FindVector(collisionCeilingAverage).modVector;
        }
    }
}
