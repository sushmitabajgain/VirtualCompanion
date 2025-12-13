using UnityEngine;
using UnityEngine.XR.Management;
using System.Collections;

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

    // -------- MONO MODE --------
    public void EnableMono()
    {
        StopAllCoroutines();
        SafeStopXR();

        vrRig.SetActive(false);
        monoRig.SetActive(true);

        EnsureSingleAudioListener();
        Debug.Log("Mono mode enabled");
    }

    // -------- VR MODE --------
    public void EnableVR()
    {
        StopAllCoroutines();
        StartCoroutine(StartVRRoutine());
    }

    private IEnumerator StartVRRoutine()
    {
        yield return SafeStartXR();

        monoRig.SetActive(false);
        vrRig.SetActive(true);

        EnsureSingleAudioListener();
        Debug.Log("VR mode enabled");
    }

    private IEnumerator SafeStartXR()
    {
        if (XRGeneralSettings.Instance == null)
            yield break;

        var manager = XRGeneralSettings.Instance.Manager;
        if (manager == null)
            yield break;

        if (!manager.isInitializationComplete)
            yield return manager.InitializeLoader();

        if (manager.activeLoader == null)
            yield break;

        manager.StartSubsystems();
    }

    private void SafeStopXR()
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
