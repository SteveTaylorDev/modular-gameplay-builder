using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_Particles : Module_Base
{
    [Header("Particle List")]
    public List<ParticleSystem> particleList;

    [Header("Settings")]
    public bool addMeshChildParticles;


    public override void UpdateModule(Motor_Base motor)
    {
        if (addMeshChildParticles)
        {
            Module_Mesh meshMod = motor.FindModule(typeof(Module_Mesh)) as Module_Mesh;
            if (meshMod != null)
            {
                List<ParticleSystem> meshParticles = new List<ParticleSystem>();
                if(meshMod.currentMeshInstance != null) meshParticles.AddRange(meshMod.currentMeshInstance.GetComponentsInChildren<ParticleSystem>());
                else Debug.LogWarning("AddMeshChildParticles is set to true on a Particles Module, yet the attached Mesh Module has no currentMeshInstance. Ensure the Mesh Module has a currentMeshInstance or disable this setting.");

                for (int i = 0; i < meshParticles.Count; i++)
                {
                    if (!particleList.Contains(meshParticles[i])) particleList.Add(meshParticles[i]);
                }
            }
            else Debug.LogWarning("AddMeshChildParticles is set to true on a Particles Module, yet a Mesh Module is not attached to the motor. Attach a Mesh Mod or disable this setting.");
        }
    }
}