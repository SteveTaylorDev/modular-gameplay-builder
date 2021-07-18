# Modular Gameplay Builder

This project was started to develop on a concept for a modular gameplay builder from a previous project.

Using a state machine behaviour model, virtual base classes were defined that can be overridden by subclasses.

The logic system was split into Conditions and Functions, which are applied to a State. 
These could then be read by a Motor (using Modules for advanced functionality like raycasting, collision detection etc).

With this setup, any new prebuilt Condition or Function behaviour can be read by the Motor, as its base class is used as the input type. 

Instead of using scripts attached as an object component, behaviour was split into scriptable objects; YAML files that store settings specific to that instance of the behaviour.
For example, the same ApplyForce function might be used for acceleration or for gravity. 
Instantiating two scriptable objects referencing the same function and storing different, user-defined variable contents in each allows for reusing code and creating complex behaviour quickly by dragging them onto States, in any order, using the Unity editor GUI.

A basic run-through of the core system is as follows:

Setup
  - Motor script is added as a component to an object by user (with rigidbody component for Physics Motor or camera component for Camera Motor)
  - Variables Module should be added to the same Motor object to allow declaring and assigning variables compatible with this state system while in editor
  - Additional desired Modules can be added as components (ground detection, input etc)
  - Using the Create menu (right click), a State scriptable object can be instantiated and set as the Current State reference on the Motor (drag and drop into Current State slot)
  - Again using the Create menu, Condition and Function objects can be instantiated, using motor variable references as input values for its desired behaviour
  - These Conditions and Functions are then attached to the State in the desired run order, with timing options to run states in Update, FixedUpdate, and LateUpdate
  - Transition Conditions can be set on a State, allowing the Current State to be swapped out for a different one if the Conditions are met, and a State scriptable object is specified.

Runtime
  - Modules attached to the Motor object are acquired and added to module reference lists (Update, FixedUpdate, LateUpdate) when Motor is first run (OnAwake)
  - Modules are then run every frame with their respective Update timing, with the Motor being fed in for variable referencing
  - The Current State is then run, with its Conditions and Functions being run with their respective Update timing, using Motor for variable referencing
      - Each Condition on a 'State Slot' is checked to ensure they all return True before running the associated Functions in the State Slot
      - If a False is found, Conditions in that State Slot stop being checked and it's Functions are ignored. Checking then continues onto the next State Slot.
  - The Motor is then moved; either by calculating the next position based on speed and direction, or setting the position directly to a vector (depending on chosen setting)
      - Motor can be moved in either Update, FixedUpdate, or LateUpdate timing
      - If Motor type is PhysicsMotor, movement is applied to rigidbody position/velocity instead of the object transform

To aid testing during runtime, and for ease of use, boolean toggle options (such as setting a Module's active state, or setting Conditions to invert the returned result) are available, and have functional effect while the game is running in editor.
