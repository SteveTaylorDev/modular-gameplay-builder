using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_Base : MonoBehaviour
{
    [Header("Module Name")]
    public string moduleName;

    [Header("Update Timing")]
    public bool fixedUpdate;
    public bool lateUpdate;
    public bool dontUpdate;

  // Check if both update timing bools are true, or if dontUpdate is true and set bools accordingly. 
    private void Update()
    {
        if (lateUpdate) fixedUpdate = false;
        if(dontUpdate)
        {
            fixedUpdate = false;
            lateUpdate = false;
        }
    }

    public virtual void UpdateModule(Motor_Base motor)
    {
        // This function will be run by the motor. It is overridden by modules that derive from this base script.
    }
}
