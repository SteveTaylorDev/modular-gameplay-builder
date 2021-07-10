using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "State_Base", menuName = "Base/States/New State")]
public class State_Base : ScriptableObject
{
    [Header("Construct")]
    public List<StateSlot> constructSlots;

    [Header("Update")]
    public List<StateSlot> updateSlots;

    [Header("FixedUpdate")]
    public List<StateSlot> fixedUpdateSlots;

    [Header("LateUpdate")]
    public List<StateSlot> lateUpdateSlots;

    [Header("OnCollisionEnter")]
    public List<StateSlot> collisionEnterSlots;

    [Header("Transition")]
    public List<StateSlot> transitionSlots;

    [Header("Destruct")]
    public List<StateSlot> destructSlots;


    public virtual void Construct(Motor_Base localMotor)
    {
        StateTemplate(constructSlots, localMotor);
    }

    public virtual void UpdateState(Motor_Base localMotor)
    {
        StateTemplate(updateSlots, localMotor);
    }

    public virtual void FixedUpdateState(Motor_Base localMotor)
    {
        StateTemplate(fixedUpdateSlots, localMotor);
    }

    public virtual void LateUpdateState(Motor_Base localMotor)
    {
        StateTemplate(lateUpdateSlots, localMotor);
    }

    public virtual void CollisionEnterState(Motor_Base localMotor)
    {
        StateTemplate(collisionEnterSlots, localMotor);
    }

    public virtual void Transition(Motor_Base localMotor)
    {
        StateTemplate(transitionSlots, localMotor);
    }

    public virtual void Destruct(Motor_Base localMotor)
    {
        StateTemplate(destructSlots, localMotor);
    }

    private void StateTemplate(List<StateSlot> stateSlots, Motor_Base localMotor)
    {
        for (int i = 0; i < stateSlots.Count; i++)
        {
            if (stateSlots[i].conditions.Count > 0)
            {
                List<bool> conditionResultBools = new List<bool>();

                for (int j = 0; j < stateSlots[i].conditions.Count; j++)
                {
                    if (stateSlots[i].conditions[j] != null)
                    {
                        if (!stateSlots[i].conditions[j].IsCondition(localMotor) && stateSlots[i].conditions[j].orConditions.Count > 0)
                        {
                            for (int k = 0; k < stateSlots[i].conditions[j].orConditions.Count; k++)
                            {
                                if (stateSlots[i].conditions[j].orConditions[k] != null && stateSlots[i].conditions[j].orConditions[k].IsCondition(localMotor)) conditionResultBools.Add(stateSlots[i].conditions[j].orConditions[k].IsCondition(localMotor));
                                else conditionResultBools.Add(stateSlots[i].conditions[j].IsCondition(localMotor));
                            }
                        }

                        else conditionResultBools.Add(stateSlots[i].conditions[j].IsCondition(localMotor));
                    }
                }

                if (!conditionResultBools.Contains(false))
                {
                    for (int j = 0; j < stateSlots[i].functions.Count; j++)
                    {
                        if (stateSlots[i].functions[j] != null) stateSlots[i].functions[j].RunFunction(localMotor);
                    }
                }
            }
            else if (stateSlots[i].functions.Count > 0)
            {
                for (int j = 0; j < stateSlots[i].functions.Count; j++)
                {
                    if (stateSlots[i].functions[j] != null) stateSlots[i].functions[j].RunFunction(localMotor);
                }
            }
        }
    }
}

[System.Serializable]
public class StateSlot
{
    public List<Condition_Base> conditions;
    public List<Function_Base> functions;
}
