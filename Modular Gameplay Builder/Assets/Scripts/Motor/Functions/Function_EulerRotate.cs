using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EulerRotate", menuName = "Base/Functions/Euler Rotate")]
public class Function_EulerRotate : Function_Base
{
    [Header("Vector to Rotate")]
    public string rotateVector;

    [Header("Quaternion to Rotate")]
    public string rotateQuaternion;
    public bool setQuaternionDirectly;
    public string quaternionSetLerpSpeed;

    [Header("Euler Vector")]
    public string eulerVector;

    [Header("Euler Angles")]
    public float xFloat;
    public string xMultiplier;
    public float yFloat;
    public string yMultiplier;
    public float zFloat;
    public string zMultiplier;

    [Header("Angle Limits")]
    public string minX;
    public string maxX;
    public string minY;
    public string maxY;
    public string minZ;
    public string maxZ;

    [Header("Options")]
    public bool ignoreTime;


    public override void RunFunction(Motor_Base motor)
    {
        float x = xFloat;
        float y = yFloat;
        float z = zFloat;

        float deltaFloat = Time.deltaTime;
        if (ignoreTime) deltaFloat = 1;

        if (xMultiplier != "") x *= motor.variablesMod.FindFloat(xMultiplier).modFloat;
        if (yMultiplier != "") y *= motor.variablesMod.FindFloat(yMultiplier).modFloat;
        if (zMultiplier != "") z *= motor.variablesMod.FindFloat(zMultiplier).modFloat;

        if (minX != "") x = Mathf.Clamp(x, motor.variablesMod.FindFloat(minX).modFloat, x);
        if (minY != "") y = Mathf.Clamp(y, motor.variablesMod.FindFloat(minY).modFloat, y);
        if (minZ != "") z = Mathf.Clamp(z, motor.variablesMod.FindFloat(minZ).modFloat, z);
        if (maxX != "") x = Mathf.Clamp(x, x, motor.variablesMod.FindFloat(maxX).modFloat);
        if (maxY != "") y = Mathf.Clamp(y, y, motor.variablesMod.FindFloat(maxY).modFloat);
        if (maxZ != "") z = Mathf.Clamp(z, z, motor.variablesMod.FindFloat(maxZ).modFloat);     
        
        if (rotateVector != "")
        {
            Quaternion eulerRotation = Quaternion.Euler(x, y, z);
            if (eulerVector != "") eulerRotation = Quaternion.Euler(motor.variablesMod.FindVector(eulerVector).modVector);

            motor.variablesMod.FindVector(rotateVector).modVector = eulerRotation * motor.variablesMod.FindVector(rotateVector).modVector * deltaFloat;
        }

        if (rotateQuaternion != "")
        {
            if (!setQuaternionDirectly)
            {
                x *= deltaFloat;
                y *= deltaFloat;
                z *= deltaFloat;

                Quaternion eulerRotation = Quaternion.Euler(x, y, z);
                if (eulerVector != "") eulerRotation = Quaternion.Euler(motor.variablesMod.FindVector(eulerVector).modVector);

                motor.variablesMod.FindQuaternion(rotateQuaternion).modQuaternion *= eulerRotation;
            }
            else
            {
                Quaternion eulerRotation = Quaternion.Euler(x, y, z);
                if (eulerVector != "") eulerRotation = Quaternion.Euler(motor.variablesMod.FindVector(eulerVector).modVector);

                if (quaternionSetLerpSpeed != "") motor.variablesMod.FindQuaternion(rotateQuaternion).modQuaternion = Quaternion.Slerp(motor.variablesMod.FindQuaternion(rotateQuaternion).modQuaternion, eulerRotation, motor.variablesMod.FindFloat(quaternionSetLerpSpeed).modFloat * deltaFloat);
                else motor.variablesMod.FindQuaternion(rotateQuaternion).modQuaternion = eulerRotation;
            }
        }
    }   
}
