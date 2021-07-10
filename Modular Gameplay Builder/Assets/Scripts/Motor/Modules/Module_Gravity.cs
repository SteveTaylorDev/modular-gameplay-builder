using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_Gravity : Module_Base
{
    [Header("Gravity")]
    [Tooltip("Sets the localGravityDirection to the GameController's globalGravityDirection.")]
    public bool useGlobalGravDir;
    [Tooltip("Sets the localGravityStrength to the GameController's globalGravityStrength.")]
    public bool useGlobalGravStrength;
    [Tooltip("Determines the strength that gravity is applied.")]
    public float localGravityStrength;
    [Tooltip("Used as the gravity direction when applying gravity force or calculating certain angles.")]
    public Vector3 localGravityDirection;


    public override void UpdateModule(Motor_Base motor)
    {
        // Sets the localGravityDirection to the GameController's globalGravityDirection if the useGlobalGravDir boolean is true.
        if (useGlobalGravDir)
        {
            if (GameController.Instance != null)
            {
                localGravityDirection = GameController.Instance.globalGravityDirection.normalized;
            }
            else
            {
                Debug.LogWarning("No static Instance script found in the GameController class. Cannot update gravity direction.");
                return;
            }
        }

        // Normalize the localGravityDirection.
        localGravityDirection = localGravityDirection.normalized;

        // Sets the localGravityStrength to the GameController's globalGravityStrength if the useGlobalGravStrength boolean is true.
        if (useGlobalGravStrength)
        {
            if (GameController.Instance != null)
            {
                localGravityStrength = GameController.Instance.globalGravityStrength;
            }
            else
            {
                Debug.LogWarning("No static Instance script found in the GameController class. Cannot update gravity strength.");
                return;
            }
        }
    }


}
