using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor_Base : MonoBehaviour
{
    [Header("States")]
    [Tooltip("The current state that is being run by this motor.")]
    public State_Base currentState;
    [Tooltip("The list of states currently available on this motor.")]
    public List<State_Base> states;

    [Header("Motor Properties")]
    [Tooltip("If true, pauses the motor's update function, preventing any code in the update loop from being run until the motor is unpaused.")]
    public bool isMotorPaused;
    [Tooltip("If true, this motor is detected as a player and added to the GameController's player list. Otherwise, this motor will not be detected as a player.")]
    public bool isPlayer;
    [Tooltip("If isPlayer is true, this is set to the player's number. The number is determined when the motor is added to the GameController's player list, but can be reassigned.")]
    public float playerNumber;

    [Header("Movement")]
    [Tooltip("If true, moves the motor in LateUpdate.")]
    public bool moveLateUpdate;
    [Tooltip("If true, moves the motor in FixedUpdate.")]
    public bool moveFixedUpdate;
    [Tooltip("If true, moveVector is calculated using moveDirection and currentSpeed. Otherwise, moveVector will have to be set manually in state.")]
    public bool autoBuildMovement;
    [Tooltip("If true, the transform.position is set directly to moveVector. Use this when setting the position directly for movement, as opposed to additive movement.")]
    public bool setToPosition;
    [Tooltip("If true, and setToPosition is true, the current position will lerp towards the new one, as opposed to being set directly.")]
    public bool lerpPosition;
    [Tooltip("The speed that the current position is lerped towards the new position, if both setToPosition and lerpPosition are true.")]
    public float positionLerpSpeed;
    [Tooltip("Used as the magnitude for calculated moveVector")]
    public string currentSpeed;
    [Tooltip("Used as the direction for calculated moveVector.")]
    public string moveDirection;
    [Tooltip("Calculated from currentSpeed and moveDirection. Gets applied to the motor as movement.")]
    public Vector3 moveVector;
    [Tooltip("Additional movement vector that is added on top of moveVector. This cannot be auto built and must be set in state.")]
    public string additionalMoveVector;

    [Header("Modules")]
    [Tooltip("The current Variables module. This gets its own reference because it is integral to the motor.")]
    public Module_Variables variablesMod;
    [Tooltip("The list of currently attached modules. (Update)")]
    public List<Module_Base> updateModules;
    [Tooltip("The list of currently attached modules. (FixedUpdate)")]
    public List<Module_Base> fixedUpdateModules;
    [Tooltip("The list of currently attached modules. (LateUpdate)")]
    public List<Module_Base> lateUpdateModules;


    protected virtual void Awake()
    {
        // Clear the modules lists before iterating through any modules that are attatched to this motor object.
        updateModules.Clear();
        fixedUpdateModules.Clear();
        lateUpdateModules.Clear();

        // Iterate through each module attached to this motor and add them to the modules list for referencing.
        foreach (var module in GetComponents<Module_Base>())
        {
            if (variablesMod == null && module.GetType() == typeof(Module_Variables)) variablesMod = module as Module_Variables;

            if (module.fixedUpdate)
            {
                fixedUpdateModules.Add(module);
            }

            if (module.lateUpdate)
            {
                lateUpdateModules.Add(module);
            }

            else if(!module.fixedUpdate) updateModules.Add(module);
        }

        // If we have found a Variables Module, iterate through each module attached to this motor again and update them.
        if (variablesMod != null)
        {
            foreach (var module in GetComponents<Module_Base>())
            {
                module.UpdateModule(this);
            }
        }

        // If the currentState was not set before running, a warning is logged. If a state component is found in the list of states on this object, it is set as currentState.
        if (currentState == null)
        {
            if (states.Count > 0 && states[0] != null)
            {
                Debug.LogWarning("No starting state was set for currentState. Setting currentState to the first state found on this GameObject.");

                currentState = states[0];
                currentState.Construct(this);
            }
            else
            {
                Debug.LogError("No states were found on this GameObject. Cannot construct the initial state.");
            }
        }
        // If the currentState is set... 
        else
        {
            // ...run the currentState's Construct function.
            currentState.Construct(this);
        }

        // If this motor is marked as a player motor...
        if (isPlayer)
        {
            // If an Instance script is found in the GameController class...
            if (GameController.Instance != null)
            {
                // Add this motor to the GameController Instance's playerList using the AddPlayer function. Feeds in this motor as input.
                GameController.Instance.AddPlayer(this);
                playerNumber = GameController.Instance.playerList.Count;
            }
            // If an Instance script is not found in the GameController class...
            else
            {
                // Re run the Awake function to give the GameController time to assign a new Instance.
                Debug.LogWarning("No static Instance script found in the GameController class. Re running this motor's Awake function until a GameController Instance is found.");
                Awake();
                return;
            }
        }
    }

    private void Update()
    {
        // If the motor is not paused...
        if (!isMotorPaused)
        {
            // Runs the main motor update loop every frame. This MotorUpdate function can be overridden by motors that derive from this base motor class.
            MotorUpdate();
            UnpauseMotor();
        }

        else PauseMotor();
    }

    private void FixedUpdate()
    {
        // If the motor is not paused...
        if (!isMotorPaused)
        {
            // Runs the motor's fixed update loop at the fixed update timestep. This MotorFixedUpdate function can be overridden by motors that derive from this base motor class.
            MotorFixedUpdate();
        }

        else PauseMotor();
    }

    private void LateUpdate()
    {
        // If the motor is not paused...
        if (!isMotorPaused)
        {
            // Runs the motor's late update loop in monobehaviour's LateUpdate call. This MotorLateUpdate function can be overridden by motors that derive from this base motor class.
            MotorLateUpdate();
        }

        else PauseMotor();
    }

    public virtual void MotorFixedUpdate()
    {
        if (moveFixedUpdate) moveLateUpdate = false;

        // Modules
        // Iterate through all attached FixedUpdate modules and run the UpdateModule function in each.
        for (int i = 0; i < fixedUpdateModules.Count; i++)
        {
            if (variablesMod == null && fixedUpdateModules[i].GetType() == typeof(Module_Variables)) variablesMod = fixedUpdateModules[i] as Module_Variables;

            if (!fixedUpdateModules[i].dontUpdate) fixedUpdateModules[i].UpdateModule(this);

            if (!fixedUpdateModules[i].fixedUpdate)
            {
                if (fixedUpdateModules[i].lateUpdate)
                {
                    lateUpdateModules.Add(fixedUpdateModules[i]);
                    fixedUpdateModules.Remove(fixedUpdateModules[i]);
                }
                else
                {
                    updateModules.Add(fixedUpdateModules[i]);
                    fixedUpdateModules.Remove(fixedUpdateModules[i]);
                }
            }
        }

        // State FixedUpdate
        // If there is a state assigned to currentState...
        if (currentState != null)
        {
            // ...run the state's FixedUpdate function, using this motor as a reference. 
            currentState.FixedUpdateState(this);
        }

        // Logs an error if no state is assigned to currentState.
        else
        {
            Debug.LogError("No state is assigned to currentState, the motor cannot function. Please assign a currentState.");
        }

        // If moveLateUpdate is false, runs the move function, which calculates velocity from directionVector (or moveDirection) and currentSpeed (or simply applies an updated moveVector).
        if (moveFixedUpdate) Move();
    }

    public virtual void MotorLateUpdate()
    {
        if (moveLateUpdate) moveFixedUpdate = false;

     // Modules
        // Iterate through all attached LateUpdate modules and run the UpdateModule function in each.
        for (int i = 0; i < lateUpdateModules.Count; i++)
        {
            if (variablesMod == null && lateUpdateModules[i].GetType() == typeof(Module_Variables)) variablesMod = lateUpdateModules[i] as Module_Variables;

            if (!lateUpdateModules[i].dontUpdate) lateUpdateModules[i].UpdateModule(this);

            if (!lateUpdateModules[i].lateUpdate)
            {
                if (lateUpdateModules[i].fixedUpdate)
                {
                    fixedUpdateModules.Add(lateUpdateModules[i]);
                    lateUpdateModules.Remove(lateUpdateModules[i]);
                }
                else
                {
                    updateModules.Add(lateUpdateModules[i]);
                    lateUpdateModules.Remove(lateUpdateModules[i]);
                }
            }
        }

     // State LateUpdate
        // If there is a state assigned to currentState...
        if (currentState != null)
        {
            // ...run the state's LateUpdate function, using this motor as a reference. 
            currentState.LateUpdateState(this);
        }

        // Logs an error if no state is assigned to currentState.
        else
        {
            Debug.LogError("No state is assigned to currentState, the motor cannot function. Please assign a currentState.");
        }

        // If moveLateUpdate is true, runs the move function, which calculates velocity from directionVector (or moveDirection) and currentSpeed (or simply applies an updated moveVector).
        if (moveLateUpdate) Move();
    }

    public virtual void MotorUpdate()
    {
        // Modules
        // Iterate through all attached modules and run the UpdateModule function in each.
        for (int i = 0; i < updateModules.Count; i++)
        {
            if (variablesMod == null && updateModules[i].GetType() == typeof(Module_Variables)) variablesMod = updateModules[i] as Module_Variables;

            if (!updateModules[i].dontUpdate) updateModules[i].UpdateModule(this);

            if (updateModules[i].lateUpdate)
            {
                lateUpdateModules.Add(updateModules[i]);
                updateModules.Remove(updateModules[i]);
            }

            if (updateModules[i].fixedUpdate)
            {
                fixedUpdateModules.Add(updateModules[i]);
                updateModules.Remove(updateModules[i]);
            }
        }

     // State Update
        // If there is a state assigned to currentState...
        if (currentState != null)
        {
            // ...run the state's Update function, using this motor as a reference. 
            currentState.UpdateState(this);

            // Run the state's Transition function, using this motor as reference.
            currentState.Transition(this);
        }

        // Logs an error if no state is assigned to currentState.
        else
        {
            Debug.LogError("No state is assigned to currentState, the motor cannot function. Please assign a currentState.");
        }

        // DEBUG SHIT
        if (Input.GetKeyDown(KeyCode.T) && playerNumber == 1)
        {
            if (Time.timeScale == 1) Time.timeScale = 0.12f;
            else Time.timeScale = 1;
        }
        if (Input.GetKey(KeyCode.C) && isPlayer) variablesMod.FindFloat(currentSpeed).modFloat = 180;
        //

        // If moveLateUpdate and moveFixedUpdate are false, runs the move function, which calculates velocity from directionVector (or moveDirection) and currentSpeed (or simply applies an updated moveVector).
        if (!moveLateUpdate && !moveFixedUpdate) Move();
    }

    protected virtual void Move()
    {
        // If autoBuildMovement is true...
        if (autoBuildMovement)
        {
            // moveDirection's magnitude is checked to see if it is above zero.
            // If so, moveVector is calculated from moveDirection and currentSpeed.
            if (variablesMod.FindVector(moveDirection).modVector.magnitude > 0) moveVector = variablesMod.FindVector(moveDirection).modVector.normalized * variablesMod.FindFloat(currentSpeed).modFloat;

            // If moveDirection has a magnitude of zero, a warning is logged and moveVector is set to Vector3.zero.
            else
            {
                Debug.LogWarning("Cannot calculate moveVector from a direction with zero magnitude. Setting moveVector to Vector3.zero.");
                moveVector = Vector3.zero;
            }
        }

        // If setToPosition is true, set the transform.position directly to moveVector.
        if (setToPosition)
        {
            if (!lerpPosition) transform.position = moveVector;
            else transform.position = Vector3.Lerp(transform.position, moveVector, positionLerpSpeed * Time.deltaTime);
        }

        // If setToPosition is false, apply moveVector additively to the motor transform to move the motor.
        else transform.position += (moveVector + variablesMod.FindVector(additionalMoveVector).modVector);
    }

    public Module_Base FindModule(System.Type targetModule, string moduleName = default(string))
    {
        for (int i = 0; i < updateModules.Count; i++)
        {
            if (updateModules[i].GetType() == targetModule)
            {
                if (moduleName == default(string) || moduleName == updateModules[i].moduleName || moduleName == "")
                    return (updateModules[i]);
            }
        }

        for (int i = 0; i < fixedUpdateModules.Count; i++)
        {
            if (fixedUpdateModules[i].GetType() == targetModule)
            {
                if (moduleName == default(string) || moduleName == fixedUpdateModules[i].moduleName || moduleName == "")
                    return (fixedUpdateModules[i]);
            }
        }

        for (int i = 0; i < lateUpdateModules.Count; i++)
        {
            if (lateUpdateModules[i].GetType() == targetModule)
            {
                if (moduleName == default(string) || moduleName == lateUpdateModules[i].moduleName || moduleName == "")
                    return (lateUpdateModules[i]);
            }
        }

        Debug.LogWarning("A " + targetModule + " module is being searched for on " + this + " motor, yet it has not been found. Add one, or change the target moduleName when searching.)");

        return (null);
    }

    private void PauseMotor()
    {
        // Set motor to paused
        isMotorPaused = true;


        // Set velocity to zero if motor is physics motor
        if (this.GetType() == typeof(Motor_Physics)) (this as Motor_Physics).motorRigidbody.velocity = Vector3.zero;


        // Pause Animators and Particles
        foreach (Module_Base module in updateModules)
        {
            if (module.GetType() == typeof(Module_Animator)) (module as Module_Animator).targetAnimator.speed = 0;
            if (module.GetType() == typeof(Module_Particles))
            { 
                foreach(ParticleSystem particle in (module as Module_Particles).particleList)
                {
                    particle.Pause();
                }
            }
        }

        foreach (Module_Base module in fixedUpdateModules)
        {
            if (module.GetType() == typeof(Module_Animator)) (module as Module_Animator).targetAnimator.speed = 0;
            if (module.GetType() == typeof(Module_Particles))
            {
                foreach (ParticleSystem particle in (module as Module_Particles).particleList)
                {
                    particle.Pause();
                }
            }
        }

        foreach (Module_Base module in lateUpdateModules)
        {
            if (module.GetType() == typeof(Module_Animator)) (module as Module_Animator).targetAnimator.speed = 0;
            if (module.GetType() == typeof(Module_Particles))
            {
                foreach (ParticleSystem particle in (module as Module_Particles).particleList)
                {
                    particle.Pause();
                }
            }
        }
    }

    private void UnpauseMotor()
    {
        // Set motor to unpaused
        isMotorPaused = false;


        // Unpause Animators
        foreach (Module_Base module in updateModules)
        {
            if (module.GetType() == typeof(Module_Animator)) (module as Module_Animator).targetAnimator.speed = 1;
            if (module.GetType() == typeof(Module_Particles))
            {
                foreach (ParticleSystem particle in (module as Module_Particles).particleList)
                {
                    if(particle.isPaused) particle.Play();
                }
            }
        }

        foreach (Module_Base module in fixedUpdateModules)
        {
            if (module.GetType() == typeof(Module_Animator)) (module as Module_Animator).targetAnimator.speed = 1;
            if (module.GetType() == typeof(Module_Particles))
            {
                foreach (ParticleSystem particle in (module as Module_Particles).particleList)
                {
                    if (particle.isPaused) particle.Play();
                }
            }
        }

        foreach (Module_Base module in lateUpdateModules)
        {
            if (module.GetType() == typeof(Module_Animator)) (module as Module_Animator).targetAnimator.speed = 1;
            if (module.GetType() == typeof(Module_Particles))
            {
                foreach (ParticleSystem particle in (module as Module_Particles).particleList)
                {
                    if (particle.isPaused) particle.Play();
                }
            }
        }
    }
}
