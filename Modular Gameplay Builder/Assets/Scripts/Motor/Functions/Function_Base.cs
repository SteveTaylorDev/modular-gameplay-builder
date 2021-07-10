using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Function_Base : ScriptableObject
{
    // This is overridden in any components that derive from this base class. Can be filled with code to perform a specific function, such as adding force to the motor.
    public abstract void RunFunction(Motor_Base motor);
    
    
}
