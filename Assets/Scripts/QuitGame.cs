using UnityEngine;
using UnityEngine.XR.Management;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        StopXRIfRunning();

#if UNITY_EDITOR
        Debug.Log("Quit requested (Editor)");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void StopXRIfRunning()
    {
        if (XRGeneralSettings.Instance == null) return;

        var manager = XRGeneralSettings.Instance.Manager;
        if (manager == null) return;

        if (manager.isInitializationComplete)
        {
            manager.StopSubsystems();
            manager.DeinitializeLoader();
        }
    }
}
