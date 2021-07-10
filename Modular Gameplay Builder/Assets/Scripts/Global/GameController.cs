using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // The first instance of an object with GameController component. Used for referencing this GameController. 
    // Variables can be read publically but can only be modified from within this script.
    public static GameController Instance { get; private set; }

    [Header("World Gravity")]
    [Tooltip("Used as the global gravity direction if a motor is not using it's own local direction. Used for angles calculated relative to the world up")]
    public Vector3 globalGravityDirection;
    [Tooltip("Used as the global gravity strength if a motor is not using it's own local strength")]
    public float globalGravityStrength;

    [Header("Players")]
    [Tooltip("The list of player controlled motors. Player motors will add themselves to this list when they are initialized.")]
    public List<Motor_Base> playerList;

    [Header("Music")]
    public AudioSource globalMusicSource;


    protected virtual void Awake()
    {
        // This checks if an object with a GameController component has been assigned to Instance or not.
        if (Instance == null)
        {
            // If the Instance has not been assigned with a GameController script, set this GameController component to the reference Instance.
            Instance = this;
            // As this is the first GameController instance, it's set not to be destroyed when loading scenes, meaning any duplicate GameControllers would get destroyed on a new scene load.
            DontDestroyOnLoad(gameObject);
        }
        // If the Instance is already assigned (and it isn't this GameObject) then destroy this object, as it contains a duplicate GameController script.
        else if (Instance.gameObject != this.gameObject)
        {
            Debug.LogWarning("Duplicate GameController objects detected. Destroying the duplicate GameObject.");
            Destroy(gameObject);
        }
        // If the Instance is already assigned to this GameObject, this must be a duplicate GameController script on the same object. This duplicate is destroyed.
        else
        {
            Debug.LogWarning("Duplicate GameController scripts found on the same object. Removing one of these GameController components.");
            Destroy(this);
        }
    }

    protected virtual void Update()
    {
        // If Instance is not set, yet this object has not been destroyed as a duplicate, something bad must have happened. Set this script to Instance and hope everything will be just fine.
        if (Instance == null)
        {
            Debug.LogError("There is no Instance script set in GameController, yet the Update function is running. Setting this GameController script to Instance. (This can happen when reloading scripts in editor Play Mode)");
            Instance = this;
        }

     // Gravity
        // Normalize the globalGravityDirection vector.
        globalGravityDirection = globalGravityDirection.normalized;

        // If globalGravityDirection's magnitude and globalGravityStrength are both above zero, set the built-in physics's gravity vector to globalGravityDirection multiplied by globalGravityStrength.
        if (globalGravityDirection.magnitude > 0 && globalGravityStrength > 0) Physics.gravity = globalGravityDirection.normalized * globalGravityStrength;

     // Music
        // Debug mute button
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!globalMusicSource.isPlaying) globalMusicSource.Play();
            else globalMusicSource.Pause();
        }
    }

    // This function should be called using the GameController Instance whenever adding player motors to the game. Takes a Motor_Base class as an input, this should be a player controlled motor.
    public virtual void AddPlayer(Motor_Base playerMotor)
    {
        playerList.Add(playerMotor);
        return;
    }
}
