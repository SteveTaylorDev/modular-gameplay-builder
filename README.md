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
  - Variables Module should be added to the same Motor object to allow declaring variables compatible with this state system while in editor
  - Additional desired Modules can be added as components (ground detection, input etc)
  - Using the Create menu, a State object is instantiated and dragged into the Current State reference on the Motor
  - Again using the Create menu, Condition and Function objects can be instantiated, using Motor Variable references as inputs for its desired behaviour
  - These Conditions and Functions are then attached to the State in the desired run order

Runtime
  -

