using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EditCameraTargetRays", menuName = "Base/Functions/(Camera) Edit Target Rays")]
public class Function_EditCameraTargetRays : Function_Base
{
    [Header("Target Ray Options")]
    public bool setCastTargetRays;
    public bool castTargetRays;
    public bool setHitSnapPosition;
    public bool hitSnapPosition;

    public override void RunFunction(Motor_Base motor)
    {
        if(setCastTargetRays) (motor as Motor_Camera).castTargetRays = castTargetRays;
        if(setHitSnapPosition) (motor as Motor_Camera).hitSnapPosition = hitSnapPosition;
    }
}
