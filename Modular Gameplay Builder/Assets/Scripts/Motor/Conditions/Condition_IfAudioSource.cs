using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IfAudioSource", menuName = "Base/Conditions/(Sounds) If Audio Source")]
public class Condition_IfAudioSource : Condition_Base
{
    [Header("Target Clip")]
    public string targetSourceClip;
    public bool findCurrentlyPlaying;

    [Header("Play Conditions")]
    public bool isPlaying;
    public bool isNotPlaying;

    [Header("Volume Condition")]
    public bool volumeLessThan;
    public bool volumeMoreThan;
    public string volumeTarget;

    [Header("Pitch Condition")]
    public bool pitchLessThan;
    public bool pitchMoreThan;
    public string pitchTarget;



    public override bool IsCondition(Motor_Base motor)
    {
        bool isBool = true;

        Module_Sounds soundsMod = motor.FindModule(typeof(Module_Sounds)) as Module_Sounds;

        AudioClip targetClip = default;

        for(int i = 0; i < soundsMod.soundsList.Count; i++)
        {
            if (soundsMod.soundsList[i].name == targetSourceClip)
            {
                targetClip = soundsMod.soundsList[i];
                break;
            }
        }

        if (targetClip == null) return false;

        AudioSource targetSource = default;

        for(int i = 0; i < soundsMod.sourcesList.Count; i++)
        {
            if (soundsMod.sourcesList[i].clip == targetClip)
            {
                if(!findCurrentlyPlaying || (findCurrentlyPlaying && soundsMod.sourcesList[i].isPlaying)) targetSource = soundsMod.sourcesList[i];
                break;
            }
        }

        if (targetSource == null) return false;

        if (isPlaying && !targetSource.isPlaying) isBool = false;
        if (isNotPlaying && targetSource.isPlaying) isBool = false;

        if (volumeLessThan && targetSource.volume > motor.variablesMod.FindFloat(volumeTarget).modFloat) isBool = false;
        if (volumeMoreThan && targetSource.volume < motor.variablesMod.FindFloat(volumeTarget).modFloat) isBool = false;

        if (pitchLessThan && targetSource.pitch > motor.variablesMod.FindFloat(pitchTarget).modFloat) isBool = false;
        if (pitchMoreThan && targetSource.pitch < motor.variablesMod.FindFloat(pitchTarget).modFloat) isBool = false;

        if (inverseCondition) return !isBool;
        else return isBool;
    }
}
