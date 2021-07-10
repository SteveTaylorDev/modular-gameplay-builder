using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public Motor_Base currentMotor;
    public Module_Animator currentAnimMod;

    public void FootstepL()
    {
        currentAnimMod.footstepL = true;
    }

    public void FootstepR()
    {
        currentAnimMod.footstepR = true;
    }
}
