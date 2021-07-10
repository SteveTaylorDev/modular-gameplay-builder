using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IfButtonHeld", menuName = "Base/Conditions/If Button Held")]
public class Condition_IfButtonHeld : Condition_Base
{
    [Header("Button")]
    public bool ifButton1Held;
    public bool ifButton2Held;
    public bool ifButton3Held;

    [Header("Timer Condition")]
    public bool ifButton1Timer;
    public bool ifButton2Timer;
    public bool ifButton3Timer;
    public bool lessThan;
    public bool moreThan;
    public float compareTime;

    [Header("Options")]
    public bool useCamTarget;


    public override bool IsCondition(Motor_Base motor)
    {
        bool isHeld = default(bool);

        Motor_Base targetMotor = motor;
        if (useCamTarget)
        {
            if (motor.GetType() != typeof(Motor_Camera)) Debug.LogWarning(this + " condition is looking for a cam target motor, yet there is not one present.");
            else if((motor as Motor_Camera).cameraTarget.parentMotor != null) targetMotor = (motor as Motor_Camera).cameraTarget.parentMotor;
        } 

        Module_Input inputMod = targetMotor.FindModule(typeof(Module_Input)) as Module_Input;

        if(inputMod == null)
        {
            Debug.LogWarning("A state containing an ifButtonHeld function is running on a motor without an Input Module. Attach one or remove this function from the state.");
            return (false);
        }
        else
        {
            if (ifButton1Held)
            {
                isHeld = inputMod.button1Held;
            }

            if (ifButton2Held)
            {
                isHeld = inputMod.button2Held;
            }

            if (ifButton3Held)
            {
                isHeld = inputMod.button3Held;
            }

            if (ifButton1Timer)
            {
                if ((lessThan && inputMod.lastButton1Time > compareTime) || (moreThan && inputMod.lastButton1Time < compareTime)) isHeld = false;
            }

            if (ifButton2Timer)
            {
                if ((lessThan && inputMod.lastButton2Time > compareTime) || (moreThan && inputMod.lastButton2Time < compareTime)) isHeld = false;
            }

            if (ifButton3Timer)
            {
                if ((lessThan && inputMod.lastButton3Time > compareTime) || (moreThan && inputMod.lastButton3Time < compareTime)) isHeld = false;
            }
        }

        if (inverseCondition) return !isHeld;
        else return isHeld;
    }
}
