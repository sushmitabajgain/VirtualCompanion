using UnityEngine;
using UnityEngine.XR.Management;
using System.Collections;

public class VRBoot : MonoBehaviour
{
    // Static flag to indicate if XR is initialized and ready
    public static bool XRReady;

    // Called when the script becomes active – set XRReady to false initially
    void OnEnable(){ XRReady = false; }

    // Called once at the start – begin coroutine to start XR subsystems
    void Start()
    {
        StartCoroutine(StartXR());
    }

    // Coroutine to handle XR initialization and startup
    IEnumerator StartXR()
    {
        // Get reference to the XR Manager (manages XR loaders and subsystems)
        var xrManager = XRGeneralSettings.Instance.Manager;

        // If no XR loader is active, try initializing it
        if (xrManager.activeLoader == null)
        {
            yield return xrManager.InitializeLoader();
        }

        // If initialization succeeded, start XR subsystems (tracking, display, etc.)
        if (xrManager.activeLoader != null)
        {
            xrManager.StartSubsystems();
        }
        else
        {
            // Log error if XR initialization failed
            Debug.LogError("Initializing XR failed.");
        }
    }

    // Called when the script or object is destroyed – cleanly shut down XR
    void OnDestroy()
    {
        var xrManager = XRGeneralSettings.Instance.Manager;

        // Ensure XR manager exists and was initialized before shutting down
        if (xrManager != null && xrManager.isInitializationComplete)
        {
            // Stop all XR subsystems and deinitialize loader
            xrManager.StopSubsystems();
            xrManager.DeinitializeLoader();
        }
    }
}
