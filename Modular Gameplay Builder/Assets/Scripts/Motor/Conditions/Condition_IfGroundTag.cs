using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IfGroundTag", menuName = "Base/Conditions/If Ground Tag")]
public class Condition_IfGroundTag : Condition_Base
{
    [Header("Target Tags")]
    public string[] targetTags;

    [Header("Ground Check Options")]
    public bool useClosestHitTag;
    public bool useAllGroundObjects;


    public override bool IsCondition(Motor_Base motor)
    {
        bool isBool = default(bool);

        Module_GroundDetection groundMod = motor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection;

        if (groundMod == null || !groundMod.isGrounded) return false;

        if(useAllGroundObjects)
        {
            for(int i = 0; i < groundMod.groundHitObjects.Count; i++)
            {
                for (int j = 0; j < targetTags.Length; j++)
                {
                    if (groundMod.groundHitObjects[i].tag == targetTags[j]) isBool = true;
                }
            }
        }

        if (useClosestHitTag)
        {
            for (int j = 0; j < targetTags.Length; j++)
            {
                if (groundMod.closestHitTag == targetTags[j]) isBool = true;
            }
        }

        if (inverseCondition) return !isBool;
        else return isBool;
    }
}
