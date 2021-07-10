using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Condition_Base : ScriptableObject
{
    public bool inverseCondition;
    public List<Condition_Base> orConditions;

    public abstract bool IsCondition(Motor_Base motor);
    
}
