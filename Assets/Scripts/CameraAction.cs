using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANDROID || UNITY_IOS
using Google.XR.Cardboard;
#endif

public class CardboardXRPoseDriver : MonoBehaviour
{
    bool displayRunning;
    readonly List<XRDisplaySubsystem> _displays = new();

    void OnEnable()
    {
        StartCoroutine(RecenterWhenReady());
    }

    IEnumerator RecenterWhenReady()
    {
#if UNITY_ANDROID || UNITY_IOS
        while (!XRReady.Running())
            yield return null;

        Api.Recenter();
#else
        yield break;
#endif
    }

    void LateUpdate()
    {
        // Check XR display state
        _displays.Clear();
        SubsystemManager.GetSubsystems(_displays);
        displayRunning = _displays.Count > 0 && _displays[0].running;
        if (!displayRunning) return;

        // Get HMD device
        InputDevice hmd = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
        if (!hmd.isValid) return;

        // Preferred: center-eye rotation
        if (hmd.TryGetFeatureValue(CommonUsages.centerEyeRotation, out Quaternion centerRot))
        {
            transform.localRotation = centerRot;
            return;
        }

        // Fallback: device rotation
        if (hmd.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion deviceRot))
        {
            transform.localRotation = deviceRot;
        }
    }
}
