using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CompareOrbitAngle", menuName = "Base/Conditions/(Camera) Compare Orbit Angle")]
public class Condition_CompareOrbitAngle: Condition_Base
{
    public bool compareXOrbit;
    public bool compareYOrbit;
    public bool compareAbsXOrbit;
    public bool compareAbsYOrbit;

    public bool lessThan;
    public bool moreThan;
    public bool orEqualTo;

    public float compareAmount;

    public override bool IsCondition(Motor_Base motor)
    {
        bool compareBool = default(bool);
        float compareFloat = 0;

        if (compareXOrbit) compareFloat = (motor as Motor_Camera).orbitX;
        if (compareAbsXOrbit) compareFloat = Mathf.Abs((motor as Motor_Camera).orbitX);
        if (compareYOrbit) compareFloat = (motor as Motor_Camera).orbitY + (motor as Motor_Camera).offset.y;
        if (compareAbsYOrbit) compareFloat = Mathf.Abs((motor as Motor_Camera).orbitY);

        if (!orEqualTo)
        {
            if ((lessThan && compareFloat < compareAmount) || (moreThan && compareFloat > compareAmount)) compareBool = true;
        }

        else
        {
            if ((lessThan && compareFloat <= compareAmount) || (moreThan && compareFloat >= compareAmount)) compareBool = true;
        }

        if (inverseCondition) return !compareBool;
        else return compareBool;
    }
}
