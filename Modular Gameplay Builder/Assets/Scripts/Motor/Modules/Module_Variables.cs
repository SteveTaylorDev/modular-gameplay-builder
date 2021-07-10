using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public class Module_Variables : Module_Base
{
    [System.Serializable]
    public class ModularMotor
    {
        public string name;
        public Motor_Base modMotor;
    }

    [System.Serializable]
    public class ModularTransform
    {
        public string name;
        public Transform modTransform;
    }

    [System.Serializable]
    public class ModularBool
    {
        public string name;
        public bool modBool;
    }

    [System.Serializable]
    public class ModularFloat
    {
        public string name;
        public float modFloat;
    }

    [System.Serializable]
    public class ModularVector
    {
        public string name;
        public Vector3 modVector;
    }

    [System.Serializable]
    public class ModularQuaternion
    {
        public string name;
        public Quaternion modQuaternion;
    }

    [System.Serializable]
    public class ModularString
    {
        public string name;
        public string modString;
    }

    [System.Serializable]
    public class ModularAnimator
    {
        public string name;
        public Animator modAnimator;
    }

    public List<ModularMotor> motors;
    public List<ModularTransform> transforms;
    public List<ModularBool> bools;
    public List<ModularFloat> floats;
    public List<ModularVector> vectors;
    public List<ModularQuaternion> quaternions;
    public List<ModularString> strings;
    public List<ModularAnimator> animators;

    // This local motor reference is to allow FindVariable functions to reference the motor that was fed in by UpdateModule.
    private Motor_Base motor;
    // This format provider ensures all parsing uses the en-US culture
    private readonly IFormatProvider culture = new CultureInfo("en-US");

    private void Awake()
    {
        this.motor = GetComponent<Motor_Base>();
    }

    public Module_Variables.ModularMotor FindMotor(string motorName)
    {
        ModularMotor nullModMotor = new ModularMotor();

        if (motorName.Contains("parentCam."))
        {
            return (motor.FindModule(typeof(Module_CameraTarget)) as Module_CameraTarget).currentCamera.variablesMod.FindMotor(motorName.Substring(10));
        }

        if (motorName.Contains("camTarget."))
        {
            if ((motor as Motor_Camera).cameraTarget.parentMotor != null) return (motor as Motor_Camera).cameraTarget.parentMotor.variablesMod.FindMotor(motorName.Substring(10));
            else Debug.LogError("A camTarget is being requested from " + this + " variables module, yet there is no camera target motor attached.");
        }

        if (motorName == "this")
        {
            nullModMotor.modMotor = motor;
            return nullModMotor;
        }

        for (int i = 0; i < motors.Count; i++)
        {
            if (motors[i].name == motorName)
            {
                return (motors[i]);
            }
        }

        if (motorName != "") Debug.LogError("The motor " + motorName + " is being requested from the local Variables Module, yet that motor is not found.");
        else Debug.LogError("A function, module or motor is looking for a motor on the local Variables Module, yet the motor name has not been defined.");

        return (nullModMotor);
    }

    public Module_Variables.ModularTransform FindTransform(string transformName)
    {
        ModularTransform nullModTransform = new ModularTransform();

        if (transformName.Contains("parentCam."))
        {
            return (motor.FindModule(typeof(Module_CameraTarget)) as Module_CameraTarget).currentCamera.variablesMod.FindTransform(transformName.Substring(10));
        }

        if (transformName.Contains("camTarget."))
        {
            if ((motor as Motor_Camera).cameraTarget.parentMotor != null) return (motor as Motor_Camera).cameraTarget.parentMotor.variablesMod.FindTransform(transformName.Substring(10));
            else Debug.LogError("A camTarget is being requested from " + this + " variables module, yet there is no camera target motor attached.");
        }

        for (int i = 0; i < transforms.Count; i++)
        {
            if (transforms[i].name == transformName)
            {
                return (transforms[i]);
            }
        }

        if (transformName != "") Debug.LogError("The transform " + transformName + " is being requested from the local Variables Module, yet that transform is not found.");
        else Debug.LogError("A function, module or motor is looking for a transform on the local Variables Module, yet the transform name has not been defined.");

        return (nullModTransform);
    }

    public Module_Variables.ModularBool FindBool(string boolName)
    {
        ModularBool nullModBool = new ModularBool();

        if (boolName == "true")
        {
            ModularBool trueModBool = new ModularBool();
            trueModBool.modBool = true;

            return (trueModBool);
        }

        if (boolName == "false")
        {
            ModularBool falseModBool = new ModularBool();
            falseModBool.modBool = false;

            return (falseModBool);
        }

        if (boolName.Contains("parentCam."))
        {
            Module_Variables camTargetVariables = (motor.FindModule(typeof(Module_CameraTarget)) as Module_CameraTarget).currentCamera.variablesMod;
            if (camTargetVariables != null) return (camTargetVariables.FindBool(boolName.Substring(10)));
        }

        if (boolName.Contains("camTarget."))
        {
            if ((motor as Motor_Camera).cameraTarget.parentMotor != null) return (motor as Motor_Camera).cameraTarget.parentMotor.variablesMod.FindBool(boolName.Substring(10));
            else Debug.LogError("A camTarget is being requested from " + this + " variables module, yet there is no camera target motor attached.");
        }

        for (int i = 0; i < bools.Count; i++)
        {
            if (bools[i].name == boolName)
            {
                return (bools[i]);
            }
        }

        if (boolName != "") Debug.LogError("The bool " + boolName + " is being requested from the local Variables Module, yet that bool is not found.");
        else Debug.LogError("A function, module or motor is looking for a bool on the local Variables Module, yet the bool name has not been defined.");

        return (nullModBool);
    }

    public Module_Variables.ModularFloat FindFloat(string floatName)
    {
        ModularFloat nullModFloat = new ModularFloat();



        if (floatName.Contains("+"))
        {
            ModularFloat addModFloat = new ModularFloat();

            string[] addStrings = floatName.Split('+');

            if (addStrings.Length > 1)
            {
                addModFloat.modFloat = motor.variablesMod.FindFloat(addStrings[0]).modFloat;

                for (int i = 0; i < addStrings.Length; i++)
                {
                    addModFloat.modFloat += motor.variablesMod.FindFloat(addStrings[i]).modFloat;
                }

                return addModFloat;
            }
        }

        if (floatName.Contains("+"))
        {
            ModularFloat addModFloat = new ModularFloat();

            string[] addStrings = floatName.Split('+');

            if (addStrings.Length > 1)
            {
                addModFloat.modFloat = motor.variablesMod.FindFloat(addStrings[0]).modFloat;

                for (int i = 0; i < addStrings.Length; i++)
                {
                    addModFloat.modFloat += motor.variablesMod.FindFloat(addStrings[i]).modFloat;
                }

                return addModFloat;
            }
        }

        if (floatName.Contains("*"))
        {
            ModularFloat addModFloat = new ModularFloat();

            string[] addStrings = floatName.Split('*');

            if (addStrings.Length > 1)
            {
                addModFloat.modFloat = motor.variablesMod.FindFloat(addStrings[0]).modFloat;

                for (int i = 0; i < addStrings.Length; i++)
                {
                    addModFloat.modFloat *= motor.variablesMod.FindFloat(addStrings[i]).modFloat;
                }

                return addModFloat;
            }
        }

        if (floatName.Contains("float."))
        {
            ModularFloat floatModFloat = new ModularFloat();

            floatModFloat.modFloat = float.Parse(floatName.Substring(6), culture);


            return floatModFloat;
        }

        if (floatName.Contains("parentCam."))
        {
            return (motor.FindModule(typeof(Module_CameraTarget)) as Module_CameraTarget).currentCamera.variablesMod.FindFloat(floatName.Substring(10));
        }

        if (floatName.Contains("camTarget."))
        {
            if ((motor as Motor_Camera).cameraTarget.parentMotor != null) return (motor as Motor_Camera).cameraTarget.parentMotor.variablesMod.FindFloat(floatName.Substring(10));
            else Debug.LogError("A camTarget is being requested from " + this + " variables module, yet there is no camera target motor attached.");
            return nullModFloat;
        }

        if (floatName.Contains(".magnitude"))
        {
            ModularFloat magnitudeModFloat = new ModularFloat();

            string[] magnitudeStrings = floatName.Split('.');

            if (magnitudeStrings.Length > 1)
            {
                if (motor.variablesMod != null) magnitudeModFloat.modFloat = motor.variablesMod.FindVector(magnitudeStrings[0]).modVector.magnitude;
                return magnitudeModFloat;
            }
        }

        for (int i = 0; i < floats.Count; i++)
        {
            if (floats[i].name == floatName)
            {
                return floats[i];
            }
        }

        if (floatName != "") Debug.LogError("The float " + floatName + " is being requested from the local Variables Module, yet that float is not found.");
        else Debug.LogError("A function, module or motor is looking for a float on the local Variables Module, yet the float name has not been defined.");

        return (nullModFloat);
    }

    public Module_Variables.ModularVector FindVector(string vectorName)
    {
        ModularVector nullModVector = new ModularVector();

        if (vectorName.Contains("*"))
        {
            ModularVector multiplyModVector = new ModularVector();

            string[] multiplyStrings = vectorName.Split('*');

            if (multiplyStrings.Length > 1)
            {
                multiplyModVector.modVector = motor.variablesMod.FindVector(multiplyStrings[0]).modVector;

                for (int i = 1; i < multiplyStrings.Length; i++)
                {
                    multiplyModVector.modVector *= motor.variablesMod.FindFloat(multiplyStrings[i]).modFloat;
                }

                return multiplyModVector;
            }
        }

        if (vectorName.Contains("+"))
        {
            ModularVector addModVector = new ModularVector();

            string[] addStrings = vectorName.Split('+');

            if (addStrings.Length > 1)
            {
                addModVector.modVector = motor.variablesMod.FindVector(addStrings[0]).modVector;

                for (int i = 1; i < addStrings.Length; i++)
                {
                    addModVector.modVector += motor.variablesMod.FindVector(addStrings[i]).modVector;
                }

                return addModVector;
            }
        }

        if (vectorName.Contains("parentCam."))
        {
            return (motor.FindModule(typeof(Module_CameraTarget)) as Module_CameraTarget).currentCamera.variablesMod.FindVector(vectorName.Substring(10));
        }

        if (vectorName.Contains("camTarget."))
        {
            if ((motor as Motor_Camera).cameraTarget != null) return (motor as Motor_Camera).cameraTarget.parentMotor.variablesMod.FindVector(vectorName.Substring(10));
            else Debug.LogError("A camTarget is being requested from " + this + " variables module, yet there is no camera target motor attached.");
            return nullModVector;
        }

        if (vectorName.Contains(".normalized"))
        {
            ModularVector normalizedModVector = new ModularVector();

            string[] normalizedStrings = vectorName.Split('.');

            if (normalizedStrings.Length > 1)
            {
                if (motor.variablesMod != null) normalizedModVector.modVector = motor.variablesMod.FindVector(normalizedStrings[0]).modVector.normalized;
                return normalizedModVector;
            }
        }

        if (vectorName.StartsWith("-"))
        {
            if (motor.variablesMod != null) nullModVector.modVector = -motor.variablesMod.FindVector(vectorName.Substring(1)).modVector;
            return (nullModVector);
        }

        if (vectorName.Contains("("))
        {
            //remove brackets
            vectorName = vectorName.Substring(1,vectorName.Length-2);

            //separate vector floats
            string[] vectors = vectorName.Split(',');
            
            nullModVector.modVector = new Vector3(float.Parse(vectors[0], culture),float.Parse(vectors[1], culture),float.Parse(vectors[2], culture));

            return (nullModVector);
        }

        for (int i = 0; i < vectors.Count; i++)
        {
            if (vectors[i].name == vectorName)
            {
                return (vectors[i]);
            }
        }

        if (vectorName != "") Debug.LogError("The vector " + vectorName + " is being requested from the local Variables Module by " + this + ", yet that vector is not found.");
        else Debug.LogError(this + " is looking for a vector on the local Variables Module, yet the vector name has not been defined.");

        return (nullModVector);
    }

    public Module_Variables.ModularQuaternion FindQuaternion(string quaternionName)
    {
        ModularQuaternion nullModQuaternion = new ModularQuaternion();

        if (quaternionName == "zero")
        {
            ModularQuaternion zeroModQuaternion = new ModularQuaternion();
            zeroModQuaternion.modQuaternion = Quaternion.Euler(0, 0, 0);
            return zeroModQuaternion;
        }

        if (quaternionName.Contains("parentCam."))
        {
            return (motor.FindModule(typeof(Module_CameraTarget)) as Module_CameraTarget).currentCamera.variablesMod.FindQuaternion(quaternionName.Substring(10));
        }

        if (quaternionName.Contains("camTarget."))
        {
            if ((motor as Motor_Camera).cameraTarget.parentMotor != null) return (motor as Motor_Camera).cameraTarget.parentMotor.variablesMod.FindQuaternion(quaternionName.Substring(10));
            else Debug.LogError("A camTarget is being requested from " + this + " variables module, yet there is no camera target motor attached.");
        }

        if (quaternionName.Contains(".rotation") && motor != null)
        {
            ModularQuaternion rotationModQuaternion = new ModularQuaternion();

            string[] rotationStrings = quaternionName.Split('.');

            if (rotationStrings.Length > 1)
            {
                if (motor.variablesMod != null) rotationModQuaternion.modQuaternion = motor.variablesMod.FindTransform(rotationStrings[0]).modTransform.rotation;
                return rotationModQuaternion;
            }
        }

        for (int i = 0; i < quaternions.Count; i++)
        {
            if (quaternions[i].name == quaternionName)
            {
                return (quaternions[i]);
            }
        }

        if (quaternionName != "") Debug.LogError("The quaternion " + quaternionName + " is being requested from the local Variables Module, yet that quaternion is not found.");
        else Debug.LogError("A function, module or motor is looking for a quaternion on the local Variables Module, yet the quaternion name has not been defined.");

        return (nullModQuaternion);
    }

    public Module_Variables.ModularString FindString(string stringName)
    {
        ModularString nullModString = new ModularString();

        if (stringName.Contains("parentCam."))
        {
            return (motor.FindModule(typeof(Module_CameraTarget)) as Module_CameraTarget).currentCamera.variablesMod.FindString(stringName.Substring(10));
        }

        if (stringName.Contains("camTarget."))
        {
            if ((motor as Motor_Camera).cameraTarget.parentMotor != null) return (motor as Motor_Camera).cameraTarget.parentMotor.variablesMod.FindString(stringName.Substring(10));
            else Debug.LogError("A camTarget is being requested from " + this + " variables module, yet there is no camera target motor attached.");
        }

        if (stringName.Contains("string."))
        {
            ModularString stringModString = new ModularString();

            stringModString.modString = stringName.Substring(7);
            return stringModString;
        }

        for (int i = 0; i < strings.Count; i++)
        {
            if (strings[i].name == stringName)
            {
                return (strings[i]);
            }
        }

        if (stringName != "") Debug.LogError("The string " + stringName + " is being requested from the local Variables Module, yet that string is not found.");
        else Debug.LogError("A function, module or motor is looking for a string on the local Variables Module, yet the string name has not been defined.");

        return (nullModString);
    }

    public Module_Variables.ModularAnimator FindAnimator(string animatorName)
    {
        ModularAnimator nullModAnimator = new ModularAnimator();

        if (animatorName.Contains("parentCam."))
        {
            return (motor.FindModule(typeof(Module_CameraTarget)) as Module_CameraTarget).currentCamera.variablesMod.FindAnimator(animatorName.Substring(10));
        }

        if (animatorName.Contains("camTarget."))
        {
            if ((motor as Motor_Camera).cameraTarget.parentMotor != null) return (motor as Motor_Camera).cameraTarget.parentMotor.variablesMod.FindAnimator(animatorName.Substring(10));
            else Debug.LogError("A camTarget is being requested from " + this + " variables module, yet there is no camera target motor attached.");
        }

        for (int i = 0; i < animators.Count; i++)
        {
            if (animators[i].name == animatorName)
            {
                return (animators[i]);
            }
        }

        if (animatorName != "") Debug.LogError("The animator " + animatorName + " is being requested from the local Variables Module, yet that animator is not found.");
        else Debug.LogError("A function, module or motor is looking for a animator on the local Variables Module, yet the animator name has not been defined.");

        return (nullModAnimator);
    }

    public override void UpdateModule(Motor_Base motor)
    {
        this.motor = motor;
    }
}
