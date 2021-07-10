using UnityEngine;

public class Module_Input : Module_Base
{
    [Header("Player Number")]
    public bool usePlayerNumber;
    public string numberInputStringAddon;

    [Header("Input Vectors")]
    public Vector3 rawInputVector;
    public string inputVector;
    public Vector3 altInputVector;
    public Vector3 mouseVector;
    public float mouseScroll;

    [Header("Input Vector Deadzones")]
    public float inputVectorDeadzone;
    public float altVectorDeadzone;
    public float mouseVectorDeadzone;
    public float mouseScrollDeadzone;

    [Header("Trigger Floats")]
    public float leftTrigger;
    public float rightTrigger;

    [Header("Trigger Deadzones")]
    public float leftTriggerDeadzone;
    public float rightTriggerDeadzone;

    [Header("Input Buttons (Press)")]
    public bool button1Press;
    public bool button2Press;
    public bool button3Press;

    [Header("Input Buttons (Held)")]
    public bool button1Held;
    public bool button2Held;
    public bool button3Held;

    [Header("Timers")]
    public float lastInputVectorTime;
    public float lastAltVectorTime;
    public float lastMouseVectorTime;
    public float lastMouseScrollTime;
    public float lastLeftTriggerTime;
    public float lastRightTriggerTime;
    public float lastButton1Time;
    public float lastButton2Time;
    public float lastButton3Time;

    [Header("Input Target")]
    public bool useInputTarget;
    public Transform inputTarget;

    [Header("Input Target Options")]
    public Vector3 inputTargetOffset;
    public bool transformOffsetByTarget;
    public float targetIgnoreDistance;


    public override void UpdateModule(Motor_Base motor)
    {
     // Input Vector
        if (inputVector != "")
        {
            Vector3 localInputVector = GetInputVector("Horizontal", "", "Vertical", motor.playerNumber);

            if (localInputVector.magnitude > inputVectorDeadzone) motor.variablesMod.FindVector(inputVector).modVector = localInputVector;
            else motor.variablesMod.FindVector(inputVector).modVector = Vector3.zero;

            if (motor.variablesMod.FindVector(inputVector).modVector == Vector3.zero) lastInputVectorTime += Time.deltaTime;
            else lastInputVectorTime = 0;

            if (useInputTarget && inputTarget != null)
            {
                Vector3 localOffset = inputTargetOffset;
                if (transformOffsetByTarget) localOffset = inputTarget.TransformDirection(localOffset);

                motor.variablesMod.FindVector(inputVector).modVector = (inputTarget.position - motor.transform.position) + localOffset;
                if (motor.variablesMod.FindVector(inputVector).modVector.magnitude < targetIgnoreDistance) motor.variablesMod.FindVector(inputVector).modVector = Vector3.zero;
            }

            rawInputVector = motor.variablesMod.FindVector(inputVector).modVector;
        }

     // Alt Input Vector

        Vector3 localAltInputVector = GetInputVector("AltHorizontal", "", "AltVertical", motor.playerNumber);

        if (localAltInputVector.magnitude > altVectorDeadzone) altInputVector = localAltInputVector;
        else altInputVector = Vector3.zero;

        if (altInputVector == Vector3.zero) lastAltVectorTime += Time.deltaTime;
        else lastAltVectorTime = 0;

     // Mouse Vector

        Vector3 localMouseVector = GetInputVector("Mouse X", "Mouse Y", "");

        if (localMouseVector.magnitude > mouseVectorDeadzone) mouseVector = localMouseVector;
        else mouseVector = Vector3.zero;

        if (mouseVector == Vector3.zero) lastMouseVectorTime += Time.deltaTime;
        else lastMouseVectorTime = 0;

     // Mouse Scroll
        if (Mathf.Abs(GetInputAxis("Mouse Scroll")) > mouseScrollDeadzone * Mathf.Sign(GetInputAxis("Mouse Scroll"))) mouseScroll = Mathf.Abs(GetInputAxis("Mouse Scroll")) * Mathf.Sign(GetInputAxis("Mouse Scroll"));
        else mouseScroll = 0;

        if (mouseScroll == 0) lastMouseScrollTime += Time.deltaTime;
        else lastMouseScrollTime = 0;

     // Triggers
        if (GetInputAxis("Left Trigger", motor.playerNumber) > leftTriggerDeadzone) leftTrigger = GetInputAxis("Left Trigger", motor.playerNumber);
        else leftTrigger = 0;

        if (leftTrigger == 0) lastLeftTriggerTime += Time.deltaTime;
        else lastLeftTriggerTime = 0;

        if (GetInputAxis("Right Trigger", motor.playerNumber) > rightTriggerDeadzone) rightTrigger = GetInputAxis("Right Trigger", motor.playerNumber);
        else rightTrigger = 0;

        if (rightTrigger == 0) lastRightTriggerTime += Time.deltaTime;
        else lastRightTriggerTime = 0;

        // Button Presses
        button1Press = GetButtonPress("Button 1", motor.playerNumber);
        button2Press = GetButtonPress("Button 2", motor.playerNumber);
        button3Press = GetButtonPress("Button 3", motor.playerNumber);

        if (!button1Press) lastButton1Time += Time.deltaTime;
        else lastButton1Time = 0;

        if (!button2Press) lastButton2Time += Time.deltaTime;
        else lastButton2Time = 0;

        if (!button3Press) lastButton3Time += Time.deltaTime;
        else lastButton3Time = 0;

        // Button Holds
        button1Held = GetButtonHeld("Button 1", motor.playerNumber);
        button2Held = GetButtonHeld("Button 2", motor.playerNumber);
        button3Held = GetButtonHeld("Button 3", motor.playerNumber);

        if (!button1Held) lastButton1Time += Time.deltaTime;
        else lastButton1Time = 0;

        if (!button2Held) lastButton2Time += Time.deltaTime;
        else lastButton2Time = 0;

        if (!button3Held) lastButton3Time += Time.deltaTime;
        else lastButton3Time = 0;
    }

    private Vector3 GetInputVector(string xString, string yString, string zString, float playerNumber = default)
    {
        if (usePlayerNumber && playerNumber >= 2)
        {
           if (xString != "") xString += numberInputStringAddon + playerNumber;
           if (yString != "") yString += numberInputStringAddon + playerNumber;
           if (zString != "") zString += numberInputStringAddon + playerNumber;
        }

        float xFloat = 0;
        float yFloat = 0;
        float zFloat = 0;

        if (xString != "") xFloat = Input.GetAxis(xString);
        if (yString != "") yFloat = Input.GetAxis(yString);
        if (zString != "") zFloat = Input.GetAxis(zString);

        return (new Vector3(xFloat, yFloat, zFloat));
    }

    private float GetInputAxis(string axisName, float playerNumber = default)
    {
        if (usePlayerNumber && playerNumber >= 2)
        {
            axisName += numberInputStringAddon + playerNumber;
        }

        return (Input.GetAxis(axisName));
    }

    private bool GetButtonPress(string buttonName, float playerNumber = default)
    {
        if (usePlayerNumber && playerNumber >= 2)
        {
            buttonName += numberInputStringAddon + playerNumber;
        }

        return (Input.GetButtonDown(buttonName));
    }

    private bool GetButtonHeld(string buttonName, float playerNumber = default)
    {
        if (usePlayerNumber && playerNumber >= 2)
        {
            buttonName += numberInputStringAddon + playerNumber;
        }

        return (Input.GetButton(buttonName));
    }
}
