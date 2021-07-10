using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundsModEdit", menuName = "Base/Functions/(Sounds) Module Edit")]
public class Function_SoundsModEdit : Function_Base
{
    [Header("Module Edit")]
    public bool clearSourcesList;
    public bool clearSoundsList;

    [Header("Source Functions")]
    public string soundName;
    public bool playSound;
    public bool playOneShot;
    public bool pauseSound;
    public bool unPauseSound;
    public bool stopSound;

    [Header("Multiple Clip Options")]
    public List<string> soundNames;
    public bool noRepeatSounds;

    [Header("Source Settings")]
    public string sourceMute;
    public string sourceVolume;
    public string sourcePitch;
    public string sourceLoop;

    [Header("Additional Conditions")]
    public bool ifIsNotPlaying;
    public bool ifSourceContainsTargetClip;
    public bool ifSourceContainsNoClip;
    public string ifSourceClipNameContains;


    public override void RunFunction(Motor_Base motor)
    {
        Module_Sounds soundsMod = motor.FindModule(typeof(Module_Sounds)) as Module_Sounds;

        if (soundsMod == null) Debug.LogWarning("A SoundsModEdit function is trying to run on this motor, yet a Sounds Module is not attached. Attach one, or remove this function.");
        else
        {
            if (clearSourcesList) soundsMod.sourcesList.Clear();
            if (clearSoundsList) soundsMod.soundsList.Clear();

            List<string> localSoundNames = new List<string>(soundNames);

            if (!localSoundNames.Contains(soundName))
            {
                for (int i = 0; i < soundsMod.soundsList.Count; i++)
                {
                    if(soundsMod.soundsList[i].name == soundName) localSoundNames.Add(soundName);
                }
            }

            if (soundName != "" || localSoundNames.Count > 0)
            {
               for (int j = 0; j < soundsMod.sourcesList.Count; j++)
               {
                    if (ifSourceClipNameContains == "" || (ifSourceClipNameContains != "" && soundsMod.sourcesList[j].clip.name.Contains(ifSourceClipNameContains)))
                    {
                        if ((!ifSourceContainsNoClip || (ifSourceContainsNoClip && soundsMod.sourcesList[j].clip == null)) || ifSourceContainsTargetClip)
                        {
                            if (!ifSourceContainsTargetClip || (ifSourceContainsTargetClip && soundsMod.sourcesList[j].clip != null && localSoundNames.Contains(soundsMod.sourcesList[j].clip.name)))
                            {
                                if (!ifIsNotPlaying || (ifIsNotPlaying && !soundsMod.sourcesList[j].isPlaying))
                                {
                                    AudioClip soundClip = default(AudioClip);
                                    string localSoundName = soundName;

                                    if (localSoundNames.Count > 1)
                                    {
                                        if (noRepeatSounds && soundsMod.sourcesList[j].clip != null) localSoundNames.Remove(soundsMod.sourcesList[j].clip.name);

                                        localSoundName = localSoundNames[Random.Range(0, localSoundNames.Count)];
                                    }

                                    for (int i = 0; i < soundsMod.soundsList.Count; i++)
                                    {
                                        if (soundsMod.soundsList[i].name == localSoundName)
                                        {
                                            soundClip = soundsMod.soundsList[i];
                                        }
                                    }

                                    if (sourceMute != "")
                                    {
                                        soundsMod.sourcesList[j].mute = motor.variablesMod.FindBool(sourceMute).modBool;
                                    }

                                    if (sourceVolume != "")
                                    {
                                        soundsMod.sourcesList[j].volume = motor.variablesMod.FindFloat(sourceVolume).modFloat;
                                    }

                                    if (sourcePitch != "")
                                    {
                                        soundsMod.sourcesList[j].pitch = motor.variablesMod.FindFloat(sourcePitch).modFloat;
                                    }

                                    if (sourceLoop != "")
                                    {
                                        soundsMod.sourcesList[j].loop = motor.variablesMod.FindBool(sourceLoop).modBool;
                                    }

                                    if (playSound)
                                    {
                                        soundsMod.sourcesList[j].clip = soundClip;
                                        soundsMod.sourcesList[j].Play();
                                        return;
                                    }

                                    if (playOneShot)
                                    {
                                        soundsMod.sourcesList[j].PlayOneShot(soundClip);
                                        return;
                                    }

                                    if (pauseSound)
                                    {
                                        soundsMod.sourcesList[j].Pause();
                                        return;
                                    }

                                    if (unPauseSound)
                                    {
                                        soundsMod.sourcesList[j].UnPause();
                                        return;
                                    }

                                    if (stopSound)
                                    {
                                        soundsMod.sourcesList[j].Stop();
                                        return;
                                    }
                                }
                            }
                        }
                    }
               }             
            }
        }
    }
}
