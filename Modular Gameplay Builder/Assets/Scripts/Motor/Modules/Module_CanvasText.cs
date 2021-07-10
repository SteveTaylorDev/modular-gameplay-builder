using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Module_CanvasText : Module_Base
{
    [Header("Text Slots")]
    public VariablesTextSlot[] textSlots;


    public override void UpdateModule(Motor_Base motor)
    {
        foreach(VariablesTextSlot slot in textSlots)
        {
            if (slot.isBool) slot.text.text = "" + motor.variablesMod.FindBool(slot.motorVariable).modBool;
            if (slot.isFloat) slot.text.text = "" + motor.variablesMod.FindFloat(slot.motorVariable).modFloat;
            if (slot.isVector) slot.text.text = "" + motor.variablesMod.FindVector(slot.motorVariable).modVector;
            if (slot.isTransformPosition) slot.text.text = "" + motor.variablesMod.FindTransform(slot.motorVariable).modTransform.position;
            if (slot.isTransformScale) slot.text.text = "" + motor.variablesMod.FindTransform(slot.motorVariable).modTransform.localScale;
            if (slot.isMaterialAlpha)
            {
                Material[] meshMaterials = (motor.FindModule(typeof(Module_Mesh), slot.meshName) as Module_Mesh).currentMeshInstance.GetComponentInChildren<MeshRenderer>().materials;

                foreach (Material material in meshMaterials)
                {
                    if ((material.name == (slot.materialName) + " (Instance)") || (material.name == slot.materialName))
                    {
                        float alpha = material.color.a;
                        slot.text.text = "" + alpha;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class VariablesTextSlot
    {
        public Text text;

        [Header("Variables")]
        public string motorVariable;
        public bool isBool;
        public bool isFloat;
        public bool isVector;
        public bool isTransformPosition;
        public bool isTransformScale;

        [Header("Materials")]
        public string meshName;
        public string materialName;
        public bool isMaterialAlpha;
    }
}
