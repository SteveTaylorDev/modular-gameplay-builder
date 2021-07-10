using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParticlesModEdit", menuName = "Base/Functions/(Particles) Module Edit")]
public class Function_ParticlesModEdit : Function_Base
{
    [Header("Module Edit")]
    public bool clearParticleList;

    [Header("Particle Functions")]
    public string particleName;
    public bool disableParticle;
    public bool pauseParticle;
    public bool enableParticle;

    [Header("Emitter Options")]
    public string emitterLoop;

    [Header("Additional Conditions")]
    public bool ifIsNotPlaying;
    public bool ifIsNotEmitting;


    public override void RunFunction(Motor_Base motor)
    {
        Module_Particles particlesMod = motor.FindModule(typeof(Module_Particles)) as Module_Particles;
        if (particlesMod == null) Debug.LogWarning("A ParticlesModEdit function is trying to run on this motor, yet a Particles Module is not attached. Attach one, or remove this function.");
        else
        {
            if (clearParticleList) particlesMod.particleList.Clear();

            if (particleName != null)
            {
                for (int i = 0; i < particlesMod.particleList.Count; i++)
                {
                    if (particlesMod.particleList[i].name == particleName)
                    {
                        if(emitterLoop != "")
                        {
                            ParticleSystem.MainModule particleMain = particlesMod.particleList[i].main;
                            particleMain.loop = motor.variablesMod.FindBool(emitterLoop).modBool;
                        }

                        if (disableParticle)
                        {
                            if (!ifIsNotPlaying || (ifIsNotPlaying && !particlesMod.particleList[i].isPlaying))
                            {
                                if (!ifIsNotEmitting || (ifIsNotEmitting && !particlesMod.particleList[i].isEmitting)) particlesMod.particleList[i].Stop();
                            }
                        }
                        if (pauseParticle)
                        {
                            if (!ifIsNotPlaying || (ifIsNotPlaying && !particlesMod.particleList[i].isPlaying))
                            {
                                if (!ifIsNotEmitting || (ifIsNotEmitting && !particlesMod.particleList[i].isEmitting)) particlesMod.particleList[i].Pause();
                            }
                        }
                        if (enableParticle)
                        {
                            if (!ifIsNotPlaying || (ifIsNotPlaying && !particlesMod.particleList[i].isPlaying))
                            {
                                if (!ifIsNotEmitting || (ifIsNotEmitting && !particlesMod.particleList[i].isEmitting)) particlesMod.particleList[i].Play();
                            }
                        }
                    }
                }
            }
        }
    }
}
