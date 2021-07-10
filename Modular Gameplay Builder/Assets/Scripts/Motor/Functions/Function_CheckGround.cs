using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckGround", menuName = "Base/Functions/(Ground) Check Ground")]
public class Function_CheckGround : Function_Base
{

    public override void RunFunction(Motor_Base motor)
    {
        Debug.Log("check ground 1");
        Module_GroundDetection groundMod = motor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;
        if(groundMod != null)
        {
            Debug.Log("check ground 2");
            groundMod.UpdateModule(motor);
        }
    }
}
