using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_Animator : Module_Base
{
    [System.Serializable]
    public class AnimatorVector
    {
        public string animatorVector;
        public string modVector;
    }

    [System.Serializable]
    public class AnimatorFloat
    {
        public string animatorFloat;
        public string modFloat;
    }

    [System.Serializable]
    public class AnimatorBool
    {
        public string animatorBool;
        public string modBool;
    }

    [Header("Target Animator")]
    public Animator targetAnimator;

    [Header("Vectors to Update")]
    public List<AnimatorVector> animatorVectors;

    [Header("Floats to Update")]
    public List<AnimatorFloat> animatorFloats;

    [Header("Bools to Update")]
    public List<AnimatorBool> animatorBools;

    [Header("(Old) Variables to Update")]
    public bool currentSpeed;
    public bool useVelocityAsSpeed;
    public float minSpeedCap;
    public float maxSpeedCap;
    public bool isGrounded;
    public bool hasInputDirection;
    public bool fallingLocalGravity;
    public bool inputToMoveAngle;

    [Header("Animation Events")]
    public bool addEventsScript;
    public AnimationEvents currentAnimationEvents;
    public bool isTransitioning;
    public bool footstepL;
    public bool footstepR;


    public override void UpdateModule(Motor_Base motor)
    {
        if (targetAnimator == null)
        {
            if(moduleName != "") targetAnimator = motor.variablesMod.FindAnimator(moduleName).modAnimator;
            else if(motor.variablesMod.animators.Count > 0) targetAnimator = motor.variablesMod.animators[0].modAnimator;
        }


        if (targetAnimator != null)
        {
            if (targetAnimator.GetAnimatorTransitionInfo(0).nameHash != 0) isTransitioning = true;
            else isTransitioning = false;

            if (addEventsScript && currentAnimationEvents == null)
            {
                currentAnimationEvents = targetAnimator.gameObject.AddComponent<AnimationEvents>();
                currentAnimationEvents.currentMotor = motor;
                currentAnimationEvents.currentAnimMod = this;
            }

            // Animator Vectors
            for(int i = 0; i < animatorVectors.Count; i++)
            {
                //targetAnimator.SetVector(animatorVectors[i].animatorVector, motor.variablesMod.FindVector(animatorVectors[i].modVector).modVector);
            }

            // Animator Floats
            for (int i = 0; i < animatorFloats.Count; i++)
            {
                targetAnimator.SetFloat(animatorFloats[i].animatorFloat, motor.variablesMod.FindFloat(animatorFloats[i].modFloat).modFloat);
            }

            // Animator Bools
            for (int i = 0; i < animatorBools.Count; i++)
            {
                targetAnimator.SetBool(animatorBools[i].animatorBool, motor.variablesMod.FindBool(animatorBools[i].modBool).modBool);
            }

            float localSpeed = motor.variablesMod.FindFloat(motor.currentSpeed).modFloat;
            if (useVelocityAsSpeed) localSpeed = motor.variablesMod.FindVector("Rigidbody Velocity").modVector.magnitude;
            if (localSpeed < minSpeedCap) localSpeed = minSpeedCap;
            if (maxSpeedCap != 0 && localSpeed > maxSpeedCap) localSpeed = maxSpeedCap;
            if (currentSpeed) targetAnimator.SetFloat("currentSpeed", localSpeed);

            if (isGrounded) targetAnimator.SetBool("isGrounded", (motor.FindModule(typeof(Module_GroundDetection)) as Module_GroundDetection).isGrounded);
            if (hasInputDirection)
            {
                bool hasInput = false;
                if (motor.variablesMod.FindVector((motor.FindModule(typeof(Module_Input)) as Module_Input).inputVector).modVector.magnitude > 0) hasInput = true;
                targetAnimator.SetBool("hasInputDirection", hasInput);
            }
            if (fallingLocalGravity)
            {
                bool isFalling = false;
                Vector3 moveVector = motor.moveVector;
                if (motor.GetType() == typeof(Motor_Physics)) moveVector = (motor as Motor_Physics).variablesMod.FindVector((motor as Motor_Physics).physicsVector).modVector;

                if (Vector3.Dot(moveVector, (motor.FindModule(typeof(Module_Gravity)) as Module_Gravity).localGravityDirection) > 0) isFalling = true;
                targetAnimator.SetBool("fallingLocalGravity", isFalling);
            }
            if(inputToMoveAngle) targetAnimator.SetFloat("inputToMoveAngle", Vector3.Angle(motor.variablesMod.FindVector((motor.FindModule(typeof(Module_Input)) as Module_Input).inputVector).modVector, motor.variablesMod.FindVector("Move Direction").modVector));
        }
    }


}
