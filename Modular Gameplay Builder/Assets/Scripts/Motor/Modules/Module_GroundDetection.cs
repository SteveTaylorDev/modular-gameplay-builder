using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_GroundDetection : Module_Base
{
    [Header("Ground Detection")]
    [Tooltip("Set to true when ground is detected.")]
    public bool isGrounded;
    [Range(0, 360)]
    [Tooltip("The maximum angle difference that a normal must be within to be considered ground. Depending on whether measureAgainstGravity is true, or a Gravity Module is present, the angle is measured against either localGravityDirection, globalGravityDirection, or orientationVector.")]
    public float requiredGroundAngle;
    [Tooltip("The current ground angle when measured against the orientation (or local gravity if a Gravity module is attached).")]
    public float localGroundAngle;
    [Tooltip("The current ground angle when measured against the global gravity direction.")]
    public float globalGroundAngle;
    [Tooltip("The average normal vector found when combining all ground normals currently detected by ground raycasts.")]
    public Vector3 averageGroundNormal;
    [Tooltip("The list of hit normals, found from either collision or ground raycasts.")]
    public List<Vector3> groundHitNormals;
    [Tooltip("The list of hit data from any current ground detecting raycasts.")]
    public List<RaycastHit> groundHitData;
    [Tooltip("The list of hit objects from any current ground detecting raycasts.")]
    public List<GameObject> groundHitObjects;

    [Header("Angle Options")]
    [Tooltip("If set to true, measures the requiredGroundAngle using either local or global gravity direction, depending on if a Gravity Module is found on this motor. If false, angle is measured against transform.up.")]
    public bool measureAgainstGravity;
    [Tooltip("The vector that is measured against when calculating ground angles.")]
    public Vector3 measureVector;

    [Header("Ground Raycasts")]
    [Tooltip("If true, ground colliders will be detected with raycasts. Otherwise, a collision module or manually setting in state must be used for ground detection.")]
    public bool useGroundRaycasts;
    [Tooltip("If true, and a Collision module is attached, any collision normals within a ground angle will be used when calculating the average ground normal.")]
    public bool combineWithCollision;
    [Tooltip("If true, and a Collision module is attached, only collision will be used for checking for ground when isGrounded is false. When grounded, rays will be used (if useGroundRaycasts is enabled)")]
    public bool disableAirRays;
    [Tooltip("The amount of ground raycasts that will be cast along the x axis of the collider. More raycasts are expensive, but they increase the accuracy of the ground detection.")]
    public int xRaycastAmount;
    [Tooltip("The amount of ground raycasts that will be cast along the z axis of the collider. More raycasts are expensive, but they increase the accuracy of the ground detection.")]
    public int zRaycastAmount;
    [Tooltip("The distance that each raycast origin is moved towards (or away) from the center of the collider.")]
    public float raycastPaddingDistance;
    [Tooltip("The length that the ground rays are cast. (This is multiplied by the collider height and scale to allow for object scaling")]
    public float groundRayLength;
    [Tooltip("The direction that ground detecting raycasts will be cast in. Can be set directly in state if setRaysToOrientation and setRaysToGravity are set to false.")]
    public Vector3 groundRayDirection;

    [Header("Closest Ground Hit")]
    [Tooltip("The distance that the closest detected ground point is from the ray origin. (This is divided by the collider height to allow for object scaling)")]
    public float closestGroundDistance;
    [Tooltip("The hit data of the closest point that is hit by ground raycasts.")]
    public RaycastHit closestGroundHit;
    [Tooltip("The tag of the closest object that is hit by ground raycasts.")]
    public string closestHitTag;
    [Tooltip("The layer of the closest object that is hit by ground raycasts.")]
    public int closestHitLayer;

    [Header("Additional Ray Options")]
    [Tooltip("If the ground rays hit a collider that's on a layer in this list, it will be ignored.")]
    public LayerMask ignoreLayers;
    [Tooltip("If true, any ray outside of the collider radius will not be cast.")]
    public bool ignoreCornerRays;
    [Tooltip("If true, the ray origin will be checked with an additional ray to see if it is being cast on the other side of a wall.")]
    public bool originWallCheck;
    [Tooltip("If true, casts a ray forward to detect ground ahead of the motor.")]
    public bool checkGroundAhead;

    [Tooltip("If true, an additional check will be performed on the ground hit to determine if it is too close to be considered ground.")]
    public bool minDistanceCheck;
    [Tooltip("The minimum ground hit distance allowed before setting isGrounded to false. (This always considers the object scale as 1, to allow for object scaling)")]
    public float minGroundDistance;
    [Tooltip("If true, an additional check will be performed on the ground hit to determine if it is too far away to be considered ground.")]
    public bool maxDistanceCheck;
    [Tooltip("The maximum ground hit distance allowed before setting isGrounded to false. (This always considers the object scale as 1, to allow for object scaling)")]
    public float maxGroundDistance;

    [Header("Debug")]
    public bool drawGroundRaycasts;
    public Color hitDrawColor;
    public Color noHitDrawColor;


    public override void UpdateModule(Motor_Base motor)
    {
        if (groundHitData != null) groundHitData.Clear();
        else groundHitData = new List<RaycastHit>();
        groundHitObjects.Clear();
        groundHitNormals.Clear();

        localGroundAngle = 0;
        globalGroundAngle = 0;
        closestGroundDistance = 0;
        closestGroundHit = default;

        averageGroundNormal = Vector3.zero;

        //groundPhysicMaterial = null;

        // These references are optional and only for extended functionality.
        Module_Collision collisionMod = motor.FindModule(typeof(Module_Collision)) as Module_Collision;
        Module_Gravity gravityMod = motor.FindModule(typeof(Module_Gravity)) as Module_Gravity;

        if (collisionMod == null && disableAirRays)
        {
            Debug.LogWarning("disableAirRays is set to true on a Ground Detection Module, yet a Collision Module is not found on the motor. Setting disableAirRays to false.");
            disableAirRays = false;
        }

        float colliderWidth = 0;
        float colliderHeight = 0;
        Vector3 colliderCenter = Vector3.zero;

        if(collisionMod != null)
        {
            colliderWidth = collisionMod.colliderWidth - raycastPaddingDistance;
            colliderHeight = Mathf.Max((collisionMod.colliderHeight /2), colliderWidth);
            if(collisionMod.colliderCenter != "") colliderCenter = motor.variablesMod.FindVector(collisionMod.colliderCenter).modVector;
        }

        if (measureAgainstGravity)
        {
            if (gravityMod != null) measureVector = -gravityMod.localGravityDirection;
            else measureVector = -GameController.Instance.globalGravityDirection;
        }

        if (useGroundRaycasts && (!disableAirRays || (disableAirRays && (isGrounded || (!isGrounded && collisionMod.collisionGroundNormals.Count > 0)))))
        {
            if (disableAirRays && averageGroundNormal == Vector3.zero && !isGrounded && Vector3.Angle(collisionMod.collisionGroundAverage, measureVector) < requiredGroundAngle)
            {
                groundRayDirection = -collisionMod.collisionGroundAverage;
            }

            if (xRaycastAmount < 2) xRaycastAmount = 2;
            if (zRaycastAmount < 2) zRaycastAmount = 2;

            for (int i = 0; i < xRaycastAmount; i++)
            {
                float xRaySpacing = (colliderWidth * 2) / (xRaycastAmount - 1);
                Vector3 xRayOrigin = (transform.position + colliderCenter + ((Vector3.Cross(-groundRayDirection, Vector3.ProjectOnPlane(motor.transform.forward, -groundRayDirection).normalized).normalized + Vector3.ProjectOnPlane(motor.transform.forward, -groundRayDirection).normalized) * (colliderWidth)) + -Vector3.Cross(-groundRayDirection, Vector3.ProjectOnPlane(motor.transform.forward, -groundRayDirection).normalized).normalized * xRaySpacing * i);

                for (int j = 0; j < zRaycastAmount; j++)
                {
                    float zRaySpacing = (colliderWidth * 2) / (zRaycastAmount - 1);
                    Vector3 raycastOrigin = xRayOrigin + -Vector3.ProjectOnPlane(motor.transform.forward, -groundRayDirection).normalized * zRaySpacing * j;

                    if (!ignoreCornerRays || (ignoreCornerRays && (motor.transform.position + colliderCenter - raycastOrigin).magnitude < colliderWidth + raycastPaddingDistance))
                    {
                        CastRays(raycastOrigin);
                    }
                }

                if (checkGroundAhead) CastRays(transform.position + transform.forward);
            }

        }
        else
        {
            if (collisionMod != null)
            {
                if (collisionMod.collisionGroundNormals.Count > 0)
                {
                    groundHitNormals = new List<Vector3>(collisionMod.collisionGroundNormals);
                    //groundPhysicMaterial = collisionMod.collisionPhysicMaterial;
                }
            }
        }

        if (groundHitNormals.Count > 0)
        {
            for (int i = 0; i < groundHitNormals.Count; i++) averageGroundNormal += groundHitNormals[i];

            averageGroundNormal /= groundHitNormals.Count;
            averageGroundNormal = averageGroundNormal.normalized;

            if (combineWithCollision)
            {
                if (collisionMod != null)
                {
                    if (collisionMod.collisionGroundAverage != Vector3.zero)
                    {
                        averageGroundNormal = (averageGroundNormal + collisionMod.collisionGroundAverage).normalized;
                    }
                }
                else
                {
                    Debug.LogWarning("combineWithCollision is true on a Ground Detection module, yet no Collision module is found on the motor. Setting combineWithCollision to false.");
                    combineWithCollision = false;
                }
            }

            if (gravityMod != null) localGroundAngle = Vector3.Angle(averageGroundNormal, -gravityMod.localGravityDirection);
            else localGroundAngle = Vector3.Angle(averageGroundNormal, motor.transform.up);
            globalGroundAngle = Vector3.Angle(averageGroundNormal, -GameController.Instance.globalGravityDirection);
        }

        if (averageGroundNormal != Vector3.zero)
        {
            isGrounded = true;
        }

        else
        {
            isGrounded = false;
            closestHitLayer = 0;
            closestHitTag = "";
        }


        Vector3 CastRays(Vector3 raycastOrigin)
        {
            if (Physics.Raycast(raycastOrigin, groundRayDirection, out RaycastHit hit, colliderHeight * groundRayLength, ~ignoreLayers) && Vector3.Angle(hit.normal, measureVector) < requiredGroundAngle)
            {
                if (originWallCheck)
                {
                    if (Physics.Raycast(transform.position, (raycastOrigin - transform.position).normalized, out RaycastHit originHit,(raycastOrigin - transform.position).magnitude) && originHit.collider != collisionMod.motorCollider)
                    {
                        if(drawGroundRaycasts) Debug.DrawRay(transform.position, raycastOrigin - transform.position, Color.red);
                        return Vector3.zero;
                    }
                    else
                    {
                        if (drawGroundRaycasts) Debug.DrawRay(transform.position, raycastOrigin - transform.position, Color.yellow);
                    }
                }

                if ((minDistanceCheck && (raycastOrigin - hit.point).magnitude / colliderHeight <= minGroundDistance) || (maxDistanceCheck && (raycastOrigin - hit.point).magnitude / colliderHeight > maxGroundDistance))
                {
                    if (drawGroundRaycasts)
                    {
                        Debug.DrawRay(raycastOrigin, groundRayDirection * groundRayLength * colliderHeight, noHitDrawColor);
                    }

                    return Vector3.zero;
                }

                if (drawGroundRaycasts)
                {
                    Debug.DrawRay(raycastOrigin, groundRayDirection * groundRayLength * colliderHeight, hitDrawColor);
                }
               
                if (groundHitData != null) groundHitData.Add(hit);
                if (!groundHitObjects.Contains(hit.collider.gameObject))
                {
                    groundHitObjects.Add(hit.collider.gameObject);
                }
                
                groundHitNormals.Add(hit.normal);
                
                if ((closestGroundDistance == 0 || ((raycastOrigin - hit.point).magnitude / colliderHeight) < closestGroundDistance))
                {
                    //groundPhysicMaterial = hit.collider.material;
                
                    closestGroundDistance = (raycastOrigin - hit.point).magnitude / colliderHeight;
                    closestGroundHit = hit;
                    closestHitTag = closestGroundHit.collider.tag;
                    closestHitLayer = closestGroundHit.collider.gameObject.layer;
                }
                
                return (hit.normal);
            }

            else
            {
                if (drawGroundRaycasts)
                {
                    Debug.DrawRay(raycastOrigin, groundRayDirection * groundRayLength * colliderHeight, noHitDrawColor);
                }

                return Vector3.zero;
            }
        }
    }
}
