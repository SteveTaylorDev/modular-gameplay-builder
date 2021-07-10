using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IfMaterialAlpha", menuName = "Base/Conditions/If Material Alpha")]
public class Condition_IfMaterialAlpha : Condition_Base
{
    [Header("Mesh Name")]
    public string meshName;

    [Header("Material Name")]
    public string materialName;

    [Header("Alpha Condition")]
    public bool moreThan;
    public bool lessThan;
    public bool equalTo;

    [Header("Target Alpha Amount")]
    public string alphaAmount;

    [Header("Options")]
    public bool debugAlpha;


    public override bool IsCondition(Motor_Base motor)
    {
        bool isAlpha = default(bool);

        if (meshName == "" || materialName == "") return false;

        Material[] meshMaterials = (motor.FindModule(typeof(Module_Mesh), meshName) as Module_Mesh).currentMeshInstance.GetComponentInChildren<MeshRenderer>().materials;

        foreach (Material material in meshMaterials)
        {
            if ((material.name == (materialName) + " (Instance)") || (material.name == materialName))
            {
                float alpha = material.color.a;

                if (debugAlpha) Debug.Log(alpha);

                if (moreThan && alpha > motor.variablesMod.FindFloat(alphaAmount).modFloat || lessThan && alpha < motor.variablesMod.FindFloat(alphaAmount).modFloat) isAlpha = true;
            }
        }

        if (inverseCondition) return !isAlpha;
        else return isAlpha;
    }
}
