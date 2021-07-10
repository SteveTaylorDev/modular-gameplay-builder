using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraFollowTarget", menuName = "Base/Functions/(Camera) Follow Target")]
public class Function_CameraFollowTarget : Function_Base
{
    [Header("Target")]
    public bool cameraTarget;
    public bool cameraTargetRigidbody;
    public bool cameraTargetMesh;

    [Header("Orbit Options")]
    public bool useOrbitAngles;
    public bool useOrbitVector;

    [Header("Orbit Lerp")]
    public string orbitLerpSpeed;

    [Header("Position Lerp")]
    public string positionLerpTime;

    [Header("Position Smooth Damp")]
    public string smoothDampRefVector;
    public string smoothDampAmount;


    public override void RunFunction(Motor_Base motor)
    {
      if (motor.setToPosition)
      {
         Module_Input inputMod = (motor as Motor_Camera).cameraTarget.parentMotor.FindModule(typeof(Module_Input)) as Module_Input;

         Vector3 localTargetPosition = Vector3.zero;
         
         if (cameraTarget)
         {
             if ((motor as Motor_Camera).overrideTarget != null) localTargetPosition = (motor as Motor_Camera).overrideTarget.position;
             else localTargetPosition = (motor as Motor_Camera).cameraTarget.transform.position;
         }
         if (cameraTargetRigidbody)
         {
             if ((motor as Motor_Camera).overrideTarget != null) localTargetPosition = (motor as Motor_Camera).overrideTarget.position;
             else localTargetPosition = ((motor as Motor_Camera).cameraTarget.parentMotor as Motor_Physics).motorRigidbody.position;
         }
         if (cameraTargetMesh)
         {
             if ((motor as Motor_Camera).overrideTarget != null) localTargetPosition = (motor as Motor_Camera).overrideTarget.position;
             else localTargetPosition = ((motor as Motor_Camera).cameraTarget.parentMotor.FindModule(typeof(Module_Mesh)) as Module_Mesh).currentMeshInstance.transform.position;
         }

         Quaternion localRotation = Quaternion.identity;

         if (useOrbitAngles)
         {
             if (useOrbitVector)
             {
                 Vector3 localOrbitVector = (motor as Motor_Camera).variablesMod.FindVector((motor as Motor_Camera).orbitVector).modVector;

                 localRotation = (Quaternion.LookRotation(localOrbitVector, motor.transform.up) * Quaternion.Euler((motor as Motor_Camera).orbitY, (motor as Motor_Camera).orbitX, 0));
             }
             else localRotation = Quaternion.FromToRotation(Vector3.up, motor.transform.up) * Quaternion.Euler((motor as Motor_Camera).orbitY, (motor as Motor_Camera).orbitX, 0);
         }
         else if (useOrbitVector)
         {
             Vector3 localOrbitVector = (motor as Motor_Camera).variablesMod.FindVector((motor as Motor_Camera).orbitVector).modVector;
             if ((motor as Motor_Camera).variablesMod.FindVector((motor as Motor_Camera).orbitVector).modVector == Vector3.zero) (motor as Motor_Camera).variablesMod.FindVector((motor as Motor_Camera).orbitVector).modVector = motor.transform.forward;

             localRotation = Quaternion.LookRotation(localOrbitVector, motor.transform.up);
         }

         if (!localRotation.IsQuaternionValid()) localRotation = (motor as Motor_Camera).orbitRotation;

         if (orbitLerpSpeed != "") (motor as Motor_Camera).orbitRotation = Quaternion.Slerp((motor as Motor_Camera).orbitRotation, localRotation, motor.variablesMod.FindFloat(orbitLerpSpeed).modFloat * Time.deltaTime);
         else (motor as Motor_Camera).orbitRotation = localRotation;

         Vector3 finalTarget = (localTargetPosition) + (motor as Motor_Camera).orbitRotation * (((motor as Motor_Camera).offset * (motor as Motor_Camera).offsetMultiplier) * (motor as Motor_Camera).cameraTarget.transform.localScale.magnitude);

         if (localTargetPosition == Vector3.zero)
         {
            Debug.LogWarning("No target was selected, SetPositionToTarget function will not run.");
            return;
         }

         if (positionLerpTime != "") motor.moveVector = Vector3.Lerp(motor.moveVector, finalTarget, motor.variablesMod.FindFloat(positionLerpTime).modFloat * Time.deltaTime);
         else if (smoothDampAmount != "") motor.moveVector = Vector3.SmoothDamp(motor.moveVector, finalTarget, ref motor.variablesMod.FindVector(smoothDampRefVector).modVector, motor.variablesMod.FindFloat(smoothDampAmount).modFloat);
         else motor.moveVector = finalTarget;
      }  
    }
}
