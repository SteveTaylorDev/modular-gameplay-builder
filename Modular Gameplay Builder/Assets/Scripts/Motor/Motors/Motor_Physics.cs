using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor_Physics : Motor_Base
{
    [Header("Physics Components")]
    [Tooltip("This rigidbody is used when applying movement or rotation.")]
    public Rigidbody motorRigidbody;

    [Header("Physics Properties")]
    [Tooltip("The object's mass will affect how forces work when applied to this motor. The higher the mass, the more force is required to move it.")]
    public float mass;
    [Tooltip("Gets applied to the rigidbody velocity. Used for forces such as gravity, wind etc.")]
    public string physicsVector;

    [Header("Physics Options")]
    [Tooltip("If true, rotationQuaternion is ignored. Used for when you want to use the rigidbody's built-in physics for rotation.")]
    public bool ignoreRotationQuaternion;
    [Tooltip("If true, moveVector and physicsVector are ignored. Used for when you want to use the rigidbody's built-in physics for movement.")]
    public bool ignoreMoveVector;

    protected override void Awake()
    {
        base.Awake();

        // If there is a rigidbody component attached to this GameObject, and motorRigidbody is not set, set the first found rigidbody component to motorRigidbody for referencing.
        if (GetComponent<Rigidbody>() != null)
        {
            if (motorRigidbody == null) motorRigidbody = GetComponent<Rigidbody>();
        }

        // If no rigidbody component is found, add one and set the motorRigidbody reference to the newly added rigidbody component.
        else
        {
            Debug.LogWarning("This physics motor GameObject does not contain a rigidbody component. Now adding a rigidbody component to this GameObject.");
            motorRigidbody = gameObject.AddComponent<Rigidbody>();
        }
    }

    public override void MotorFixedUpdate()
    {
        base.MotorFixedUpdate();

        if(isMotorPaused)
        {
            // Set the motor's rigidbody velocity to Vector3.zero, to stop any movement.
            motorRigidbody.velocity = Vector3.zero;
        }
    }

    protected override void Move()
    {
     // Mass
        // Limits mass to just above zero.
        if (mass < 0.001f) mass = 0.001f;

        // Sets rigidbody mass to reflect motor mass (for consistancy and tidiness)
        motorRigidbody.mass = mass;

        // If autoBuildMovement is true...
        if (autoBuildMovement)
        {
            // moveDirection's magnitude is checked to see if it is above zero.
            // If so, moveVector is calculated from moveDirection and currentSpeed.
            if (variablesMod.FindVector(moveDirection).modVector.magnitude > 0) moveVector = variablesMod.FindVector(moveDirection).modVector.normalized * variablesMod.FindFloat(currentSpeed).modFloat;

            // If moveDirection has a magnitude of zero, a warning is logged and moveVector is set to Vector3.zero.
            else
            {
                Debug.LogWarning("Cannot calculate moveVector from a direction with zero magnitude. Setting moveVector to Vector3.zero.");
                moveVector = Vector3.zero;
            }
        }

        // If ignoreMoveVector is false...
        if (!ignoreMoveVector)
        {
            // If setToPosition is true, move the rigidbody position, using MovePosition, directly to moveVector.
            if (setToPosition) motorRigidbody.MovePosition(moveVector);

            // If setToPosition is false, combine the moveVector and physicsVector, divide physicsVector by the motor's mass, and apply directly to the rigidbody's velocity.
            else motorRigidbody.velocity = (moveVector + variablesMod.FindVector(additionalMoveVector).modVector) + (variablesMod.FindVector(physicsVector).modVector / mass);
        }
    }
}
