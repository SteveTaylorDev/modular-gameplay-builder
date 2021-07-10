using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorTrigger : MonoBehaviour
{
    [Header("Target Conditions")]
    public string[] targetTags;
    public LayerMask targetLayers;

    [Header("Current Target")]
    public Motor_Base motor;

    [Header("On Trigger")]
    public TriggerSlot enter;
    public TriggerSlot stay;
    public TriggerSlot exit;



    private void OnTriggerEnter(Collider other)
    {
        if (targetLayers == (targetLayers | (1 << other.gameObject.layer))) motor = other.GetComponent<Motor_Base>();

        if (targetTags.Length > 0)
        {
            for (int i = 0; i < targetTags.Length; i++)
            {
                if (other.gameObject.tag == targetTags[i]) motor = other.GetComponent<Motor_Base>();
            }
        }

        if (motor != null) RunFunctions(enter, motor);
    }

    private void OnTriggerStay(Collider other)
    {
        if (targetLayers == (targetLayers | (1 << other.gameObject.layer))) motor = other.GetComponent<Motor_Base>();

        if (targetTags.Length > 0)
        {
            for (int i = 0; i < targetTags.Length; i++)
            {
                if (other.gameObject.tag == targetTags[i]) motor = other.GetComponent<Motor_Base>();
            }
        }

        if (motor != null) RunFunctions(stay, motor);
    }

    private void OnTriggerExit(Collider other)
    {
        if (targetLayers == (targetLayers | (1 << other.gameObject.layer))) motor = other.GetComponent<Motor_Base>();

        if (targetTags.Length > 0)
        {
            for (int i = 0; i < targetTags.Length; i++)
            {
                if (other.gameObject.tag == targetTags[i]) motor = other.GetComponent<Motor_Base>();
            }
        }

        if (motor != null) RunFunctions(exit, motor);

        motor = null;
    }



    private void RunFunctions(TriggerSlot triggerSlot, Motor_Base motor)
    {
        if (triggerSlot.conditions.Count > 0)
        {
            List<bool> conditionResultBools = new List<bool>();

            for (int j = 0; j < triggerSlot.conditions.Count; j++)
            {
                if (triggerSlot.conditions[j] != null)
                {
                    if (triggerSlot.conditions[j].IsCondition(motor) && triggerSlot.conditions[j].orConditions.Count > 0)
                    {
                        for (int k = 0; k < triggerSlot.conditions[j].orConditions.Count; k++)
                        {
                            if (triggerSlot.conditions[j].orConditions[k] != null && triggerSlot.conditions[j].orConditions[k].IsCondition(motor)) conditionResultBools.Add(triggerSlot.conditions[j].orConditions[k].IsCondition(motor));
                            else conditionResultBools.Add(triggerSlot.conditions[j].IsCondition(motor));
                        }
                    }

                    else conditionResultBools.Add(triggerSlot.conditions[j].IsCondition(motor));
                }
            }

            if (!conditionResultBools.Contains(false))
            {
                for (int j = 0; j < triggerSlot.functions.Count; j++)
                {
                    if (triggerSlot.functions[j] != null) triggerSlot.functions[j].RunFunction(motor);
                }
            }
        }
        else if (triggerSlot.functions.Count > 0)
        {
            for (int j = 0; j < triggerSlot.functions.Count; j++)
            {
                if (triggerSlot.functions[j] != null) triggerSlot.functions[j].RunFunction(motor);
            }
        }
    }

    [System.Serializable]
    public class TriggerSlot
    {
        public List<Condition_Base> conditions;
        public List<Function_Base> functions;
    }
}
