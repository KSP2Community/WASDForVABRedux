using HarmonyLib;
using KSP.OAB;
using UnityEngine;

namespace WasdForVab
{
    public class WasdPatches
    {
        public class WasdPatchesState
        {
            public float Yaw = 0;
            public float Pitch = 0;
            public Vector3d LookDirection;
            public float MovementSpeed = 20.5f;
            public ObjectAssemblyCameraManager CameraManager;
            public Vector3d CameraPos = new Vector3d();
            public bool IsCtrlPressed = false;
            public bool IsEnabled = true;
            public WasdConfig Config;

            public void OnLoad(ObjectAssemblyCameraManager cameraManager)
            {
                this.CameraManager = cameraManager;
                Pitch = cameraManager.CameraGimbal.transform.rotation.eulerAngles.x;
                Yaw = cameraManager.CameraGimbal.transform.rotation.eulerAngles.z;
            }

            public void OnMove(Vector3d inputVector, float deltaTime)
            {
                if (CameraManager != null && IsEnabled)
                {
                    Vector3d forward = CameraManager.GimbalTransform.transform.forward;
                    Vector3d right = -Vector3d.Cross(forward, Vector3d.up);
                    Vector3d up = Vector3d.up;
                    Vector3d movement = forward * inputVector.z + right * inputVector.x + up * inputVector.y;
                    if (Config != null)
                    {
                        movement *= deltaTime * Config.BaseSpeed.Value;
                    }
                    else
                    {
                        movement *= deltaTime * MovementSpeed;
                    }
                    CameraManager.GimbalTransform.position = CameraManager.GimbalTransform.position + movement;
                }
            }
        }

        public static WasdPatchesState patchState = new WasdPatchesState();

        [HarmonyPatch(typeof(ObjectAssemblyInputHandler), "OnSelectAllPrimaryAssembly")]
        class OnSelectAllPrimaryAssemblyPatch
        {
            static bool Prefix()
            {
                // Prevent ctrl+A binding from firing while we're in control
                return !patchState.IsCtrlPressed;
            }
        };


        [HarmonyPatch(typeof(ObjectAssemblyPlacementTool), "ProcessInputCameraRotation")]
        class ProcessInputCameraRotationPatch
        {
            static bool Prefix(Vector3 orbitTargetPos, float prevYaw, float prevPitch, float deltaYaw, float deltaPitch, float distance, ref Quaternion lookRotation, ref Vector3 lookDirection, ref Vector3 lookPosition)
            {
                if (!patchState.IsEnabled)
                {
                    return true;
                }

                var cameraSensitivity = 1.0f;
                if (patchState.Config != null)
                {
                    cameraSensitivity = patchState.Config.CameraSensitivity.Value;
                }

                var currentPitch = patchState.CameraManager.GimbalTransform.transform.eulerAngles.x;
                var currentYaw = patchState.CameraManager.GimbalTransform.transform.eulerAngles.y;
                var newPitch = currentPitch + (deltaPitch * cameraSensitivity);
                var newYaw = currentYaw + (deltaYaw * cameraSensitivity);

                if (currentPitch > 180.0f && currentPitch + deltaPitch < 270.0f)
                {
                    newPitch = 270.0f;
                }
                else if (currentPitch < 90.0f && currentPitch + deltaPitch > 89.0f)
                {
                    newPitch = 89.0f;
                }

                Vector3d lookDir = new Vector3d(0, 0, 1);
                lookRotation = QuaternionD.AngleAxis(newYaw, Vector3d.up) * QuaternionD.AngleAxis(newPitch, Vector3d.right);
                lookDir = lookRotation * lookDir;
                lookDirection = lookDir;
                lookPosition = patchState.CameraManager.GimbalTransform.position;
                return false;
            }
        }
    }
}