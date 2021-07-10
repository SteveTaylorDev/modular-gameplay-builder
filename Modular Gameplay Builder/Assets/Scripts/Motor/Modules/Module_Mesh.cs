using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_Mesh : Module_Base
{
    [Header("Mesh Objects")]
    public GameObject targetMesh;
    public GameObject currentMeshInstance;
    public Transform meshParent;

    [Header("Mesh Renderers")]
    public MeshRenderer meshInstanceRenderer;
    public SkinnedMeshRenderer meshInstanceSkinnedRenderer;

    [Header("Mesh Options")]
    public bool addMeshTransformToVariableMod;
    public bool addMeshAnimatorsToVariableMod;


    public override void UpdateModule(Motor_Base motor)
    {
        Module_Variables variablesMod = (motor.FindModule(typeof(Module_Variables)) as Module_Variables);

        if (targetMesh != null && currentMeshInstance == null)
        {
            if (meshParent == null) currentMeshInstance = GameObject.Instantiate(targetMesh);
            else currentMeshInstance = GameObject.Instantiate(targetMesh, meshParent);
            currentMeshInstance.name = targetMesh.name;

            meshInstanceRenderer = currentMeshInstance.GetComponentInChildren<MeshRenderer>();
            meshInstanceSkinnedRenderer = currentMeshInstance.GetComponentInChildren<SkinnedMeshRenderer>();
        }

        if (currentMeshInstance != null)
        {
            if (addMeshTransformToVariableMod)
            {
                Module_Variables.ModularTransform meshTransform = new Module_Variables.ModularTransform();
                meshTransform.modTransform = currentMeshInstance.transform;
                meshTransform.name = currentMeshInstance.name;

                for (int j = 0; j < variablesMod.transforms.Count; j++)
                {
                    if (variablesMod.transforms[j].name == meshTransform.name) return;
                }

                variablesMod.transforms.Add(meshTransform);
                addMeshTransformToVariableMod = false;
            }

            if (addMeshAnimatorsToVariableMod)
            {
                Animator[] meshAnimators = currentMeshInstance.GetComponentsInChildren<Animator>();

                for(int i = 0; i < meshAnimators.Length; i++)
                {
                    Module_Variables.ModularAnimator meshAnim = new Module_Variables.ModularAnimator();
                    meshAnim.modAnimator = meshAnimators[i];
                    meshAnim.name = meshAnimators[i].name;

                    for (int j = 0; j < variablesMod.animators.Count; j++)
                    {
                        if (variablesMod.animators[j].name == meshAnim.name) return;
                    }

                    variablesMod.animators.Add(meshAnim);
                }

                addMeshAnimatorsToVariableMod = false;
            }
        }
    }
}
