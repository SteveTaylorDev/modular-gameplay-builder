using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IfButtonPress", menuName = "Base/Conditions/If Button Press")]
public class Condition_IfButtonPress : Condition_Base
{
    [Header("Button")]
    public bool ifButton1Press;
    public bool ifButton2Press;
    public bool ifButton3Press;

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
        bool isPressed = default(bool);

        Motor_Base targetMotor = motor;
        if (useCamTarget)
        {
            if (motor.GetType() != typeof(Motor_Camera)) Debug.LogWarning(this + " condition is looking for a cam target motor, yet there is not one present.");
            else if ((motor as Motor_Camera).cameraTarget.parentMotor != null) targetMotor = (motor as Motor_Camera).cameraTarget.parentMotor;
        }

        Module_Input inputMod = targetMotor.FindModule(typeof(Module_Input)) as Module_Input;

        if(inputMod == null)
        {
            Debug.LogWarning("An state containing an inputPress function is running on a motor without an Input Module. Attach one or remove this function from the state.");
            return (false);
        }
        else
        {
            if (ifButton1Press)
            {
                isPressed = inputMod.button1Press;
            }

            if (ifButton2Press)
            {
                isPressed = inputMod.button2Press;
            }

            if (ifButton3Press)
            {
                isPressed = inputMod.button3Press;
            }

            if (ifButton1Timer)
            {
                if ((lessThan && inputMod.lastButton1Time > compareTime) || (moreThan && inputMod.lastButton1Time < compareTime)) isPressed = false;
            }

            if (ifButton2Timer)
            {
                if ((lessThan && inputMod.lastButton2Time > compareTime) || (moreThan && inputMod.lastButton2Time < compareTime)) isPressed = false;
            }

            if (ifButton3Timer)
            {
                if ((lessThan && inputMod.lastButton3Time > compareTime) || (moreThan && inputMod.lastButton3Time < compareTime)) isPressed = false;
            }
        }

        if (inverseCondition) return !isPressed;
        else return isPressed;
    }
}
