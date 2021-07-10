using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeshModEdit", menuName = "Base/Functions/(Mesh) Module Edit")]
public class Function_MeshModEdit : Function_Base
{
    [Header("Mesh Name")]
    public string meshName;

    [Header("Mesh Options")]
    public bool disableMesh;
    public bool castMeshShadow;


    public override void RunFunction(Motor_Base motor)
    {
        Module_Mesh meshMod = motor.FindModule(typeof(Module_Mesh), meshName) as Module_Mesh;
        if(meshMod != null)
        {
            if (disableMesh) meshMod.currentMeshInstance.SetActive(false);
            else meshMod.currentMeshInstance.SetActive(true);

            if (meshMod.meshInstanceRenderer != null)
            {
                if (castMeshShadow) meshMod.meshInstanceRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                else meshMod.meshInstanceRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            if (meshMod.meshInstanceSkinnedRenderer != null)
            {
                if (castMeshShadow) meshMod.meshInstanceSkinnedRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                else meshMod.meshInstanceSkinnedRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
    }
}
