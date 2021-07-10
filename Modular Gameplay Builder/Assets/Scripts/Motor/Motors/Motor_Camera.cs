using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor_Camera : Motor_Base
{
    [Header("Camera Components")]
    public Camera motorCam;

    [Header("Camera Properties")]
    public Vector3 offset;
    public float offsetMultiplier;
    public Vector3 rectOffset;

    [Header("Camera Target")]
    public Module_CameraTarget cameraTarget;
    public Transform overrideTarget;

    [Header("Target Ray Check")]
    public bool castTargetRays;
    public LayerMask collisionLayer;
    public RaycastHit[] targetRayHits;
    public RaycastHit closestHit;
    public bool hitSnapPosition;
    public bool debugTargetRays;

    [Header("Orbit Variables")]
    public float orbitX;
    public float orbitY;
    public string orbitVector;
    public Quaternion orbitRotation;


    protected override void Awake()
    {
        base.Awake();

        motorCam = GetComponent<Camera>();
    }

    public override void MotorFixedUpdate()
    {
        base.MotorFixedUpdate();
    }

    public override void MotorLateUpdate()
    {
        base.MotorLateUpdate();
    }

    public override void MotorUpdate()
    {
        if (Mathf.Abs(orbitX) >= 360) orbitX -= Mathf.Sign(orbitX) * 360;
        if (Mathf.Abs(orbitY) >= 360) orbitY -= Mathf.Sign(orbitY) * 360;
        if (offsetMultiplier == 0) offsetMultiplier = 1;

        if (motorCam == null) motorCam = GetComponent<Camera>();
        if (cameraTarget.currentCamera == null) cameraTarget.currentCamera = this;

        base.MotorUpdate();
    }

    protected override void Move()
    {
        base.Move();
        if (castTargetRays)
        {
            Vector3 targetPosition = cameraTarget.transform.position;

            targetRayHits = new RaycastHit[4];
            closestHit = new RaycastHit();

            List<Vector3> rayOrigins = new List<Vector3>();

            rayOrigins.Add(motorCam.ViewportToWorldPoint(new Vector3(0, 0, motorCam.nearClipPlane)));
            rayOrigins.Add(motorCam.ViewportToWorldPoint(new Vector3(0, 1, motorCam.nearClipPlane)));
            rayOrigins.Add(motorCam.ViewportToWorldPoint(new Vector3(1, 0, motorCam.nearClipPlane)));
            rayOrigins.Add(motorCam.ViewportToWorldPoint(new Vector3(1, 1, motorCam.nearClipPlane)));

            for (int i = 0; i < rayOrigins.Count; i++)
            {
                if (debugTargetRays) Debug.DrawRay(targetPosition, rayOrigins[i] - targetPosition);

                if (Physics.SphereCast(targetPosition, 0.4f, (rayOrigins[i]) - targetPosition, out targetRayHits[i], (rayOrigins[i] - targetPosition).magnitude, collisionLayer))
                {
                    if (closestHit.point == Vector3.zero || (targetRayHits[i].point - targetPosition).magnitude < (closestHit.point - targetPosition).magnitude)
                    {
                        closestHit = targetRayHits[i];
                    }
                }
            }

            if (hitSnapPosition && closestHit.point != Vector3.zero)
            {
                Vector3 newPosition = transform.position + ((((closestHit.point + closestHit.normal * 0.5f) - transform.position).magnitude) * transform.forward);
                //Vector3 newPosition = closestHit.point;

                transform.position = newPosition;
            }
        }
    }
}
