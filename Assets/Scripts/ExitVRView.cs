using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;
#if UNITY_ANDROID || UNITY_IOS
using Google.XR.Cardboard;
#endif
using System.Collections;

public class CardboardExitHandler : MonoBehaviour
{
    // Name of the menu scene to load when exiting VR
    public string menuScene = "ModeSelect";

    // Reference to VR camera (disabled during exit)
    public Camera vrCamera;

    // Flag to ensure exit is triggered only once
    bool exiting;


    // ====== Update Loop ======
    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        // On mobile: check if the Cardboard "close" button was pressed
        // If pressed, start coroutine to exit VR
        if (!exiting && Api.IsCloseButtonPressed)
            StartCoroutine(ExitVR());
#endif
    }


    // ====== Exit VR Process ======
    IEnumerator ExitVR()
    {
        exiting = true; // Prevent multiple exits

        // Disable VR camera to stop rendering
        if (vrCamera) vrCamera.enabled = false;

        // Load the menu scene additively (on top of current VR scene)
        var load = SceneManager.LoadSceneAsync(menuScene, LoadSceneMode.Additive);
        yield return load;

        // Set the menu scene as active
        var menu = SceneManager.GetSceneByName(menuScene);
        SceneManager.SetActiveScene(menu);

        // Stop and deinitialize XR subsystems to fully exit VR mode
        var mgr = XRGeneralSettings.Instance.Manager;
        if (mgr != null)
        {
            mgr.StopSubsystems();
            yield return null;              
            mgr.DeinitializeLoader();
        }

        // Finally, unload the VR scene (the one this script belongs to)
        yield return SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
