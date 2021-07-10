using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EditPostProcessing", menuName = "Base/Functions/Edit Post Processing")]
public class Function_EditPostProcessing : Function_Base
{
    [Header("Motion Blur")]
    public bool disableBlur;
    public bool enableBlur;




    public override void RunFunction(Motor_Base motor)
    {

    }
}
