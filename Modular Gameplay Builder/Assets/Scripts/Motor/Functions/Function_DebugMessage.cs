using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebugMessage", menuName = "Base/Functions/Debug Message")]
public class Function_DebugMessage : Function_Base
{
    public string debugMessage;

    public override void RunFunction(Motor_Base motor)
    {
        Debug.Log(debugMessage);
    }
}
