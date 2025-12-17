using UnityEngine;
using UnityEngine.XR.Management;
using System.Collections;

#if UNITY_ANDROID
using Google.XR.Cardboard;
#endif

public class PlayerModeSwitcher : MonoBehaviour
{
    public static PlayerModeSwitcher Instance;

    [Header("Assign from Player")]
    public GameObject monoRig;
    public GameObject vrRig;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ================= MONO MODE =================
    public void EnableMono()
    {
        StopAllCoroutines();
        StartCoroutine(EnableMonoRoutine());
    }

    private IEnumerator EnableMonoRoutine()
    {
        yield return SafeStopXR();

        vrRig.SetActive(false);
        monoRig.SetActive(true);

        EnsureSingleAudioListener();
        if (CompanionManager.Instance != null)
        {
            CompanionManager.Instance.SetMode(CompanionMode.Mono);
        }

        Debug.Log("Mono mode enabled");
    }

    // ================= VR MODE =================
    public void EnableVR()
    {
        StopAllCoroutines();
        StartCoroutine(EnableVRRoutine());
    }

    private IEnumerator EnableVRRoutine()
    {
        yield return SafeStartXR();

        monoRig.SetActive(false);
        vrRig.SetActive(true);

        EnsureSingleAudioListener();

        if (CompanionManager.Instance != null)
            CompanionManager.Instance.SetMode(CompanionMode.VR);

        Debug.Log("VR mode enabled");
    }

    // ================= XR START =================
    private IEnumerator SafeStartXR()
    {
#if UNITY_ANDROID
        // 🔒 FIXED ORIENTATION FOR VR
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        yield return null;
#endif

        var gs = XRGeneralSettings.Instance;
        if (gs == null)
            yield break;

        var manager = gs.Manager;
        if (manager == null)
            yield break;

        if (!manager.isInitializationComplete)
            yield return manager.InitializeLoader();

        if (manager.activeLoader == null)
            yield break;

        manager.StartSubsystems();
    }

    // ================= XR STOP =================
    private IEnumerator SafeStopXR()
    {
        var gs = XRGeneralSettings.Instance;
        if (gs != null)
        {
            var manager = gs.Manager;
            if (manager != null && manager.isInitializationComplete)
            {
                manager.StopSubsystems();
                manager.DeinitializeLoader();
            }
        }

#if UNITY_ANDROID
        // 🔒 KEEP LANDSCAPE LEFT (NO ROTATION)
        Screen.orientation = ScreenOrientation.LandscapeLeft;
#endif

        yield return null;
        yield return new WaitForEndOfFrame();
    }

    // ================= ANDROID SYSTEM EXIT =================
    void OnApplicationPause(bool paused)
    {
#if UNITY_ANDROID
        if (paused)
        {
            StopAllCoroutines();
            StartCoroutine(SafeStopXR());
        }
#endif
    }

    void OnApplicationFocus(bool hasFocus)
    {
#if UNITY_ANDROID
        if (!hasFocus)
        {
            StopAllCoroutines();
            StartCoroutine(SafeStopXR());
        }
#endif
    }

    // ================= AUDIO =================
    private void EnsureSingleAudioListener()
    {
        var listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        bool found = false;

        foreach (var l in listeners)
        {
            if (!found && l.gameObject.activeInHierarchy)
            {
                l.enabled = true;
                found = true;
            }
            else
            {
                l.enabled = false;
            }
        }
    }
}
