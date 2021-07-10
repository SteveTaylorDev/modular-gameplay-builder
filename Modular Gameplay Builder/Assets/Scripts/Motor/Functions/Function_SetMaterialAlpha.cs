using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SetMaterialAlpha", menuName = "Base/Functions/Set Material Alpha")]
public class Function_SetMaterialAlpha : Function_Base
{
    [Header("Mesh Name")]
    public string meshName;

    [Header("Particle Name")]
    public string particleName;

    [Header("Material Name")]
    public string materialName;

    [Header("Material Alpha")]
    public float alphaAmount;
    public string alphaMultiplier;
    public string minAlpha;
    public string maxAlpha;

    [Header("Options")]
    public bool ignoreTime;
    public float lerpSpeed;
    public string lerpMultiplier;
    public string minLerpSpeed;
    public string maxLerpSpeed;


    public override void RunFunction(Motor_Base motor)
    {
        if ((meshName == "" && particleName == "") || materialName == "") return;

        Material[] materialsList = default;

        if (meshName != "")
        {
            materialsList = (motor.FindModule(typeof(Module_Mesh), meshName) as Module_Mesh).currentMeshInstance.GetComponentInChildren<MeshRenderer>().materials;
        }

        if (particleName != "")
        {

        }

        foreach(Material material in materialsList)
        {
            float deltaFloat = 1;
            if (!ignoreTime) deltaFloat = Time.deltaTime;

            if ((material.name == (materialName) + " (Instance)") || (material.name == materialName))
            {
                float localAlpha = alphaAmount;
                if (alphaMultiplier != "") localAlpha *= motor.variablesMod.FindFloat(alphaMultiplier).modFloat;

                if (minAlpha != "") localAlpha = Mathf.Clamp(localAlpha, motor.variablesMod.FindFloat(minAlpha).modFloat, localAlpha);
                if (maxAlpha != "") localAlpha = Mathf.Clamp(localAlpha, localAlpha, motor.variablesMod.FindFloat(maxAlpha).modFloat);

                Color color = material.color;

                float localLerpSpeed = lerpSpeed;
                if (lerpMultiplier != "") localLerpSpeed *= motor.variablesMod.FindFloat(lerpMultiplier).modFloat;

                if (minLerpSpeed != "") localLerpSpeed = Mathf.Clamp(localLerpSpeed, motor.variablesMod.FindFloat(minLerpSpeed).modFloat, localLerpSpeed);
                if (maxLerpSpeed != "") localLerpSpeed = Mathf.Clamp(localLerpSpeed, localLerpSpeed, motor.variablesMod.FindFloat(maxLerpSpeed).modFloat);

                if (localLerpSpeed == 0) material.color = new Color(color.r, color.g, color.b, localAlpha);
                else material.color = new Color(color.r, color.g, color.b, Mathf.Lerp(color.a, localAlpha, localLerpSpeed * deltaFloat));
            }
        }
    }
}
