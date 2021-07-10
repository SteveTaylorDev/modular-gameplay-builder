using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_Sounds: Module_Base
{
    [Header("Audio Sources")]
    public string audioSourceAmount;
    public List<AudioSource> sourcesList;

    [Header("Sounds List")]
    public List<AudioClip> soundsList;


    private void Awake()
    {
        AudioSource[] attachedSources = GetComponents<AudioSource>();

        for (int i = 0; i < attachedSources.Length; i++)
        {
            if (!sourcesList.Contains(attachedSources[i])) sourcesList.Add(attachedSources[i]);
        }
    }

    public override void UpdateModule(Motor_Base motor)
    {
        if (audioSourceAmount != "" && sourcesList.Count < motor.variablesMod.FindFloat(audioSourceAmount).modFloat)
        {
            AudioSource[] attachedSources = motor.GetComponents<AudioSource>();

            for (int i = 0; i < attachedSources.Length; i++)
            {
                if (!sourcesList.Contains(attachedSources[i])) sourcesList.Add(attachedSources[i]);
            }

            float newSourceAmount = motor.variablesMod.FindFloat(audioSourceAmount).modFloat - sourcesList.Count;

            for (int i = 0; i < newSourceAmount; i++)
            {
                sourcesList.Add(motor.gameObject.AddComponent<AudioSource>());
            }
        }
    }
}