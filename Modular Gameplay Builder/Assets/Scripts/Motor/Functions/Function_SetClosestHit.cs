using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SetClosestHit", menuName = "Base/Functions/Set Closest Hit")]
public class Function_SetClosestHit : Function_Base
{
    [Header("Closest Hit Variables")]
    public string closestHitTag;


    public override void RunFunction(Motor_Base motor)
    {
        Module_GroundDetection groundMod = motor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;
        if (groundMod == null) return;

        if (closestHitTag != "")
        {
            groundMod.closestHitTag = closestHitTag;
        }
    }
}
